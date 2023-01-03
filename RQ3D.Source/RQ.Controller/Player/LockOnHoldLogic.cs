using Assets.RQ_Assets.Scripts.Components;
using RQ.Controller.Camera;
using RQ.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RQ.Controller.Player
{
    [Serializable]
    public class LockOnHoldLogic : ILockOnLogic
    {
        [SerializeField]
        private GameObject _lockGO;
        private LockOnComponent _component;
        private bool _strafePressed;
        public bool IsStrafePressed() => _strafePressed;
        private PlayerController _playerController;
        private List<GameObject> _lockList = null;
        private int _lockListIndex = 0;
        private ThirdPersonUserControl _thirdPersonUserControl;
        private float _lastLockReleaseTime;
        private ThirdPersonCameraController _camera;

        public LockOnHoldLogic(LockOnComponent component, PlayerController playerController)
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
            _strafePressed = true;
            _lockGO = null;
            // Determine whether to get the next target, or to create the list and get the first target.

            // Button pressed within the alotted time? Select the next target in the previously generated list
            if (_lockList != null && _lockList.Count != 0 && Time.time - _lastLockReleaseTime < _component.ClickNextTargetTime)
            {
                Debug.Log("Getting next target since within time limit.");
                _lockGO = GetIncrementTargetLock();
            }
            else
            {
                _lockList = (List<GameObject>)_component.CreateTargetList();
                if (_lockList != null && _lockList.Count != 0)
                {
                    _lockListIndex = 0;
                    _lockGO = _lockList[_lockListIndex];
                }
            }
            if (_lockGO == null)
            {
                //MessageDispatcher.Instance.DispatchMsg(_cameraOverShoulderMessage, 0f, _componentRepository.GetId(), _camera.GetComponentRepository().GetId(), null);
                _thirdPersonUserControl.SetMovementType(MovementType.ByCamera);
                _playerController.CalculateMovementType();
                //_playerController.SetCameraBasedOnMovementType();
            }
            else
            {
                //Debug.Log($"(LockOnComponent) Setting camera to {_cameraLockOnMessage}");
                SendCameraLockOnMessage();
                _thirdPersonUserControl.SetMovementType(MovementType.ByPlayer); // Lock on - tank controls
                _playerController.CalculateMovementType();
                //_playerController.SetCameraBasedOnMovementType();
            }
            TargetLockPressed(_lockGO);
            _component.GetComponentRepository().Target = _lockGO;
        }

        public void ProcessButtonUp()
        {
            _strafePressed = false;
            _lockGO = null;
            _component.GetComponentRepository().Target = null;
            _camera.AutoLockOntoTarget = false;
            //Debug.Log($"(LockOnComponent) Setting camera to {_cameraNormalMessage}");
            _playerController.CalculateMovementType();
            //_playerController.SetCameraBasedOnMovementType();
        }

        public GameObject GetIncrementTargetLock()
        {
            if (_lockList == null)
                return null;
            _lockListIndex++;
            if (_lockListIndex > _lockList.Count - 1)
                _lockListIndex = 0;
            return _lockList[_lockListIndex];
        }

        public void TargetLockPressed(GameObject lockGameObject)
        {
            if (_camera == null)
                _camera = UnityEngine.Camera.main.GetComponentInParent<ThirdPersonCameraController>();

            if (lockGameObject == null)
            {
                _camera.SetLockedOnTarget(null);
                Debug.Log($"(LockOnComponent) Setting camera to {_component.CameraCenterMessage} behind player");
                MessageDispatcher.Instance.DispatchMsg(_component.CameraCenterMessage, 0f, _component.GetComponentRepository().GetId(), _camera.GetComponentRepository().GetId(), null);
                _camera.AutoLockOntoTarget = false;
                //_centerAction?.Invoke();
                //MessageDispatcher.Instance.DispatchMsg("CenterCamera", 0f, _mainCharacter.GetId(), _camera.GetComponentRepository().GetId(), null);
                //_camera.CenterCamera(true);
            }
            else
            {
                _camera.SetLockedOnTarget(lockGameObject);
                _camera.AutoLockOntoTarget = true;
                //closest?.Destroy();
                //MessageDispatcher.Instance.DispatchMsg(_cameraLockOnMessage, 0f, _componentRepository.GetId(), _camera.GetId(), null);
                //_lockOn?.Invoke();
            }
        }

        public void SendCameraLockOnMessage()
        {
            MessageDispatcher.Instance.DispatchMsg(_component.CameraLockOnMessage, 0f, _component.GetComponentRepository().GetId(), _camera.GetComponentRepository().GetId(), null);
        }

        public bool IsATargetLocked()
        {
            return _lockGO != null;
        }
    }
}
