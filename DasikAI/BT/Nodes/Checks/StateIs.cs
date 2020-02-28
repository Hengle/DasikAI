﻿﻿﻿using System.Collections.Generic;
using System.Linq;
using DasikAI.Common.Controller;
using DasikAI.BT.CustomTypes;
using DasikAI.Common.Attributes;
using DasikAI.BT.Base;
using DasikAI.BT.Base.Blocks;
using DasikAI.Common.DSO;
using DasikAI.BT.Nodes.DSO;
using XNode;

namespace DasikAI.BT.Nodes.Checks
{
	[AINode("Checks/StateIs")]
	public class StateIs : AIBlockCheck
	{
		[Node.Output(dynamicPortList = true, backingValue = ShowBackingValue.Always,
			connectionType = ConnectionType.Multiple)]
		public StatesEnum[] States;

		public override IDataStoreObject Initialize(AgentController controller)
		{
			base.Initialize(controller);
			IDataStoreObject dso;
			if (controller.GraphController.SharedStoreObjects.ContainsKey(typeof(StateDSO)))
			{
				dso = controller.GraphController.SharedStoreObjects[typeof(StateDSO)];
				return dso;
			}

			dso = new StateDSO();
			controller.GraphController.SharedStoreObjects.Add(typeof(StateDSO), dso);
			return dso;
		}

		public override IEnumerable<AINode> Next(IDataStoreObject dataStoreObject, AgentController controller)
		{
			var currentState = ((StateDSO) dataStoreObject).State;
			var result = Enumerable.Empty<AINode>();
			for (int i = 0; i < States.Length; i++)
			{
				var state = States[i].SelectedValue;
				if (state.Equals(currentState))
					result = result.Union(
						GetPort(nameof(States) + " " + i)
							.GetConnections()
							.Select(port => port.node as AINode));
			}

			return result;
		}
	}
}