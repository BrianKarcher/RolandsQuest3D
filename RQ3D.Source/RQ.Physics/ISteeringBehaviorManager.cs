using UnityEngine;

namespace RQ.Physics
{
    public interface ISteeringBehaviorManager
    {
        IPhysicsComponent GetTargetAgent1();
        void SetTargetAgent1(IPhysicsComponent target);
        //CollisionComponent CollisionComponent { get; set; }
        IPhysicsComponent Entity { get; set; }
        //IPhysicsAffector SteeringPhysicsAffector { get; set; }
        //PhysicsComponent TargetAgent2 { get; set; }
        float ViewDistance { get; set; }
        ISteeringBehavior GetSteeringBehavior(behavior_type behaviortype);

        Vector3 Calculate();
        void CalculateSteeringModes();
        //void Deserialize(SteeringBehaviorData data);
        float ForwardComponent();
        Vector3 GetForce();
        //ISteeringBehavior GetSteeringBehavior(behavior_type behaviortype);
        bool IsOn(behavior_type bt);
        void OnDrawGizmos();
        //SteeringBehaviorData Serialize();
        //void SetSummingMethod(SteeringBehaviorManager.summing_method sm);
        void Setup(IPhysicsComponent entity, Transform transform);
        float SideComponent();
        void TurnOff(behavior_type behaviortype);
        void TurnOn(behavior_type behaviortype);
        Vector3 GetTarget3();
        void SetTarget3(Vector3 target);
        Vector3 GetOffset();
    }
}
