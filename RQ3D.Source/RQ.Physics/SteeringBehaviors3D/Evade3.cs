using UnityEngine;

namespace RQ.Physics.SteeringBehaviors3D
{
    public class Evade3 : SteeringBehaviorBase3
    {
        public Evade3(SteeringBehaviorManager manager) : base(manager)
        {
            ConstantWeight = Constants3.EvadeWeight;
        }

        //----------------------------- Evade ------------------------------------
        //
        //  similar to pursuit except the agent Flees from the estimated future
        //  position of the pursuer
        //------------------------------------------------------------------------
        protected override Vector3 CalculateForce()
        {
            return SteeringBehaviorCalculations3.Evade(SteeringBehaviorManager.TargetAgent1, SteeringBehaviorManager.Entity);
        }
    }
}
