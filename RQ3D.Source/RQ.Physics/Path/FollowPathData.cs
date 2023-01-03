using RQ.Physics.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RQ.Physics.Path
{
    [Serializable]
    public class FollowPathData
    {
        public string WaypointUniqueId { get; set; }

        public PathType PathType;

        public PathWalkingDirection PathWalkingDirection;

        public int CurrentWaypoint { get; set; }
    }
}
