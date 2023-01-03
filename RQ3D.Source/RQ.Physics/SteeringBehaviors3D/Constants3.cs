namespace RQ.Physics.SteeringBehaviors3D
{
    public class Constants3
    {
        public const float NumAgents = 300f;

        public const float NumObstacles = 7f;
        public const float MinObstacleRadius = 10f;
        public const float MaxObstacleRadius = 30f;



        //number of horizontal cells used for spatial partitioning
        public const float NumCellsX = 7f;
        //number of vertical cells used for spatial partitioning
        public const float NumCellsY = 7f;


        //how many samples the smoother will use to average a value
        public const float NumSamplesForSmoothing = 10f;


        //this is used to multiply the steering force AND all the multipliers
        //found in SteeringBehavior
        public const float SteeringForceTweaker = 200.0f;

        public const float SteeringForce = 2.0f;
        public const float MaxSpeed = 150.0f;
        public const float VehicleMass = 1.0f;
        public const float VehicleScale = 3.0f;

        //use these values to tweak the amount that each steering force
        //contributes to the total steering force
        public const float SeparationWeight = 1.0f;
        public const float AlignmentWeight = 1.0f;
        public const float CohesionWeight = 2.0f;
        public const float ObstacleAvoidanceWeight = 10.0f;
        public const float WallAvoidanceWeight = 10.0f;
        public const float WanderWeight = 1.0f;
        public const float SeekWeight = 1.0f;
        public const float FleeWeight = 1.0f;
        public const float ArriveWeight = 1.0f;
        public const float PursuitWeight = 1.0f;
        public const float OffsetPursuitWeight = 1.0f;
        public const float InterposeWeight = 1.0f;
        public const float HideWeight = 1.0f;
        public const float EvadeWeight = 1.0f;
        public const float FollowPathWeight = 1.0f;

        //how close a neighbour must be before an agent perceives it (considers it
        //to be within its neighborhood)
        public const float ViewDistance = 50.0f;

        //used in obstacle avoidance
        public const float MinDetectionBoxLength = 40.0f;

        //used in wall avoidance
        public const float WallDetectionFeelerLength = .08f;

        //these are the probabilities that a steering behavior will be used
        //when the Prioritized Dither calculate method is used to sum
        //combined behaviors
        public const float prWallAvoidance = 0.5f;
        public const float prObstacleAvoidance = 0.5f;
        public const float prSeparation = 0.2f;
        public const float prAlignment = 0.3f;
        public const float prCohesion = 0.6f;
        public const float prWander = 0.8f;
        public const float prSeek = 0.8f;
        public const float prFlee = 0.6f;
        public const float prEvade = 1.0f;
        public const float prHide = 0.8f;
        public const float prArrive = 0.5f;
    }
}
