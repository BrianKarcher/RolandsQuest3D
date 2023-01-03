using System.Collections.Generic;

namespace RQ.Physics
{
    public enum behavior_type
    {
        none = 0,
        seek = 1,
        flee = 2,
        arrive = 3,
        wander = 4,
        //cohesion = 5,
        //separation = 6,
        //allignment = 7,
        obstacle_avoidance = 8,
        wall_avoidance = 9,
        follow_path = 10,
        pursuit = 11,
        evade = 12,
        interpose = 13,
        hide = 14,
        //flock = 15,
        offset_pursuit = 16,
        radius_clamp = 17,
        seek3d = 18,
        flee3d = 19,
        arrive3d = 20,
        wander3d = 21,
        //cohesion3d = 22,
        //separation3d = 23,
        //allignment3d = 24,
        obstacle_avoidance3d = 25,
        wall_avoidance3d = 26,
        follow_path3d = 27,
        pursuit3d = 28,
        evade3d = 29,
        interpose3d = 30,
        hide3d = 31,
        //flock3d = 32,
        offset_pursuit3d = 33,
        radius_clamp3d = 34
    }

    public class behavior_typeComparer : IEqualityComparer<behavior_type>
    {

        public bool Equals(behavior_type x, behavior_type y)
        {

            return x == y;

        }

        public int GetHashCode(behavior_type x)
        {

            return (int)x;

        }

    }
}
