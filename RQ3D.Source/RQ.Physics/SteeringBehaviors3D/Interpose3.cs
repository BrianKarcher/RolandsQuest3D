using UnityEngine;

namespace RQ.Physics.SteeringBehaviors3D
{
    public class Interpose3 : SteeringBehaviorBase3
    {
        public Interpose3(SteeringBehaviorManager manager)
            : base(manager)
        {
            ConstantWeight = Constants3.InterposeWeight;
        }

        //this results in a steering force that attempts to steer the vehicle
        //to the center of the vector connecting two moving agents.
        protected override Vector3 CalculateForce()
        {
            return SteeringBehaviorCalculations3.Interpose(SteeringBehaviorManager.TargetAgent1, 
                SteeringBehaviorManager.TargetAgent2, SteeringBehaviorManager.Entity);
        }
    }
}
