using System;
using System.Collections.Generic;
using RQ.Base.Extensions;
using RQ.Common.Components;
using RQ.Common.Container;
using UnityEngine;

namespace RQ.Physics
{
    /// <summary>
    /// Testable version of the Physics logic
    /// </summary>
    [Serializable]
    public class PhysicsLogic
    {
        // TODO: Remove this
        [SerializeField] private bool _isGrounded;
        public Vector3 GroundNormal { get; private set; }

        [SerializeField] private PhysicsData _physicsData;
        public PhysicsData OriginalPhysicsData { get; private set; }

        private IMovementController _movementController;
        public Vector3 BounceFromLocation { get; set; }
        public GameObject BounceFromObject { get; set; }

        public void Construct(PhysicsData physicsData)
        {
            _physicsData = physicsData;
        }

        public void Awake()
        {
            OriginalPhysicsData = new PhysicsData();
            OriginalPhysicsData.CopyFrom(_physicsData);
            // Cache distance squared for LOS
            _physicsData.LOSSquared = _physicsData.LineOfSight * _physicsData.LineOfSight;
        }

        public void SetMovementController(IMovementController movementController)
        {
            _movementController = movementController;
        }

        public PhysicsData GetPhysicsData()
        {
            return _physicsData;
        }

        public PhysicsData GetOriginalPhysicsData()
        {
            return OriginalPhysicsData;
        }

        public int GetClosestObject(IList<int> instanceIds)
        {
            int closestUniqueId = 0;
            float closestDistance = 10000000f;

            var thisPos = _movementController.GetWorldPos3();

            //foreach (var usableObject in uniqueIds)
            for (int i = 0; i < instanceIds.Count; i++)
            {
                var usableObject = instanceIds[i];
                //Vector2D usableObjectLocation = Vector2D.Zero();
                var entity = EntityContainer.Instance.GetEntity(usableObject.ToString());
                var physicsComponent = entity.Components.GetComponent<IPhysicsComponent>();
                var usableObjectLocation = physicsComponent == null ? entity.transform.position : physicsComponent.GetWorldPos3();
                //MessageDispatcher.Instance.DispatchMsg(0f,
                //    string.Empty, usableObject, RQ.Enums.Telegrams.GetPos, null,
                //    (location) => usableObjectLocation = (Vector2D)location);

                var distanceSq = Vector3.SqrMagnitude(thisPos - usableObjectLocation);
                if (distanceSq < closestDistance)
                {
                    // Found a closer object
                    closestDistance = distanceSq;
                    closestUniqueId = usableObject;
                }
            }

            return closestUniqueId;
        }

        public bool GetIsGrounded()
        {
            return _isGrounded;
        }

        public void SetIsGrounded(bool isGrounded)
        {
            _isGrounded = isGrounded;
        }

        public void CheckGroundStatus()
        {
            RaycastHit hitInfo;
            //#if UNITY_EDITOR
            //			// helper to visualise the ground check ray in the scene view
            Debug.DrawLine(_movementController.transform.position + (Vector3.up * 0.1f), _movementController.transform.position + (Vector3.up * 0.1f) + (Vector3.down * _physicsData.GroundCheckDistance));
            //#endif
            // 0.1f is a small offset to start the ray from inside the character
            // it is also good to note that the transform position in the sample assets is at the base of the character
            bool groundRaycast = UnityEngine.Physics.Raycast(_movementController.transform.position + new Vector3(0f, 0.2f, 0f), Vector3.down, out hitInfo, _physicsData.GroundCheckDistance, _physicsData.GroundLayer.value);
            // The raycast and the Unity rigidbody sometimes disagree, so if either show ground or not moving vertically, assume entity is on the ground.
            if (!groundRaycast && Mathf.Abs(_movementController.GetVelocity3().y) > 0.05f)
            {
                _isGrounded = false;
                GroundNormal = Vector3.up;
                //m_Animator.applyRootMotion = false;
            }
            else
            {
                GroundNormal = hitInfo.normal;
                _isGrounded = true;
                //m_Animator.applyRootMotion = true;
            }
        }

        public bool PosInFOV2(Vector2 pos)
        {
            var vectorToTarget = pos - _movementController.GetWorldPos2();

            var angle = Vector2.Angle(_movementController.transform.forward.xz(), vectorToTarget);

            return angle <= GetPhysicsData().FieldOfView;
        }

        public bool PosInFOV3(Vector3 pos)
        {
            var vectorToTarget = pos - _movementController.GetWorldPos3();

            var angle = Vector3.Angle(_movementController.transform.forward, vectorToTarget);

            return angle <= GetPhysicsData().FieldOfView;
        }

        public bool CheckLOSDistance(GameObject entity)
        {
            // Using distance squared space because a square root is slow
            var distanceSq = (_movementController.GetWorldPos3() - entity.transform.position).sqrMagnitude;
            //Debug.Log($"(CheckLOS Distance: {distanceSq}");
            return distanceSq <= _physicsData.LOSSquared;
        }

        public bool HasLineOfSight(Vector3 target, int obstacleLayerMask)
        {
            var currentPos = _movementController.GetComponentRepository().GetHeadPosition();
            //var layerMask = 1 << _obstacleLayerMask;

            

            bool hasLineOfSight = !UnityEngine.Physics.Raycast(currentPos, target - currentPos, (target - currentPos).magnitude, obstacleLayerMask);
            if (hasLineOfSight)
                Debug.DrawLine(currentPos, target, Color.blue);
            else
                Debug.DrawLine(currentPos, target, Color.red);
            return hasLineOfSight;
        }

        public bool HasLineOfSight(GameObject target, int obstacleLayerMask)
        {
            var targetEntity = target.GetComponent<IEntity>();
            if (targetEntity == null)
                return HasLineOfSight(target.transform.position, obstacleLayerMask);
            //if (targetEntity == null)
            //    return HasLineOfSight(targetEntity.GetHeadPosition(), obstacleLayerMask);
            //var otherPhysicsComponent = entity.Components.GetComponent<PhysicsComponent>();
            //if (otherPhysicsComponent == null)
            //    return HasLineOfSight(target.transform.position, obstacleLayerMask);
            //var otherPos = otherPhysicsComponent.GetWorldPos3();
            var otherPos = targetEntity.GetHeadPosition();

            return HasLineOfSight(otherPos, obstacleLayerMask);
        }

        public bool IsFacingBigDrop(float distance)
        {
            var componentBase = (IComponentBase)_movementController;
            var pos = componentBase.GetComponentRepository().GetPosition() + (componentBase.transform.forward * distance);
            var dir = Vector3.down;
            bool raycast = UnityEngine.Physics.Raycast(pos, dir, 2.0f, _physicsData.TerrainLayer);
            if (_physicsData.ShowDebug)
            {
                Debug.DrawLine(pos, pos + (dir * 2.0f), raycast ? Color.cyan : Color.green, 1.0f);
            }
            return !raycast;
        }
    }
}
