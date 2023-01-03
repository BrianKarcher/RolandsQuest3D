using RQ.Common.Components;
using UnityEngine;

namespace RQ.Physics
{
    public interface IPhysicsComponent : IComponentBase
    {
        PhysicsData GetPhysicsData();
        Vector2 GetVelocity2();
        Vector3 GetVelocity3();
        //void AddVelocity3(Vector3 velocity);
        //Vector2D GetFeetPosition();
        Vector2 GetFeetWorldPosition2();
        Vector3 GetFeetWorldPosition3();
        Vector3 GetLocalPos3();
        Vector3 GetWorldPos3();
        Vector2 GetWorldPos2();
        //void SetWorldPos(Vector2 new_pos);
        void SetWorldPos3(Vector3 new_pos);
        void Stop();
        //Transform transform { get; }
        ISteeringBehaviorManager GetSteering();
        //IPhysicsAffector GetPhysicsAffector(string name);
        //void SetPhysicsAffector(string name, IPhysicsAffector physicsAffector);
    }
}
