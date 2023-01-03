using RQ.Base.Extensions;
using UnityEngine;

namespace RQ.Physics.SteeringBehaviors2D
{
    public class Seek2 : SteeringBehaviorBase2
    {
        public Seek2(SteeringBehaviorManager manager)
            : base(manager)
        {
            _constantWeight = Constants2.SeekWeight;
        }
        //this behavior moves the agent towards a target position
        protected override Vector2 CalculateForce()
        {
            return SteeringBehaviorCalculations2.Seek(_steeringBehaviorManager.Target.xz(), _steeringBehaviorManager.Entity);
        }

        //public override void Serialize(SteeringBehaviorData data)
        //{
        //}

        //public override void Deserialize(SteeringBehaviorData data)
        //{
        //}

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (!_steeringBehaviorManager.DrawDebug)
                return;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(_steeringBehaviorManager.Entity.transform.position, 
                _steeringBehaviorManager.Target);
        }
    }
}
