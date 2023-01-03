using RQ.Common.Components;
using RQ.Physics.Path;
using System.Collections.Generic;
using UnityEngine;

namespace RQ.Controller.Physics
{
    [AddComponentMenu("RQ/Components/Waypoint Component")]
    public class WaypointComponent : ComponentBase<WaypointComponent>
    {
        [SerializeField]
        private GameObject _waypoints;

        [SerializeField]
        private FollowPathData _followPathData;
        public FollowPathData FollowPathData => _followPathData;

        private List<Vector3> _points;

        protected override void Awake()
        {
            base.Awake();
            _points = new List<Vector3>();
            if (_waypoints == null)
                return;
            for (int i = 0; i < _waypoints.transform.childCount; i++)
            {
                _points.Add(_waypoints.transform.GetChild(i).position);
            }
        }

        public List<Vector3> GetWaypoints3()
        {
            return _points;
        }
    }
}
