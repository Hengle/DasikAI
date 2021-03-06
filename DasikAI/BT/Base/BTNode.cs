﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DasikAI.Common.Controller;
using DasikAI.Common.Base.DSO;
using DasikAI.Common.Base;
using XNode;

namespace DasikAI.BT.Base
{
	public abstract class BTNode : AINode
	{
		public override void OnCreateConnection(NodePort from, NodePort to)
		{
			base.OnCreateConnection(from, to);
			SetFieldValue(from.node == this ? from : to);
		}

		public override void OnRemoveConnection(NodePort port)
		{
			base.OnRemoveConnection(port);
			SetFieldValue(port);
		}

		protected void SetFieldValue(NodePort port)
		{
			var spaceIndex = port.fieldName.IndexOf(" ");
			var field = GetType().GetField(port.fieldName.Contains(" ")
					? port.fieldName.Substring(0, spaceIndex)
					: port.fieldName,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			if (field.FieldType.IsArray)
			{
				if (port.IsInput)
				{
					var result = port.GetConnections().Select(nodePort => nodePort.node).ToArray();

					Array fieldValue = (Array) field.GetValue(this);
					if (!fieldValue.Length.Equals(result.Length))
						Resize(ref fieldValue, result.Length);

					Array.Copy(result, fieldValue, result.Length);

					field.SetValue(this, fieldValue);
				}
				else
				{
					Array fieldValue = (Array) field.GetValue(this);
					if (fieldValue.GetType().GetElementType().IsAssignableFrom(port.ValueType.GetElementType()))
					{
						var result = port.GetConnections().Select(nodePort => nodePort.node).FirstOrDefault();
						var portIndex = int.Parse(port.fieldName.Substring(spaceIndex));

						var portsCount = port.node.Ports.Count(nodePort => nodePort.fieldName.StartsWith(field.Name)) -
						                 1;
						if (!fieldValue.Length.Equals(portsCount))
							Resize(ref fieldValue, portsCount);

						fieldValue.SetValue(result, portIndex);
						field.SetValue(this, fieldValue);
					}
				}
			}
			else
			{
				var result = port.GetConnections().Select(nodePort => nodePort.node).FirstOrDefault();
				field.SetValue(this, result);
			}
		}

		private static void Resize(ref Array array, int newSize)
		{
			Type elementType = array.GetType().GetElementType();
			Array newArray = Array.CreateInstance(elementType, newSize);
			Array.Copy(array, newArray, Math.Min(array.Length, newArray.Length));
			array = newArray;
		}

		public override object GetValue(NodePort port)
		{
			return this;
		}

		public abstract IEnumerable<AINode> Next(Context context);
	}
}