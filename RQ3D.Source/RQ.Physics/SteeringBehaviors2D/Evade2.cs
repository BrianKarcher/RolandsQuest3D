using UnityEngine;

namespace RQ.Physics.SteeringBehaviors2D
{
    public class Evade2 : SteeringBehaviorBase2, ISteeringBehavior
    {
        public Evade2(SteeringBehaviorManager manager) : base(manager)
        {
            _constantWeight = Constants2.EvadeWeight;
        }

        //----------------------------- Evade ------------------------------------
        //
        //  similar to pursuit except the agent Flees from the estimated future
        //  position of the pursuer
        //------------------------------------------------------------------------
        protected override Vector2 CalculateForce()
        {
            return SteeringBehaviorCalculations2.Evade(_steeringBehaviorManager.TargetAgent1, _steeringBehaviorManager.Entity);
        }
    }
}
