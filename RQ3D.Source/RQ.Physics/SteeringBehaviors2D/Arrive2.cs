using UnityEngine;

namespace RQ.Physics.SteeringBehaviors2D
{
    public class Arrive2 : SteeringBehaviorBase2, ISteeringBehavior
    {
        private Deceleration2 _deceleration;
        protected float _weight;

        public Arrive2(SteeringBehaviorManager manager) : base(manager)
        {
            _weight = Constants2.ArriveWeight;
            _constantWeight = Constants2.ArriveWeight;
            _deceleration = Deceleration2.normal;
        }

        //--------------------------- CalculateForce -----------------------------
        //
        //  This behavior is similar to seek but it attempts to arrive at the
        //  target with a zero velocity
        //------------------------------------------------------------------------
        protected override Vector2 CalculateForce()
        {
            return SteeringBehaviorCalculations2.Arrive(_steeringBehaviorManager.Target,
                _steeringBehaviorManager.Entity, _deceleration);
        }

        public void SetDeceleration(Deceleration2 deceleration)
        {
            _deceleration = deceleration;
        }

        //public override void Serialize(SteeringBehaviorData data)
        //{
        //    data.Deceleration = _deceleration;
        //    data.Weight = _weight;
        //}

        //public override void Deserialize(SteeringBehaviorData data)
        //{
        //    _deceleration = data.Deceleration;
        //    _weight = data.Weight;
        //}
    }
}
