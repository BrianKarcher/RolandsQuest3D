using RQ.AI;
using UnityEngine;
using RQ.Common;
using RQ.Common.Components;
using RQ.Common.Container;
using RQ.Base.Extensions;

namespace RQ.Physics.SteeringBehaviors3D
{
    // Many of the Steering Behavior calculations call one another, so placing them all here
    public static class SteeringBehaviorCalculations3
    {
        //------------------------------- Seek -----------------------------------
        //
        //  Given a target, this behavior returns a steering force which will
        //  direct the agent towards the target
        //------------------------------------------------------------------------
        public static Vector3 Seek(Vector3 TargetPos, IPhysicsComponent entity)
        {
            //var physicsAffector = entity.GetSteering().SteeringPhysicsAffector;
            var DesiredVelocity = (TargetPos - entity.GetFeetWorldPosition3()).normalized
                                    * entity.GetPhysicsData().MaxSpeed;

            return (DesiredVelocity - entity.GetVelocity3());
        }

        //----------------------------- Flee -------------------------------------
        //
        //  Does the opposite of Seek
        //------------------------------------------------------------------------
        public static Vector3 Flee(Vector3 TargetPos, IPhysicsComponent entity)
        {
            //var physicsAffector = entity.GetSteering().SteeringPhysicsAffector;
            //only flee if the target is within 'panic distance'. Work in distance
            //squared space.
            /* const double PanicDistanceSq = 100.0f * 100.0;
             if (Vec2DDistanceSq(m_pVehicle->Pos(), target) > PanicDistanceSq)
             {
               return Vector2D(0,0);
             }
             */

            var DesiredVelocity = (entity.GetWorldPos3() - TargetPos).normalized
                                      * entity.GetPhysicsData().MaxSpeed;

            return (DesiredVelocity - entity.GetVelocity3());
        }

        //--------------------------- Arrive -------------------------------------
        //
        //  This behavior is similar to seek but it attempts to arrive at the
        //  target with a zero velocity
        //------------------------------------------------------------------------
        public static Vector3 Arrive(Vector3 TargetPos, IPhysicsComponent entity,
                                          Deceleration3 deceleration)
        {
            //var physicsAffector = entity.GetSteering().SteeringPhysicsAffector;
            var ToTarget = TargetPos - entity.GetFeetWorldPosition3();

            //calculate the distance to the target
            float dist = ToTarget.magnitude;

            if (dist > 0)
            {
                //because Deceleration is enumerated as an int, this value is required
                //to provide fine tweaking of the deceleration..
                const float DecelerationTweaker = 0.3f;

                //calculate the speed required to reach the target given the desired
                //deceleration
                float speed = dist / ((float)deceleration * DecelerationTweaker);

                //make sure the velocity does not exceed the max
                speed = Mathf.Min(speed, entity.GetPhysicsData().MaxSpeed);

                //from here proceed just like Seek except we don't need to normalize 
                //the ToTarget vector because we have already gone to the trouble
                //of calculating its length: dist. 
                var DesiredVelocity = ToTarget * speed / dist;

                return (DesiredVelocity - entity.GetVelocity3());
            }

            return Vector3.zero;
        }

        //------------------------------ Pursuit ---------------------------------
        //
        //  this behavior creates a force that steers the agent towards the 
        //  evader
        //------------------------------------------------------------------------
        public static Vector3 Pursuit(IPhysicsComponent evader, IPhysicsComponent entity)
        {
            //var physicsAffector = entity.GetSteering().SteeringPhysicsAffector;
            //if the evader is ahead and facing the agent then we can just seek
            //for the evader's current position.
            var ToEvader = evader.GetWorldPos3() - entity.GetWorldPos3();

            float RelativeHeading = Vector2.Dot(entity.transform.forward, evader.transform.forward);

            if (Vector2.Dot(ToEvader, entity.transform.forward) > 0 &&
                 (RelativeHeading < -0.95f))  //acos(0.95)=18 degs
            {
                return Seek(evader.GetWorldPos3(), entity);
            }

            //Not considered ahead so we predict where the evader will be.

            //the lookahead time is propotional to the distance between the evader
            //and the pursuer; and is inversely proportional to the sum of the
            //agent's velocities
            float LookAheadTime = ToEvader.magnitude /
                                  (entity.GetPhysicsData().MaxSpeed + evader.GetVelocity3().magnitude);

            //now seek to the predicted future position of the evader
            return Seek(evader.GetFeetWorldPosition3() + evader.GetVelocity3() * LookAheadTime, entity);
        }

        //----------------------------- Evade ------------------------------------
        //
        //  similar to pursuit except the agent Flees from the estimated future
        //  position of the pursuer
        //------------------------------------------------------------------------
        public static Vector3 Evade(IPhysicsComponent pursuer, IPhysicsComponent entity)
        {
            //var physicsAffector = entity.GetSteering().SteeringPhysicsAffector;
            /* Not necessary to include the check for facing direction this time */

            Vector3 ToPursuer = pursuer.GetWorldPos3() - entity.GetWorldPos3();

            //uncomment the following two lines to have Evade only consider pursuers 
            //within a 'threat range'
            const float ThreatRange = 100.0f;
            if (ToPursuer.sqrMagnitude > ThreatRange * ThreatRange) return Vector3.zero;

            //the lookahead time is propotional to the distance between the pursuer
            //and the pursuer; and is inversely proportional to the sum of the
            //agents' velocities
            float LookAheadTime = ToPursuer.magnitude /
                                   (entity.GetPhysicsData().MaxSpeed + pursuer.GetVelocity3().magnitude);

            //now flee away from predicted future position of the pursuer
            return Flee(pursuer.GetFeetWorldPosition3() + pursuer.GetVelocity3() * LookAheadTime, entity);
        }

        //--------------------------- Wander -------------------------------------
        //
        //  This behavior makes the agent wander about randomly
        //------------------------------------------------------------------------
        //public static Vector2D Wander(SpriteBase entity, ref Vector2D wanderTarget, float wanderJitter,
        //    float wanderRadius, float wanderDistance, Transform transform)
        //{
        //    //this behavior is dependent on the update rate, so this line must
        //    //be included when using time independent framerate.
        //    float JitterThisTimeSlice = wanderJitter;

        //    //first, add a small random vector to the target's position
        //    wanderTarget += new Vector2D(RandomClamped() * JitterThisTimeSlice,
        //                                RandomClamped() * JitterThisTimeSlice);

        //    //reproject this new vector back on to a unit circle
        //    wanderTarget.Normalize();

        //    //increase the length of the vector to the same as the radius
        //    //of the wander circle
        //    wanderTarget *= wanderRadius;

        //    //move the target into a position WanderDist in front of the agent
        //    Vector2D target = wanderTarget + new Vector2D(wanderDistance, 0);

        //    //project the target into world space
        //    //Vector2D Target = Transformations.PointToWorldSpace(target,
        //    //                                     entity.GetHeading(),
        //    //                                     entity.GetSide(),
        //    //                                     entity.GetPos());

        //    //Transform t;
        //    //t.
        //    //Mathf.

        //    Vector2D Target = transform.TransformPoint(target.ToVector3(0f));

        //    //and steer towards it
        //    return Target - entity.GetPos();
        //}

        private static float RandomClamped()
        {
            return UnityEngine.Random.Range(-1f, 1f);
        }


        //    public static Vector2D Wander(IBasicPhysicsComponent entity, ref Vector2D wanderTarget, float wanderJitter,
        //float wanderRadius, float wanderDistance, Transform transform, ref Vector2D last_wander_pos)
        //    {
        //        //Vector3 old_target = wander_target;

        //        wanderTarget += (wanderDistance * -transform.forward) + (transform.position - last_wander_pos);
        //        wanderTarget += new Vector3(UnityEngine.Random.Range(-wanderJitter, wanderJitter),
        //                                    UnityEngine.Random.Range(-wanderJitter, wanderJitter),
        //                                    UnityEngine.Random.Range(-wanderJitter, wanderJitter));

        //        last_wander_pos = transform.position;

        //        //put to circle
        //        var to_wander_target = wanderTarget - transform.position;
        //        to_wander_target.Normalize();
        //        Vector2D desired_velocity = to_wander_target * entity.GetPhysicsData().MaxSpeed;
        //        to_wander_target *= wanderRadius;

        //        //more forward distance of circle
        //        to_wander_target += (transform.forward * wanderDistance);
        //        wanderTarget = transform.position + to_wander_target;

        //        Vector2D steering_force = desired_velocity - entity.GetPhysicsData().Velocity;

        //        //zero out z axis for 2d simulation
        //        steering_force = new Vector3(steering_force.x, steering_force.y, 0);

        //        return steering_force;

        //    }

        public static Vector3 RadiusClamp(IPhysicsComponent entity)
        {
            var data = entity.GetPhysicsData();
            var target = entity.GetSteering().GetTarget3().xz();
            // Estimate position entity will be in, in one second
            var projectedNearFutureLocation = entity.transform.position + entity.GetVelocity3();
            // TODO Fix this, this is a bug (distance is the same if inside or outside radius clamp)
            var distanceToFutureLocation = Vector2.Distance(projectedNearFutureLocation, target);
            //AIComponent aiComponent;
            if (distanceToFutureLocation < data.MaxDistanceToTarget)
                return Vector3.zero;

            var overshoot = distanceToFutureLocation - data.MaxDistanceToTarget;
            var vectorToTargetNormal = ((Vector2)target - (Vector2)entity.transform.position).normalized;
            var force = vectorToTargetNormal * overshoot;
            return force;
        }

        public static Vector3 Wander(IPhysicsComponent entity, ref Vector3 wanderTarget, float wanderJitter,
            float wanderRadius, float wanderDistance, Transform transform, ref Vector3 last_wander_pos)
        {
            //this behavior is dependent on the update rate, so this line must
            //be included when using time independent framerate.
            float JitterThisTimeSlice = wanderJitter * Time.deltaTime;

            var deltaWanderTarget = new Vector3(RandomClamped() * JitterThisTimeSlice,
                                        RandomClamped() * JitterThisTimeSlice,
                                        0f);

            //first, add a small random vector to the target's position
            wanderTarget += deltaWanderTarget;

            //reproject this new vector back on to a unit circle
            wanderTarget.Normalize();

            //increase the length of the vector to the same as the radius
            //of the wander circle
            wanderTarget *= wanderRadius;

            //move the target into a position WanderDist in front of the agent
            Vector3 target = wanderTarget + new Vector3(0f, 0f, wanderDistance);
            //Vector2D target = wanderTarget + new Vector2D(wanderDistance, 0);
            //Vector3 target = wanderTarget + entity.GetPhysicsData().Heading * wanderDistance; //new Vector2D(wanderDistance, 0);
            // PointToWorldSpace only works if there is a Heading
            //if (entity.transform.forward == Vector3.zero)
            //{
            //    //var animationComponent = entity.Repo
            //    // Todo - Update this to head towards the direction the entity is facing.
            //    //entity.GetPhysicsData().Heading = Vector2D.Vec2DNormalize(new Vector2D(1f, 0f));
            //    entity.GetPhysicsData().Heading = new Vector3(1f, 0f);
            //    var heading = entity.GetPhysicsData().Heading;
            //    // Set the side perpindicular to the heading
            //    entity.GetPhysicsData().Side = new Vector2(-heading.y, heading.x);
            //}

            //project the target into world space
            //Vector2 Target = Transformations.PointToWorldSpace(target,
            //                                     entity.GetPhysicsData().Heading,
            //                                     entity.GetPhysicsData().Side,
            //                                     entity.GetWorldPos());

            Vector3 Target = transform.TransformPoint(target);

            //calculate the center of the wander circle
            //Vector2D m_vTCC = PointToWorldSpace(Vector2D(m_dWanderDistance * m_pVehicle->BRadius(), 0),
            //                                     m_pVehicle->Heading(),
            //                                     m_pVehicle->Side(),
            //                                     m_pVehicle->Pos());
            ////draw the wander circle
            //gdi->GreenPen();
            //gdi->HollowBrush();
            //gdi->Circle(m_vTCC, m_dWanderRadius * m_pVehicle->BRadius());

            ////draw the wander target
            //gdi->RedPen();
            //gdi->Circle(PointToWorldSpace((m_vWanderTarget + Vector2D(m_dWanderDistance, 0)) * m_pVehicle->BRadius(),
            //                              m_pVehicle->Heading(),
            //                              m_pVehicle->Side(),
            //                              m_pVehicle->Pos()), 3);

            //and steer towards it
            //var physicsAffector = entity.GetSteering().SteeringPhysicsAffector;
            var force = Target - entity.GetFeetWorldPosition3();
            force = force.normalized * entity.GetPhysicsData().MaxSpeed;
            return force;

        }

        //--------------------------- Wander -------------------------------------
        //
        //  This behavior makes the agent wander about randomly
        //------------------------------------------------------------------------
        //Vector2D SteeringBehavior::Wander()
        //{
        //    //this behavior is dependent on the update rate, so this line must
        //    //be included when using time independent framerate.
        //    double JitterThisTimeSlice = m_dWanderJitter * m_pVehicle->TimeElapsed();

        //    //first, add a small random vector to the target's position
        //    m_vWanderTarget += Vector2D(RandomClamped() * JitterThisTimeSlice,
        //                                RandomClamped() * JitterThisTimeSlice);

        //    //reproject this new vector back on to a unit circle
        //    m_vWanderTarget.Normalize();

        //    //increase the length of the vector to the same as the radius
        //    //of the wander circle
        //    m_vWanderTarget *= m_dWanderRadius;

        //    //move the target into a position WanderDist in front of the agent
        //    Vector2D target = m_vWanderTarget + Vector2D(m_dWanderDistance, 0);

        //    //project the target into world space
        //    Vector2D Target = PointToWorldSpace(target,
        //                                         m_pVehicle->Heading(),
        //                                         m_pVehicle->Side(),
        //                                         m_pVehicle->Pos());

        //    //and steer towards it
        //    return Target - m_pVehicle->Pos();
        //}

        //public static Vector2D Wander(SpriteBase entity, ref Vector2D wanderTarget, float wanderJitter,
        //    float wanderRadius, float wanderDistance, Transform transform)
        //{
        //    // Calculate the circle center
        //    Vector2D circleCenter;
        //    var heading = entity.GetHeading();
        //    circleCenter = new Vector2D(heading.x, heading.y);
        //    //circleCenter.normalize();
        //    circleCenter *= wanderRadius;
        //    //
        //    // Calculate the displacement force
        //    Vector2D displacement;
        //    displacement = new Vector2D(0, -1);
        //    displacement *= wanderDistance;
        //    //
        //    // Randomly change the vector direction
        //    // by making it change its current angle
        //    setAngle(displacement, wanderJitter);
        //    //
        //    // Change wanderAngle just a bit, so it
        //    // won't have the same value in the
        //    // next game frame.
        //    wanderTarget += (RandomClamped() * .5f); //* ANGLE_CHANGE - ANGLE_CHANGE * .5;
        //    //
        //    // Finally calculate and return the wander force
        //    Vector2D wanderForce;
        //    wanderForce = circleCenter + displacement;
        //    return wanderForce;
        //}

        //public static void setAngle(Vector2D vector, float value)
        //{
        //    float len = vector.Length();
        //    vector.x = Mathf.Cos(value) * len;
        //    vector.y = Mathf.Sin(value) * len;
        //}

        //---------------------- ObstacleAvoidance -------------------------------
        //
        //  Given a vector of CObstacles, this method returns a steering force
        //  that will prevent the agent colliding with the closest obstacle
        //------------------------------------------------------------------------
        //public static Vector2D ObstacleAvoidance(IBasicPhysicsComponent entity)
        //{
        //    var direction = entity.GetPhysicsData().Heading;
        //    var position = entity.GetFeetWorldPosition3();
        //    Vector2 force = Vector2.zero;
        //    // 1 << 10 is the layer for Enemies.
        //    int layerMask = 1 << 10;
        //    //Vector2 dir = (casaContador.position - transform.position).normalized;
        //    RaycastHit hit;

        //    if (UnityEngine.Physics.Raycast(position, direction.ToVector3(0f), out hit, 0.3f, layerMask))
        //    {
        //        //Debug.LogWarning("Cast hit obstacle");
        //        if (hit.transform != entity.transform)
        //        {
        //            //Debug.DrawLine(entity.transform.position, hit.point, Color.blue);
        //            force += (Vector2)hit.normal * 1f;
        //        }
        //    }
        //    else
        //    {
        //        Debug.DrawRay(position + new Vector3(0, 0, -.1f), direction.ToVector3(0f), Color.white, 0.3f);
        //    }

        //    Vector2 leftR = position;
        //    Vector2 rightR = position;

        //    var side = entity.GetPhysicsData().Side;

        //    // huh? Entity not always facing the same direction
        //    leftR -= (Vector2)side * 0.16f;
        //    rightR += (Vector2)side * 0.16f;

        //    if (UnityEngine.Physics.Raycast(leftR, direction.ToVector3(0f), out hit, 0.3f, layerMask))
        //    {
        //        //Debug.LogWarning("Cast hit obstacle");
        //        if (hit.transform != entity.transform)
        //        {
        //            Debug.DrawLine(entity.transform.position, hit.point, Color.red);
        //            force += (Vector2)hit.normal * 1f;
        //        }
        //    }
        //    else
        //    {
        //        Debug.DrawRay((Vector3)leftR + new Vector3(0, 0, -.1f), direction.ToVector3(0f), Color.white, 0.3f);
        //    }


        //    if (UnityEngine.Physics.Raycast(rightR, direction.ToVector3(0f), out hit, 0.3f, layerMask))
        //    {
        //        //Debug.LogWarning("Cast hit obstacle");
        //        if (hit.transform != entity.transform)
        //        {
        //            Debug.DrawLine(entity.transform.position, hit.point, Color.yellow);
        //            force += (Vector2)hit.normal * 1f;
        //        }
        //    }
        //    else
        //    {
        //        Debug.DrawRay((Vector3)rightR + new Vector3(0, 0, -.1f), direction.ToVector3(0f), Color.white, 0.3f);
        //    }
        //    var scaledForce = force.normalized * entity.GetPhysicsData().MaxSpeed;
        //    return scaledForce;
        //}

        //public static Vector2D ObstacleAvoidance(IBasicPhysicsComponent entity)
        //{
        //    var direction = entity.GetPhysicsData().Heading;
        //    var position = entity.GetFeetWorldPosition3();
        //    Vector2 force = Vector2.zero;
        //    // 1 << 10 is the layer for Enemies.
        //    int layerMask = 1 << 10;
        //    //Vector2 dir = (casaContador.position - transform.position).normalized;
        //    RaycastHit hit;
        //    var castDistance = 0.8f;
        //    bool avoided = true;

        //    List<Vector3> feelers = new List<Vector3>();


        //    if (UnityEngine.Physics.Raycast(position, direction.ToVector3(0f), out hit, castDistance, layerMask))
        //    {
        //        Debug.LogWarning("Cast hit obstacle");
        //        if (hit.transform != entity.transform)
        //        {
        //            Debug.DrawLine(entity.transform.position, hit.point, Color.blue);
        //            force += (Vector2)hit.normal * 0.8f / hit.distance;
        //        }
        //    }
        //    else
        //    {
        //        Debug.DrawLine(position + new Vector3(0, 0, -.1f), position + (direction.ToVector3(0f) * castDistance) + new Vector3(0, 0, -.1f), Color.white);
        //    }

        //    Vector3 leftR = position;
        //    Vector3 rightR = position;

        //    var side = entity.GetPhysicsData().Side;

        //    // huh? Entity not always facing the same direction
        //    leftR -= side.ToVector3(0f) * 0.16f;
        //    rightR += side.ToVector3(0f) * 0.16f;

        //    if (UnityEngine.Physics.Raycast(leftR, direction.ToVector3(0f), out hit, castDistance, layerMask))
        //    {
        //        //Debug.LogWarning("Cast hit obstacle");
        //        if (hit.transform != entity.transform)
        //        {
        //            Debug.DrawLine(entity.transform.position, hit.point, Color.red);
        //            force += (Vector2)hit.normal * 1f / hit.distance;
        //        }
        //    }
        //    else
        //    {
        //        Debug.DrawLine(leftR + new Vector3(0, 0, -.1f), leftR + (direction.ToVector3(0f) * castDistance) + new Vector3(0, 0, -.1f), Color.white);
        //        //Debug.DrawRay((Vector3)leftR + new Vector3(0, 0, -.1f), direction.ToVector3(0f), Color.white, 0.3f);
        //    }


        //    if (UnityEngine.Physics.Raycast(rightR, direction.ToVector3(0f), out hit, castDistance, layerMask))
        //    {
        //        //Debug.LogWarning("Cast hit obstacle");
        //        if (hit.transform != entity.transform)
        //        {
        //            Debug.DrawLine(entity.transform.position, hit.point, Color.yellow);
        //            force += (Vector2)hit.normal * 1f / hit.distance;
        //        }
        //    }
        //    else
        //    {
        //        Debug.DrawLine(rightR + new Vector3(0, 0, -.1f), rightR + (direction.ToVector3(0f) * castDistance) + new Vector3(0, 0, -.1f), Color.white);
        //        //Debug.DrawRay((Vector3)rightR + new Vector3(0, 0, -.1f), direction.ToVector3(0f), Color.white, 0.3f);
        //    }
        //    var scaledForce = force.normalized * entity.GetPhysicsData().MaxSpeed;
        //    return scaledForce;
        //}


        //public static Vector2D ObstacleAvoidance(IBasicPhysicsComponent entity)
        //{
        //    var direction = entity.GetPhysicsData().Heading;
        //    var position = entity.GetFeetWorldPosition3();
        //    Vector2 force = Vector2.zero;
        //    // 1 << 10 is the layer for Enemies.
        //    int layerMask = 1 << 10;
        //    //Vector2 dir = (casaContador.position - transform.position).normalized;
        //    RaycastHit hit;
        //    var castDistance = 0.8f;
        //    //bool avoided = true;

        //    //List<Vector3> feelers = new List<Vector3>();
        //    var distance = 1f;
        //    if (UnityEngine.Physics.BoxCast(entity.transform.position, new Vector3(0.16f, 0.16f, 0f), direction.ToVector3(0f),
        //        out hit, Quaternion.identity, castDistance, layerMask))
        //    {
        //        force = (Vector2)hit.normal;
        //        distance = hit.distance;
        //        //hit.
        //        //Gizmos.
        //    }

        //    //Debug.DrawLine(position + new Vector3(0, 0, -.1f), position + (direction.ToVector3(0f) * castDistance) + new Vector3(0, 0, -.1f), Color.white);

        //    //if (UnityEngine.Physics.Raycast(position, direction.ToVector3(0f), out hit, castDistance, layerMask))
        //    //{
        //    //    Debug.LogWarning("Cast hit obstacle");
        //    //    if (hit.transform != entity.transform)
        //    //    {
        //    //        Debug.DrawLine(entity.transform.position, hit.point, Color.blue);
        //    //        force += (Vector2)hit.normal * 0.8f / hit.distance;
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    Debug.DrawLine(position + new Vector3(0, 0, -.1f), position + (direction.ToVector3(0f) * castDistance) + new Vector3(0, 0, -.1f), Color.white);
        //    //}

        //    //Vector3 leftR = position;
        //    //Vector3 rightR = position;

        //    //var side = entity.GetPhysicsData().Side;

        //    //// huh? Entity not always facing the same direction
        //    //leftR -= side.ToVector3(0f) * 0.16f;
        //    //rightR += side.ToVector3(0f) * 0.16f;

        //    //if (UnityEngine.Physics.Raycast(leftR, direction.ToVector3(0f), out hit, castDistance, layerMask))
        //    //{
        //    //    //Debug.LogWarning("Cast hit obstacle");
        //    //    if (hit.transform != entity.transform)
        //    //    {
        //    //        Debug.DrawLine(entity.transform.position, hit.point, Color.red);
        //    //        force += (Vector2)hit.normal * 1f / hit.distance;
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    Debug.DrawLine(leftR + new Vector3(0, 0, -.1f), leftR + (direction.ToVector3(0f) * castDistance) + new Vector3(0, 0, -.1f), Color.white);
        //    //    //Debug.DrawRay((Vector3)leftR + new Vector3(0, 0, -.1f), direction.ToVector3(0f), Color.white, 0.3f);
        //    //}


        //    //if (UnityEngine.Physics.Raycast(rightR, direction.ToVector3(0f), out hit, castDistance, layerMask))
        //    //{
        //    //    //Debug.LogWarning("Cast hit obstacle");
        //    //    if (hit.transform != entity.transform)
        //    //    {
        //    //        Debug.DrawLine(entity.transform.position, hit.point, Color.yellow);
        //    //        force += (Vector2)hit.normal * 1f / hit.distance;
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    Debug.DrawLine(rightR + new Vector3(0, 0, -.1f), rightR + (direction.ToVector3(0f) * castDistance) + new Vector3(0, 0, -.1f), Color.white);
        //    //    //Debug.DrawRay((Vector3)rightR + new Vector3(0, 0, -.1f), direction.ToVector3(0f), Color.white, 0.3f);
        //    //}

        //    var scaledForce = force.normalized * entity.GetPhysicsData().MaxSpeed / distance;
        //    return scaledForce;
        //}


        public static Vector3 ObstacleAvoidance(IPhysicsComponent entity)
        //Vector2D SteeringBehavior::ObstacleAvoidance(const std::vector<BaseGameEntity*>& obstacles)
        {
            //var physicsAffector = entity.GetSteering().SteeringPhysicsAffector;
            float MinDetectionBoxLength = 0.5f;
            var physicsData = entity.GetPhysicsData();
            //the detection box length is proportional to the agent's velocity
            var m_dDBoxLength = MinDetectionBoxLength +
                            (entity.GetVelocity3().magnitude / entity.GetPhysicsData().MaxSpeed) *
                            MinDetectionBoxLength;

            //tag all obstacles within range of the box for processing
            //m_pVehicle->World()->TagObstaclesWithinViewRange(m_pVehicle, m_dDBoxLength);

            //this will keep track of the closest intersecting obstacle (CIB)
            IEntity ClosestIntersectingObstacle = null;

            //this will be used to track the distance to the CIB
            float DistToClosestIP = float.MaxValue;

            //this will record the transformed local coordinates of the CIB
            Vector3 LocalPosOfClosestObstacle = Vector3.zero;
            //    // 1 << 10 is the layer for Enemies.
            //       12 = Bounding Radius
            int layerMask = 1 << 12;
            var rays = UnityEngine.Physics.BoxCastAll(entity.transform.position, new Vector3(0.16f, 0.16f, 0f), 
                entity.transform.forward, Quaternion.identity, m_dDBoxLength, layerMask);
            //if (UnityEngine.Physics.BoxCast(entity.transform.position, new Vector3(0.16f, 0.16f, 0f), direction.ToVector3(0f),
            //    out hit, Quaternion.identity, castDistance, layerMask))

            //  std::vector<BaseGameEntity*>::const_iterator curOb = obstacles.begin();

            //while(curOb != obstacles.end())
            //{
            //IBasicPhysicsComponent otherPhysicsComponent
            foreach (var ray in rays)
            {
                var otherCollisionComponent = ray.collider.transform.GetComponent<IComponentBase>();
                if (otherCollisionComponent == null)
                    continue;
                var otherEntity = otherCollisionComponent.GetComponentRepository();
                if (otherEntity == null)
                    continue;
                var otherPhysicsComponent = otherEntity.Components.GetComponent<IPhysicsComponent>();
                if (otherPhysicsComponent == entity)
                    continue;
                //Closest
                //if the obstacle has been tagged within range proceed
                //if ((* curOb)->IsTagged())
                //{
                //calculate this obstacle's position in local space
                //Vector2D LocalPos = Transformations.PointToLocalSpace(otherEntity.transform.position,
                //                                       physicsData.Heading,
                //                                       physicsData.Side,
                //                                       entity.transform.position);

                Vector3 LocalPos = entity.transform.InverseTransformPoint(entity.transform.position);

                //if the local position has a negative x value then it must lay
                //behind the agent. (in which case it can be ignored)
                if (LocalPos.x < 0)
                    continue;
                //if the distance from the x axis to the object's position is less
                //than its radius + half the width of the detection box then there
                //is a potential intersection.
                float ExpandedRadius = otherPhysicsComponent.GetPhysicsData().BoundingRadius + physicsData.BoundingRadius;

                if (Mathf.Abs(LocalPos.y) >= ExpandedRadius)
                    continue;
                //now to do a line/circle intersection test. The center of the 
                //circle is represented by (cX, cY). The intersection points are 
                //given by the formula x = cX +/-sqrt(r^2-cY^2) for y=0. 
                //We only need to look at the smallest positive value of x because
                //that will be the closest point of intersection.
                float cX = LocalPos.x;
                float cY = LocalPos.y;

                //we only need to calculate the sqrt part of the above equation once
                float SqrtPart = Mathf.Sqrt(ExpandedRadius * ExpandedRadius - cY * cY);

                float ip = cX - SqrtPart;

                if (ip <= 0.0)
                {
                    ip = cX + SqrtPart;
                }

                //test to see if this is the closest so far. If it is keep a
                //record of the obstacle and its local coordinates
                if (ip >= DistToClosestIP)
                    continue;
                DistToClosestIP = ip;

                ClosestIntersectingObstacle = otherEntity;

                LocalPosOfClosestObstacle = LocalPos;



                //}

                //++curOb;
            }

            // Couldn't find a closest obstacle? Return
            if (ClosestIntersectingObstacle == null)
                return Vector3.zero;

            //if we have found an intersecting obstacle, calculate a steering 
            //force away from it
            Vector3 SteeringForce = Vector3.zero;

            if (ClosestIntersectingObstacle != null)
            {
                //the closer the agent is to an object, the stronger the 
                //steering force should be
                float multiplier = 1.0f + (m_dDBoxLength - LocalPosOfClosestObstacle.x) /
                                    m_dDBoxLength;

                var otherPhysicsComponent = ClosestIntersectingObstacle.Components.GetComponent<IPhysicsComponent>();
                //calculate the lateral force
                SteeringForce.y = (otherPhysicsComponent.GetPhysicsData().BoundingRadius -
                               LocalPosOfClosestObstacle.y) * multiplier;

                //apply a braking force proportional to the obstacles distance from
                //the vehicle. 
                const float BrakingWeight = 0.2f;

                SteeringForce.x = (otherPhysicsComponent.GetPhysicsData().BoundingRadius -
                                               LocalPosOfClosestObstacle.x) *
                                               BrakingWeight;
            }
            Vector2 finalForce = Transformations.VectorToWorldSpace(SteeringForce,
                                      entity.transform.forward,
                                      entity.transform.right);
            //finally, convert the steering vector from local to world space
            //return finalForce .normalized * physicsData.MaxSpeed;
            return finalForce;
        }


        //---------------------- ObstacleAvoidance -------------------------------
        //
        //  Given a vector of CObstacles, this method returns a steering force
        //  that will prevent the agent colliding with the closest obstacle
        //------------------------------------------------------------------------
        //Vector2D SteeringBehavior::ObstacleAvoidance(const std::vector<BaseGameEntity*>& obstacles)
        //{
        //  //the detection box length is proportional to the agent's velocity
        //  m_dDBoxLength = Prm.MinDetectionBoxLength + 
        //                  (m_pVehicle->Speed()/m_pVehicle->MaxSpeed()) *
        //                  Prm.MinDetectionBoxLength;

        //  //tag all obstacles within range of the box for processing
        //  m_pVehicle->World()->TagObstaclesWithinViewRange(m_pVehicle, m_dDBoxLength);

        //        //this will keep track of the closest intersecting obstacle (CIB)
        //        BaseGameEntity* ClosestIntersectingObstacle = NULL;

        //        //this will be used to track the distance to the CIB
        //        double DistToClosestIP = MaxDouble;

        //        //this will record the transformed local coordinates of the CIB
        //        Vector2D LocalPosOfClosestObstacle;

        //        std::vector<BaseGameEntity*>::const_iterator curOb = obstacles.begin();

        //  while(curOb != obstacles.end())
        //  {
        //    //if the obstacle has been tagged within range proceed
        //    if ((* curOb)->IsTagged())
        //    {
        //      //calculate this obstacle's position in local space
        //      Vector2D LocalPos = PointToLocalSpace((*curOb)->Pos(),
        //                                             m_pVehicle->Heading(),
        //                                             m_pVehicle->Side(),
        //                                             m_pVehicle->Pos());

        //      //if the local position has a negative x value then it must lay
        //      //behind the agent. (in which case it can be ignored)
        //      if (LocalPos.x >= 0)
        //      {
        //        //if the distance from the x axis to the object's position is less
        //        //than its radius + half the width of the detection box then there
        //        //is a potential intersection.
        //        double ExpandedRadius = (*curOb)->BRadius() + m_pVehicle->BRadius();

        //        if (fabs(LocalPos.y) < ExpandedRadius)
        //        {
        //          //now to do a line/circle intersection test. The center of the 
        //          //circle is represented by (cX, cY). The intersection points are 
        //          //given by the formula x = cX +/-sqrt(r^2-cY^2) for y=0. 
        //          //We only need to look at the smallest positive value of x because
        //          //that will be the closest point of intersection.
        //          double cX = LocalPos.x;
        //        double cY = LocalPos.y;

        //        //we only need to calculate the sqrt part of the above equation once
        //        double SqrtPart = sqrt(ExpandedRadius * ExpandedRadius - cY * cY);

        //        double ip = cX - SqrtPart;

        //          if (ip <= 0.0)
        //          {
        //            ip = cX + SqrtPart;
        //          }

        //          //test to see if this is the closest so far. If it is keep a
        //          //record of the obstacle and its local coordinates
        //          if (ip<DistToClosestIP)
        //          {
        //            DistToClosestIP = ip;

        //            ClosestIntersectingObstacle = * curOb;

        //    LocalPosOfClosestObstacle = LocalPos;
        //          }         
        //        }
        //      }
        //    }

        //    ++curOb;
        //  }

        //  //if we have found an intersecting obstacle, calculate a steering 
        //  //force away from it
        //  Vector2D SteeringForce;

        //  if (ClosestIntersectingObstacle)
        //  {
        //    //the closer the agent is to an object, the stronger the 
        //    //steering force should be
        //    double multiplier = 1.0 + (m_dDBoxLength - LocalPosOfClosestObstacle.x) /
        //                        m_dDBoxLength;

        ////calculate the lateral force
        //SteeringForce.y = (ClosestIntersectingObstacle->BRadius()-
        //                       LocalPosOfClosestObstacle.y)  * multiplier;

        ////apply a braking force proportional to the obstacles distance from
        ////the vehicle. 
        //const double BrakingWeight = 0.2;

        //SteeringForce.x = (ClosestIntersectingObstacle->BRadius() - 
        //                       LocalPosOfClosestObstacle.x) *
        //                       BrakingWeight;
        //  }

        //  //finally, convert the steering vector from local to world space
        //  return VectorToWorldSpace(SteeringForce,
        //                            m_pVehicle->Heading(),
        //                            m_pVehicle->Side());
        //}



        //--------------------------- WallAvoidance --------------------------------
        //
        //  This returns a steering force that will keep the agent away from any
        //  walls it may encounter
        //------------------------------------------------------------------------
        //public static Vector2D WallAvoidance()
        //{
        //  //the feelers are contained in a std::vector, m_Feelers
        //  CreateFeelers();

        //  double DistToThisIP    = 0.0;
        //  double DistToClosestIP = double.MaxValue;

        //  //this will hold an index into the vector of walls
        //  int ClosestWall = -1;

        //  Vector2D SteeringForce,
        //            point,         //used for storing temporary info
        //            ClosestPoint;  //holds the closest intersection point

        //  //examine each feeler in turn
        //  for (unsigned int flr=0; flr<m_Feelers.size(); ++flr)
        //  {
        //    //run through each wall checking for any intersection points
        //    for (unsigned int w=0; w<walls.size(); ++w)
        //    {
        //      if (LineIntersection2D(m_pVehicle->Pos(),
        //                             m_Feelers[flr],
        //                             walls[w].From(),
        //                             walls[w].To(),
        //                             DistToThisIP,
        //                             point))
        //      {
        //        //is this the closest found so far? If so keep a record
        //        if (DistToThisIP < DistToClosestIP)
        //        {
        //          DistToClosestIP = DistToThisIP;

        //          ClosestWall = w;

        //          ClosestPoint = point;
        //        }
        //      }
        //    }//next wall


        //    //if an intersection point has been detected, calculate a force  
        //    //that will direct the agent away
        //    if (ClosestWall >=0)
        //    {
        //      //calculate by what distance the projected position of the agent
        //      //will overshoot the wall
        //      Vector2D OverShoot = m_Feelers[flr] - ClosestPoint;

        //      //create a force in the direction of the wall normal, with a 
        //      //magnitude of the overshoot
        //      SteeringForce = walls[ClosestWall].Normal() * OverShoot.Length();
        //    }

        //  }//next feeler

        //  return SteeringForce;
        //}

        ////------------------------------- CreateFeelers --------------------------
        ////
        ////  Creates the antenna utilized by WallAvoidance
        ////------------------------------------------------------------------------
        //public static IList<Vector2D> CreateFeelers()
        //{
        //    List<Vector2D> feelers;

        //  //feeler pointing straight in front
        //  feelers.Add(m_pVehicle->Pos() + m_dWallDetectionFeelerLength * m_pVehicle->Heading());

        //  //feeler to left
        //  Vector2D temp = m_pVehicle->Heading();
        //  Vec2DRotateAroundOrigin(temp, HalfPi * 3.5f);
        //  m_Feelers[1] = m_pVehicle->Pos() + m_dWallDetectionFeelerLength/2.0f * temp;

        //  //feeler to right
        //  temp = m_pVehicle->Heading();
        //  Vec2DRotateAroundOrigin(temp, HalfPi * 0.5f);
        //  m_Feelers[2] = m_pVehicle->Pos() + m_dWallDetectionFeelerLength/2.0f * temp;
        //}


        //---------------------------- Separation --------------------------------
        //
        // this calculates a force repelling from the other neighbors
        //------------------------------------------------------------------------
        //Vector2D SteeringBehavior::Separation(const vector<Vehicle*> &neighbors)
        //{  
        //  Vector2D SteeringForce;

        //  for (unsigned int a=0; a<neighbors.size(); ++a)
        //  {
        //    //make sure this agent isn't included in the calculations and that
        //    //the agent being examined is close enough. ***also make sure it doesn't
        //    //include the evade target ***
        //    if((neighbors[a] != m_pVehicle) && neighbors[a]->IsTagged() &&
        //      (neighbors[a] != m_pTargetAgent1))
        //    {
        //      Vector2D ToAgent = m_pVehicle->Pos() - neighbors[a]->Pos();

        //      //scale the force inversely proportional to the agents distance  
        //      //from its neighbor.
        //      SteeringForce += Vec2DNormalize(ToAgent)/ToAgent.Length();
        //    }
        //  }

        //  return SteeringForce;
        //}


        //---------------------------- Alignment ---------------------------------
        //
        //  returns a force that attempts to align this agents heading with that
        //  of its neighbors
        //------------------------------------------------------------------------
        //Vector2D SteeringBehavior::Alignment(const vector<Vehicle*>& neighbors)
        //{
        //  //used to record the average heading of the neighbors
        //  Vector2D AverageHeading;

        //  //used to count the number of vehicles in the neighborhood
        //  int    NeighborCount = 0;

        //  //iterate through all the tagged vehicles and sum their heading vectors  
        //  for (unsigned int a=0; a<neighbors.size(); ++a)
        //  {
        //    //make sure *this* agent isn't included in the calculations and that
        //    //the agent being examined  is close enough ***also make sure it doesn't
        //    //include any evade target ***
        //    if((neighbors[a] != m_pVehicle) && neighbors[a]->IsTagged() &&
        //      (neighbors[a] != m_pTargetAgent1))
        //    {
        //      AverageHeading += neighbors[a]->Heading();

        //      ++NeighborCount;
        //    }
        //  }

        //  //if the neighborhood contained one or more vehicles, average their
        //  //heading vectors.
        //  if (NeighborCount > 0)
        //  {
        //    AverageHeading /= (double)NeighborCount;

        //    AverageHeading -= m_pVehicle->Heading();
        //  }

        //  return AverageHeading;
        //}


        //-------------------------------- Cohesion ------------------------------
        //
        //  returns a steering force that attempts to move the agent towards the
        //  center of mass of the agents in its immediate area
        //------------------------------------------------------------------------
        //Vector2D SteeringBehavior::Cohesion(const vector<Vehicle*> &neighbors)
        //{
        //  //first find the center of mass of all the agents
        //  Vector2D CenterOfMass, SteeringForce;

        //  int NeighborCount = 0;

        //  //iterate through the neighbors and sum up all the position vectors
        //  for (unsigned int a=0; a<neighbors.size(); ++a)
        //  {
        //    //make sure *this* agent isn't included in the calculations and that
        //    //the agent being examined is close enough ***also make sure it doesn't
        //    //include the evade target ***
        //    if((neighbors[a] != m_pVehicle) && neighbors[a]->IsTagged() &&
        //      (neighbors[a] != m_pTargetAgent1))
        //    {
        //      CenterOfMass += neighbors[a]->Pos();

        //      ++NeighborCount;
        //    }
        //  }

        //  if (NeighborCount > 0)
        //  {
        //    //the center of mass is the average of the sum of positions
        //    CenterOfMass /= (double)NeighborCount;

        //    //now seek towards that position
        //    SteeringForce = Seek(CenterOfMass);
        //  }

        //  //the magnitude of cohesion is usually much larger than separation or
        //  //allignment so it usually helps to normalize it.
        //  return Vec2DNormalize(SteeringForce);
        //}


        /* NOTE: the next three behaviors are the same as the above three, except
                  that they use a cell-space partition to find the neighbors
        */


        //---------------------------- Separation --------------------------------
        //
        // this calculates a force repelling from the other neighbors
        //
        //  USES SPACIAL PARTITIONING
        //------------------------------------------------------------------------
        //Vector2D SteeringBehavior::SeparationPlus(const vector<Vehicle*> &neighbors)
        //{  
        //  Vector2D SteeringForce;

        //  //iterate through the neighbors and sum up all the position vectors
        //  for (BaseGameEntity* pV = m_pVehicle->World()->CellSpace()->begin();
        //                         !m_pVehicle->World()->CellSpace()->end();     
        //                       pV = m_pVehicle->World()->CellSpace()->next())
        //  {    
        //    //make sure this agent isn't included in the calculations and that
        //    //the agent being examined is close enough
        //    if(pV != m_pVehicle)
        //    {
        //      Vector2D ToAgent = m_pVehicle->Pos() - pV->Pos();

        //      //scale the force inversely proportional to the agents distance  
        //      //from its neighbor.
        //      SteeringForce += Vec2DNormalize(ToAgent)/ToAgent.Length();
        //    }

        //  }

        //  return SteeringForce;
        //}

        //---------------------------- Alignment ---------------------------------
        //
        //  returns a force that attempts to align this agents heading with that
        //  of its neighbors
        //
        //  USES SPACIAL PARTITIONING
        //------------------------------------------------------------------------
        //Vector2D SteeringBehavior::AlignmentPlus(const vector<Vehicle*> &neighbors)
        //{
        //  //This will record the average heading of the neighbors
        //  Vector2D AverageHeading;

        //  //This count the number of vehicles in the neighborhood
        //  double    NeighborCount = 0.0;

        //  //iterate through the neighbors and sum up all the position vectors
        //  for (MovingEntity* pV = m_pVehicle->World()->CellSpace()->begin();
        //                         !m_pVehicle->World()->CellSpace()->end();     
        //                     pV = m_pVehicle->World()->CellSpace()->next())
        //  {
        //    //make sure *this* agent isn't included in the calculations and that
        //    //the agent being examined  is close enough
        //    if(pV != m_pVehicle)
        //    {
        //      AverageHeading += pV->Heading();

        //      ++NeighborCount;
        //    }

        //  }

        //  //if the neighborhood contained one or more vehicles, average their
        //  //heading vectors.
        //  if (NeighborCount > 0.0)
        //  {
        //    AverageHeading /= NeighborCount;

        //    AverageHeading -= m_pVehicle->Heading();
        //  }

        //  return AverageHeading;
        //}


        //-------------------------------- Cohesion ------------------------------
        //
        //  returns a steering force that attempts to move the agent towards the
        //  center of mass of the agents in its immediate area
        //
        //  USES SPACIAL PARTITIONING
        //------------------------------------------------------------------------
        //Vector2D SteeringBehavior::CohesionPlus(const vector<Vehicle*> &neighbors)
        //{
        //  //first find the center of mass of all the agents
        //  Vector2D CenterOfMass, SteeringForce;

        //  int NeighborCount = 0;

        //  //iterate through the neighbors and sum up all the position vectors
        //  for (BaseGameEntity* pV = m_pVehicle->World()->CellSpace()->begin();
        //                         !m_pVehicle->World()->CellSpace()->end();     
        //                       pV = m_pVehicle->World()->CellSpace()->next())
        //  {
        //    //make sure *this* agent isn't included in the calculations and that
        //    //the agent being examined is close enough
        //    if(pV != m_pVehicle)
        //    {
        //      CenterOfMass += pV->Pos();

        //      ++NeighborCount;
        //    }
        //  }

        //  if (NeighborCount > 0)
        //  {
        //    //the center of mass is the average of the sum of positions
        //    CenterOfMass /= (double)NeighborCount;

        //    //now seek towards that position
        //    SteeringForce = Seek(CenterOfMass);
        //  }

        //  //the magnitude of cohesion is usually much larger than separation or
        //  //allignment so it usually helps to normalize it.
        //  return Vec2DNormalize(SteeringForce);
        //}


        //--------------------------- Interpose ----------------------------------
        //
        //  Given two agents, this method returns a force that attempts to 
        //  position the vehicle between them
        //------------------------------------------------------------------------
        public static Vector3 Interpose(IPhysicsComponent AgentA,
                                             IPhysicsComponent AgentB,
                                                IPhysicsComponent entity)
        {
            //var physicsAffector = entity.GetSteering().SteeringPhysicsAffector;
            //first we need to figure out where the two agents are going to be at 
            //time T in the future. This is approximated by determining the time
            //taken to reach the mid way point at the current time at at max speed.
            Vector3 MidPoint = (AgentA.GetWorldPos3() + AgentB.GetWorldPos3()) / 2.0f;

            float TimeToReachMidPoint = Vector3.Distance(entity.GetWorldPos3(), MidPoint) /
                                         entity.GetPhysicsData().MaxSpeed;

            //now we have T, we assume that agent A and agent B will continue on a
            //straight trajectory and extrapolate to get their future positions
            Vector3 APos = AgentA.GetWorldPos3() + AgentA.GetVelocity3() * TimeToReachMidPoint;
            Vector3 BPos = AgentB.GetWorldPos3() + AgentB.GetVelocity3() * TimeToReachMidPoint;

            //calculate the mid point of these predicted positions
            MidPoint = (APos + BPos) / 2.0f;

            //then steer to Arrive at it
            return Arrive(MidPoint, entity, Deceleration3.fast);
        }

        //--------------------------- Hide ---------------------------------------
        //  given another agent position to hide from and a list of BaseGameEntitys this
        //  method attempts to put an obstacle between itself and its opponent
        //------------------------------------------------------------------------
        //public static Vector2D Hide(const Vehicle*           hunter,
        //                                 const vector<BaseGameEntity*>& obstacles)
        //{
        //  double    DistToClosest = MaxDouble;
        //  Vector2D BestHidingSpot;

        //  std::vector<BaseGameEntity*>::const_iterator curOb = obstacles.begin();
        //  std::vector<BaseGameEntity*>::const_iterator closest;

        //  while(curOb != obstacles.end())
        //  {
        //    //calculate the position of the hiding spot for this obstacle
        //    Vector2D HidingSpot = GetHidingPosition((*curOb)->Pos(),
        //                                             (*curOb)->BRadius(),
        //                                              hunter->Pos());

        //    //work in distance-squared space to find the closest hiding
        //    //spot to the agent
        //    double dist = Vec2DDistanceSq(HidingSpot, m_pVehicle->Pos());

        //    if (dist < DistToClosest)
        //    {
        //      DistToClosest = dist;

        //      BestHidingSpot = HidingSpot;

        //      closest = curOb;
        //    }  

        //    ++curOb;

        //  }//end while

        //  //if no suitable obstacles found then Evade the hunter
        //  if (DistToClosest == MaxFloat)
        //  {
        //    return Evade(hunter);
        //  }

        //  //else use Arrive on the hiding spot
        //  return Arrive(BestHidingSpot, fast);
        //}

        //------------------------- GetHidingPosition ----------------------------
        //
        //  Given the position of a hunter, and the position and radius of
        //  an obstacle, this method calculates a position DistanceFromBoundary 
        //  away from its bounding radius and directly opposite the hunter
        //------------------------------------------------------------------------
        //Vector2D SteeringBehavior::GetHidingPosition(const Vector2D& posOb,
        //                                              const double     radiusOb,
        //                                              const Vector2D& posHunter)
        //{
        //  //calculate how far away the agent is to be from the chosen obstacle's
        //  //bounding radius
        //  const double DistanceFromBoundary = 30.0;
        //  double       DistAway    = radiusOb + DistanceFromBoundary;

        //  //calculate the heading toward the object from the hunter
        //  Vector2D ToOb = Vec2DNormalize(posOb - posHunter);

        //  //scale it to size and add to the obstacles position to get
        //  //the hiding spot.
        //  return (ToOb * DistAway) + posOb;
        //}


        //------------------------------- FollowPath -----------------------------
        //
        //  Given a series of Vector2Ds, this method produces a force that will
        //  move the agent along the waypoints in order. The agent uses the
        // 'Seek' behavior to move to the next waypoint - unless it is the last
        //  waypoint, in which case it 'Arrives'
        //------------------------------------------------------------------------
        public static Vector3 FollowPath(IPhysicsComponent entity, Path3 path)
        {
            //if (path.IsFinished())
            //    return entity.GetVelocity() * -1;
            //move to next target if close enough to current target (working in
            //distance squared space)

            //if (path.IsGoingToLastWaypoint() && path.DecelerateToFinalWaypoint)
            //    return Arrive(path.CurrentWaypoint(), entity, Deceleration.normal);
            //else
            if (!path.CurrentWaypoint(out var waypoint))
                return Vector3.zero;

            return Seek(waypoint, entity);

            //if (!path.IsFinished())
            //{
            //    return Seek(path.CurrentWaypoint(), entity);
            //}

            //else
            //{
            //    return Seek(path.CurrentWaypoint(), entity);
            //    //return Arrive(path.CurrentWaypoint(), entity, Deceleration.normal);
            //}
        }

        //------------------------- Offset Pursuit -------------------------------
        //
        //  Produces a steering force that keeps a vehicle at a specified offset
        //  from a leader vehicle
        //------------------------------------------------------------------------
        public static Vector3 OffsetPursuit(IPhysicsComponent leader,
                                                  Vector3 offset, IPhysicsComponent entity)
        {
            //var physicsAffector = entity.GetSteering().SteeringPhysicsAffector;
            //calculate the offset's position in world space
            //Vector2D WorldOffsetPos = Transformations.PointToWorldSpace(offset,
            //                                                leader.GetPhysicsData().Heading,
            //                                                leader.GetPhysicsData().Side,
            //                                                leader.GetWorldPos());

            var WorldOffsetPos = leader.GetWorldPos3() + offset;

            //Vector2 ToOffset = WorldOffsetPos - entity.GetFeetWorldPosition();

            return Seek(WorldOffsetPos, entity);

            //var offsetMagnitude = Mathf.Min(ToOffset.magnitude, entity.GetPhysicsData().MaxSpeed);

            //the lookahead time is propotional to the distance between the leader
            //and the pursuer; and is inversely proportional to the sum of both
            //agent's velocities
            //float LookAheadTime = offsetMagnitude /
            //          (entity.GetPhysicsData().MaxSpeed + leader.GetVelocity().magnitude);
            //float LookAheadTime = ToOffset.magnitude /
            //                      (entity.GetPhysicsData().MaxSpeed + leader.GetVelocity().magnitude);

            //now Arrive at the predicted future position of the offset
            //return Arrive(ToOffset + leader.GetVelocity() * LookAheadTime, entity,
            //    Deceleration.fast);
            //return Seek(ToOffset + leader.GetVelocity() * LookAheadTime, entity);
        }
    }
}
