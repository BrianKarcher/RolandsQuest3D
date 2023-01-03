using RQ.Base.Extensions;
using UnityEngine;

namespace RQ.Physics.SteeringBehaviors2D
{
    public abstract class SteeringBehaviorBase2 : ISteeringBehavior
    {
        protected SteeringBehaviorManager _steeringBehaviorManager;
        
        protected float _constantWeight;

        public SteeringBehaviorBase2(SteeringBehaviorManager manager)
        {
            _steeringBehaviorManager = manager;
        }

        //---------------------- CalculateDithered ----------------------------
        //
        //  this method sums up the active behaviors by assigning a probabilty
        //  of being calculated to each behavior. It then tests the first priority
        //  to see if it should be calcukated this simulation-step. If so, it
        //  calculates the steering force resulting from this behavior. If it is
        //  more than zero it returns the force. If zero, or if the behavior is
        //  skipped it continues onto the next priority, and so on.
        //
        //  NOTE: Not all of the behaviors have been implemented in this method,
        //        just a few, so you get the general idea
        //------------------------------------------------------------------------
        public Vector3 CalculateDithered()
        {
            Vector3 force = Vector3.zero;
            float weight = GetWeight();
            if (UnityEngine.Random.Range(0f, 1f) <  weight)
            {
                force = CalculateForce().xz() *
                                     weight / GetConstantWeight(); // Not sure of different between weight and constant weight other than weight can be modified at runtime

                if (!(force == Vector3.zero))
                {
                    force = Vector3.ClampMagnitude(force, _steeringBehaviorManager.Entity.GetPhysicsData().MaxForce);
                    //force.Truncate(_steeringBehaviorManager.SteeringPhysicsAffector.MaxForce);

                    return force;
                }
            }
            return Vector3.zero;
        }

        //---------------------- CalculateWeightedSum ----------------------------
        //
        //  this simply sums up all the active behaviors X their weights and 
        //  truncates the result to the max available steering force before 
        //  returning
        //------------------------------------------------------------------------
        public Vector3 CalculateWeightedSum()
        {
            var force2 = CalculateForce();
            var force3 = force2.xz();
            var localForce = _steeringBehaviorManager.Entity.transform.InverseTransformDirection(force3);

            return force3 * GetWeight();
        }

        //---------------------- CalculatePrioritized ----------------------------
        //
        //  this method calls each active steering behavior in order of priority
        //  and acumulates their forces until the max steering force magnitude
        //  is reached, at which time the function returns the steering force 
        //  accumulated to that  point
        //------------------------------------------------------------------------
        public Vector3 CalculatePrioritized()
        {
            return CalculateForce().xz() * GetWeight();
        }

        //public bool IsOn()
        //{
        //    return _isOn;
        //}

        //public void TurnOn()
        //{
        //    _isOn = true;
        //}

        //public void TurnOff()
        //{
        //    _isOn = false;
        //}

        public float GetWeight()
        {
            return _constantWeight; //_steeringBehaviorManager.SteeringPhysicsAffector.Mass; //_weight;
        }

        protected abstract Vector2 CalculateForce();

        public float GetConstantWeight()
        {
            return _constantWeight;
        }

        public virtual void OnDrawGizmos()
        {

        }
    }
}
