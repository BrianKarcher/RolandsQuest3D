using Rewired;
using RQ.Base.Extensions;
using RQ.Common.Components;
using RQ.Messaging;
using RQ.Physics;
using System;
using UnityEngine;

namespace RQ.Controller.Player
{
    public enum MovementType
    {
        ByCamera = 0, // Normal controls - when nothing is targetted
        ByPlayer = 1 // tank controls - for when something is targetted
    }

    [RequireComponent(typeof(PlayerController))]
    [AddComponentMenu("RQ/Components/Third Person User Control")]
    public class ThirdPersonUserControl : ComponentBase<ThirdPersonUserControl>
    {
        [SerializeField] private string _targetLockAction;
        [SerializeField] private string _jumpAction;
        [SerializeField] private string _blockAction;
        [SerializeField] private string _crouchAction;
        [SerializeField] private string _sprintAction;

        //private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam; // A reference to the main camera in the scenes transform
        [SerializeField]
        [Obsolete]
        private bool _enablePlayerInput = true;
        private Rewired.Player _player;
        //private Vector3 _move; // the world-relative desired move direction, calculated from the camForward and user input.
        //private PlayerController _playerController;
        //private Vector3 m_CamForward; // The current forward direction of the camera
        private PhysicsComponent _physicsComponent;

        private long _enableInputId;
        private bool _isCrouching = false;
        [SerializeField]
        private MovementType _movementType;

        protected override void Awake()
        {
            base.Awake();
            _movementType = MovementType.ByCamera;
            if (UnityEngine.Camera.main != null)
            {
                m_Cam = UnityEngine.Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.",
                    gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }
            //_playerController = GetComponent<PlayerController>();
            _physicsComponent = GetComponent<PhysicsComponent>();
        }

        private void Start()
        {
            if (!ReInput.isReady)
            {
                Debug.LogError("Rewired not ready.");
                return;
            }
            _player = ReInput.players.Players[0];
        }

        public override void StartListening()
        {
            base.StartListening();

            // TODO Stop listening to this message at the right time
            _enableInputId = MessageDispatcher.Instance.StartListening("EnableInput", _componentRepository.GetId(), (data) =>
            {
                var enable = data.ExtraInfo.ToString();
                if (enable == "1")
                    SetEnablePlayerInput(true);
                if (enable == "0")
                    SetEnablePlayerInput(false);
            });
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher.Instance.StopListening("EnableInput", _componentRepository.GetId(), _enableInputId);
        }

        //private void Update()
        //{
        //    if (!_enablePlayerInput)
        //        return;

        //    var movementInputData = GetInputData();
        //    _playerController.PerformThirdPersonUpdate(movementInputData);
        //}

        public MovementInputData GetInputData()
        {
            var movementInputData = new MovementInputData();
            Vector2 axisInput = GetAxisInput();
            movementInputData.DirectionalInput_Raw = axisInput;
            movementInputData.DirectionalInput_CameraRelative = GetInputAxisVector(axisInput);

            if (_player == null)
            {
                movementInputData.JumpPressed = false;
            }
            else
            {
                movementInputData.JumpPressed = _player.GetButtonDown(_jumpAction);

                if (_player.GetButtonDown(_crouchAction))
                {
                    _isCrouching = !_isCrouching;
                    //_playerController.SetCrouch(_isCrouching);
                    if (_isCrouching)
                        movementInputData.TurnCrouchOn = true;
                    else
                        movementInputData.TurnCrouchOff = true;
                }
                if (_player.GetButtonDown(_sprintAction))
                {
                    MessageDispatcher.Instance.DispatchMsg("Sprint", 0f, _componentRepository.GetId(), _componentRepository.GetId(), null);
                }
                if (_player.GetButtonUp(_sprintAction))
                {
                    MessageDispatcher.Instance.DispatchMsg("EndSprint", 0f, _componentRepository.GetId(), _componentRepository.GetId(), null);
                }
            }

            return movementInputData;
        }

        public Vector3 GetInputAxisVector()
        {
            return GetInputAxisVector(GetAxisInput());
            //if (_movementType == MovementType.ByCamera)
            //    return GetCameraRelativeInputAxisVector();
            //else
            //    return GetCharacterRelativeInputAxisVector();
        }

        public Vector3 GetInputAxisVector(Vector2 axisInput)
        {
            return GetCameraRelativeInputAxisVector(axisInput);
            //if (_movementType == MovementType.ByCamera)
            //    return GetCameraRelativeInputAxisVector();
            //else
            //    return GetCharacterRelativeInputAxisVector();
        }

        private Vector3 GetCameraRelativeInputAxisVector(Vector2 axisInput)
        {


            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                var m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                return axisInput.y * m_CamForward + axisInput.x * m_Cam.right;
                //_move *= _physicsComponent.GetPhysicsData().MaxSpeed;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                return axisInput.y * Vector3.forward + axisInput.x * Vector3.right;
                //_move *= _physicsComponent.GetPhysicsData().MaxSpeed;
            }
        }

        public Vector2 GetAxisInput()
        {
            // read inputs
            if (_player == null)
            {
                return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            }
            else
            {
                return new Vector2(_player.GetAxis("Horizontal"), _player.GetAxis("Vertical"));
            }
        }

        //private Vector3 GetCharacterRelativeInputAxisVector()
        //{
        //    Vector2 axis;

        //    // read inputs
        //    if (_player == null)
        //    {
        //        axis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        //    }
        //    else
        //    {
        //        axis = new Vector2(_player.GetAxis("Horizontal"), _player.GetAxis("Vertical"));
        //    }

        //    // calculate character relative direction to move:
        //    var forwardVector = Vector3.Scale(transform.forward, new Vector3(1, 0, 1)).normalized;
        //    var force = axis.y * forwardVector + axis.x * transform.right;
        //    Debug.Log($"CharacterRelative Input Force: {force}");
        //    return force;
        //    //_move *= _physicsComponent.GetPhysicsData().MaxSpeed;
        //}

        public void SetEnablePlayerInput(bool enablePlayerInput)
        {
            _enablePlayerInput = enablePlayerInput;
            //_playerController.ApplyInput(enablePlayerInput);
        }

        public bool GetEnablePlayerInput()
        {
            return _enablePlayerInput;
            //_playerController.ApplyInput(enablePlayerInput);
        }

        public void SetMovementType(MovementType movementType)
        {
            _movementType = movementType;
        }
    }
}
