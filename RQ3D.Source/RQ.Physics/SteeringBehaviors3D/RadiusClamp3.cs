using UnityEngine;

namespace RQ.Physics.SteeringBehaviors3D
{
    public class RadiusClamp3 : SteeringBehaviorBase3
    {
        private Vector3 _clampVector;

        public RadiusClamp3(SteeringBehaviorManager manager)
            : base(manager)
        {
            ConstantWeight = Constants3.InterposeWeight;
        }
        //this results in a steering force that attempts to steer the vehicle
        //to the center of the vector connecting two moving agents.
        protected override Vector3 CalculateForce()
        {
            _clampVector = SteeringBehaviorCalculations3.RadiusClamp(SteeringBehaviorManager.Entity);
            return _clampVector;
        }

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            Gizmos.color = Color.red;
            //Gizmos.DrawLine(SteeringBehaviorManager.Entity.transform.position,
            //    SteeringBehaviorManager.Entity.transform.position + 
            //   _clampVector.ToVector3(SteeringBehaviorManager.Entity.transform.position.z));
            Gizmos.DrawLine(SteeringBehaviorManager.Entity.transform.position,
                SteeringBehaviorManager.Entity.transform.position +
                _clampVector);
            Gizmos.color = Color.green;
            var targetVector = SteeringBehaviorManager.Entity.GetSteering().GetTarget3();
            Gizmos.DrawSphere(targetVector, 0.04f);
            var physicsData = SteeringBehaviorManager.Entity.GetPhysicsData();
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(targetVector, physicsData.MaxDistanceToTarget);
            //Gizmos.DrawCube(targetVector.ToVector3(0f), new Vector3(physicsData.MaxDistanceToTarget, physicsData.MaxDistanceToTarget, 0f));
        }
    }
}
