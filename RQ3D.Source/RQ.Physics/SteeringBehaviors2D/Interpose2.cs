using UnityEngine;

namespace RQ.Physics.SteeringBehaviors2D
{
    public class Interpose2 : SteeringBehaviorBase2
    {
        public Interpose2(SteeringBehaviorManager manager)
            : base(manager)
        {
            _constantWeight = Constants2.InterposeWeight;
        }
        //this results in a steering force that attempts to steer the vehicle
        //to the center of the vector connecting two moving agents.
        protected override Vector2 CalculateForce()
        {
            return SteeringBehaviorCalculations2.Interpose(_steeringBehaviorManager.TargetAgent1, 
                _steeringBehaviorManager.TargetAgent2, _steeringBehaviorManager.Entity);
        }
    }
}
