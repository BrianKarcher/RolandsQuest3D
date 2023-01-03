using RQ.Base.Extensions;
using UnityEngine;

namespace RQ.Physics.SteeringBehaviors2D
{
    public class Feeler
    {
        public Vector3 LocalDirection { get; set; }
        public Vector2 Direction { get; set; }
        public bool IsTouching { get; set; }
        public Vector2 LocalMoveAway { get; set; }
    }

    public class WallAvoidance2 : SteeringBehaviorBase2
    {
        //a vertex buffer to contain the feelers rqd for wall avoidance  
        //private List<Vector2D> _feelers;

        //the length of the 'feeler/s' used in wall detection
        private float _wallDetectionFeelerLength;
        private readonly IPhysicsComponent _physicsComponent;
        //private CollisionComponent _collisionComponent;
        //private AIComponent _aIComponent;
        //private IList<Vector2D> _feelers;
        //private Vector2[] _feelers = new Vector2[3];
        //private Vector2D _closestFeeler;
        //private bool[] _feelersTouching = new bool[3];
        private bool _isMovingAwayFromWall;
        private Vector2 _moveDirection;
        private Feeler[] _feelers;

        public WallAvoidance2(SteeringBehaviorManager manager)
            : base(manager)
        {
            _physicsComponent = _steeringBehaviorManager.Entity;

            //_collisionComponent = _steeringBehaviorManager.CollisionComponent;
            _constantWeight = Constants2.WallAvoidanceWeight;
            //_feelers = new List<Vector2D>();
            //_wallDetectionFeelerLength = Constants2.WallDetectionFeelerLength;
            _wallDetectionFeelerLength = _physicsComponent.GetPhysicsData().SteeringData.WallDetectionFeelerLength;
            _feelers = new Feeler[3];
            // Straight ahead
            _feelers[0] = new Feeler();
            _feelers[0].LocalDirection = new Vector3(0f, 0f, 1f);
            _feelers[0].LocalMoveAway = new Vector2(-1f, -1f);
            // Ahead and to the right
            _feelers[1] = new Feeler();
            _feelers[1].LocalDirection = new Vector3(1f, 0f, 1f);
            _feelers[1].LocalMoveAway = new Vector2(-1f, 0f);
            // Ahead and to the left
            _feelers[2] = new Feeler();
            _feelers[2].LocalDirection = new Vector3(-1f, 0f, 1f);
            _feelers[2].LocalMoveAway = new Vector2(1f, 0f);
        }

        //this returns a steering force which will attempt to keep the agent 
        //away from any obstacles it may encounter
        protected override Vector2 CalculateForce()
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
        public Vector2 CalculateWallAvoidance()
        {
            //the feelers are contained in a std::vector, m_Feelers
            CalculateFeelers();

            //float DistToThisIP    = 0.0f;
            float DistToClosestIP = float.MaxValue;

            //this will hold an index into the vector of walls
            //int ClosestWall = -1;

            Vector2 SteeringForce = Vector2.zero;
            //          point,         //used for storing temporary info
            //          ClosestPoint;  //holds the closest intersection point

            //if (_collisionComponent == null)
            //    GetCollisionComponent();
            //if (_aIComponent == null)
            //    _aIComponent = base._steeringBehaviorManager.Entity.GetComponentRepository().Components.GetComponent<AIComponent>();
            // Still null? Cannot locate, exit gracefully.
            //if (_collisionComponent == null)
            //    return Vector2.zero;

            //var layerMask = _collisionComponent.GetEnvironmentLayerMask();
            //var layerMask = 1 << LayerMask.NameToLayer("Environment");
            var layerMask = 0;
            var avoidLayerMask = _steeringBehaviorManager.Entity.GetPhysicsData().AvoidLayersMask;
            //var avoidLayersMasks = _aIComponent.AvoidLayersMasks;
            //foreach (var avoidLayerMask in avoidLayersMasks)
            layerMask = avoidLayerMask.value;
            //LayerMask layerMask;
            //EntityController.
            //if (level == Enum.LevelLayer.LevelOne)
            //    layerMask = GameController.

            RaycastHit closestRaycast = new RaycastHit();
            //Vector2 closestFeeler = Vector2.zero;
            bool found = false;
            int hitCount = 0;
            var feetPos = _physicsComponent.GetFeetWorldPosition3() + 
                (_physicsComponent.transform.forward * _physicsComponent.GetPhysicsData().SteeringData.FeelerOffset);
            //foreach (var feeler in _feelers)
            Feeler closestFeeler = null;
            for (int i = 0; i < 3; i++)
            {
                var feeler = _feelers[i];

                bool hit = UnityEngine.Physics.Raycast(feetPos, feeler.Direction.xz(),
                    out var raycastHit, feeler.Direction.magnitude, layerMask);
                _feelers[i].IsTouching = hit;
                if (!hit)
                    continue;
                //var rayHit = Physics2D.Raycast(_physicsComponent.GetFeetWorldPosition(), feeler,
                //    _wallDetectionFeelerLength, layerMask);

                if (raycastHit.collider != null)
                {
                    if (raycastHit.distance < DistToClosestIP)
                    {
                        closestRaycast = raycastHit;
                        closestFeeler = _feelers[i];
                        //closestFeeler = feeler;
                        DistToClosestIP = raycastHit.distance;
                        found = true;                        
                    }
                    hitCount++;
                }
            }

            if (_isMovingAwayFromWall)
            {
                // Continue moving away from the wall until none of the feelers hit

            }
            else
            {

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
                var overShoot = closestFeeler.Direction.magnitude - closestRaycast.distance;

                //create a force in the direction of the wall normal, with a 
                //magnitude of the overshoot
                //SteeringForce = closestRaycast.normal.xz() * overShoot;
                var steeringForceLocal = closestFeeler.LocalMoveAway.xz();
                SteeringForce = _steeringBehaviorManager.Entity.transform.TransformDirection(steeringForceLocal).xz() * overShoot;
                _isMovingAwayFromWall = true;
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

            if (SteeringForce.magnitude != 0)
            {
                //Debug.Log("Entity " + _physicsComponent.GetName() + " WallAvoidance force = " + SteeringForce.ToString());
            }

            return SteeringForce;
        }

        //------------------------------- CreateFeelers --------------------------
        //
        //  Creates the antenna utilized by WallAvoidance
        //------------------------------------------------------------------------
        public void CalculateFeelers()
        {
            var heading = _physicsComponent.transform.forward * 1.0f; // make the feelers go only one unit ahead
            //var halfPi = Mathf.PI / 2;

            //feeler pointing straight in front
            _feelers[0].Direction = _steeringBehaviorManager.Entity.transform.TransformDirection(_feelers[0].LocalDirection).xz();
            _feelers[0].Direction = _feelers[0].Direction.normalized * _wallDetectionFeelerLength;
            //_feelers[0] = heading.xz();

            //var transform = _entity.GetTransform();
            //transform.Rotate()
            //feeler to left
            //Vector2 temp = heading;
            //Mathf.
            //Quaternion.AngleAxis(30, )
            //Vector2 temp = 
            //temp = Transformations.Vec2DRotateAroundOrigin(temp, halfPi * 3.5f);
            //Vec2DRotateAroundOrigin(temp, );
            _feelers[1].Direction = _steeringBehaviorManager.Entity.transform.TransformDirection(_feelers[1].LocalDirection).xz();
            _feelers[1].Direction = _feelers[1].Direction.normalized * _wallDetectionFeelerLength;

            //feeler to right
            //temp = heading;
            //temp = Transformations.Vec2DRotateAroundOrigin(temp, halfPi * 0.5f);
            //Vec2DRotateAroundOrigin(temp, halfPi * 0.5f);
            _feelers[2].Direction = _steeringBehaviorManager.Entity.transform.TransformDirection(_feelers[2].LocalDirection).xz();
            _feelers[2].Direction = _feelers[2].Direction.normalized * _wallDetectionFeelerLength;
        }

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            //foreach (var ray in _feelers)
            for (int i = 0; i < 3; i++)
            {
                if (_feelers[i].IsTouching)
                {
                    Gizmos.color = new Color(255, 0, 0, 0.3f);
                }
                else
                    Gizmos.color = new Color(255, 255, 255, 0.3f);
                var feetPos = _physicsComponent.GetFeetWorldPosition3() +
                    (_physicsComponent.transform.forward * _physicsComponent.GetPhysicsData().SteeringData.FeelerOffset);
                //var pos = _steeringBehaviorManager.Entity.transform.position;
                var pos = feetPos;
                Gizmos.DrawLine(pos, pos + _feelers[i].Direction.xz());
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
