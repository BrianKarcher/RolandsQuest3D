using Assets.RQ_Assets.Scripts.Components;
using Rewired;
using RQ.Base.Components;
using RQ.Base.Extensions;
using RQ.Base.Item;
using RQ.Base.Skill;
using RQ.Common.Components;
using RQ.Controller.Camera;
using RQ.Controller.Component;
using RQ.Messaging;
using RQ.Physics;
using System;
using System.Collections;
using UnityEngine;

namespace RQ.Controller.Player
{
    //[Serializable]
    //public class StaminaCosts
    //{
    //    [SerializeField]
    //    private float _sidehopCost;
    //    public float SidehopCost => _sidehopCost;

    //    [SerializeField]
    //    private float _backflipCost;
    //    public float BackflipCost => _backflipCost;

    //    [SerializeField]
    //    private float _deflectCost;
    //    public float DeflectCost => _deflectCost;

    //    [SerializeField]
    //    private float _blockCost;
    //    public float BlockCost => _blockCost;
    //}

    [AddComponentMenu("RQ/Components/Player Controller")]
    [RequireComponent(typeof(PhysicsComponent))]
    [RequireComponent(typeof(Animator))]
    public class PlayerController : ComponentBase<PlayerController>
    {
        [SerializeField] private string _runAnim;
        [SerializeField] private string _turnSpeedAnim;
        [SerializeField] private string _forwardSpeedAnim;
        [SerializeField] private string _verticalSpeedAnim;
        [SerializeField]
        private string _sideSpeedAnim;
        //public string SideSpeedAnim => _sideSpeedAnim;
        [SerializeField]
        private string _strafeAnim;
        public string StrafeAnim => _strafeAnim;

        //[SerializeField]
        //private StaminaCosts _staminaCosts;
        //public StaminaCosts StaminaCosts => _staminaCosts;

        [SerializeField] private string _crouchAnimTrigger;
        [Obsolete]
        [SerializeField] private float _crouchMultiplier = 0.3f;
        [SerializeField] private LayerMask _targetLockLayers;
        [SerializeField] private LayerMask _terrainLayer;
        [SerializeField] private float _targetLockRadius = 5;
        public float TargetLockRadius => _targetLockRadius;
        [SerializeField] private float _targetLockDistance = 40;
        [SerializeField] private string _targetLockAction;
        [Obsolete]
        [SerializeField] float _groundCheckDistance = 0.1f;
        [Obsolete]
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private float _jumpDistance;
        [SerializeField] private bool _debug;
        [SerializeField] private GameObject _rightArmShoulderForIK;
        public GameObject RightArmShoulderForIK => _rightArmShoulderForIK;
        [SerializeField] private ShooterComponent _shooter;
        public ShooterComponent Shooter => _shooter;
        [SerializeField]
        private Renderer[] _meshRenderers;

        [Tooltip("Used to position the player when he grabs onto a cliff/wall.")]
        [SerializeField]
        private GameObject _handGameObject;
        public GameObject HandGameObject => _handGameObject;        

        private PhysicsComponent _physicsComponent;

        private Animator _animator;

        //private bool m_Jump; 
        
        private bool _targetLockMode = false;        

        private Rigidbody _rigidBody3D;

        // TODO Unseralize this!
        [SerializeField]
        private Vector3 _targetVelocity;

        //[SerializeField]
        //[Obsolete]
        //private bool _applyInput = true;

        // TODO Unseralize this!
        [SerializeField]
        private bool _isCrouching = false;

        public bool AutoRotateSameAsCamera { get; set; }

        private LockOnComponent _lockOnComponent;

        private long _itemAddedId;
        private OverShoulderMovementLogic _overShoulderLogic;
        private LockOnMovementLogic _lockOnLogic;
        private ThirdPersonMovementLogic _thirdPersonLogic;

        private ThirdPersonUserControl _thirdPersonUserControl;
        [SerializeField]
        private bool _isMovementActive;

        [SerializeField]
        private string _cameraNormalMessage;
        [SerializeField]
        private string _cameraLockedOnMessage;
        [SerializeField]
        private string _cameraCenterMessage;
        [SerializeField]
        private string _cameraOverShoulderMessage;
        private ThirdPersonCameraController _camera;

        [SerializeField]
        private float _cliffCheckForward;
        [SerializeField]
        private float _cliffCheckDown;

        [SerializeField]
        private float _hangingCheckOffsetHasTerrain;
        [SerializeField]
        private float _hangingCheckOffsetHasTerrainDepth;
        [SerializeField]
        private float _hangingCheckOffsetHasNoTerrain;
        [SerializeField]
        private float _hangingCheckOffsetHasNoTerrainDepth;
        [SerializeField]
        private Vector3 _aboveStandingLocationOffset;
        [SerializeField]
        private bool _canHang;
        public bool CanHang { get => _canHang; set => _canHang = value; }

        private float _hangCliffHeight;
        public float HangCliffHeight => _hangCliffHeight;

        private Vector3 _hangCliffNormal;
        public Vector3 HangCliffNormal => _hangCliffNormal;

        //private bool _isStrafeTriggered = false;

        //[SerializeField]
        //private EntityCommonComponent ShooterGO;

        public enum MovementLogic
        {
            None = 0,
            ThirdPerson = 1,
            OverShoulder = 2,
            Strafe = 3
        }

        private MovementLogic _skillMovementLogic;

        //private Rewired.Player _player;

        //private Vector3 _velocity;

        //private Coroutine _senseSurroundingsCoroutine;
        private Rewired.Player _rewiredPlayer;

        protected override void Awake()
        {
            base.Awake();
            _overShoulderLogic = new OverShoulderMovementLogic(this);
            _thirdPersonLogic = new ThirdPersonMovementLogic(this);
            _lockOnLogic = new LockOnMovementLogic(this);
            if (_thirdPersonUserControl == null)
                _thirdPersonUserControl = GetComponent<ThirdPersonUserControl>();
            _isMovementActive = false;
            if (_lockOnComponent == null)
                _lockOnComponent = GetComponent<LockOnComponent>();
            if (_camera == null)
                _camera = GameObject.FindObjectOfType<ThirdPersonCameraController>();
            // get the transform of the main camera


            // get the third person character ( this should never be null due to require component )
            _physicsComponent = GetComponent<PhysicsComponent>();
            _animator = GetComponent<Animator>();
            //_animator.applyRootMotion = true;
            _rigidBody3D = GetComponent<Rigidbody>();
        }

        protected void Start()
        {
            if (_rewiredPlayer == null)
                _rewiredPlayer = ReInput.players.GetPlayer(0);
            //_player = ReInput.players.Players[0];
            //_senseSurroundingsCoroutine = StartCoroutine(SenseSurroundings());
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            //StopCoroutine(_senseSurroundingsCoroutine);
        }

        private MovementInputData _movementData;

        private void Update()
        {
            _movementData.Clear();
        }

        // private void LateUpdate()
        // {
        //     // TODO: This is slow
        //     if (_currentMovementLogic is OverShoulderMovementLogic)
        //     {
        //         LookAtTarget();
        //     }
        // }

        public override void StartListening()
        {
            base.StartListening();
            _itemAddedId = MessageDispatcher.Instance.StartListening("ItemAdded", _componentRepository.GetId(), (data) =>
            {                
                var itemConfigAndCount = (ItemDesc)data.ExtraInfo;
                Debug.Log($"{itemConfigAndCount.ItemConfig.name}({itemConfigAndCount.Qty}) added to player inventory");
                if (itemConfigAndCount.ItemConfig.ItemType == ItemTypeEnum.Shard)
                {
                    // If the player acquired a Shard let the UI know so it can update
                    MessageDispatcher.Instance.DispatchMsg("SetShardCount", 0f, _componentRepository.GetId(), "Hud Controller", itemConfigAndCount);
                }
                if (itemConfigAndCount.ItemConfig.ItemType == ItemTypeEnum.Mold)
                {
                    // If the player acquired a Mold let the UI know so it can update
                    MessageDispatcher.Instance.DispatchMsg("AddMold", 0f, _componentRepository.GetId(), "Hud Controller", itemConfigAndCount.ItemConfig);
                }
            });
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher.Instance.StopListening("ItemAdded", _componentRepository.GetId(), _itemAddedId);
        }

        /// <summary>
        /// Check for cliff edges, etc.
        /// </summary>
        /// <returns></returns>
        //private IEnumerator SenseSurroundings()
        //{
        //    while (true)
        //    {
        //        //Debug.LogWarning("Sensing surroundings to player");
        //        //DetectNearCliff();

        //        //AirborneDetectGrabbableHangSpot();

        //        // TODO: Run at 10 FPS
        //        //yield return new WaitForSeconds(1.0f);
        //        yield return new WaitForSeconds(0.1f);
        //    }
        //}

        public bool AirborneDetectGrabbableHangSpot(float delay)
        {
            //if (_movementData == null)
            //    return;

            // Gotta be airborne
            if (_physicsComponent.Controller.GetIsGrounded())
            {
                //Debug.Log("No HangSpot check - Is Grounded");
                return false;
            }

            var verticalVelocity = _physicsComponent.GetVelocity3().y;

            // Don't grab on if going up or not falling
            if (verticalVelocity >= 0)
            {
                //Debug.Log("No HangSpot check - Velocity >= 0");
                return false;
            }

            var localMove = transform.InverseTransformDirection(_movementData.DirectionalInput_CameraRelative);

            // If player is not pressing forward relative to the player controller, cannot grab onto ledge.
            if (localMove.z <= 0.25f)
                return false;

            var midPos = _componentRepository.GetPosition();
            var midCastStart = midPos + new Vector3(0f, _hangingCheckOffsetHasTerrain, 0);

            //RaycastHit cliffCollider;
            var isCliffInFrontOfPlayer = UnityEngine.Physics.Raycast(midCastStart, transform.forward, out var cliffCollider, _hangingCheckOffsetHasTerrainDepth, _terrainLayer);
            Debug.DrawLine(midCastStart, midCastStart + (transform.forward * _hangingCheckOffsetHasTerrainDepth), isCliffInFrontOfPlayer ? Color.red : Color.green, delay);
            //Debug.Log($"Cliff in front of player? {isCliffInFrontOfPlayer}");
            if (!isCliffInFrontOfPlayer)
                return false;
            // The collider hit needs to be vertical to the world.
            var cliffToWorldDot = Vector3.Dot(Vector3.up, cliffCollider.normal);
            //if (cliffDot > -0.95f && cliffDot < 0.95f)
            if (cliffToWorldDot < -0.05f || cliffToWorldDot > 0.05f)
            {
                Debug.Log($"Cliff is not vertical - Normal Dot {cliffToWorldDot}");
                return false;
            }

            // Player needs to be facing the cliff
            var cliffToPlayerDot = Vector2.Dot(transform.forward.xz(), cliffCollider.normal.xz());
            if (cliffToPlayerDot > -0.7f)
            {
                Debug.Log($"Player is not facing cliff - player forward {transform.forward}, cliff normal {cliffCollider.normal} - Player-Cliff Normal Dot {cliffToPlayerDot}");
                return false;
            }

            // The collider hit needs to be vertical to the world.
            // Which means the normal needs to be horizontal.
            //if (cliffCollider.normal.y < -0.1f || cliffCollider.normal.y > -0.1f)
            //{
            //    Debug.Log("Cliff is not vertical");
            //    return;
            //}
            //Debug.Log("Cliff is vertical");
            // TODO Check to see if there is room to initiate a hang - another Raycast most likely looking for no terrain.
            
            // Make sure the area above the previous wall has no terrain - a place for the player to climb onto.
            var aboveMidCastStart = midPos + new Vector3(0f, _hangingCheckOffsetHasNoTerrain, 0);
            var isCliffInFrontOfHigherPlayer = UnityEngine.Physics.Raycast(aboveMidCastStart, transform.forward, out var higherCliffCollider, _hangingCheckOffsetHasNoTerrainDepth, _terrainLayer);

            Debug.DrawLine(aboveMidCastStart, aboveMidCastStart + (transform.forward * _hangingCheckOffsetHasNoTerrainDepth), isCliffInFrontOfHigherPlayer ? Color.red : Color.green, delay);
            //Debug.Log($"Open area on top in front of player? {!isCliffInFrontOfPlayer}");

            if (isCliffInFrontOfHigherPlayer)
                return false;

            // Make sure the player can stand on top of the section above.            
            var aboveStandingLocation = _componentRepository.GetHeadPosition() + transform.TransformDirection(_aboveStandingLocationOffset);
            //var aboveStandingLocation = _componentRepository.GetHeadPosition() + transform.TransformDirection(new Vector3(0f, 1.0f, 1.5f));
            var isPossibleStandingCollider = UnityEngine.Physics.Raycast(aboveStandingLocation, Vector3.down, out var standingCollider, 2.0f, _terrainLayer);

            Debug.DrawLine(aboveStandingLocation, aboveStandingLocation + (Vector3.down * 2.0f), isPossibleStandingCollider ? Color.red : Color.green, delay);
            //Debug.Log($"Is a possible standing collider above? {isPossibleStandingCollider}");

            // if normal y is near 1, then it is horizontal
            var standingNormalY = standingCollider.normal.y;

            //Debug.Log($"Standing normal Y: {standingNormalY}");
            //Debug.Log($"Standing Normal = {standingCollider.normal}");
            var standingNormalDot = Vector3.Dot(standingCollider.normal, Vector3.up);
            //Debug.Log($"Standing Normal Dot = {standingNormalDot}");
            // standingNormalDot should be near zero if collider is horizontal
            //if (standingNormalDot < -0.05f || standingNormalDot > 0.05f)
            //    return false;
            if (Mathf.Abs(standingNormalDot) < 0.95f)
            {
                Debug.Log($"Standing platform is not horizontal - Normal Dot {standingNormalDot}");
                return false;
            }

            Debug.Log($"Hang cliff height: {standingCollider.point.y}");
            _hangCliffHeight = standingCollider.point.y;
            _hangCliffNormal = cliffCollider.normal;
            Debug.LogWarning("Can Hang: Tada!");
            //throw new NotImplementedException();
            return true;
        }

        private void DetectNearCliff()
        {
            var floorPos = _componentRepository.GetFootPosition();
            var floorCastStart = floorPos + new Vector3(0f, 0.1f, 0);
            var floorCastForward = floorCastStart + (transform.forward * _cliffCheckForward);
            var bottomDepthCastPos = floorCastForward + (Vector3.down * _cliffCheckDown);

            if (_physicsComponent.Controller.GetIsGrounded())
            {
                // Check if there is a cliff in front of the player
                bool isTerrainInFront = false;
                bool isTerrainDown = false;
                // No terrain directly in front of you?
                if (UnityEngine.Physics.Raycast(floorCastStart, transform.forward, 0.75f, _terrainLayer))
                {
                    isTerrainInFront = true;
                    Debug.LogWarning("Wall in front of player.");
                }
                else
                {
                    // Is it a large enough gap?
                    if (UnityEngine.Physics.Raycast(floorCastForward, Vector3.down, 1f, _terrainLayer))
                    {
                        isTerrainDown = true;
                        Debug.LogWarning("Not enough room to hang.");
                    }
                    else
                    {
                        Debug.LogWarning("Cliff in front of player.");
                    }
                }
                Debug.DrawLine(floorCastStart, floorCastForward, isTerrainInFront ? Color.red : Color.green, 1.0f);
                Debug.DrawLine(floorCastForward, bottomDepthCastPos, isTerrainDown ? Color.red : Color.green, 1.0f);
            }
        }

        public void SetTargetVelocity(Vector3 velocity)
        {
            _targetVelocity = velocity;
        }

        public void SetRotation(float yaw)
        {
            transform.rotation =
                Quaternion.Euler(transform.rotation.eulerAngles.x, yaw,
                transform.rotation.eulerAngles.z);
        }

        //public void UpdateAnimator(float forwardAmount, float turnAmount, float sideAmount)
        //{
        //    _animator.SetFloat(_turnSpeedAnim, turnAmount);
        //    _animator.SetFloat(_sideSpeedAnim, sideAmount);
        //    // If the character is not moving - say it collides, then there is no forward amount to animate.
        //    forwardAmount = _rigidBody3D.velocity.sqrMagnitude < 0.0001f ? 0f : forwardAmount;
        //    //Debug.Log($"ForwardAmount: {_forwardAmount}");
        //    //Debug.Log($"Velocity: {_rigidBody3D.velocity.sqrMagnitude}");
        //    //Debug.Log($"NewForwardAmount: {forwardAmount}");
        //    _animator.SetFloat(_forwardSpeedAnim, forwardAmount);
        //    // VerticleSpeed is an airbone attribute.
        //    if (_physicsComponent.Controller.GetIsGrounded())
        //        _animator.SetFloat(_verticalSpeedAnim, 0f);
        //    else
        //        _animator.SetFloat(_verticalSpeedAnim, _rigidBody3D.velocity.y);
        //}

        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            if (!_isMovementActive)
                return;
            // Player can only apply velocity on the xz plane
            // Strip out y velocity

            //var deltaVelocity = targetVelocity - currentVelocity;
            //_physicsComponent.AddForce(deltaVelocity, ForceMode.VelocityChange);
            //_physicsComponent.SetVelocity3(_targetVelocity);
            //CheckGroundStatus();

            _canHang = AirborneDetectGrabbableHangSpot(Time.fixedDeltaTime);

            if (_physicsComponent.Controller.GetIsGrounded())
            {
                _physicsComponent.SetVelocity2(_targetVelocity.xz());
                //AddForce();
            }
            else
            {
                var currentVelocity = _physicsComponent.GetVelocity3().xz();
                var deltaVelocity = _targetVelocity - currentVelocity.xz();
                _physicsComponent.AddForce(deltaVelocity);
            }
            // Add gravity
            //_physicsComponent.AddForce(new Vector3(0, -29.8f, 0));

            // Set Velocity 2 allows gravity to still take effect


            // pass all parameters to the character control script
            //m_Character.Move(m_Move, crouch, m_Jump);
            //m_Jump = false;
            //UpdateAnimator();
        }

        public void SetTint(string namedColor, Color tint)
        {
            Debug.Log($"Setting tint of {namedColor} to {tint.ToString()} ");
            for (int p = 0; p < _meshRenderers.Length; p++)
            {
                for (int i = 0; i < _meshRenderers[p].materials.Length; i++)
                {
                    _meshRenderers[p].materials[i].SetColor(namedColor, tint);
                }
            }
            //_meshRenderer.material.SetColor(namedColor, tint);
        }

        public Color GetTint(string namedColor)
        {
            return _meshRenderers[0].material.GetColor(namedColor);
        }

        public void ProcessJump()
        {
            _physicsComponent.JumpViaDistance(_jumpDistance);
        }

        //public void SetEnablePlayerInput(bool enable)
        //{
        //    _enablePlayerInput = enable;
        //}

        //private void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawLine(transform.position, transform.position + transform.forward * _targetLockDistance);
        //    Gizmos.DrawLine(transform.position + transform.right * _targetLockRadius, transform.position + transform.right * _targetLockRadius + transform.forward * _targetLockDistance);
        //    Gizmos.DrawLine(transform.position + transform.right * -_targetLockRadius, transform.position + transform.right * -_targetLockRadius + transform.forward * _targetLockDistance);
        //    //Gizmos.DrawSphere(transform.position, 1);
        //}

        //public void ApplyInput(bool apply)
        //{
        //    _applyInput = apply;
        //}

        //void OnDrawGizmos()
        //{
        //    if (!_debug)
        //        return;

        //    //Check if there has been a hit yet
        //    //if (_hitDetect)
        //    //{
        //    //    Gizmos.color = Color.red;
        //    //    //Gizmos.matrix = transform.localToWorldMatrix;
        //    //    var attackPos = transform.TransformPoint(_attackData.Offset);
        //    //    //Draw a Ray forward from GameObject toward the maximum distance
        //    //    Gizmos.DrawRay(attackPos, transform.forward * _attackData.Distance);
        //    //    //Draw a cube at the maximum distance
        //    //    Gizmos.DrawWireCube(attackPos + transform.forward * _attackData.Distance, _attackData.Size);

        //    //    ////Draw a Ray forward from GameObject toward the hit
        //    //    //Gizmos.DrawRay(transform.position, transform.forward * m_Hit.distance);
        //    //    ////Draw a cube that extends to where the hit exists
        //    //    //Gizmos.DrawWireCube(transform.position + transform.forward * m_Hit.distance, transform.localScale);
        //    //}
        //    ////If there hasn't been a hit yet, draw the ray at the maximum distance
        //    //else
        //    //{
        //        Gizmos.color = Color.green;
        //    Gizmos.DrawWireSphere(transform.position, _targetLockRadius);
        //        //Gizmos.matrix = transform.localToWorldMatrix;
        //        //var attackPos = transform.TransformPoint(_attackData.Offset);
        //        //Draw a Ray forward from GameObject toward the maximum distance
        //        //Gizmos.DrawRay(attackPos, transform.forward * _attackData.Distance);
        //        //Draw a cube at the maximum distance
        //        //Gizmos.DrawWireCube(attackPos + transform.forward * _attackData.Distance, _attackData.Size);
        //        //Gizmos.DrawWireCube(attackPos, _attackData.Size);
        //    //}
        //}

        public void SetCrouch(bool crouch)
        {
            if (_isCrouching != crouch)
            {
                if (crouch)
                    MessageDispatcher.Instance.DispatchMsg("Crouch", 0f, _componentRepository.GetId(), _componentRepository.GetId(), null);
                else
                    MessageDispatcher.Instance.DispatchMsg("EndCrouch", 0f, _componentRepository.GetId(), _componentRepository.GetId(), null);
            }
            _isCrouching = crouch;
            _animator.SetBool(_crouchAnimTrigger, _isCrouching);
            //_physicsComponent.GetPhysicsData().MaxSpeedMultiplier = walk ? 0.4f : 1f;
        }

        public void RightArmLookAt(Vector3 position)
        {
            _rightArmShoulderForIK.transform.LookAt(position);
            // Shooter upward direction, right now, will be the cross product of the right arm

            _shooter.transform.LookAt(position);
            //_rightArmShoulderForIK.transform.rotation = armRotation;
        }

        public void LookAtTarget()
        {
            if (_componentRepository.Target != null)
                transform.LookAt(_componentRepository.Target.transform);            
        }

        public void CalculateMovementType()
        {
            if (_lockOnComponent.IsStrafePressed())
            {
                MessageDispatcher.Instance.DispatchMsg("LockOnLegs", 0f, _componentRepository.GetId(), _componentRepository.GetId(), null);
            }
            else if (_skillMovementLogic != MovementLogic.None)
            {
                if (_skillMovementLogic == MovementLogic.OverShoulder)
                {
                    MessageDispatcher.Instance.DispatchMsg("OverShoulderLegs", 0f, _componentRepository.GetId(), _componentRepository.GetId(), null);
                }
                else
                {
                    MessageDispatcher.Instance.DispatchMsg("ThirdPersonLegs", 0f, _componentRepository.GetId(), _componentRepository.GetId(), null);
                }
            }
            else
            {
                MessageDispatcher.Instance.DispatchMsg("ThirdPersonLegs", 0f, _componentRepository.GetId(), _componentRepository.GetId(), null);
            }
            //Debug.Log($"Calculated movement type as {_currentMovementLogic}");
        }

        public void SetSkillMovementLogic(MovementLogic movementLogic)
        {
            _skillMovementLogic = movementLogic;
            CalculateMovementType();
        }

        //public MovementLogic GetCurrentMovementLogic()
        //{
        //    return _currentMovementLogic;
        //}

        public bool GetMovementActive()
        {
            return _isMovementActive;
        }

        public void SetMovementActive(bool isActive)
        {
            //Debug.LogError($"Setting player movement to {isActive}");
            _isMovementActive = isActive;
        }

        //public void SetCameraBasedOnMovementType()
        //{
        //    var currentMovementLogic = GetCurrentMovementLogic();
        //    Debug.Log($"Current Movement Logic: {currentMovementLogic}");
        //    string cameraMessage = string.Empty;
        //    if (currentMovementLogic == MovementLogic.Strafe)
        //    {
        //        if (_lockOnComponent.IsATargetLocked())
        //            cameraMessage = _cameraLockedOnMessage;
        //        else
        //            cameraMessage = _cameraCenterMessage;
        //    }
        //    else if (currentMovementLogic == RQ.Controller.Player.PlayerController.MovementLogic.ThirdPerson)
        //    {
        //        cameraMessage = _cameraNormalMessage;
        //    }
        //    else if (currentMovementLogic == RQ.Controller.Player.PlayerController.MovementLogic.OverShoulder)
        //    {
        //        cameraMessage = _cameraOverShoulderMessage;
        //    }
        //    Debug.Log($"Sending camera message {cameraMessage}");
        //    MessageDispatcher.Instance.DispatchMsg(cameraMessage, 0f, _componentRepository.GetId(), _camera.GetComponentRepository().GetId(), null);
        //}
    }
}


