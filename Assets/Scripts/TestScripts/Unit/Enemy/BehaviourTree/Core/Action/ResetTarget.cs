using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using UnityEngine;

namespace BehaviourTree
{
    public class ResetTarget : ActionNode
    {
        protected override void OnEnd() {}

        protected override void OnStart() {}

        protected override NodeState OnUpdate()
        {
            blackboard.target = blackboard.originPos;
            return NodeState.Success;
        }
    }
}
