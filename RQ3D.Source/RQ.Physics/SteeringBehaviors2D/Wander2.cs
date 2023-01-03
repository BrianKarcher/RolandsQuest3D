using RQ.Base.Extensions;
using UnityEngine;

namespace RQ.Physics.SteeringBehaviors2D
{
    public class Wander2 : SteeringBehaviorBase2
    {
        //private const float WanderJitterPerSec = .5f;
        //private const float WanderJitterPerSec = 1f;
        private Vector2 last_wander_pos = Vector2.zero;

        private Transform _transform;

        //the current position on the wander circle the agent is
        //attempting to steer towards
        private Vector2 _wanderTarget = new Vector2(0,0);

        //explained above
        private float _wanderJitter;
        private float _wanderRadius;
        private float _wanderDistance;

        public Wander2(SteeringBehaviorManager manager, Transform transform)
            : base(manager)
        {
            _transform = transform;
            _constantWeight = Constants2.WanderWeight;
            _wanderDistance = manager.Entity.GetPhysicsData().SteeringData.WanderDist;
            _wanderJitter = manager.Entity.GetPhysicsData().SteeringData.WanderJitterPerSec;
            _wanderRadius = manager.Entity.GetPhysicsData().SteeringData.WanderRad;
            float theta = UnityEngine.Random.Range(0f, 1f) * Mathf.PI * 2;
            //create a vector to a target position on the wander circle
            _wanderTarget = new Vector2(_wanderRadius * Mathf.Cos(theta),
                                        _wanderRadius * Mathf.Sin(theta));


        }        

        //this behavior makes the agent wander about randomly
        protected override Vector2 CalculateForce()
        {
            return SteeringBehaviorCalculations2.Wander(_steeringBehaviorManager.Entity, ref _wanderTarget, _wanderJitter,
                _wanderRadius, _wanderDistance, _transform, ref last_wander_pos);
        }

        public override void OnDrawGizmos()
        {
            var entity = _steeringBehaviorManager.Entity;
            //calculate the center of the wander circle
            //Vector3 m_vTCC = Transformations.PointToWorldSpace(new Vector2(_wanderDistance /** m_pVehicle->BRadius()*/, 0),
            //                         entity.transform.forward,
            //                                     entity.transform.right,
            //                                     entity.GetWorldPos2()).xz();
            Vector3 m_vTCC = entity.transform.TransformPoint(new Vector3(0f, 0f, _wanderDistance));


            //draw the wander circle
            //Debug.
            Gizmos.color = new Color(0, 255, 0, .3f);
            Gizmos.DrawSphere(m_vTCC + new Vector3(0,1f,0), _wanderRadius);
            //gdi->GreenPen();
            //gdi->HollowBrush();
            //gdi->Circle(m_vTCC, m_dWanderRadius * m_pVehicle->BRadius());

            //draw the wander target
            Gizmos.color = new Color(255, 0, 0, .3f);
            //gdi->RedPen();
            //var targetPos = Transformations.PointToWorldSpace((_wanderTarget + new Vector2(_wanderDistance, 0)) /** m_pVehicle->BRadius() */,
            //                              entity.transform.forward,
            //                                     entity.transform.right,
            //                                     entity.GetWorldPos2()).xz();
            var targetPos = entity.transform.TransformPoint(_wanderTarget.xz() + new Vector3(0f, 0f, _wanderDistance));
            Gizmos.DrawSphere(targetPos + new Vector3(0, 1f, 0), 0.5f);
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
