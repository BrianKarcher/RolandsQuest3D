using RQ.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RQ.Physics.SteeringBehaviors3D
{
    public class FollowPath3 : SteeringBehaviorBase3
    {
        //used in path following
        // Determines the distance you can be from a waypoint before triggering the next one
        //public float WaypointSeekDist = .08f;
        //the distance (squared) a vehicle has to be from a path waypoint before
        //it starts seeking to the next waypoint
        public float WaypointSeekDistSq;

        //pointer to any current path
        public Path3 Path { get; set; }

        /// <summary>
        /// The new force gets sent to the WaypointChanged delegate so the receiver knows what the new direction is
        /// </summary>
        public Action<Vector3> WaypointChanged;
        //private bool _isFinished;

        //public bool IsFinished { get { return _isFinished; } set { _isFinished = value; } }

        //public event Action PathComplete;

        public FollowPath3(SteeringBehaviorManager manager)
            : base(manager)
        {
            CreateWaypointSeekDistSq();
            ConstantWeight = Constants3.FollowPathWeight;
            //create a Path
            Path = new Path3();
            //Path.LoopOn();
            //_isFinished = false;
            //manager.GetSteeringBehavior(behavior_type.follow_path).;
        }

        private void CreateWaypointSeekDistSq()
        {
            WaypointSeekDistSq = SteeringBehaviorManager.Entity.GetPhysicsData().SteeringData.WaypointSeekDistSq;
        }

        //given a series of Vector2Ds, this method produces a force that will
        //move the agent along the waypoints in order
        protected override Vector3 CalculateForce()
        {
            //if (Path.IsFinished())
            //{
            //    _isFinished = true;
            //    //_steeringBehaviorManager.
            //    return Vector2D.Zero();
            //}
            bool hasWaypointChanged = CheckAndUpdateWaypoint(SteeringBehaviorManager.Entity.GetFeetWorldPosition3());

            var force = SteeringBehaviorCalculations3.FollowPath(SteeringBehaviorManager.Entity, Path);

            if (hasWaypointChanged)
            {
                WaypointChanged?.Invoke(force);
            }
            return force;
        }

        public bool CheckAndUpdateWaypoint(Vector3 position)
        {
            if (!Path.CurrentWaypoint(out var currentWaypointPos))
                return false;
            var feetPos = position;
            var distanceSq = (currentWaypointPos - feetPos).sqrMagnitude;
            if (distanceSq < WaypointSeekDistSq)
            {
                Debug.Log("Setting next waypoint");
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
            Vector3 wayPoint;
            if (!Path.CurrentWaypoint(out wayPoint))
                return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(SteeringBehaviorManager.Entity.transform.position, wayPoint);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(wayPoint, 0.04f);

        }

        public void SetPath(List<Vector3> new_path)
        {
            Path.Set(new_path);
        }

        public bool ComparePath(List<Vector3> path)
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
