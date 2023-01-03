using RQ.AI;
using RQ.Base.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RQ.Physics.SteeringBehaviors2D
{
    public class FollowPath2 : SteeringBehaviorBase2
    {
        //used in path following
        // Determines the distance you can be from a waypoint before triggering the next one
        //public float WaypointSeekDist = .08f;
        //the distance (squared) a vehicle has to be from a path waypoint before
        //it starts seeking to the next waypoint
        public float WaypointSeekDistSq;

        //pointer to any current path
        public Path2 Path { get; set; }

        /// <summary>
        /// The new force gets sent to the WaypointChanged delegate so the receiver knows what the new direction is
        /// </summary>
        public Action<Vector2> WaypointChanged;
        //private bool _isFinished;

        //public bool IsFinished { get { return _isFinished; } set { _isFinished = value; } }

        //public event Action PathComplete;

        public FollowPath2(SteeringBehaviorManager manager)
            : base(manager)
        {
            CreateWaypointSeekDistSq();
            _constantWeight = Constants2.FollowPathWeight;
            //create a Path
            Path = new Path2();
            //Path.LoopOn();
            //_isFinished = false;
            //manager.GetSteeringBehavior(behavior_type.follow_path).;
        }

        private void CreateWaypointSeekDistSq()
        {
            WaypointSeekDistSq = _steeringBehaviorManager.Entity.GetPhysicsData().SteeringData.WaypointSeekDistSq;
        }

        //given a series of Vector2Ds, this method produces a force that will
        //move the agent along the waypoints in order
        protected override Vector2 CalculateForce()
        {
            //if (Path.IsFinished())
            //{
            //    _isFinished = true;
            //    //_steeringBehaviorManager.
            //    return Vector2D.Zero();
            //}
            bool hasWaypointChanged = CheckAndUpdateWaypoint(_steeringBehaviorManager.Entity.GetFeetWorldPosition2());

            var force = SteeringBehaviorCalculations2.FollowPath(_steeringBehaviorManager.Entity, Path);

            if (hasWaypointChanged)
            {
                WaypointChanged?.Invoke(force);
            }
            return force;
        }

        public bool CheckAndUpdateWaypoint(Vector2 position)
        {
            if (!Path.CurrentWaypoint(out var currentWaypointPos))
                return false;
            var feetPos = position;
            var distanceSq = (currentWaypointPos - feetPos).sqrMagnitude;
            if (distanceSq < WaypointSeekDistSq)
            {
                //_steeringBehaviorManager.Entity.Stop();
                Path.SetNextWaypoint();
                return true;
            }
            return false;
        }

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            //if (!Path.HasWaypoints())
            //    return;
            Vector2 wayPoint;
            if (!Path.CurrentWaypoint(out wayPoint))
                return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_steeringBehaviorManager.Entity.transform.position, wayPoint.xz());
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(wayPoint.xz(), 0.04f);

        }

        public void SetPath(List<Vector2> new_path)
        {
            Path.Set(new_path);
        }

        public bool ComparePath(List<Vector2> path)
        {
            //bool isEqual = true;
            var currentWaypoints = Path.GetWaypoints();
            if (currentWaypoints == null)
                return false;
            if (path.Count != currentWaypoints.Count)
                return false;
            for (int i = 0; i < currentWaypoints.Count; i++)
            {
                if (path[i] != currentWaypoints[i])
                    return false;
            }
            return true;
        }
        //public void SetPath(Vector2D[] new_path)
        //{
        //    Path.Set(new_path);
        //}
        //public void CreateRandomPath(int num_waypoints, int mx, int my, int cx, int cy)
        //{
        //    Path.CreateRandomPath(num_waypoints, mx, my, cx, cy);
        //}
    }
}
