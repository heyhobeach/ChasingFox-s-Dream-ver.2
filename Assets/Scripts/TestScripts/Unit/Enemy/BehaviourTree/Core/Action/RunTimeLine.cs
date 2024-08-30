using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace BehaviourTree
{
    public class RunTimeLine : ActionNode
    {
        public bool isCan;

        private PlayableDirector playableDirector;

        private void OnEnable()
        {
            playableDirector = blackboard.playableDirector;
            isCan = true;
            playableDirector.stopped -= OnTimeLineStoped;
            playableDirector.stopped += OnTimeLineStoped;
        }
        protected override void OnEnd() {}

        protected override void OnStart()
        {
            if(playableDirector != null && isCan && playableDirector.state != PlayState.Playing)
            {
                playableDirector.Play();
            }
        }

        protected override NodeState OnUpdate()
        {
            if(playableDirector == null || !isCan || playableDirector.state != PlayState.Playing)
            {
                if(playableDirector.state == PlayState.Playing) TimeLineStop();
                return NodeState.Failure;
            }
            else return NodeState.Success;
        }

        private void OnTimeLineStoped(PlayableDirector pd)
        {
            isCan = false;
            playableDirector.enabled = false;
        }
        [MesageTarget] public void TimeLineStop() => playableDirector.Stop();
    }
}