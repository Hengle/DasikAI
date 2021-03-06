﻿using System;
using System.Linq;
using DasikAI.Common.Attributes;
using DasikAI.BT.Base;
using DasikAI.Common.Base.ParamSources;
using DasikAI.Common.Utility;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace DasikAI.BT.Editor.Data.Graph
{
	[CustomNodeGraphEditor(typeof(BTGraph))]
	public class BTGraphEditor : NodeGraphEditor
	{
		public override void OnOpen()
		{
			base.OnOpen();
			base.window.titleContent = new GUIContent("Dasik Behaviour Tree");
		}

		/// <summary> 
		/// Overriding GetNodeMenuName lets you control if and how nodes are categorized.
		/// In this example we are sorting out all node types that are not in the XNode.Examples namespace.
		/// </summary>
		public override string GetNodeMenuName(System.Type type)
		{
			var attr = type.GetCustomAttributes(typeof(AINodeAttribute), true).FirstOrDefault();
			return (attr as AINodeAttribute)?.EditorName ?? base.GetNodeMenuName(type);
		}

		public override Color GetPortColor(NodePort port)
		{
			//var nodeBaseType = port.node.GetType().BaseType;
			//if (nodeBaseType != null &&
			//	(nodeBaseType.IsGenericType || nodeBaseType.IsConstructedGenericType) &&
			//	nodeBaseType.GetGenericTypeDefinition().IsAssignableFrom(typeof(ParamSource<>)))
			//{
			//	var color = base.GetPortColor(port);
			//	color.a = 0.1f;
			//	return color;
			//}
			var color = base.GetPortColor(port);
			if (IsParamSource(port.node.GetType())
			|| (port.IsConnected && IsParamSource(port.Connection.node.GetType())))
				color.a *= 0.1f;
			return color;
		}

		public override Color GetTypeColor(Type type)
		{
			var color = base.GetTypeColor(type);
			if (IsParamSource(type))
				color.a *= 0.1f;
			return color;
		}
		
		public override Gradient GetNoodleGradient(NodePort output, NodePort input)
		{
			Gradient grad = new Gradient();

			// If dragging the noodle, draw solid, slightly transparent
			if (input == null)
			{
				Color a = GetTypeColor(output.ValueType);
				grad.SetKeys(
					new[] { new GradientColorKey(a, 0f) },
					new[] { new GradientAlphaKey(0.6f, 0f) }
				);
			}
			// If normal, draw gradient fading from one input color to the other
			else
			{
				Color a = GetTypeColor(output.ValueType);
				Color b = GetTypeColor(input.ValueType);
				// If any port is hovered, tint white
				if (window.hoveredPort == output || window.hoveredPort == input)
				{
					a = Color.Lerp(a, Color.white, 0.8f);
					b = Color.Lerp(b, Color.white, 0.8f);
				}
				grad.SetKeys(
					new[] { new GradientColorKey(a, 0f), new GradientColorKey(b, 1f) },
					new[] { new GradientAlphaKey(a.a, 0f), new GradientAlphaKey(b.a, 1f) }
				);
			}
			return grad;
		}

		protected virtual bool IsParamSource(Type type)
		{
			return type.IsAssignableFromGeneric(typeof(ParamSource<>));
		}
	}
}