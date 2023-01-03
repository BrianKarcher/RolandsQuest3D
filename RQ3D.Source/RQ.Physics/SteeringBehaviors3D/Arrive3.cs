using UnityEngine;

namespace RQ.Physics.SteeringBehaviors3D
{
    public class Arrive3 : SteeringBehaviorBase3
    {
        private Deceleration3 _deceleration;
        protected float _weight;

        public Arrive3(SteeringBehaviorManager manager) : base(manager)
        {
            _weight = Constants3.ArriveWeight;
            ConstantWeight = Constants3.ArriveWeight;
            _deceleration = Deceleration3.normal;
        }

        //--------------------------- CalculateForce -----------------------------
        //
        //  This behavior is similar to seek but it attempts to arrive at the
        //  target with a zero velocity
        //------------------------------------------------------------------------
        protected override Vector3 CalculateForce()
        {
            return SteeringBehaviorCalculations3.Arrive(SteeringBehaviorManager.Target,
                SteeringBehaviorManager.Entity, _deceleration);
        }

        public void SetDeceleration(Deceleration3 deceleration)
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
