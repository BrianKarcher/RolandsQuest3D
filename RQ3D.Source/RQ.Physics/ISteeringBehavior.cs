using UnityEngine;

namespace RQ.Physics
{
    public interface ISteeringBehavior
    {
        //---------------------- CalculatePrioritized ----------------------------
        //
        //  this method calls each active steering behavior in order of priority
        //  and acumulates their forces until the max steering force magnitude
        //  is reached, at which time the function returns the steering force 
        //  accumulated to that  point
        //------------------------------------------------------------------------
        Vector3 CalculatePrioritized();
        //---------------------- CalculateWeightedSum ----------------------------
        //
        //  this simply sums up all the active behaviors X their weights and 
        //  truncates the result to the max available steering force before 
        //  returning
        //------------------------------------------------------------------------
        Vector3 CalculateWeightedSum();
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
        Vector3 CalculateDithered();
        //Vector2D CalculateForce();
        //multipliers. These can be adjusted to effect strength of the  
        //appropriate behavior. Useful to get flocking the way you require
        //for example.
        float GetWeight();
        float GetConstantWeight();
        //SpriteBase SetTarget(SpriteBase target);
        //void TurnOn();
        //void TurnOff();
        //void Serialize(SteeringBehaviorData data);

        //void Deserialize(SteeringBehaviorData data);
        void OnDrawGizmos();
    }
}
