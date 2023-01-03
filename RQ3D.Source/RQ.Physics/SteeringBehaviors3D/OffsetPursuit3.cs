using UnityEngine;

namespace RQ.Physics.SteeringBehaviors3D
{
    public class OffsetPursuit3 : SteeringBehaviorBase3
    {
        //private CollisionComponent collisionComponent;

        public OffsetPursuit3(SteeringBehaviorManager manager)
            : base(manager)
        {
            ConstantWeight = Constants3.OffsetPursuitWeight;
        }
        //this behavior maintains a position, in the direction of offset
        //from the target vehicle
        protected override Vector3 CalculateForce()
        {
            return SteeringBehaviorCalculations3.OffsetPursuit(SteeringBehaviorManager.TargetAgent1,
                SteeringBehaviorManager.Offset, SteeringBehaviorManager.Entity);
        }

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            var targetPos = SteeringBehaviorManager.TargetAgent1.GetWorldPos3() + SteeringBehaviorManager.Offset;
            bool hasLineOfSight = HasLineOfSight(targetPos);
            Gizmos.color = hasLineOfSight ? Color.blue : Color.red;
            Gizmos.DrawLine(SteeringBehaviorManager.Entity.transform.position, targetPos);
            Gizmos.DrawSphere(targetPos, 0.16f);
        }

        public bool HasLineOfSight(Vector3 target)
        {
            //if (collisionComponent == null)
            //    collisionComponent = _steeringBehaviorManager.Entity.GetComponentRepository().Components.GetComponent<CollisionComponent>();
            //var obstacleLayerMask = collisionComponent.GetEnvironmentLayerMask();
            
            int obstacleLayerMask = SteeringBehaviorManager.Entity.GetPhysicsData().GetAvoidLayerMasks();

            //var obstacleLayerMask = LayerMask.NameToLayer("Environment");
            var currentPos = SteeringBehaviorManager.Entity.transform.position;
            //var ray = new Ray(currentPos, target - currentPos);

            //var rayCasts = UnityEngine.Physics.RaycastAll(currentPos, target - currentPos, (target - currentPos).magnitude, obstacleLayerMask);
            //var layerMask = 1 << _obstacleLayerMask;
            return (!UnityEngine.Physics.Raycast(currentPos, target - currentPos, (target - currentPos).magnitude, obstacleLayerMask));
        }
    }
}
