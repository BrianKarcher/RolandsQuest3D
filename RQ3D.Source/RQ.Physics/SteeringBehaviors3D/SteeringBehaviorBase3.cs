using UnityEngine;

namespace RQ.Physics.SteeringBehaviors3D
{
    public abstract class SteeringBehaviorBase3 : ISteeringBehavior
    {
        protected SteeringBehaviorManager SteeringBehaviorManager;
        //protected bool _isOn;
        
        protected float ConstantWeight;

        public SteeringBehaviorBase3(SteeringBehaviorManager manager)
        {
            SteeringBehaviorManager = manager;
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
            float weight = GetWeight();
            if (Random.Range(0f, 1f) <  weight)
            {
                var force = CalculateForce() *
                                     weight / GetConstantWeight(); // Not sure of different between weight and constant weight other than weight can be modified at runtime

                if (force != Vector3.zero)
                {
                    force = Vector3.ClampMagnitude(force, SteeringBehaviorManager.Entity.GetPhysicsData().MaxForce);
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
            return CalculateForce() * GetWeight();
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
            return CalculateForce() * GetWeight();
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
            return ConstantWeight; //_steeringBehaviorManager.SteeringPhysicsAffector.Mass; //_weight;
        }

        protected abstract Vector3 CalculateForce();

        public float GetConstantWeight()
        {
            return ConstantWeight;
        }

        public virtual void OnDrawGizmos()
        {

        }

        //public abstract void Serialize(SteeringBehaviorData data);

        //public abstract void Deserialize(SteeringBehaviorData data);
    }
}
