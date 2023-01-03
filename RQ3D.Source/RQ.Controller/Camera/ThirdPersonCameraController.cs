using Rewired;
using RQ.Base.Components;
using RQ.Base.Extensions;
using RQ.Common.Components;
using RQ.Messaging;
using UnityEngine;
using System.Collections.Generic;
using RQ.Common.Container;

namespace RQ.Controller.Camera
{
    [AddComponentMenu("RQ/Components/Third Person Camera Controller")]
    public class ThirdPersonCameraController : ComponentBase<ThirdPersonCameraController>
    {
        [SerializeField] [Tooltip("Link the target of the camera to follow him")] GameObject target;
        public GameObject Target => target;

        [SerializeField] private GameObject _lockOnIndicatorPrefab;

        private RotateTowardsComponent _lockOnIndicator;

        [SerializeField] private Vector3 _lockOnOffset;

        private Vector3 _targetLocalPosOffset;

        private Vector3 _currentTargetLocalPosOffset;
        //private Vector3 _currentTargetLocalPos;
        [SerializeField] private float _targetLocalPosOffsetSpeed;

        /// <summary>
        /// When using lock on, the player character will be the anchor, the lock on target will 
        /// determine camera facing direction (yaw)
        /// </summary>
        [SerializeField]
        private GameObject _lockOnTarget;

        [SerializeField]
        private LayerMask _occlusionMask;

        [SerializeField] private bool _showDebug;

        [SerializeField] private Vector3 _boundingBox;

        [SerializeField]
        [Tooltip("Other targets we will attempt to keep in the camera's view. Useful for lock-on.")]
        private GameObject[] _otherTargets;

        [SerializeField]
        private Vector3 _targetOffsetFar = new Vector3(0, 1, 0);

        [SerializeField]
        private Vector3 _targetOffsetClose = new Vector3(0, 1, 0);

        [SerializeField]
        private Vector3 _cameraPivotLocationFar;

        [SerializeField]
        private Vector3 _cameraPivotLocationClose;

        [SerializeField]
        private UnityEngine.Camera _camera;

        [SerializeField]
        private float _depthFar;

        [SerializeField]
        private float _depthClose;

        [SerializeField]
        private float _startingCameraDistance;
        public float StartingCameraDistance => _startingCameraDistance;

        //[SerializeField]
        //private Quaternion _rotation;
        private float _rotation;

        [SerializeField]
        private float _rotationSpeed = 30.0f;

        [SerializeField]
        private float _cameraDistanceSpeed = 0.5f;

        [SerializeField]
        private string _hRotateAction;

        [SerializeField]
        private string _vRotateAction;

        [SerializeField]
        private string _centerCameraAction;

        [SerializeField]
        private float _centerCameraSpeed;

        /// <summary>
        /// The higher the value, the faster the camera catches up to the target when their Y values changes, such as when jumping or falling.
        /// </summary>
        [SerializeField] private float _cameraYSpringForce;

        public float yaw;
        public float pitch;
        public float roll;

        private Vector3 _currentTargetDistanceOffset = new Vector3(0, 1, 0);
        private Quaternion _currentCameraPivotLocation;
        private float _currentDepth;

        private Vector3 _lastTargetPos;

        private Vector3 _currentTargetPos;

        /// <summary>
        /// Camera distance has a value between 0 and 1, 0 being closest and 1 being furthest. Three different variables are calculated from it.
        /// Unseralize it when done testing.
        /// </summary>
        [SerializeField]
        private float _cameraDistance;
        public float CameraDistance => _cameraDistance;

        private Rewired.Player _player;
        public Rewired.Player Player => _player;

        private RQ.Controller.Player.PlayerController _playerController;


        private float _distance;

        private bool _autoLockOntoTarget;
        public bool AutoLockOntoTarget { get => _autoLockOntoTarget; set => _autoLockOntoTarget = value; }

        //private float _currentRotation;

        protected override void Awake()
        {
            base.Awake();
            _cameraDistance = _startingCameraDistance;
        }

        private void Start()
        {
            if (!ReInput.isReady)
            {
                Debug.LogError("Rewired not ready.");
            }
            else
            {
                _player = ReInput.players.Players[0];
            }
            
            SetFromTransform(transform);
            if (target == null)
                target = EntityContainer.Instance.GetMainCharacter().gameObject;
            _currentTargetPos = target.transform.position;
            _playerController = target.GetComponent<RQ.Controller.Player.PlayerController>();
            //_rotation = _camera.transform.rotation;
            //SetCameraRotation();
        }

        public bool Raycast(float maxDistance, int layerMask, out RaycastHit hitInfo)
        {
            // Example usage:
            //     Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            // RaycastHit hit;
            // if (Physics.Raycast(ray, out hit))
            //     print("I'm looking at " + hit.transform.name);
            // else
            //     print("I'm looking at nothing!");

            Ray ray = _camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            // Create a vector at the center of our camera's viewport
            // Vector3 rayOrigin = _camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

            // var rtn = UnityEngine.Physics.Raycast(rayOrigin, _camera.transform.forward, out hitInfo, maxDistance, layerMask);
            var rtn = UnityEngine.Physics.Raycast(ray, out hitInfo, maxDistance, layerMask);
            Debug.DrawRay(ray.origin, ray.direction, Color.green, 1.0f);
            return rtn;
        }

        //public void Raycast(Vector3 origin, Vector3 direction, )
        //{
        //    _camera.ray
        //    UnityEngine.Physics.Raycast()
        //}

        public void SetFromTransform(Transform t)
        {
            pitch = t.eulerAngles.x;
            yaw = t.eulerAngles.y;
            roll = t.eulerAngles.z;
            //x = t.position.x;
            //y = t.position.y;
            //z = t.position.z;
        }

        //private void SetCameraRotation()
        //{
        //    _camera.transform.rotation = _rotation;
        //}

        //private void Update()
        //{
        //    //var rotation = _rotation.eulerAngles.y;
        //    //float rotation = 0f;

        //    //var h_delta = _player.GetAxis("Rotate Camera Horizontal");
        //    //var v_delta = _player.GetAxis("Rotate Camera Vertical");

        //    //if (_depth < _depthFar)
        //    //    _depth = _depthFar;
        //    //if (_depth > _depthClose)
        //    //    _depth = _depthClose;

        //    //yaw = -1.0f - Mathf.Log10(_depth);

        //    // 20f is the starting yaw rotation
        //    //yaw = 20f + Mathf.Abs(_depth);
        //    //yaw = 20f + Mathf.Log10(Mathf.Abs(_depth));

        //    //if (yaw > 85)
        //    //    yaw = 85;
        //    //if (yaw < -40)
        //    //    yaw = -40;

        //    //if (Input.GetKey(KeyCode.Comma))
        //    //    rotation -= _rotationSpeed * Time.deltaTime;
        //    //if (Input.GetKey(KeyCode.Period))
        //    //    rotation += _rotationSpeed * Time.deltaTime;
        //    //_rotation.SetAxisAngle(Vector3.up, _rotation);
        //    //_rotation.Set(45f, rotation, 0f, 0f);

        //    //_camera.transform.rotation = _rotation;
        //    //_camera.transform.rotation.SetAxisAngle(Vector3.up, _rotation);
        //    //transform.RotateAround(Vector3.up, rotation * Time.deltaTime);
        //    //transform.RotateAround(Vector3.up, pitch);
        //    //transform.RotateAround(Vector3.right, yaw);
        //    //transform.Rotate(new Vector3(yaw, pitch, 0f));
        //    //transform.rotation.eulerAngles.z = 0f;

        //    //transform.rotation = Quaternion.Euler(yaw, pitch, roll);

        //}

        //private CalculateCameraTransfromsBasedOnDistance()
        //{

        //}

        public Vector2 GetPlayerRotationInput()
        {
            return new Vector2(_player.GetAxis(_hRotateAction), _player.GetAxis(_vRotateAction));
        }

        //private void LateUpdate()
        //{
        //    //if (_player == null)
        //    //    return;
        //    if (target == null)
        //        return;

        //    //var playerRotationInput = GetPlayerRotationInput();

        //    //// The Camera Distance is used to calculate the position, vertical pivot, and desired depth.
        //    //CalculateCameraDistance(playerRotationInput);

        //    //SetPivotPointToTargetPosition();

        //    //if (!_isLockedOn)
        //    //{
        //    //    if (_isCenteringCamera)
        //    //    {
        //    //        ProcessRotateCameraBehindTarget();
        //    //    }

        //    //    ProcessRotateCameraBasedOnTargetMovement();
        //    //}

        //    //IncrementYaw(playerRotationInput.x);

        //    //if (_isLockedOn)
        //    //{
        //    //    RotateTowardsLockOnTarget();
        //    //}
        //    ////yaw += v * _rotationSpeed * Time.deltaTime;

        //    //SetCameraPivotRotation();

        //    //ProcessDesiredDepth();

        //    ////_camera.transform.localPosition = new Vector3(_camera.transform.localPosition.x, _camera.transform.localPosition.y,
        //    ////    _currentDepth);
        //    ////_camera.transform.localPosition = Vector3.Slerp(_camera.transform.localPosition,
        //    ////    new Vector3(_camera.transform.localPosition.x, _camera.transform.localPosition.y,
        //    ////    _currentDepth), 5f);

        //    //YawClamp();

        //    _lastTargetPos = target.transform.position;
        //}

        public void ProcessDesiredDepth()
        {
            ProcessDesiredDepth(_depthClose, _depthFar);
        }

        public void ProcessDesiredDepth(float depthClose, float depthFar)
        {
            var desiredDepth = depthClose + _cameraDistance * (depthFar - depthClose);

            var hit = CalculateDesiredDepthAfterRaycasts(desiredDepth, out desiredDepth);

            if (hit && desiredDepth > _currentDepth)
            {
                _currentDepth = desiredDepth;
            }
            else
            {
                _currentDepth = Mathf.Lerp(_currentDepth, desiredDepth, 10f * Time.deltaTime);
                //_currentDepth = Mathf.MoveTowards(_currentDepth, desiredDepth, 10f * Time.deltaTime);
            }

            _camera.transform.localPosition = new Vector3(_camera.transform.localPosition.x, _camera.transform.localPosition.y,
                _currentDepth);
        }

        /// <summary>
        /// Sets the rotation based on CameraDistance between PivotLocationFar and PivotLocationClose
        /// </summary>
        public void SetCameraPivotRotation()
        {
            var cameraPivotRotation = CalculateRotationBasedOnDistance();
            pitch = cameraPivotRotation.x;
            roll = cameraPivotRotation.z;
            //var cameraPivot = Vector3.Scale(cameraPivotRotation, new Vector3(1, 0, 1)) + new Vector3(0, yaw, 0);
            var cameraPivot = new Vector3(pitch, yaw, roll);
            _currentCameraPivotLocation = Quaternion.Euler(cameraPivot);
            transform.rotation = _currentCameraPivotLocation;
        }

        public void LerpCameraRotation(float deltaYaw, float deltaPitch, float deltaRoll)
        {
            yaw += deltaYaw;
            pitch += deltaPitch;
            roll += deltaRoll;
            var cameraPivot = new Vector3(pitch, yaw, roll);
            _currentCameraPivotLocation = Quaternion.Euler(cameraPivot);
            transform.rotation = _currentCameraPivotLocation;
        }

        public Vector3 GetRotation()
        {
            return new Vector3(pitch, yaw, roll);
        }

        public Vector3 CalculateRotationBasedOnDistance()
        {
            var cameraVerticalPivot = _cameraPivotLocationClose +
                          _cameraDistance * (_cameraPivotLocationFar - _cameraPivotLocationClose);
            return cameraVerticalPivot;
        }

        public void SetCameraRotation(Quaternion rotation)
        {
            transform.rotation = rotation;
        }

        public void IncrementYaw(float amount)
        {
            yaw += amount * _rotationSpeed * Time.deltaTime;
        }

        public void ProcessPivotPointToTargetPosition()
        {
            _currentTargetDistanceOffset = _targetOffsetClose + _cameraDistance * (_targetOffsetFar - _targetOffsetClose);

            // Lerp the vertical change
            //float newY = Mathf.Lerp(transform.position.y + _currentTargetOffset.y, target.transform.position.y, Time.deltaTime * 2f);
            // Lerp towards the targetLocalPosOffset
            //_currentTargetLocalPosOffset = Vector3.Lerp(_currentTargetLocalPosOffset, _targetLocalPosOffset,
            //    _targetLocalPosOffsetSpeed * Time.deltaTime);
            _currentTargetLocalPosOffset = Vector3.MoveTowards(_currentTargetLocalPosOffset, _targetLocalPosOffset, 
                _targetLocalPosOffsetSpeed * Time.deltaTime);

            // Convert the target offset from local space to world space. Must be transformed from the target, not the camera. The camera 
            // would add its rotation into the mix which we don't want.
            var currentTargetPosOffset = target.transform.TransformDirection(_currentTargetLocalPosOffset);
            //var currentTargetPosOffset = transform.TransformDirection(_currentTargetLocalPosOffset);
            //aasdada
            //transform.localPosition += _currentTargetLocalPosOffset;

            _currentTargetPos.y = Mathf.Lerp(_currentTargetPos.y, target.transform.position.y, Time.deltaTime * _cameraYSpringForce);

            _currentTargetPos.x = target.transform.position.x;
            _currentTargetPos.z = target.transform.position.z;
            //float newY = Mathf.Lerp(target.transform.position.y, transform.position.y, Time.deltaTime * 2f);

            // Set the new pivot position
            //transform.position = Vector3.Lerp(transform.position, new Vector3(target.transform.position.x, target.transform.position.y,
            //             target.transform.position.z)
            //         + _currentTargetOffset, Time.deltaTime * 10f);

            //transform.position = new Vector3(target.transform.position.x, newY,
            //                         target.transform.position.z)
            //                     + _currentTargetOffset;
            var newPos = _currentTargetPos + _currentTargetDistanceOffset + currentTargetPosOffset;
            transform.position = newPos;
            //_currentTargetLocalPosOffset =  Vector3.MoveTowards(_currentTargetLocalPosOffset, _targetLocalPosOffset, 1f * Time.deltaTime);

            // Debug.Log("Third Person Camera Controller Pivot Point (Immediate): " + newPos);
        }

        public void ProcessPivotPointToTargetPositionImmediate()
        {
            _currentTargetDistanceOffset = _targetOffsetClose + _cameraDistance * (_targetOffsetFar - _targetOffsetClose);

            // Lerp the vertical change
            //float newY = Mathf.Lerp(transform.position.y + _currentTargetOffset.y, target.transform.position.y, Time.deltaTime * 2f);
            // Lerp towards the targetLocalPosOffset
            //_currentTargetLocalPosOffset = Vector3.Lerp(_currentTargetLocalPosOffset, _targetLocalPosOffset,
            //    _targetLocalPosOffsetSpeed * Time.deltaTime);
            _currentTargetLocalPosOffset = _targetLocalPosOffset;
            var currentTargetPosOffset = target.transform.TransformDirection(_currentTargetLocalPosOffset);
            //_currentTargetLocalPosOffset = Vector3.MoveTowards(_currentTargetLocalPosOffset, _targetLocalPosOffset,
            //    _targetLocalPosOffsetSpeed * Time.deltaTime);

            //var currentTargetPosOffset = transform.TransformDirection(_currentTargetLocalPosOffset);
            //aasdada
            //transform.localPosition += _currentTargetLocalPosOffset;

            //_currentTargetPos.y = Mathf.Lerp(_currentTargetPos.y, target.transform.position.y, Time.deltaTime * _cameraYSpringForce);
            _currentTargetPos = target.transform.position;
            //_currentTargetPos.y = target.transform.position.y;
            //_currentTargetPos.x = target.transform.position.x;
            //_currentTargetPos.z = target.transform.position.z;
            //float newY = Mathf.Lerp(target.transform.position.y, transform.position.y, Time.deltaTime * 2f);

            // Set the new pivot position
            //transform.position = Vector3.Lerp(transform.position, new Vector3(target.transform.position.x, target.transform.position.y,
            //             target.transform.position.z)
            //         + _currentTargetOffset, Time.deltaTime * 10f);

            //transform.position = new Vector3(target.transform.position.x, newY,
            //                         target.transform.position.z)
            //                     + _currentTargetOffset;
            var newPos = _currentTargetPos + _currentTargetDistanceOffset + currentTargetPosOffset;
            transform.position = newPos;
            // Debug.Log($@"Third Person Camera Controller Pivot Point (Immediate): CurrentTargetPos: {_currentTargetPos}, CurrentTargetDistanceOffset: {_currentTargetDistanceOffset},
            // CurrentTargetPosOffset: {currentTargetPosOffset}");
            //_currentTargetLocalPosOffset =  Vector3.MoveTowards(_currentTargetLocalPosOffset, _targetLocalPosOffset, 1f * Time.deltaTime);


        }

        public void ProcessCameraPositionOffsetImmediate()
        {
            var currentTargetPosOffset = transform.TransformDirection(_currentTargetLocalPosOffset);
            var newPos = _currentTargetPos + _currentTargetDistanceOffset + currentTargetPosOffset;
            //_camera.transform.position = newPos;
            transform.position = newPos;
            Debug.Log($@"Third Person Camera Controller Pivot Point (Immediate): CurrentTargetPos: {_currentTargetPos}, CurrentTargetDistanceOffset: {_currentTargetDistanceOffset},
                CurrentTargetPosOffset: {currentTargetPosOffset}");
        }

        public void CalculateCameraDistance(Vector2 playerRotationInput)
        {
            _cameraDistance += playerRotationInput.y * _cameraDistanceSpeed * Time.deltaTime;

            _cameraDistance = Mathf.Clamp01(_cameraDistance);
        }

        /// <summary>
        /// Used by the Lock-On to make the camera face the target.
        /// </summary>
        public void RotateTowardsLockOnTarget()
        {
            if (_lockOnTarget == null)
                return;
            // Look towards the locked on target
            var destRotation = Quaternion.LookRotation(_lockOnTarget.transform.position - transform.position);
            yaw = Mathf.MoveTowardsAngle(yaw, destRotation.eulerAngles.y,
                _centerCameraSpeed * Time.deltaTime);
            ////transform.LookAt();
            ////var destRotation = Quaternion.Euler(0f, target.transform.rotation.eulerAngles.y, 0f);
            //var currentRotation = Quaternion.Euler(0f, yaw, 0f);
            ////var rotation = Quaternion.Lerp(currentRotation, destRotation, _centerCameraSpeed * Time.deltaTime);
            //var rotation = Quaternion.RotateTowards(currentRotation, destRotation, _centerCameraSpeed * Time.deltaTime);
            ////var rotateAngle = target.transform.rotation.eulerAngles.y - yaw;
            ////var rotateDirection = rotateAngle > 180f ? -1 : 1;

            //yaw += rotation.eulerAngles.y - yaw;
        }

        public void LookAtLockOnTarget()
        {
            if (_lockOnTarget == null)
                return;
            // Look towards the locked on target
            var destRotation = Quaternion.LookRotation(_lockOnTarget.transform.position - transform.position);
            //yaw = Mathf.MoveTowardsAngle(yaw, destRotation.eulerAngles.y,
            //    _centerCameraSpeed * Time.deltaTime);
            yaw = destRotation.eulerAngles.y;
            ////transform.LookAt();
            ////var destRotation = Quaternion.Euler(0f, target.transform.rotation.eulerAngles.y, 0f);
            //var currentRotation = Quaternion.Euler(0f, yaw, 0f);
            ////var rotation = Quaternion.Lerp(currentRotation, destRotation, _centerCameraSpeed * Time.deltaTime);
            //var rotation = Quaternion.RotateTowards(currentRotation, destRotation, _centerCameraSpeed * Time.deltaTime);
            ////var rotateAngle = target.transform.rotation.eulerAngles.y - yaw;
            ////var rotateDirection = rotateAngle > 180f ? -1 : 1;

            //yaw += rotation.eulerAngles.y - yaw;
        }

        public bool IsLockOnTargetAlive()
        {
            if (_lockOnTarget == null)
                return false;
            return _lockOnTarget.gameObject != null;
        }

        /// <summary>
        /// Target moving left or right adjusts the yaw slightly
        /// </summary>
        public void ProcessRotateCameraBasedOnTargetMovement()
        {
            var targetMoveWorldDirection = target.transform.position - _lastTargetPos;
            if (targetMoveWorldDirection.sqrMagnitude > 0.001f && targetMoveWorldDirection != Vector3.zero)
            {
                // Get the directional movement of the target in relation to the camera's local space
                var targetLocalSpaceDirection = transform.InverseTransformDirection(targetMoveWorldDirection);
                // Running left or right?
                if (Vector2.Angle(Vector2.right, targetLocalSpaceDirection.xz()) < 45)
                {
                    yaw += _rotationSpeed * 0.5f * Time.deltaTime;
                    // Then gradually rotate the pitch of the camera to make the camera feel more fluid.
                    //var rotateToPlayerIncrement = Vector3.Slerp(transform.forward, target.transform.forward, 3f);
                    //if (targetLocalSpaceDirection.x > 0)
                    //{

                    //}
                    //else
                    //{
                    //    pitch -= _rotationSpeed * 0.1f * Time.deltaTime;
                    //}
                    //pitch += rotateToPlayerIncrement.z;
                }
                if (Vector2.Angle(-Vector2.right, targetLocalSpaceDirection.xz()) < 45)
                {
                    yaw -= _rotationSpeed * 0.5f * Time.deltaTime;
                }
            }
            _lastTargetPos = target.transform.position;
        }

        public void ProcessRotateCameraBehindTarget()
        {
            // LerpAngle correctly handles the case when the angle wraps around 360 degrees
            //var rotation = Mathf.LerpAngle(yaw, target.transform.rotation.eulerAngles.y,
            //    10f * Time.deltaTime);

            yaw = Mathf.MoveTowardsAngle(yaw, target.transform.rotation.eulerAngles.y,
                _centerCameraSpeed * Time.deltaTime);

            _cameraDistance = Mathf.MoveTowards(_cameraDistance, _startingCameraDistance, _cameraDistanceSpeed * Time.deltaTime * 10f);

            //_cameraDistance = Mathf.Lerp(_cameraDistance, _startingCameraDistance, _cameraDistanceSpeed * Time.deltaTime * 10f);

            ////var destRotation = Quaternion.Euler(0f, target.transform.rotation.eulerAngles.y, 0f);
            ////var currentRotation = Quaternion.Euler(0f, yaw, 0f);

            ////var rotation = Quaternion.RotateTowards(currentRotation, destRotation, _centerCameraSpeed * Time.deltaTime);
            //var rotation = Quaternion.Lerp(currentRotation, destRotation, _centerCameraSpeed * Time.deltaTime);
            //var rotateAngle = target.transform.rotation.eulerAngles.y - yaw;
            //var rotateDirection = rotateAngle > 180f ? -1 : 1;

            //yaw += rotation - yaw;

            //yaw += rotateDirection * _centerCameraSpeed * Time.deltaTime;
            // Reached target pitch?

        }

        public void ProcessDesiredDepthVector(float depthClose, float depthFar)
        {
            var desiredDepth = depthClose + _cameraDistance * (depthFar - depthClose);
            var desiredPos = transform.TransformPoint(0f, 0f, desiredDepth);

            var hit = CalculateDesiredDepthAfterRaycasts(desiredPos, out desiredPos);
            // Convert to local space to get the desired depth
            var localDesiredPos = transform.InverseTransformPoint(desiredPos);

            //if (hit && desiredDepth > _currentDepth)
            if (hit && desiredDepth > localDesiredPos.z)
            {
                //_currentDepth = desiredDepth;
                _currentDepth = localDesiredPos.z;
            }
            else
            {
                //_currentDepth = Mathf.Lerp(_currentDepth, desiredDepth, 10f * Time.deltaTime);
                _currentDepth = Mathf.Lerp(_currentDepth, localDesiredPos.z, 10f * Time.deltaTime);
            }

            _camera.transform.localPosition = new Vector3(_camera.transform.localPosition.x, _camera.transform.localPosition.y,
                _currentDepth);
        }

        private bool CalculateDesiredDepthAfterRaycasts(Vector3 desiredPos, out Vector3 newDesiredPos)
        {
            var raycastHit = false;
            // Is there terrain between the player and the camera?
            //var distance = 
            //if (Physics.Raycast(target.transform.position, _camera.transform.position - target.transform.position, out var hit,
            //    Mathf.Abs(desiredDepth), _occlusionMask.value))
            //var currentDesiredDepth = desiredDepth -= 0.5f;
            //Vector3 boundingTopLeftForward = transform.TransformPoint(new Vector3(-_boundingBox.x, _boundingBox.y, currentDesiredDepth + _boundingBox.z));
            //Vector3 boundingTopRightForward = transform.TransformPoint(new Vector3(_boundingBox.x, _boundingBox.y, currentDesiredDepth + _boundingBox.z));
            //Vector3 boundingBottomLeftForward = transform.TransformPoint(new Vector3(-_boundingBox.x, -_boundingBox.y, currentDesiredDepth + _boundingBox.z));
            //Vector3 boundingBottomRightForward = transform.TransformPoint(new Vector3(_boundingBox.x, -_boundingBox.y, currentDesiredDepth + _boundingBox.z));

            //var currentDesiredDepth = desiredDepth -= 0.5f;
            Vector3 boundingCenter = _camera.transform.position;
            Vector3 boundingTopLeftForward = _camera.transform.TransformPoint(new Vector3(-_boundingBox.x, _boundingBox.y, _boundingBox.z));
            Vector3 boundingTopRightForward = _camera.transform.TransformPoint(new Vector3(_boundingBox.x, _boundingBox.y, _boundingBox.z));
            Vector3 boundingBottomLeftForward = _camera.transform.TransformPoint(new Vector3(-_boundingBox.x, -_boundingBox.y, _boundingBox.z));
            Vector3 boundingBottomRightForward = _camera.transform.TransformPoint(new Vector3(_boundingBox.x, -_boundingBox.y, _boundingBox.z));

            //Vector3 boundingTopLeftForward = _camera.transform.TransformPoint(new Vector3(-_boundingBox.x, _boundingBox.y, _boundingBox.z));
            //Vector3 boundingTopRightForward = _camera.transform.TransformPoint(new Vector3(_boundingBox.x, _boundingBox.y, _boundingBox.z));
            //Vector3 boundingBottomLeftForward = _camera.transform.TransformPoint(new Vector3(-_boundingBox.x, -_boundingBox.y, _boundingBox.z));
            //Vector3 boundingBottomRightForward = _camera.transform.TransformPoint(new Vector3(_boundingBox.x, -_boundingBox.y, _boundingBox.z));

            //Vector3 boundingTopLeftBackward = _camera.transform.TransformPoint(new Vector3(-_boundingBox.x, _boundingBox.y, -_boundingBox.z));
            //Vector3 boundingTopRightBackward = _camera.transform.TransformPoint(new Vector3(_boundingBox.x, _boundingBox.y, -_boundingBox.z));
            //Vector3 boundingBottomLeftBackward = _camera.transform.TransformPoint(new Vector3(-_boundingBox.x, -_boundingBox.y, -_boundingBox.z));
            //Vector3 boundingBottomRightBackward = _camera.transform.TransformPoint(new Vector3(_boundingBox.x, -_boundingBox.y, -_boundingBox.z));

            // Extend the casts out a little further so the camera doesn't go through the wall accidently.
            var currentDesiredPos = desiredPos;
            //currentDesiredDepth = RaycastDepthCheck(transform.TransformPoint(new Vector3(0f, 0f, currentDesiredDepth)), currentDesiredDepth); // - 0.5f) + 0.5f;
            if (RaycastDepthCheck(boundingCenter, currentDesiredPos, out currentDesiredPos))
                raycastHit = true;
            //currentDesiredDepth = RaycastDepthCheck(_camera.transform.position, currentDesiredDepth); // - 0.5f) + 0.5f;
            if (RaycastDepthCheck(boundingTopLeftForward, currentDesiredPos, out currentDesiredPos)) // - 0.5f) + 0.5f;
                raycastHit = true;
            if (RaycastDepthCheck(boundingTopRightForward, currentDesiredPos, out currentDesiredPos)) // - 0.5f) + 0.5f;
                raycastHit = true;
            if (RaycastDepthCheck(boundingBottomLeftForward, currentDesiredPos, out currentDesiredPos)) // - 0.5f) + 0.5f;
                raycastHit = true;
            if (RaycastDepthCheck(boundingBottomRightForward, currentDesiredPos, out currentDesiredPos)) // - 0.5f) + 0.5f;
                raycastHit = true;
            //desiredDepth += 0.5f;

            //desiredDepth = RaycastDepthCheck(boundingTopLeftBackward, desiredDepth);
            //desiredDepth = RaycastDepthCheck(boundingTopRightBackward, desiredDepth);
            //desiredDepth = RaycastDepthCheck(boundingBottomLeftBackward, desiredDepth);
            //desiredDepth = RaycastDepthCheck(boundingBottomRightBackward, desiredDepth);

            //if (UnityEngine.Physics.SphereCast(targetPos, 2.0f, cameraDepthTestDirection, out hit,
            //    Mathf.Abs(desiredDepth)))
            //{
            //    Debug.Log("Camera collided with terrain.");
            //    // Place camera at the collision
            //    desiredDepth = -Vector3.Distance(hit.point, target.transform.position) + .1f;

            //}
            //else
            //{
            //    //_currentDepth = Mathf.Lerp(_currentDepth, desiredDepth, 10f * Time.deltaTime);
            //    //_currentDepth = desiredDepth;
            //}
            //return Mathf.Max(currentDesiredPos, desiredPos);
            //return desiredDepth;
            newDesiredPos = currentDesiredPos;
            return raycastHit;
        }

        private bool RaycastDepthCheck(Vector3 cameraPos, Vector3 desiredCameraPos, out Vector3 newDesiredCameraPos)
        {
            bool raycastHit = false;
            var targetPos = (_componentRepository as EntityCommonComponent).GetPosition();
            var cameraDepthTestDirection = cameraPos - targetPos;
            if (_showDebug)
                Debug.DrawLine(targetPos, desiredCameraPos, Color.blue);
            //Debug.DrawLine(targetPos, targetPos + cameraDepthTestDirection.normalized * Mathf.Abs(desiredDepth), Color.blue);

            //if (_terrainCollider != null)
            //{
            //    if (_terrainCollider.Raycast(new Ray(targetPos, cameraDepthTestDirection), out var hit,
            //        Mathf.Abs(desiredDepth)))
            //    {
            //        //Debug.Log("Camera collided with terrain.");
            //        // Place camera at the collision
            //        desiredDepth = -Vector3.Distance(hit.point, target.transform.position) + 0.1f;
            //        //_currentDepth = Mathf.Lerp(_currentDepth, desiredDepth, 10f * Time.deltaTime);
            //    }
            //}
            //else
            //{
            //if (UnityEngine.Physics.Raycast(new Ray(targetPos, cameraDepthTestDirection), out var hit,
            //    Mathf.Abs(desiredDepth), _occlusionMask.value))
            if (UnityEngine.Physics.Raycast(new Ray(targetPos, cameraDepthTestDirection), out var hit,
                (targetPos - desiredCameraPos).magnitude, _occlusionMask.value))
            {
                //Debug.Log("Camera collided with terrain.");
                // Place camera at the collision
                var targetToDesiredPos = desiredCameraPos - target.transform.position;
                var shortenedTargetToDesiredPos = targetToDesiredPos.normalized * Mathf.Max(0f, hit.distance - 1.0f);

                desiredCameraPos = target.transform.position + shortenedTargetToDesiredPos;
                //desiredDepth = -Vector3.Distance(hit.point, target.transform.position) + 0.1f;
                //_currentDepth = Mathf.Lerp(_currentDepth, desiredDepth, 10f * Time.deltaTime);
                raycastHit = true;
            }
            //}
            newDesiredCameraPos = desiredCameraPos;
            return raycastHit;
        }

        private List<float> _castDepthsList = new List<float>();
        private bool CalculateDesiredDepthAfterRaycasts(float desiredDepth, out float newDesiredDepth)
        {
            bool hit = false;
            // Is there terrain between the player and the camera?
            //var distance = 
            //if (Physics.Raycast(target.transform.position, _camera.transform.position - target.transform.position, out var hit,
            //    Mathf.Abs(desiredDepth), _occlusionMask.value))
            var currentDesiredDepth = desiredDepth - 0.5f;
            // Vector3 boundingTopLeftForward = transform.TransformPoint(new Vector3(-_boundingBox.x, _boundingBox.y, currentDesiredDepth + _boundingBox.z));
            // Vector3 boundingTopRightForward = transform.TransformPoint(new Vector3(_boundingBox.x, _boundingBox.y, currentDesiredDepth + _boundingBox.z));
            // Vector3 boundingBottomLeftForward = transform.TransformPoint(new Vector3(-_boundingBox.x, -_boundingBox.y, currentDesiredDepth + _boundingBox.z));
            // Vector3 boundingBottomRightForward = transform.TransformPoint(new Vector3(_boundingBox.x, -_boundingBox.y, currentDesiredDepth + _boundingBox.z));

            //Vector3 boundingTopLeftForward = _camera.transform.TransformPoint(new Vector3(-_boundingBox.x, _boundingBox.y, _boundingBox.z));
            //Vector3 boundingTopRightForward = _camera.transform.TransformPoint(new Vector3(_boundingBox.x, _boundingBox.y, _boundingBox.z));
            //Vector3 boundingBottomLeftForward = _camera.transform.TransformPoint(new Vector3(-_boundingBox.x, -_boundingBox.y, _boundingBox.z));
            //Vector3 boundingBottomRightForward = _camera.transform.TransformPoint(new Vector3(_boundingBox.x, -_boundingBox.y, _boundingBox.z));

            //Vector3 boundingTopLeftBackward = _camera.transform.TransformPoint(new Vector3(-_boundingBox.x, _boundingBox.y, -_boundingBox.z));
            //Vector3 boundingTopRightBackward = _camera.transform.TransformPoint(new Vector3(_boundingBox.x, _boundingBox.y, -_boundingBox.z));
            //Vector3 boundingBottomLeftBackward = _camera.transform.TransformPoint(new Vector3(-_boundingBox.x, -_boundingBox.y, -_boundingBox.z));
            //Vector3 boundingBottomRightBackward = _camera.transform.TransformPoint(new Vector3(_boundingBox.x, -_boundingBox.y, -_boundingBox.z));

            // Extend the casts out a little further so the camera doesn't go through the wall accidently.

            // Filling an array is more memory efficient than creating a new 
            //int depthHitCounter = 0;
            // _castDepthsList.Clear();

            float tempDepth;
            // SphereCast doesn't work for really close terrain, so use a Raycast also and return closest.
            if (RaycastDepthCheck(transform.TransformPoint(new Vector3(0f, 0f, desiredDepth - 0.5f)), desiredDepth - 0.5f, out tempDepth)) // - 0.5f) + 0.5f;
            {
                currentDesiredDepth = tempDepth;
                hit = true;
            }
                
            // //currentDesiredDepth = RaycastDepthCheck(_camera.transform.position, currentDesiredDepth); // - 0.5f) + 0.5f;
            // if (RaycastDepthCheck(boundingTopLeftForward, currentDesiredDepth, out currentDesiredDepth)) // - 0.5f) + 0.5f;
            //     hit = true;
            // if (RaycastDepthCheck(boundingTopRightForward, currentDesiredDepth, out currentDesiredDepth)) // - 0.5f) + 0.5f;
            //     hit = true;
            // if (RaycastDepthCheck(boundingBottomLeftForward, currentDesiredDepth, out currentDesiredDepth)) // - 0.5f) + 0.5f;
            //     hit = true;
            // if (RaycastDepthCheck(boundingBottomRightForward, currentDesiredDepth, out currentDesiredDepth)) // - 0.5f) + 0.5f;
            //     hit = true;
            //desiredDepth += 0.5f;

            if (SphereCastDepthCheck(_camera.transform.position, desiredDepth - 0.5f, 0.5f, out tempDepth))
            {
                if (tempDepth > currentDesiredDepth)
                {
                    currentDesiredDepth = tempDepth;
                }
                hit = true;
            }

            //desiredDepth = RaycastDepthCheck(boundingTopLeftBackward, desiredDepth);
            //desiredDepth = RaycastDepthCheck(boundingTopRightBackward, desiredDepth);
            //desiredDepth = RaycastDepthCheck(boundingBottomLeftBackward, desiredDepth);
            //desiredDepth = RaycastDepthCheck(boundingBottomRightBackward, desiredDepth);

            //if (UnityEngine.Physics.SphereCast(targetPos, 2.0f, cameraDepthTestDirection, out hit,
            //    Mathf.Abs(desiredDepth)))
            //{
            //    Debug.Log("Camera collided with terrain.");
            //    // Place camera at the collision
            //    desiredDepth = -Vector3.Distance(hit.point, target.transform.position) + .1f;

            //}
            //else
            //{
            //    //_currentDepth = Mathf.Lerp(_currentDepth, desiredDepth, 10f * Time.deltaTime);
            //    //_currentDepth = desiredDepth;
            //}

            // Return the closest point to the player
            newDesiredDepth = Mathf.Max(currentDesiredDepth, desiredDepth);
            //return desiredDepth;
            return hit;
        }
        
        private bool RaycastDepthCheck(Vector3 cameraPos, float desiredDepth, out float newDesiredDepth)
        {
            bool hit = false;
            var targetPos = (_componentRepository as EntityCommonComponent).GetPosition();
            var cameraDepthTestDirection = cameraPos - targetPos;
            if (_showDebug)
                Debug.DrawLine(targetPos, targetPos + cameraDepthTestDirection.normalized * Mathf.Abs(desiredDepth), Color.blue);

            //if (_terrainCollider != null)
            //{
            //    if (_terrainCollider.Raycast(new Ray(targetPos, cameraDepthTestDirection), out var hit,
            //        Mathf.Abs(desiredDepth)))
            //    {
            //        //Debug.Log("Camera collided with terrain.");
            //        // Place camera at the collision
            //        desiredDepth = -Vector3.Distance(hit.point, target.transform.position) + 0.1f;
            //        //_currentDepth = Mathf.Lerp(_currentDepth, desiredDepth, 10f * Time.deltaTime);
            //    }
            //}
            //else
            //{
            if (UnityEngine.Physics.Raycast(new Ray(targetPos, cameraDepthTestDirection), out var hitInfo,
                Mathf.Abs(desiredDepth), _occlusionMask.value))
            {
                //Debug.Log("Camera collided with terrain.");
                // Place camera at the collision
                desiredDepth = -Vector3.Distance(hitInfo.point, targetPos) + 0.1f;
                //_currentDepth = Mathf.Lerp(_currentDepth, desiredDepth, 10f * Time.deltaTime);
                hit = true;
            }
            //}
            newDesiredDepth = desiredDepth;
            //return desiredDepth;
            return hit;
        }

        Collider[] _colliderTemp = new Collider[10];
        private bool SphereCastDepthCheck(Vector3 cameraPos, float desiredDepth, float radius, out float newDesiredDepth)
        {
            bool hit = false;
            var targetPos = (_componentRepository as EntityCommonComponent).GetPosition();
            var cameraDepthTestDirection = cameraPos - targetPos;
            if (_showDebug)
                Debug.DrawLine(targetPos, targetPos + cameraDepthTestDirection.normalized * Mathf.Abs(desiredDepth), Color.blue);

            //if (_terrainCollider != null)
            //{
            //    if (_terrainCollider.Raycast(new Ray(targetPos, cameraDepthTestDirection), out var hit,
            //        Mathf.Abs(desiredDepth)))
            //    {
            //        //Debug.Log("Camera collided with terrain.");
            //        // Place camera at the collision
            //        desiredDepth = -Vector3.Distance(hit.point, target.transform.position) + 0.1f;
            //        //_currentDepth = Mathf.Lerp(_currentDepth, desiredDepth, 10f * Time.deltaTime);
            //    }
            //}
            //else
            //{

            // int count = UnityEngine.Physics.OverlapSphereNonAlloc(targetPos, radius, _colliderTemp, _occlusionMask.value);
            // for (int i = 0; i < count; i++)
            // {

            // }
            // if (UnityEngine.Physics.OverlapSphere((targetPos, radius, _occlusionMask.value))

            if (UnityEngine.Physics.SphereCast(targetPos, radius, cameraDepthTestDirection, out var hitInfo, 
                Mathf.Abs(desiredDepth), _occlusionMask.value))
            {
                desiredDepth = -Vector3.Distance(hitInfo.point, target.transform.position) + 0.3f;
                hit = true;
            }

            // if (UnityEngine.Physics.SphereCast(targetPos, radius, cameraDepthTestDirection, out var hitInfo, 
            //     Mathf.Abs(desiredDepth), _occlusionMask.value))
            // {
            //     desiredDepth = -Vector3.Distance(hitInfo.point, target.transform.position) + 0.3f;
            //     hit = true;
            // }
            // if (UnityEngine.Physics.Raycast(new Ray(targetPos, cameraDepthTestDirection), out var hitInfo,
            //     Mathf.Abs(desiredDepth), _occlusionMask.value))
            // {
            //     //Debug.Log("Camera collided with terrain.");
            //     // Place camera at the collision
            //     desiredDepth = -Vector3.Distance(hitInfo.point, target.transform.position) + 0.1f;
            //     //_currentDepth = Mathf.Lerp(_currentDepth, desiredDepth, 10f * Time.deltaTime);
            //     hit = true;
            // }
            //}
            newDesiredDepth = desiredDepth;
            //return desiredDepth;
            return hit;
        }

        public void YawClamp()
        {
            if (yaw > 180)
                yaw -= 360;
            if (yaw < -180)
                yaw += 360;
        }

        public void SetLockedOnTarget(GameObject lockOnTarget)
        {
            var lockOnIndicator = GenerateLockedOnIndicator();

            lockOnIndicator.gameObject.SetActive(lockOnTarget != null);
            lockOnIndicator.SetTarget(_camera.gameObject);
            lockOnIndicator.transform.parent = lockOnTarget?.transform;
            lockOnIndicator.transform.localPosition = Vector3.zero;
            if (lockOnTarget != null)
            {
                Debug.Log($"(ThirdPersonCameraController) Locking into {lockOnTarget.name}");
                //MessageDispatcher.Instance.DispatchMsg("LockOn", 0f, this.GetId(), _componentRepository.GetId(), null);
            }
            //_isLockedOn = target != null;
            _lockOnTarget = lockOnTarget;
        }

        public GameObject GetLockedOnTarget()
        {
            return _lockOnTarget;
        }

        private RotateTowardsComponent GenerateLockedOnIndicator()
        {
            if (_lockOnIndicator == null)
                _lockOnIndicator = GameObject.Instantiate(_lockOnIndicatorPrefab).GetComponent<RotateTowardsComponent>();
            _lockOnIndicator.gameObject.SetActive(false);
            return _lockOnIndicator;
        }

        public RotateTowardsComponent GetLockedOnIndicator()
        {
            return _lockOnIndicator;
        }

        public void EnableLockOn(bool enable)
        {
            if (_lockOnIndicator == null || _lockOnIndicator.gameObject == null)
                return;
            _lockOnIndicator.gameObject.SetActive(enable);
        }

        public float GetYaw()
        {
            return yaw;
        }

        public void SetDepthFar(float depthFar)
        {
            _depthFar = depthFar;
        }

        public float GetDepthFar()
        {
            return _depthFar;
        }

        public void SetDepthClose(float depthClose)
        {
            _depthClose = depthClose;
        }

        public float GetDepthClose()
        {
            return _depthClose;
        }

        public void SetTargetLocalPosOffset(Vector3 localOffset)
        {
            _targetLocalPosOffset = localOffset;
        }

        public Vector3 GetTargetLocalPosOffset()
        {
            return _targetLocalPosOffset;
        }

        public Vector3 GetLockOnOffset()
        {
            return _lockOnOffset;
        }

        //public void SetAutoLockOntoTarget(bool autoLockOntoTarget)
        //{
        //    _autoLockOntoTarget = autoLockOntoTarget;
        //}

        //public bool GetAutoLockOntoTarget()
        //{
        //    return _autoLockOntoTarget;
        //}
    }
}
