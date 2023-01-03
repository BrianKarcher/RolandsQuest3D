using System.Collections.Generic;
using UnityEngine;

namespace RQ.Physics.SteeringBehaviors2D
{
    public class ObstacleAvoidance2 : SteeringBehaviorBase2, ISteeringBehavior
    {
        //length of the 'detection box' utilized in obstacle avoidance
        private float _boxLength;

        //private List<PhysicsComponent> obstacles;

        public ObstacleAvoidance2(SteeringBehaviorManager manager) : base(manager)
        {
            _constantWeight = Constants2.ObstacleAvoidanceWeight;
            _boxLength = Constants2.MinDetectionBoxLength;
        }
        //this returns a steering force which will attempt to keep the agent 
        //away from any obstacles it may encounter
        protected override Vector2 CalculateForce()
        {
            //return Vector2D.Zero();
            return SteeringBehaviorCalculations2.ObstacleAvoidance(_steeringBehaviorManager.Entity);
        }

        public float GetDBoxLength()
        {
            return _boxLength;
        }

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            //Debug.
            
        }

        //public override void Serialize(SteeringBehaviorData data)
        //{
        //    data.BoxLength = _boxLength;
        //    if (obstacles != null)
        //    {
        //        data.Obstacles = obstacles.Select(i => i.UniqueId).ToList();
        //    }
        //}

        //public override void Deserialize(SteeringBehaviorData data)
        //{
        //    //var entityResolver = _steeringBehaviorManager.GetEntityResolver();
        //    //var entityMap = EntityController._instance.GetEntityMap();
        //    _boxLength = data.BoxLength;
        //    if (data.Obstacles != null)
        //        obstacles = data.Obstacles.Select(i => EntityContainer._instance.GetEntity<IComponentRepository>(i).GetComponent<PhysicsComponent>()).ToList();
        //}
    }
}
