using UnityEngine;

namespace RQ.Physics.SteeringBehaviors3D
{
    public class Wander3 : SteeringBehaviorBase3
    {
        //the radius of the constraining circle for the wander behavior
        private const float WanderRad = 1.2f;
        //private const float WanderRad = .04f;
        //private const float WanderRad = 5f;
        //distance the wander circle is projected in front of the agent
        private const float WanderDist = 3.0f;
        //private const float WanderDist = .8f;
        //private const float WanderDist = 10f;
        //the maximum amount of displacement along the circle each frame
        //private const float WanderJitterPerSec = 80f;
        private const float WanderJitterPerSec = 10f;
        //private const float WanderJitterPerSec = .5f;
        //private const float WanderJitterPerSec = 1f;
        private Vector3 last_wander_pos = Vector3.zero;

        private Transform _transform;

        //the current position on the wander circle the agent is
        //attempting to steer towards
        private Vector3 _wanderTarget = new Vector3(0,0,0);

        //explained above
        private float _wanderJitter;
        private float _wanderRadius;
        private float _wanderDistance;

        public Wander3(SteeringBehaviorManager manager, Transform transform)
            : base(manager)
        {
            _transform = transform;
            ConstantWeight = Constants3.WanderWeight;
            _wanderDistance = WanderDist;
            _wanderJitter = WanderJitterPerSec;
            _wanderRadius = WanderRad;
            float theta = UnityEngine.Random.Range(0f, 1f) * Mathf.PI * 2;
            //create a vector to a target position on the wander circle
            _wanderTarget = new Vector3(_wanderRadius * Mathf.Cos(theta),
                                        0f,
                                        _wanderRadius * Mathf.Sin(theta));
        }        

        //this behavior makes the agent wander about randomly
        protected override Vector3 CalculateForce()
        {
            return SteeringBehaviorCalculations3.Wander(SteeringBehaviorManager.Entity, ref _wanderTarget, _wanderJitter,
                _wanderRadius, _wanderDistance, _transform, ref last_wander_pos);
        }

        public override void OnDrawGizmos()
        {
            var entity = SteeringBehaviorManager.Entity;
            //calculate the center of the wander circle
            Vector3 m_vTCC = entity.transform.TransformPoint(new Vector3(0f, 0f, _wanderDistance));
            //Vector2D m_vTCC = Transformations.PointToWorldSpace(new Vector2D(_wanderDistance /** m_pVehicle->BRadius()*/, 0),
            //                         entity.GetPhysicsData().Heading,
            //                                     entity.GetPhysicsData().Side,
            //                                     entity.GetWorldPos());

            //draw the wander circle
            //Debug.
            Gizmos.color = new Color(0, 255, 0, .3f);
            Gizmos.DrawSphere(m_vTCC, _wanderRadius);
            //gdi->GreenPen();
            //gdi->HollowBrush();
            //gdi->Circle(m_vTCC, m_dWanderRadius * m_pVehicle->BRadius());

            //draw the wander target
            Gizmos.color = new Color(255, 0, 0, .3f);
            //gdi->RedPen();
            var targetPos = entity.transform.TransformPoint(_wanderTarget + new Vector3(0f, 0f, _wanderDistance));
            //var targetPos = Transformations.PointToWorldSpace((_wanderTarget + new Vector2D(_wanderDistance, 0)) /** m_pVehicle->BRadius() */,
            //                              entity.GetPhysicsData().Heading,
            //                                     entity.GetPhysicsData().Side,
            //                                     entity.GetWorldPos());
            Gizmos.DrawSphere(targetPos, 0.08f);
            //gdi->Circle(, .08f);
        }

        public float WanderJitter()
        {
            return _wanderJitter;
        }
        public float WanderDistance()
        {
            return _wanderDistance;
        }
        public float WanderRadius()
        {
            return _wanderRadius;
        }

        //public override void Serialize(SteeringBehaviorData data)
        //{
        //    data.last_wander_pos = last_wander_pos;
        //    data._wanderTarget = _wanderTarget;
        //}

        //public override void Deserialize(SteeringBehaviorData data)
        //{
        //    last_wander_pos = data.last_wander_pos;
        //    _wanderTarget = data._wanderTarget;
        //}
    }
}
