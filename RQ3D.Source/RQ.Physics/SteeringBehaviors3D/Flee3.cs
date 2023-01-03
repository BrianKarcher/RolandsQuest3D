using UnityEngine;

namespace RQ.Physics.SteeringBehaviors3D
{
    public class Flee3 : SteeringBehaviorBase3
    {
        public Flee3(SteeringBehaviorManager manager)
            : base(manager)
        {
            ConstantWeight = Constants3.FleeWeight;
        }

        //this behavior returns a vector that moves the agent away
        //from a target position
        protected override Vector3 CalculateForce()
        {
            return SteeringBehaviorCalculations3.Flee(SteeringBehaviorManager.Target, SteeringBehaviorManager.Entity);
        }
    }
}
