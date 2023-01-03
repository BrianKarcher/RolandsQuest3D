using RQ.Common.Components;
using RQ.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RQ.Controller.Attack
{
    [Serializable]
    [AddComponentMenu("RQ/Components/Attack Parry Component")]
    public class AttackParryComponent : ComponentBase
    {
        [SerializeField]
        private GameObject _parry;
        [SerializeField]
        private float _parryDelay;
        [SerializeField]
        private float _parryLength;

        [SerializeField]
        private float _parryStartTime;
        [SerializeField]
        private float _parryEndTime;
        [SerializeField]
        private bool _isActive = false;
        [SerializeField]
        private bool _isTimerSet = false;

        private void Update()
        {
            if (_isTimerSet && !_isActive && Time.time > _parryStartTime)
            {
                Debug.Log("Attack Parry trigger +");
                _isActive = true;
                _isTimerSet = false;
                _parryEndTime = Time.time + _parryLength;
                _parryStartTime = 0f;
                _parry.SetActive(true);
            }

            if (_isActive && Time.time > _parryEndTime)
            {
                Debug.Log("Attack Parry trigger -");
                _isActive = false;
                _parryEndTime = 0f;
                _parry.SetActive(false);
            }
        }

        public void StartTimer()
        {
            _isActive = false;
            _isTimerSet = true;
            _parryStartTime = Time.time + _parryDelay;
            _parryEndTime = 0f;
        }

        public void Stop()
        {
            Debug.Log("Attack Parry Stopped");
            _parryStartTime = 0f;
            _parryEndTime = 0f;
            _isActive = false;
            _parry.SetActive(false);
            MessageDispatcher.Instance.DispatchMsg("ParryTriggerDisabled", 0f, _componentRepository.GetId(), null, null);
        }
    }
}
