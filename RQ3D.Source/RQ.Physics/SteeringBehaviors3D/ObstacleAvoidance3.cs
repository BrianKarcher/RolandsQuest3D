using UnityEngine;

namespace RQ.Physics.SteeringBehaviors3D
{
    public class ObstacleAvoidance3 : SteeringBehaviorBase3
    {
        //length of the 'detection box' utilized in obstacle avoidance
        private float _boxLength;

        //private List<PhysicsComponent> obstacles;

        public ObstacleAvoidance3(SteeringBehaviorManager manager) : base(manager)
        {
            ConstantWeight = Constants3.ObstacleAvoidanceWeight;
            _boxLength = Constants3.MinDetectionBoxLength;
        }
        //this returns a steering force which will attempt to keep the agent 
        //away from any obstacles it may encounter
        protected override Vector3 CalculateForce()
        {
            //return Vector2D.Zero();
            return SteeringBehaviorCalculations3.ObstacleAvoidance(SteeringBehaviorManager.Entity);
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
    }
}
