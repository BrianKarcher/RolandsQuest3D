using RQ.Common.Components;
using RQ.Messaging;
using UnityEngine;

namespace UtilityManager
{
    [AddComponentMenu("RQ/Components/AI/Intelligence Component")]
    public class IntelligenceComponent : ComponentBase<IntelligenceComponent>
    {
        private IntelligenceBehavior _intelligenceBehavior;
        private long _globalEntityDiedId;
        /// <summary>
        /// Doing this in Start to ensure that all entities are registered
        /// </summary>
        public void Start()
        {
            _intelligenceBehavior = GetComponent<IntelligenceBehavior>();
            _intelligenceBehavior.Init(GetComponentRepository());
        }

        public override void StartListening()
        {
            base.StartListening();
            _globalEntityDiedId = MessageDispatcher.Instance.StartListening("GlobalEntityDied", _componentRepository.GetId(), (data) =>
            {
                Debug.LogError("GlobalEntityDied received");
                _intelligenceBehavior.RemoveTarget((int)data.ExtraInfo);
            });
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher.Instance.StopListening("GlobalEntityDied", _componentRepository.GetId(), _globalEntityDiedId);
        }
    }
}
