using UnityEngine;

namespace RQ.Physics.SteeringBehaviors3D
{
    public class Hide3 : SteeringBehaviorBase3
    {
        public Hide3(SteeringBehaviorManager manager)
            : base(manager)
        {
            ConstantWeight = Constants3.HideWeight;
        }

        // TODO Implement this, it sounds like we can use it and it'll be fun!

        //helper method for Hide. Returns a position located on the other
        //side of an obstacle to the pursuer
        //private Vector2D GetHidingPosition(Vector2D posOb,
        //                            float radiusOb,
        //                            Vector2D posHunter);

        //given another agent position to hide from and a list of BaseGameEntitys this
        //method attempts to put an obstacle between itself and its opponent
        protected override Vector3 CalculateForce()
        {
            return Vector3.zero;
            //return SteeringBehaviorCalculations.Hide(_steeringBehaviorManager.Target, _steeringBehaviorManager.Entity);
        }
    }
}
