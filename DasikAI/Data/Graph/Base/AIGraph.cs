﻿using System;
using System.Linq;
using System.Reflection;
using DasikAI.Example.Data.Graph.Nodes.ParamSources;
using DasikAI.Scripts.Data.CustomTypes;
using UnityEngine;
using XNode;

namespace DasikAI.Scripts.Data.Graph.Base
{
	[CreateAssetMenu(fileName = "New AI State Graph", menuName = "Dasik AI/AI State Graph")]
	public class AIGraph : NodeGraph
	{
		public AIBlock Root;
		public StatesParamSource StatesSource;

		public override Node AddNode(Type type)
		{
			Node node = base.AddNode(type);
			if (Root == null)
				Root = node as AIBlock;

			if (type == typeof(StatesParamSource))
			{
				if (StatesSource != null)
				{
					RemoveNode(node);
					throw new NotImplementedException("States concatenation unsupported");
				}

				StatesSource = node as StatesParamSource;
				UpdateStates();
			}
			UpdateStates(node);
			return node;
		}

		public override void RemoveNode(Node node)
		{
			if (node == Root)
				Root = nodes.FirstOrDefault() as AIBlock;
			if (node == StatesSource)
			{
				StatesSource = null;
				UpdateStates();
			}

			base.RemoveNode(node);
		}

		public override void Clear()
		{
			Root = null;
			StatesSource = null;
			UpdateStates();
			base.Clear();
		}

		public void UpdateStates()
		{
			foreach (var node in nodes)
			{
				if (node == StatesSource)
					continue;
				UpdateStates(node);
			}
		}

		public void UpdateStates(Node node)
		{
			var fields = node.GetType().GetFields(
					BindingFlags.Public |
					BindingFlags.NonPublic |
					BindingFlags.Instance);

			foreach (var info in fields)
			{
				if (info.FieldType.IsArray)
				{
					if (info.FieldType.GetElementType().IsAssignableFrom(typeof(StatesEnum)))
					{
						Array oldStates = (Array)info.GetValue(node);
						if (oldStates == null || StatesSource == null)
						{
							info.SetValue(node, null);
						}
						else
						{
							for (int i = 0; i < oldStates.Length; i++)
							{
								var newState = StatesSource.States;
								var oldState = oldStates.GetValue(i) as StatesEnum;
								if (oldState != null)
									newState.SelectedValue = oldState.SelectedValue;
								oldStates.SetValue(newState, i);
							}
							//fieldInfo.SetValue(node, oldStates);
						}
					}
				}
				else
				{
					if (info.FieldType == typeof(StatesEnum))
					{
						if (StatesSource == null)
							info.SetValue(node, null);
						else
						{
							var newState = StatesSource.States;
							if (info.GetValue(node) is StatesEnum oldState)
								newState.SelectedValue = oldState.SelectedValue;
							info.SetValue(node, newState);
						}
					}
				}
			}

			var properties = node.GetType().GetProperties(
				BindingFlags.Public |
				BindingFlags.NonPublic |
				BindingFlags.Instance);

			foreach (var info in properties)
			{
				if (info.PropertyType.IsArray)
				{
					if (info.PropertyType.GetElementType().IsAssignableFrom(typeof(StatesEnum)))
					{
						Array oldStates = (Array)info.GetValue(node);
						if (oldStates == null || StatesSource == null)
						{
							info.SetValue(node, null);
						}
						else
						{
							for (int i = 0; i < oldStates.Length; i++)
							{
								var newState = StatesSource.States;
								var oldState = oldStates.GetValue(i) as StatesEnum;
								if (oldState != null)
									newState.SelectedValue = oldState.SelectedValue;
								oldStates.SetValue(newState, i);
							}
							//fieldInfo.SetValue(node, oldStates);
						}
					}
				}
				else
				{
					if (info.PropertyType == typeof(StatesEnum))
					{
						if (StatesSource == null)
							info.SetValue(node, null);
						else
						{
							var newState = StatesSource.States;
							if (info.GetValue(node) is StatesEnum oldState)
								newState.SelectedValue = oldState.SelectedValue;
							info.SetValue(node, newState);
						}
					}
				}
			}
		}
	}
}
