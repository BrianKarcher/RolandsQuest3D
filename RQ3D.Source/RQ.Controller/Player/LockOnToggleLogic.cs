using Assets.RQ_Assets.Scripts.Components;
using RQ.Common.Container;
using RQ.Controller.Camera;
using RQ.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RQ.Controller.Player
{
    [Serializable]
    public class LockOnToggleLogic : ILockOnLogic
    {
        [SerializeField]
        private IEntity _lockGO;
        public IEntity LockGO => _lockGO;
        private LockOnComponent _component;
        [SerializeField]
        private bool _strafePressed;
        public bool IsStrafePressed() => _strafePressed;
        private PlayerController _playerController;
        [SerializeField]
        private List<GameObject> _lockList = null;
        [SerializeField]
        private int _lockListIndex = 0;
        private ThirdPersonUserControl _thirdPersonUserControl;
        private float _lastLockPressedTime;
        private ThirdPersonCameraController _camera;

        public LockOnToggleLogic(LockOnComponent component, PlayerController playerController)
        {
            _component = component;
            // TODO: Ugh...
            if (_camera == null)
                _camera = GameObject.FindObjectOfType<ThirdPersonCameraController>();
            if (_thirdPersonUserControl == null)
                _thirdPersonUserControl = component.GetComponentRepository().Components.GetComponent<ThirdPersonUserControl>();
            _playerController = playerController;
            _strafePressed = false;
        }

        public void ProcessButtonDown()
        {
            Debug.Log("Target Lock button pressed");
            //_strafePressed = true;
            //_lockGO = null;
            // Determine whether to get the next target, or to create the list and get the first target.

            // Button pressed within the alotted time? Select the next target in the previously generated list
            //if (_lockList != null && _lockList.Count != 0 && Time.time - _lastLockReleaseTime < _component.ClickNextTargetTime)
            //{
            //    Debug.Log("Getting next target since within time limit.");
            //    _lockGO = GetIncrementTargetLock();
            //}
            //else
            //{

            CalculateEntity();

            //if (_lockList != null && _lockList.Count != 0)
            //{
            //    _lockListIndex = 0;
            //    _lockGO = _lockList[_lockListIndex];
            //}
            //}

            TargetLockPressed(_lockGO);
            _component.GetComponentRepository().Target = _lockGO?.gameObject;

            if (_lockGO == null)
            {
                _strafePressed = !_strafePressed;
                //MessageDispatcher.Instance.DispatchMsg(_cameraOverShoulderMessage, 0f, _componentRepository.GetId(), _camera.GetComponentRepository().GetId(), null);
                _thirdPersonUserControl.SetMovementType(MovementType.ByCamera);
                _playerController.CalculateMovementType();
                //_playerController.SetCameraBasedOnMovementType();
            }
            else
            {
                _strafePressed = true;
                //Debug.Log($"(LockOnComponent) Setting camera to {_cameraLockOnMessage}");
                SendCameraLockOnMessage();
                _thirdPersonUserControl.SetMovementType(MovementType.ByPlayer); // Lock on - tank controls
                _playerController.CalculateMovementType();
                //_playerController.SetCameraBasedOnMovementType();
            }

            _lastLockPressedTime = Time.time;
        }

        private IEntity CalculateEntity()
        {
            _lockList = (List<GameObject>)_component.CreateTargetList();
            var closestLockEntity = _lockList.FirstOrDefault()?.GetComponent<IEntity>();
            if (_lockList == null || !_lockList.Any())
            {
                _lockListIndex = -1;
                _lockGO = null;
            }
            // If it's been a while since you last pressed Lock On and the closest entity is not currently selected, select it
            else if (Time.time > _lastLockPressedTime + _component.ClickNextTargetTime && _lockGO != closestLockEntity)
            {
                _lockListIndex = 0;
                _lockGO = closestLockEntity;
            }
            else // Get the next entity in the list
            {
                _lockListIndex = _lockList.IndexOf(_lockGO?.gameObject);
                _lockGO = GetIncrementTargetLock();
            }
            return _lockGO;
        }

        public void ProcessButtonUp()
        {
            //_strafePressed = false;
            //_lockGO = null;
            //_component.GetComponentRepository().Target = null;
            //_camera.AutoLockOntoTarget = false;
            ////Debug.Log($"(LockOnComponent) Setting camera to {_cameraNormalMessage}");
            //_playerController.CalculateMovementType();
            //_playerController.SetCameraBasedOnMovementType();
        }

        public IEntity GetIncrementTargetLock()
        {
            if (_lockList == null || !_lockList.Any())
            {
                _lockListIndex = 0;
                return null;
            }
            _lockListIndex++;
            // If you reach the end then revert to "Nothing locked".
            if (_lockListIndex > _lockList.Count - 1)
            {
                _lockListIndex = -1;
                return null;
            }
            return _lockList[_lockListIndex].GetComponent<IEntity>();
        }

        public void TargetLockPressed(IEntity lockGameObject)
        {
            if (_camera == null)
                _camera = UnityEngine.Camera.main.GetComponentInParent<ThirdPersonCameraController>();

            if (lockGameObject == null)
            {
                _camera.SetLockedOnTarget(null);
                //Debug.Log($"(LockOnComponent) Setting camera to {_component.CameraCenterMessage} behind player");
                //MessageDispatcher.Instance.DispatchMsg(_component.CameraCenterMessage, 0f, _component.GetComponentRepository().GetId(), _camera.GetComponentRepository().GetId(), null);
                _camera.AutoLockOntoTarget = false;
                //_centerAction?.Invoke();
                //MessageDispatcher.Instance.DispatchMsg("CenterCamera", 0f, _mainCharacter.GetId(), _camera.GetComponentRepository().GetId(), null);
                //_camera.CenterCamera(true);
            }
            else
            {
                _camera.SetLockedOnTarget(lockGameObject.gameObject);
                _camera.AutoLockOntoTarget = true;
                //closest?.Destroy();
                //MessageDispatcher.Instance.DispatchMsg(_cameraLockOnMessage, 0f, _componentRepository.GetId(), _camera.GetId(), null);
                //_lockOn?.Invoke();
            }
        }

        public void SendCameraLockOnMessage()
        {
            Debug.Log($"Sending camera message {_component.CameraLockOnMessage}");
            MessageDispatcher.Instance.DispatchMsg(_component.CameraLockOnMessage, 0f, _component.GetComponentRepository().GetId(), _camera.GetComponentRepository().GetId(), null);
        }

        public bool IsATargetLocked()
        {
            return _lockGO != null;
        }
    }
}
