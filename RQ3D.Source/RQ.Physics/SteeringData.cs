using System;
using UnityEngine;

namespace RQ.Physics
{
    [Serializable]
    public class SteeringData
    {
        [SerializeField]
        private float _waypointSeekDistSq = .1f;
        public float WaypointSeekDistSq => _waypointSeekDistSq;

        [SerializeField]
        private float _separationWeight = 1.0f;
        public float SeparationWeight => _separationWeight;

        [SerializeField]
        private float _alignmentWeight = 1.0f;
        public float AlignmentWeight => _alignmentWeight;

        [SerializeField]
        private float _cohesionWeight = 2.0f;
        public float CohesionWeight => _cohesionWeight;

        [SerializeField]
        private float _obstacleAvoidanceWeight = 10.0f;
        public float ObstacleAvoidanceWeight => _obstacleAvoidanceWeight;

        [SerializeField]
        private float _wallAvoidanceWeight = 10.0f;
        public float WallAvoidanceWeight => _wallAvoidanceWeight;

        [SerializeField]
        private float _wanderWeight = 1.0f;
        public float WanderWeight => _wanderWeight;

        [SerializeField]
        private float _seekWeight = 1.0f;
        public float SeekWeight => _seekWeight;

        [SerializeField]
        private float _fleeWeight = 1.0f;
        public float FleeWeight => _fleeWeight;

        [SerializeField]
        private float _arriveWeight = 1.0f;
        public float ArriveWeight => _arriveWeight;

        [SerializeField]
        private float _pursuitWeight = 1.0f;
        public float PursuitWeight => _pursuitWeight;

        [SerializeField]
        private float _offsetPursuitWeight = 1.0f;
        public float OffsetPursuitWeight => _offsetPursuitWeight;

        [SerializeField]
        private float _interposeWeight = 1.0f;
        public float InterposeWeight => _interposeWeight;

        [SerializeField]
        private float _hideWeight = 1.0f;
        public float HideWeight => _hideWeight;

        [SerializeField]
        private float _evadeWeight = 1.0f;
        public float EvadeWeight => _evadeWeight;

        [SerializeField]
        private float _followPathWeight = 1.0f;
        public float FollowPathWeight => _followPathWeight;

        [SerializeField]
        private float _radiusClampWeight = 0.5f;
        public float RadiusClampWeight => _radiusClampWeight;

        [SerializeField]
        private float _feelerOffset;

        public float FeelerOffset => _feelerOffset;

        /// <summary>
        /// Used for obstacle avoidance
        /// </summary>
        [SerializeField]
        private float _boundingRadius;

        public float BoundingRadius => _boundingRadius;

        /// <summary>
        /// the radius of the constraining circle for the wander behavior
        /// </summary>
        [SerializeField]
        private float _wanderRad = 1.2f;
        public float WanderRad => _wanderRad;
        /// <summary>
        /// distance the wander circle is projected in front of the agent
        /// </summary>
        [SerializeField]
        private float _wanderDist = 3.0f;
        public float WanderDist => _wanderDist;

        //private const float WanderDist = .8f;
        //private const float WanderDist = 10f;
        //
        //private const float WanderJitterPerSec = 80f;
        /// <summary>
        /// the maximum amount of displacement along the circle each frame
        /// </summary>
        [SerializeField]
        private float _wanderJitterPerSec = 10f;
        public float WanderJitterPerSec => _wanderJitterPerSec;

        //[SerializeField]
        //private Vector3 _radiusClampTarget;
        //public Vector3 RadiusClampTarget => _radiusClampTarget;

        //[SerializeField]
        //private float _radiusClampDistance = 3.0f;
        //public float RadiusClampDistance => _radiusClampDistance;

        [SerializeField]
        private float _wallDetectionFeelerLength = 2.0f;
        public float WallDetectionFeelerLength => _wallDetectionFeelerLength;
    }
}
