using RQ.Common.Components;
using RQ.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RQ.Controller.Camera
{
    [AddComponentMenu("RQ/Components/Rotate Towards")]
    public class RotateTowardsComponent : ComponentBase<RotateTowardsComponent>
    {
        [SerializeField]
        private GameObject _target;
        private long _updateIndex;
        private Action<Telegram> _updateDel;

        protected override void Awake()
        {
            base.Awake();
            _updateDel = (Telegram data) =>
            {
                LookAt();
            };
        }

        private void LookAt()
        {
            if (_target == null)
                return;
            //var targetVector = _target.transform.position - transform.position;
            transform.LookAt(_target.transform);
        }

        public override void StartListening()
        {
            base.StartListening();
            _updateIndex = MessageDispatcher.Instance.StartListening("LookAt", _componentRepository.GetId(), _updateDel);
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher.Instance.StopListening("LookAt", _componentRepository.GetId(), _updateIndex);
        }

        public void SetTarget(GameObject target)
        {
            _target = target;
        }
    }
}
