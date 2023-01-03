using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RQ.Physics
{
    public static class PhysicsHelper
    {
        // Cache the function delegate. Must cache all delegates!
        //public Func<RaycastHit, RaycastHit, int> RaycastDistanceCompareDel;
        public static System.Comparison<RaycastHit> RaycastDistanceCompareDel;

        static PhysicsHelper()
        {
            RaycastDistanceCompareDel = RaycastDistanceCompare;
        }

        private static int RaycastDistanceCompare(RaycastHit lhs, RaycastHit rhs)
        {
            if (lhs.distance > rhs.distance)
                return 1;
            if (rhs.distance > lhs.distance)
                return -1;
            return 0;
        }
    }
}
