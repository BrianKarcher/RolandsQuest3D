using Assets.RQ_Assets.Scripts.Components;
using RQ.Base.Attributes;
using RQ.Common.Components;
using RQ.Common.Container;
using RQ.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RQ.Controller.Player
{
    /// <summary>
    /// Determines whether or not the player can currently parry an attack
    /// </summary>
    [AddComponentMenu("RQ/Components/Player Parry Component")]
    public class PlayerParryComponent : ComponentBase<PlayerParryComponent>
    {
        [Tag]
        [SerializeField]
        private string _tag;

        private string _currentOtherParryEntityId;
        public string CurrentOtherParryEntityId => _currentOtherParryEntityId;
        private GameObject _currentOtherParryGameObject;
        public GameObject CurrentOtherParryGameObject => _currentOtherParryGameObject;

        [SerializeField]
        private bool _canParry;
        public bool CanParry { get { return _canParry; } set { _canParry = value; } }
        private long _parryTriggerDisabledId;
        private Action<Telegram> _parryTriggerDisabledDel;
        private LockOnComponent _lockOnComponent;

        protected override void Awake()
        {
            base.Awake();
            CanParry = false;
            _parryTriggerDisabledDel = (data) =>
            {
                var otherEntity = EntityContainer.Instance.GetEntity(data.SenderId);
                if (otherEntity == null)
                    return;
                
                PerformTriggerExit(otherEntity);
            };
        }

        private void Start()
        {
            if (_lockOnComponent == null)
                _lockOnComponent = _componentRepository.Components.GetComponent<LockOnComponent>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag != _tag)
                return;

            var otherEntity = other.attachedRigidbody?.GetComponent<IEntity>();

            if (otherEntity == null)
                return;

            if (_lockOnComponent.LockGO == null)
                return;

            // Can only parry what you are locked on to.
            if (otherEntity.GetId() != _lockOnComponent.LockGO.GetId())
                return;

            Debug.Log("Player INside Attack Parry trigger");
            CanParry = true;
            _currentOtherParryEntityId = otherEntity.GetId();
            _currentOtherParryGameObject = otherEntity.gameObject;

            //Debug.Log($"Changing scenes to {_sceneConfig.SceneName} - {_spawnPointUniqueId}");
            //GameStateController.Instance.LoadScene(_sceneConfig.SceneName, _spawnPointUniqueId);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!CanParry)
                return;

            if (other.tag != _tag)
                return;

            var otherEntity = other.attachedRigidbody.GetComponent<IEntity>();

            PerformTriggerExit(otherEntity);

            //_componentRepository.SendMessage("OnTriggerExit", other, SendMessageOptions.DontRequireReceiver);
            //SendMessage();
        }

        private void PerformTriggerExit(IEntity otherEntity)
        {
            if (otherEntity == null)
                return;

            // Ignore if not the object we are parry locked
            if (otherEntity.GetId() != _currentOtherParryEntityId)
                return;

            if (_lockOnComponent.LockGO == null)
                return;

            // Can only parry what you are locked on to.
            if (otherEntity.GetId() != _lockOnComponent.LockGO.GetId())
                return;

            Debug.Log("Player OUTside Attack Parry trigger");

            CanParry = false;
            _currentOtherParryEntityId = null;
            _currentOtherParryGameObject = null;
        }

        public override void StartListening()
        {
            base.StartListening();
            _parryTriggerDisabledId = MessageDispatcher.Instance.StartListening("ParryTriggerDisabled", _componentRepository.GetId(), _parryTriggerDisabledDel);
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher.Instance.StopListening("ParryTriggerDisabled", _componentRepository.GetId(), _parryTriggerDisabledId);
        }
    }
}
