using RQ.Base.Extensions;
using UnityEngine;

namespace RQ.Physics.SteeringBehaviors2D
{
    public class RadiusClamp2 : SteeringBehaviorBase2, ISteeringBehavior
    {
        private Vector2 _clampVector;

        public RadiusClamp2(SteeringBehaviorManager manager)
            : base(manager)
        {
            _constantWeight = _steeringBehaviorManager.Entity.GetPhysicsData().SteeringData.RadiusClampWeight;
        }
        //this results in a steering force that attempts to steer the vehicle
        //to the center of the vector connecting two moving agents.
        protected override Vector2 CalculateForce()
        {
            _clampVector = SteeringBehaviorCalculations2.RadiusClamp(_steeringBehaviorManager.Entity);
            return _clampVector;
        }

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_steeringBehaviorManager.Entity.transform.position,
               _steeringBehaviorManager.Entity.transform.position + 
               _clampVector.xz());
            Gizmos.color = Color.green;
            var targetVector = _steeringBehaviorManager.Entity.GetSteering().GetTarget3();
            Gizmos.DrawSphere(targetVector, 0.04f);
            var physicsData = _steeringBehaviorManager.Entity.GetPhysicsData();
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(targetVector, physicsData.MaxDistanceToTarget);
            //Gizmos.DrawCube(targetVector.ToVector3(0f), new Vector3(physicsData.MaxDistanceToTarget, physicsData.MaxDistanceToTarget, 0f));
        }
    }
}
