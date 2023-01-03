using System.Collections.Generic;
using UnityEngine;

namespace RQ.Physics.SteeringBehaviors3D
{
    public class WallAvoidance3 : SteeringBehaviorBase3, ISteeringBehavior
    {
        //a vertex buffer to contain the feelers rqd for wall avoidance  
        //private List<Vector2D> _feelers;

        //the length of the 'feeler/s' used in wall detection
        private float _wallDetectionFeelerLength;
        private IPhysicsComponent _physicsComponent;
        //private CollisionComponent _collisionComponent;
        //private AIComponent _aIComponent;
        //private IList<Vector2D> _feelers;
        private Vector3[] _feelers = new Vector3[3];
        //private Vector2D _closestFeeler;
        private bool[] _feelersTouching = new bool[3];

        public WallAvoidance3(SteeringBehaviorManager manager)
            : base(manager)
        {
            _physicsComponent = SteeringBehaviorManager.Entity;
            //GetCollisionComponent();

            //_collisionComponent = _steeringBehaviorManager.CollisionComponent;
            ConstantWeight = Constants3.WallAvoidanceWeight;
            //_feelers = new List<Vector2D>();
            _wallDetectionFeelerLength = Constants3.WallDetectionFeelerLength;
        }
        //this returns a steering force which will attempt to keep the agent 
        //away from any obstacles it may encounter
        protected override Vector3 CalculateForce()
        {
            //return Vector2D.Zero();
            return CalculateWallAvoidance();
        }

        //public List<Vector2D> GetFeelers()
        //{
        //    return _feelers;
        //}

        //--------------------------- WallAvoidance --------------------------------
        //
        //  This returns a steering force that will keep the agent away from any
        //  walls it may encounter
        //------------------------------------------------------------------------
        public Vector3 CalculateWallAvoidance()
        {
            //the feelers are contained in a std::vector, m_Feelers
            CalculateFeelers();

            //float DistToThisIP    = 0.0f;
            float DistToClosestIP = float.MaxValue;

            //this will hold an index into the vector of walls
            //int ClosestWall = -1;

            Vector3 SteeringForce = Vector3.zero;
            //          point,         //used for storing temporary info
            //          ClosestPoint;  //holds the closest intersection point

            //if (_collisionComponent == null)
            //    GetCollisionComponent();
            //if (_aIComponent == null)
            //    _aIComponent = base.SteeringBehaviorManager.Entity.GetComponentRepository().Components.GetComponent<AIComponent>();
            // Still null? Cannot locate, exit gracefully.
            //if (_collisionComponent == null)
            //    return Vector2.zero;

            //var layerMask = _collisionComponent.GetEnvironmentLayerMask();
            //var layerMask = 1 << LayerMask.NameToLayer("Environment");
            var layerMask = 0;
            var avoidLayersMask = SteeringBehaviorManager.Entity.GetPhysicsData().AvoidLayersMask;
            //var avoidLayersMasks = _aIComponent.AvoidLayersMasks;
            layerMask = avoidLayersMask;
            //foreach (var avoidLayerMask in avoidLayersMasks)
            //    layerMask |= avoidLayerMask;
            //LayerMask layerMask;
            //EntityController.
            //if (level == Enum.LevelLayer.LevelOne)
            //    layerMask = GameController.

            RaycastHit closestRaycast = new RaycastHit();
            Vector2 closestFeeler = Vector2.zero;
            bool found = false;
            int hitCount = 0;
            var feetPos = _physicsComponent.GetFeetWorldPosition3() + 
                (_physicsComponent.transform.forward * _physicsComponent.GetPhysicsData().SteeringData.FeelerOffset);
            //foreach (var feeler in _feelers)
            for (int i = 0; i < 3; i++)
            {
                var feeler = _feelers[i];

                bool hit = UnityEngine.Physics.Raycast(feetPos, feeler,
                    out var raycastHit, feeler.magnitude, layerMask);
                _feelersTouching[i] = hit;
                if (!hit)
                    continue;
                //var rayHit = Physics2D.Raycast(_physicsComponent.GetFeetWorldPosition(), feeler,
                //    _wallDetectionFeelerLength, layerMask);

                if (raycastHit.collider != null)
                {
                    if (raycastHit.distance < DistToClosestIP)
                    {
                        closestRaycast = raycastHit;
                        closestFeeler = feeler;
                        DistToClosestIP = raycastHit.distance;
                        found = true;                        
                    }
                    hitCount++;
                }
            }
            
            // All three feelers hit? Just turn left. Turning left prevents the bot from getting stuck in the wall.
            if (hitCount == 3)
            {
                var heading = _physicsComponent.transform.forward;
                return new Vector2(-heading.y, heading.x);
            }

            //if an intersection point has been detected, calculate a force  
            //that will direct the agent away
            if (found)
            {
                //_closestFeeler = closestFeeler;
                //calculate by what distance the projected position of the agent
                //will overshoot the wall
                var overShoot = closestFeeler.magnitude - closestRaycast.distance;

                //create a force in the direction of the wall normal, with a 
                //magnitude of the overshoot
                SteeringForce = closestRaycast.normal * overShoot;
            }



            //examine each feeler in turn
            //for (unsigned int flr=0; flr<m_Feelers.size(); ++flr)
            //{
            //  //run through each wall checking for any intersection points
            //  for (unsigned int w=0; w<walls.size(); ++w)
            //  {
            //    if (LineIntersection2D(m_pVehicle->Pos(),
            //                           m_Feelers[flr],
            //                           walls[w].From(),
            //                           walls[w].To(),
            //                           DistToThisIP,
            //                           point))
            //    {
            //      //is this the closest found so far? If so keep a record
            //      if (DistToThisIP < DistToClosestIP)
            //      {
            //        DistToClosestIP = DistToThisIP;

            //        ClosestWall = w;

            //        ClosestPoint = point;
            //      }
            //    }
            //  }//next wall



            //  if (ClosestWall >=0)
            //  {

            //  }

            //}//next feeler

            //if (SteeringForce.magnitude != 0)
            //{
            //    //Debug.Log("Entity " + _physicsComponent.GetName() + " WallAvoidance force = " + SteeringForce.ToString());
            //}

            return SteeringForce;
        }

        //private void GetCollisionComponent()
        //{
        //    if (_collisionComponent == null)
        //        _collisionComponent = SteeringBehaviorManager.CollisionComponent;
        //}

        //------------------------------- CreateFeelers --------------------------
        //
        //  Creates the antenna utilized by WallAvoidance
        //------------------------------------------------------------------------
        public void CalculateFeelers()
        {
            var heading = _physicsComponent.transform.forward * 0.4f; // make the feelers go only one tile ahead
            var halfPi = Mathf.PI / 2;

            //feeler pointing straight in front
            _feelers[0] = heading;

            // Get direction vector pointing to the left and forward at 45 degrees
            _feelers[1] = SteeringBehaviorManager.Entity.transform.TransformDirection(-1, 0, 1);

            // Get direction vector pointing to the right and forward at 45 degrees
            _feelers[2] = SteeringBehaviorManager.Entity.transform.TransformDirection(1, 0, 1);

            //var transform = _entity.GetTransform();
            //transform.Rotate()
            //feeler to left
            //Vector3 temp = heading;
            ////Mathf.
            //temp = Transformations.Vec2DRotateAroundOrigin(temp, halfPi * 3.5f);
            ////Vec2DRotateAroundOrigin(temp, );
            //_feelers[1] = temp;

            ////feeler to right
            //temp = heading;
            //temp = Transformations.Vec2DRotateAroundOrigin(temp, halfPi * 0.5f);
            ////Vec2DRotateAroundOrigin(temp, halfPi * 0.5f);
            //_feelers[2] = temp;
        }

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            //foreach (var ray in _feelers)
            for (int i = 0; i < 3; i++)
            {
                if (_feelersTouching[i])
                {
                    Gizmos.color = new Color(255, 0, 0, 0.3f);
                }
                else
                    Gizmos.color = new Color(255, 255, 255, 0.3f);
                var feetPos = _physicsComponent.GetFeetWorldPosition3() +
                    (_physicsComponent.transform.forward * _physicsComponent.GetPhysicsData().SteeringData.FeelerOffset);
                //var pos = _steeringBehaviorManager.Entity.transform.position;
                var pos = feetPos;
                Gizmos.DrawLine(pos, pos + _feelers[i]);
            }
            
        }

        //public override void Serialize(SteeringBehaviorData data)
        //{
        //    //data.Feelers = _feelers;
        //}

        //public override void Deserialize(SteeringBehaviorData data)
        //{
        //    //_feelers = data.Feelers;
        //}
    }
}
