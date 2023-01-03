using System;
using UnityEngine;

namespace RQ.Physics
{
    [Serializable]
    public class PhysicsData
    {
        [SerializeField]
        private bool _instantForce = false;
        public bool InstantForce { get => _instantForce; set => _instantForce = value; }

        public enum TurnModeEnum
        {
            TowardsSteeringForce = 0,
            TowardsSteeringBehaviorTarget = 1,
            TowardsTarget = 2,
            None = 3
        }

        [SerializeField]
        private TurnModeEnum _turnMode;
        public TurnModeEnum TurnMode { get => _turnMode; set => _turnMode = value; }

        [SerializeField]
        private bool _autoTurn = true;
        public bool AutoTurn { get => _autoTurn; set => _autoTurn = value;}

        [SerializeField]
        private bool _autoApplyToAnimator;
        public bool AutoApplyToAnimator { get => _autoApplyToAnimator; set => _autoApplyToAnimator = value; }

        [SerializeField]
        private SteeringData _steeringData;
        public SteeringData SteeringData => _steeringData;

        [SerializeField]
        private bool _showDebug = false;
        public bool ShowDebug => _showDebug;

        [SerializeField]
        private float _gravity = -9.8f;
        public float Gravity { get => _gravity; set => _gravity = value; }

        [SerializeField]
        private Vector3 _gravityVector = new Vector3(0f, -9.8f, 0f);
        public Vector3 GravityVector { get => _gravityVector; set => _gravityVector = value; }

        [SerializeField]
        private bool _applyGravity = true;
        public bool ApplyGravity { get => _applyGravity; set => _applyGravity = value; }

        [SerializeField] private float _movingTurnSpeed = 360;
        public float MovingTurnSpeed => _movingTurnSpeed;

        [SerializeField] private float _stationaryTurnSpeed = 180;
        public float StationaryTurnSpeed => _stationaryTurnSpeed;

        [SerializeField]
        private Vector3 _initialVelocity;

        [SerializeField]
        private Vector3 _size;

        /// <summary>
        /// This field is read only, used for debugging and determining current velocity
        /// </summary>
        [SerializeField]
        private Vector3 _velocity;

        [SerializeField]
        private Vector3 _jumpVelocity;
        public Vector3 JumpVelocity => _jumpVelocity;

        //[HideInInspector]
        //a normalized vector pointing in the direction the entity is heading. 
        //[SerializeField]
        //private Vector3 _heading;
        //public Vector3 Heading
        //{
        //    get => _heading;
        //    set => _heading = value;
        //}

        //[HideInInspector]
        //a vector perpendicular to the heading vector
        //[SerializeField]
        //private Vector3 _side;
        //public Vector3 Side { get { return _side; } set { _side = value; } }

        [SerializeField]
        private Vector3 _offset;

        [SerializeField]
        private float _mass = 1f;

        //the maximum speed this entity may travel at.
        [SerializeField]
        private float _maxSpeed = 6f;
        public float MaxSpeed { get { return _maxSpeed; } set { _maxSpeed = value; } }

        //[SerializeField]
        //private float OriginalMaxSpeed = 1f;

        [SerializeField]
        private float _maxSpeedMultiplier = 1f;
        public float MaxSpeedMultiplier { get { return _maxSpeedMultiplier; } set { _maxSpeedMultiplier = value;  } }

        [SerializeField]
        private float _maxDistanceToTarget;
        public float MaxDistanceToTarget { get { return _maxDistanceToTarget; } }

        [SerializeField]
        private bool _directionRotate = false;
        public bool DirectionRotate { get { return _directionRotate; } set { _directionRotate = value; } }
        //public bool 

        [SerializeField]
        private float _boundingRadius = 1.0f;
        public float BoundingRadius { get { return _boundingRadius; } set { _boundingRadius = value; } }

        //public PhysicsAffector[] physicsAffector;


        //[SerializeField]
        //private Vector2D _externalForce;
        //[HideInInspector]
        //public Vector2D ExternalForce { get { return _externalForce; } set { _externalForce = value; } }

        //[SerializeField]
        //private Vector2D _inputForce;
        //[HideInInspector]
        //public Vector2D InputForce { get { return _inputForce; } set { _inputForce = value; } }
        //[HideInInspector]
        //[SerializeField]
        //private Vector2 _externalVelocity;
        //private Vector2 ExternalVelocity { get { return _externalVelocity; } set { _externalVelocity = value; } }

        //[SerializeField]
        //private Vector2 _inputVelocity;
        //public Vector2 InputVelocity { get { return _inputVelocity; } set { _inputVelocity = value; } }

        //[HideInInspector]
        //public float OriginalZ = -0.5f;
        //public float ZOffset = 0.0f;
        //[HideInInspector]
        //public float _zOffset;

        //[SerializeField]
        //private Direction _headingDirection = Direction.Right;


        /// <summary>
        /// the maximum force this entity can produce to power itself 
        /// (think rockets and thrust)
        /// </summary>
        [SerializeField]
        private float _maxForce = 6f;
        public float MaxForce { get { return _maxForce; } set { _maxForce = value; } }

        [SerializeField]
        private float _forceMultiplier = 1f;
        public float ForceMultiplier { get { return _forceMultiplier; } set { _forceMultiplier = value; } }

        [SerializeField]
        private float _friction;
        //[HideInInspector]
        //public float Friction { get { return _friction; } set { _friction = value; } }

        /// <summary>
        /// the maximum rate (radians per second)this vehicle can rotate 
        /// </summary>
        [SerializeField]
        private float _maxTurnRate = 6f;
        public float MaxTurnRate { get => _maxTurnRate; set => _maxTurnRate = value; }

        //[SerializeField]
        //private Vector3 _footOffset;

        //public Vector3 FootOffset
        //{
        //    get => _footOffset;
        //    set => _footOffset = value;
        //}

        [SerializeField]
        private GameObject _foot;
        public GameObject Foot { get => _foot; set => _foot = value; }

        [SerializeField]
        private float _fieldOfView;
        public float FieldOfView { get { return _fieldOfView; } set { _fieldOfView = value; } }

        [SerializeField]
        private float _lineOfSight;
        public float LineOfSight { get { return _lineOfSight; } set { _lineOfSight = value; } }
        // Cache distance squared for LOS
        public float LOSSquared;

        [SerializeField]
        private float _dragForce;
        public float DragForce { get { return _dragForce; } set { _dragForce = value; } }

        [SerializeField]
        private LayerMask _avoidLayersMask;
        public LayerMask AvoidLayersMask { get => _avoidLayersMask; set => _avoidLayersMask = value; }

        [SerializeField] float _groundCheckDistance = 0.4f;
        public float GroundCheckDistance { get => _groundCheckDistance; set => _groundCheckDistance = value; }
        [SerializeField] private LayerMask _groundLayer;
        public LayerMask GroundLayer { get => _groundLayer; set => _groundLayer = value; }

        [SerializeField] private LayerMask _terrainLayer;
        public LayerMask TerrainLayer => _terrainLayer;

        public int GetAvoidLayerMasks()
        {
            //int mask = 0;

            //for (int i = 0; i < _avoidLayersMasks.Length; i++)
            //{
            //    mask |= _avoidLayersMasks[i];
            //}
            //return mask;
            return _avoidLayersMask.value;
        }

        //[HideInInspector]
        //public float InitialZ { get; set; }

        //public float CurrentZ;

        //public float ZDepthByY;

        //public float ZOffsetByLevel;

        //public float ZOffset;

        //public SteeringBehaviorData SteeringBehaviourData { get; set; }

        [SerializeField]
        private bool IsEnabled { get; set; }

        public virtual void CopyFrom(PhysicsData from)
        {
            this.InstantForce = from.InstantForce;
            this._groundCheckDistance = from._groundCheckDistance;
            this._groundLayer = from._groundLayer;
            this._jumpVelocity = from._jumpVelocity;
            this._gravity = from._gravity;
            this._directionRotate = from._directionRotate;
            this._movingTurnSpeed = from._movingTurnSpeed;
            this._stationaryTurnSpeed = from._stationaryTurnSpeed;
            //this._externalForce = from._externalForce;
            //this._externalVelocity = from._externalVelocity;
            this._fieldOfView = from._fieldOfView;
            //this._footOffset = from._footOffset;
            this._friction = from._friction;
            //this._headingDirection = from._headingDirection;
            //this._inputForce = from._inputForce;
            //this._inputVelocity = from._inputVelocity;
            this._lineOfSight = from._lineOfSight;
            this._maxTurnRate = from._maxTurnRate;
            //this._damagedBounceForce = from._damagedBounceForce;
            //this._damageDrag = from._damageDrag;
            //this._heading = from._heading;
            this._mass = from._mass;
            this._maxForce = from._maxForce;
            this._maxSpeed = from._maxSpeed;
            this._maxSpeedMultiplier = from._maxSpeedMultiplier;
            //this.OriginalZ = from.OriginalZ;
            //this._side = from._side;
            this._size = from._size;
            //this.Velocity = from.Velocity;
            //this.ZDepthByY = from.ZDepthByY;
            //this.ZOffsetByLevel = from.ZOffsetByLevel;
            //this.SteeringBehaviourData = from.SteeringBehaviourData;
        }
    }
}
