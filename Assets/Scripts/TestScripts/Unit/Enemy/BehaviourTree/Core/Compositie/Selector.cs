using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{    
    public class Selector : CompositeNode
    {
        int crurent;

        protected override void OnEnd() {}

        protected override void OnStart() => crurent = 0;

        protected override NodeState OnUpdate()
        {
            switch(children[crurent].Update())
            {
                case NodeState.Running: return NodeState.Running;
                case NodeState.Failure: crurent++; break;
                case NodeState.Success: return NodeState.Success;
            }
            return crurent == children.Count ? NodeState.Failure : NodeState.Running;
        }
    }
}
