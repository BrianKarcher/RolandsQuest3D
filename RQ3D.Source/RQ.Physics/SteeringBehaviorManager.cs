using System;
using System.Collections.Generic;
using RQ.Base.Extensions;
using RQ.Physics.SteeringBehaviors2D;
using RQ.Physics.SteeringBehaviors3D;
using UnityEngine;

namespace RQ.Physics
{
    [Serializable]
    public class SteeringBehaviorManager : ISteeringBehaviorManager
    {
        [SerializeField]
        private bool _drawDebug = false;
        public bool DrawDebug => _drawDebug;

        private Dictionary<behavior_type, ISteeringBehavior> _allSteeringBehaviors;
        private Dictionary<behavior_type, ISteeringBehavior> _steeringBehaviors;
        //public IPhysicsAffector SteeringPhysicsAffector { get; set; }

        //these can be used to keep track of friends, pursuers, or prey
        public IPhysicsComponent TargetAgent1;
        public IPhysicsComponent TargetAgent2 { get; set; }

        //the current target
        public Vector3 Target;

        public Vector3 Offset;

        public string SteeringMode;

        //a pointer to the owner of this instance
        public IPhysicsComponent Entity { get; set; }

        //public CollisionComponent CollisionComponent { get; set; }

        //the steering force created by the combined effect of all
        //the selected behaviors
        [SerializeField]
        private Vector3 _steeringForce;

        //how far the agent can 'see'
        public float ViewDistance { get; set; }

        //public bool IsPathFinished { get; set; }

        //private IEntityResolver _entityResolver { get; set; }

        //binary flags to indicate whether or not a behavior should be active
        //private behavior_type _flags;

        public enum summing_method
        {
            weighted_average = 0,
            prioritized = 1,
            dithered = 2
        }

        //is cell space partitioning to be used or not?
        //private bool _cellSpaceOn;

        //what type of method is used to sum any active behavior
        private summing_method _summingMethod;

        //public SteeringBehaviorManager(IBasicPhysicsComponent entity, Transform transform)
        //{

        //}

        private Transform _transform;

        public void Setup(IPhysicsComponent entity, Transform transform)
        {
            _transform = transform;
            Entity = entity;

            //SteeringPhysicsAffector = entity.GetPhysicsAffector("Steering");
            //CollisionComponent = collisionComponent;
            //_entityResolver = entityResolver;
            //_entity = agent;
            //_flags = behavior_type.none;

            //_viewDistance = Constants.ViewDistance;

            TargetAgent1 = null;
            TargetAgent2 = null;

            //_cellSpaceOn = false;
            _summingMethod = summing_method.weighted_average;

            _steeringBehaviors = new Dictionary<behavior_type, ISteeringBehavior>();
            _allSteeringBehaviors = new Dictionary<behavior_type, ISteeringBehavior>();
        }

        public ISteeringBehavior CreateBehavior(behavior_type behaviorType)
        {
            switch (behaviorType)
            {
                case behavior_type.arrive:
                    return new Arrive2(this);
                case behavior_type.arrive3d:
                    return new Arrive3(this);
                case behavior_type.evade:
                    return new Evade2(this);
                case behavior_type.evade3d:
                    return new Evade3(this);
                case behavior_type.flee:
                    return new Flee2(this);
                case behavior_type.flee3d:
                    return new Flee3(this);
                case behavior_type.follow_path:
                    return new FollowPath2(this);
                case behavior_type.follow_path3d:
                    return new FollowPath3(this);
                case behavior_type.hide:
                    return new Hide2(this);
                case behavior_type.hide3d:
                    return new Hide3(this);
                case behavior_type.interpose:
                    return new Interpose2(this);
                case behavior_type.interpose3d:
                    return new Interpose3(this);
                case behavior_type.obstacle_avoidance:
                    return new ObstacleAvoidance2(this);
                case behavior_type.obstacle_avoidance3d:
                    return new ObstacleAvoidance3(this);
                case behavior_type.offset_pursuit:
                    return new OffsetPursuit2(this);
                case behavior_type.offset_pursuit3d:
                    return new OffsetPursuit3(this);
                case behavior_type.pursuit:
                    return new Pursuit2(this);
                case behavior_type.pursuit3d:
                    return new Pursuit3(this);
                case behavior_type.radius_clamp:
                    return new RadiusClamp2(this);
                case behavior_type.radius_clamp3d:
                    return new RadiusClamp3(this);
                case behavior_type.seek:
                    return new Seek2(this);
                case behavior_type.seek3d:
                    return new Seek3(this);
                case behavior_type.wall_avoidance:
                    return new WallAvoidance2(this);
                case behavior_type.wall_avoidance3d:
                    return new WallAvoidance3(this);
                case behavior_type.wander:
                    return new Wander2(this, _transform);
                case behavior_type.wander3d:
                    return new Wander3(this, _transform);
            }

            //_allSteeringBehaviors = new Dictionary<behavior_type, ISteeringBehavior>(new behavior_typeComparer());
            //// This order is very important, it determines the order that the force calculations are run
            //// In general, it is ordered from most important to least important
            //// @todo address this, this is a lot of class instantiation
            //_allSteeringBehaviors.Add(behavior_type.wall_avoidance, new WallAvoidance2(this));
            //_allSteeringBehaviors.Add(behavior_type.obstacle_avoidance, new ObstacleAvoidance2(this));
            //_allSteeringBehaviors.Add(behavior_type.evade, new Evade2(this));
            //_allSteeringBehaviors.Add(behavior_type.flee, new Flee2(this));
            ////_steeringBehaviors.Add(behavior_type.separation, new Separation2(this));
            ////_steeringBehaviors.Add(behavior_type.alignment, new Alignment2(this));
            ////_steeringBehaviors.Add(behavior_type.cohesion, new Cohesion2(this));
            //_allSteeringBehaviors.Add(behavior_type.seek, new Seek2(this));
            //_allSteeringBehaviors.Add(behavior_type.arrive, );
            //_allSteeringBehaviors.Add(behavior_type.wander, new Wander2(this, transform));
            //_allSteeringBehaviors.Add(behavior_type.pursuit, new Pursuit2(this));
            //_allSteeringBehaviors.Add(behavior_type.offset_pursuit, new OffsetPursuit2(this));
            //_allSteeringBehaviors.Add(behavior_type.interpose, new Interpose2(this));
            //_allSteeringBehaviors.Add(behavior_type.hide, new Hide2(this));
            //_allSteeringBehaviors.Add(behavior_type.follow_path, new FollowPath2(this));
            //_allSteeringBehaviors.Add(behavior_type.radius_clamp, new RadiusClamp2(this));
            return null;
        }

        //public IEntityResolver GetEntityResolver()
        //{
        //    return _entityResolver;
        //}

        //public MovingEntity GetEntity()
        //{
        //    return _entity;
        //}

        //public SpriteBase GetTargetAgent1()
        //{
        //    return _targetAgent1;
        //}

        //public SpriteBase GetTargetAgent2()
        //{
        //    return _targetAgent2;
        //}

        //public void SetEntity(MovingEntity entity)
        //{
        //    _entity = entity;
        //}

        public Vector3 GetTarget3()
        {
            return Target;
        }

        public void SetTarget3(Vector3 target)
        {
            Target = target;
        }

        //public void Set

        //this function tests if a specific bit of m_iFlags is set
        public bool IsOn(behavior_type bt)
        {
            return _steeringBehaviors.ContainsKey(bt);
        }

        public void TurnOn(behavior_type behaviortype)
        {
            // _allSteeringBehaviors caches the steering behaviors
            if (!_allSteeringBehaviors.TryGetValue(behaviortype, out var steeringBehavior))
            {
                steeringBehavior = CreateBehavior(behaviortype);
                _allSteeringBehaviors[behaviortype] = steeringBehavior;
            }
            _steeringBehaviors[behaviortype] = steeringBehavior;
            CalculateSteeringModes();
        }
        public void TurnOff(behavior_type behaviortype)
        {
            _steeringBehaviors.Remove(behaviortype);
            CalculateSteeringModes();
        }

        public void CalculateSteeringModes()
        {
            SteeringMode = string.Empty;
            foreach (var behavior in _steeringBehaviors)
            {
                SteeringMode += behavior.Key + " ";
            }
        }

        public ISteeringBehavior GetSteeringBehavior(behavior_type behaviortype)
        {
            return _steeringBehaviors[behaviortype];
        }

        //----------------------- Calculate --------------------------------------
        //
        //  calculates the accumulated steering force according to the method set
        //  in m_SummingMethod
        //------------------------------------------------------------------------
        public Vector3 Calculate()
        {
            //reset the steering force
            _steeringForce = Vector2.zero;

            //use space partitioning to calculate the neighbours of this vehicle
            //if switched on. If not, use the standard tagging system
            //if (!isSpacePartitioningOn())
            //{
            //    //tag neighbors if any of the following 3 group behaviors are switched on
            //    if (On(separation) || On(allignment) || On(cohesion))
            //    {
            //        m_pVehicle->World()->TagVehiclesWithinViewRange(m_pVehicle, m_dViewDistance);
            //    }
            //}
            //else
            //{

            //    //calculate neighbours in cell-space if any of the following 3 group
            //    //behaviors are switched on
            //    if (On(behavior_type.separation) || On(behavior_type.allignment) || On(behavior_type.cohesion))
            //    {
            //        m_pVehicle->World()->CellSpace()->CalculateNeighbors(m_pVehicle->Pos(), m_dViewDistance);
            //    }
            //}

            switch (_summingMethod)
            {
                case summing_method.weighted_average:

                    _steeringForce = CalculateWeightedSum();
                    break;

                case summing_method.prioritized:

                    _steeringForce = CalculatePrioritized();
                    break;

                case summing_method.dithered:

                    _steeringForce = CalculateDithered();
                    break;

                default:
                    _steeringForce = new Vector3(0, 0, 0);
                    break;
            }//end switch

            return _steeringForce;
        }

        //---------------------- CalculatePrioritized ----------------------------
        //
        //  this method calls each active steering behavior in order of priority
        //  and acumulates their forces until the max steering force magnitude
        //  is reached, at which time the function returns the steering force 
        //  accumulated to that  point
        //------------------------------------------------------------------------
        private Vector3 CalculatePrioritized()
        {
            var force = Vector3.zero;

            foreach (KeyValuePair<behavior_type, ISteeringBehavior> behavior in _steeringBehaviors)
            {
                force = behavior.Value.CalculatePrioritized();

                if (!AccumulateForce(ref _steeringForce, force)) return _steeringForce;
            }

            return _steeringForce;
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
        private Vector3 CalculateDithered()
        {
            //reset the steering force
            _steeringForce = Vector3.zero;

            foreach (KeyValuePair<behavior_type, ISteeringBehavior> behavior in _steeringBehaviors)
            {
                _steeringForce = behavior.Value.CalculateDithered();

                if (_steeringForce != Vector3.zero)
                {
                    var maxForce = Entity.GetPhysicsData().MaxForce;
                    _steeringForce = Vector3.ClampMagnitude(_steeringForce, maxForce);
                    return _steeringForce;
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
        private Vector3 CalculateWeightedSum()
        {
            foreach (KeyValuePair<behavior_type, ISteeringBehavior> behavior in _steeringBehaviors)
            {
                _steeringForce += behavior.Value.CalculateWeightedSum();
            }
            var maxForce = Entity.GetPhysicsData().MaxForce;
            _steeringForce = Vector3.ClampMagnitude(_steeringForce, maxForce);
            return _steeringForce;
        }

        //--------------------- AccumulateForce ----------------------------------
        //
        //  This function calculates how much of its max steering force the 
        //  vehicle has left to apply and then applies that amount of the
        //  force to add.
        //------------------------------------------------------------------------
        private bool AccumulateForce(ref Vector3 RunningTot, Vector3 ForceToAdd)
        {
            //calculate how much steering force the vehicle has used so far
            float MagnitudeSoFar = RunningTot.magnitude;

            //calculate how much steering force remains to be used by this vehicle
            float MagnitudeRemaining = Entity.GetPhysicsData().MaxForce - MagnitudeSoFar;

            //return false if there is no more force left to use
            if (MagnitudeRemaining <= 0.0) return false;

            //calculate the magnitude of the force we want to add
            float MagnitudeToAdd = ForceToAdd.magnitude;

            //if the magnitude of the sum of ForceToAdd and the running total
            //does not exceed the maximum force available to this vehicle, just
            //add together. Otherwise add as much of the ForceToAdd vector is
            //possible without going over the max.
            if (MagnitudeToAdd < MagnitudeRemaining)
            {
                RunningTot += ForceToAdd;
            }

            else
            {
                //add it to the steering force
                RunningTot += ForceToAdd.normalized * MagnitudeRemaining;
            }

            return true;
        }

        //calculates the component of the steering force that is parallel
        //with the vehicle heading
        public float ForwardComponent()
        {
            //return Vector2.Dot(Entity.GetPhysicsData().Heading, _steeringForce);
            return Vector2.Dot(_transform.forward, _steeringForce);
        }

        //calculates the component of the steering force that is perpendicuar
        //with the vehicle heading
        public float SideComponent()
        {
            return Vector2.Dot(_transform.right, _steeringForce);
            //return Vector2.Dot(Entity.GetPhysicsData().Side, _steeringForce);
            //return Entity.GetPhysicsData().Side.Dot(_steeringForce);
        }

        public Vector3 GetForce()
        {
            return _steeringForce;
        }

        public void SetSummingMethod(summing_method sm)
        {
            _summingMethod = sm;
        }

        //public SteeringBehaviorData Serialize()
        //{
        //    var data = new SteeringBehaviorData();

        //    if (TargetAgent1 != null)
        //        data.TargetAgent1 = TargetAgent1.UniqueId;
        //    else
        //        data.TargetAgent1 = string.Empty;
        //    if (TargetAgent2 != null)
        //        data.TargetAgent2 = TargetAgent2.UniqueId;
        //    else
        //        data.TargetAgent2 = string.Empty;
        //    data.Offset = Offset;

        //    //the current target
        //    data.Target = Target;

        //    var activeBehaviorTypes = new List<behavior_type>();

        //    foreach (var behavior in _steeringBehaviors)
        //    {
        //        if (behavior.Value.IsOn())
        //        {
        //            activeBehaviorTypes.Add(behavior.Key);
        //        }
        //        behavior.Value.Serialize(data);
        //    }

        //    data.ActiveBehaviors = activeBehaviorTypes;

        //    return data;
        //}

        //public void Deserialize(SteeringBehaviorData data)
        //{
        //    //var entityMap = EntityController._instance.GetEntityMap();

        //    // TODO Fix this, we have an issue with locating a remote entity with just a component GUID
        //    // Not an entity GUID
        //    //if (data.TargetAgent1 == string.Empty)
        //    //    TargetAgent1 = null;
        //    //else
        //    //    TargetAgent1 = (IKineticObject) _entityResolver.GetEntity(entityMap[data.TargetAgent1]);

        //    //if (data.TargetAgent2 == -1)
        //    //    TargetAgent2 = null;
        //    //else
        //    //    TargetAgent2 = (IKineticObject) _entityResolver.GetEntity(entityMap[data.TargetAgent2]);
        //    Target = data.Target;
        //    Offset = data.Offset;

        //    foreach (var behavior in _steeringBehaviors)
        //    {
        //        behavior.Value.Deserialize(data);
        //        behavior.Value.TurnOff();
        //    }

        //    data.ActiveBehaviors.ForEach(i => TurnOn(i));
        //}

        public IPhysicsComponent GetTargetAgent1()
        {
            return TargetAgent1;
        }

        public void SetTargetAgent1(IPhysicsComponent target)
        {
            TargetAgent1 = target;
        }

        public Vector3 GetOffset()
        {
            return Offset;
        }

        public void OnDrawGizmos()
        {
            if (_steeringBehaviors == null)
                return;
            foreach (KeyValuePair<behavior_type, ISteeringBehavior> behavior in _steeringBehaviors)
            {
                behavior.Value.OnDrawGizmos();
            }
        }
    }
}
