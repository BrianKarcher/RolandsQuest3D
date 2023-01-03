using RQ.Base.Extensions;
using UnityEngine;

namespace RQ.Physics.SteeringBehaviors2D
{
    public class Flee2 : SteeringBehaviorBase2, ISteeringBehavior
    {
        public Flee2(SteeringBehaviorManager manager)
            : base(manager)
        {
            _constantWeight = Constants2.FleeWeight;
        }
        //this behavior returns a vector that moves the agent away
        //from a target position
        protected override Vector2 CalculateForce()
        {
            return SteeringBehaviorCalculations2.Flee(_steeringBehaviorManager.Target.xz(), _steeringBehaviorManager.Entity);
        }
    }
}
