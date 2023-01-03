using RQ.Common.Components;
using UnityEngine;

namespace RQ.Controller.AI
{
    public class PlayMakerStateMachineComponent : ComponentBase<PlayMakerStateMachineComponent>
    {
        [SerializeField]
        private PlayMakerFSM _pm;

        protected override void Awake()
        {
            base.Awake();
            if (_pm == null)
            {
                _pm = GetComponent<PlayMakerFSM>();
            }
        }

        public void SetTemplate(FsmTemplate template)
        {
            _pm.SetFsmTemplate(template);
        }

        public void StartFsm()
        {
            _pm.Fsm.Start();
        }

        public void StopFsm()
        {
            _pm.Fsm.Stop();
        }

        public bool IsFinished => _pm.Fsm.Finished;
    }
}
