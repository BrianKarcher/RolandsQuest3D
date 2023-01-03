using UnityEngine;

namespace RQ.Physics.SteeringBehaviors2D
{
    public class Pursuit2 : SteeringBehaviorBase2, ISteeringBehavior
    {
        public Pursuit2(SteeringBehaviorManager manager)
            : base(manager)
        {
            _constantWeight = Constants2.PursuitWeight;
        }

        //this behavior predicts where an agent will be in time T and seeks
        //towards that point to intercept it.
        protected override Vector2 CalculateForce()
        {
            return SteeringBehaviorCalculations2.Pursuit(_steeringBehaviorManager.TargetAgent1, _steeringBehaviorManager.Entity);
        }
    }
}
