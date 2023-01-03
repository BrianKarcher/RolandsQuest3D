using UnityEngine;

namespace RQ.Physics.SteeringBehaviors3D
{
    public class Seek3 : SteeringBehaviorBase3
    {
        public Seek3(SteeringBehaviorManager manager)
            : base(manager)
        {
            ConstantWeight = Constants3.SeekWeight;
        }
        //this behavior moves the agent towards a target position
        protected override Vector3 CalculateForce()
        {
            return SteeringBehaviorCalculations3.Seek(SteeringBehaviorManager.Target, SteeringBehaviorManager.Entity);
        }

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(SteeringBehaviorManager.Entity.transform.position,
                SteeringBehaviorManager.Target);
        }
    }
}
