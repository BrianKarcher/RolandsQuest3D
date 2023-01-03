using UnityEngine;

namespace RQ.Physics.SteeringBehaviors3D
{
    public class Pursuit3 : SteeringBehaviorBase3
    {
        public Pursuit3(SteeringBehaviorManager manager)
            : base(manager)
        {
            ConstantWeight = Constants3.PursuitWeight;
        }

        //this behavior predicts where an agent will be in time T and seeks
        //towards that point to intercept it.
        protected override Vector3 CalculateForce()
        {
            return SteeringBehaviorCalculations3.Pursuit(SteeringBehaviorManager.TargetAgent1, SteeringBehaviorManager.Entity);
        }
    }
}
