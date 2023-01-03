using RQ.Common.Components;
using RQ.Common.Container;
using System.Collections.Generic;
using RQ.Base.Extensions;
using UnityEngine;
using RQ.Controller;

namespace RQ.Physics
{
    [AddComponentMenu("RQ/Components/Physics")]
    public class PhysicsComponent : ComponentBase<PhysicsComponent>, IPhysicsComponent, IMovementController
    {
        [SerializeField]
        private PhysicsLogic _controller;
        [SerializeField]
        private bool _applySpeedLimit = true;

        public PhysicsLogic Controller => _controller;

        private Rigidbody _rigidBody3D;
        //the steering behavior class
        [SerializeField]
        private SteeringBehaviorManager _steering;

        [SerializeField]
        private bool _isEnabled;

        [SerializeField]
        private string _animForwardSpeedVar;

        private Animator _animator;
        private AnimationComponent _animationComponent;
        private Vector3 _steeringForce;

        public void Construct(PhysicsLogic logic)
        {
            _controller = logic;
        }

        protected override void Awake()
        {
            base.Awake();
            _rigidBody3D = GetComponent<Rigidbody>();
            _controller?.SetMovementController(this);
            _isEnabled = true;
            _controller.Awake();
            _steering.Setup(this, transform);
            _animator = GetComponent<Animator>();
        }

        protected void Start()
        {
            if (_animationComponent == null)
            {
                _animationComponent = _componentRepository.Components.GetComponent<AnimationComponent>();
            }
        }

        protected void FixedUpdate()
        {
            _controller.CheckGroundStatus();

            if (!_isEnabled)
                return;
            //if (_controller.GetPhysicsData().IsStopped)
            //{
            //    Stop();
            //    return;
            //}

            //_physicsData.Velocity += (_physicsData.InputForce / 50f);

            //if (_physicsData.DragForce != 0f)
            //{
            //    // Calculate the drag force and add it to Velocity.
            //    var normalizedVelocity = ((Vector2)_physicsData.Velocity).normalized;
            //    var currentDragForce = normalizedVelocity * -1f * _physicsData.DragForce / 50f;
            //    //if (currentDragForce )
            //    _physicsData.Velocity += (Vector2D)currentDragForce;
            //}

            // Update by Brian Karcher 9/26/2018
            // Having the RigidBody control the Velocity value. This allows us to take into account
            // collision physics automatically via said RigidBody.

            //var force = Vector3.zero;

            ProcessSteeringBehaviorForce2();

            // Strip out the gravity (Y axis), apply the forward movement, then convert back to Vector 3.
            //force = transform.TransformPoint(new Vector3(0f, 0f, _forwardAmount));
            //var localForce = new Vector3(0f, 0f, _forwardAmount);
            var forward = transform.forward;
            //force += (transform.forward.xz() * _forwardAmount).xz();
            //force += new Vector3(0f, 0f, _forwardAmount);

            ////foreach (var affector in _physicsAffectorsList.Values)
            //for (int i = 0; i < _physicsAffectorsList.Count; i++)
            //{
            //    var affector = _physicsAffectorsList[i];
            //    if (affector.Enabled)
            //        force += affector.CalculateForce();
            //    //newVelocity += affector.Velocity;
            //}
            //localForce = localForce * _controller.GetPhysicsData().ForceMultiplier;
            var maxSpeed = _controller.GetPhysicsData().MaxSpeed * _controller.GetPhysicsData().MaxSpeedMultiplier;
            //if (force.magnitude > maxSpeed)
            //    force = force.normalized * maxSpeed;
            //_physicsData.Velocity = newVelocity;

            //_physicsData.ExternalVelocity += (_physicsData.ExternalForce / 50f);
            //_physicsData.InputVelocity += (_physicsData.InputForce / 50);
            //_physicsData.InputVelocity.Truncate(_physicsData.MaxSpeed);

            //update the position
            // This will be done by sending the Velocity to the MB, which will then update the
            // RigidBody's velocity.  This will give us automatic collision detection.

            //////////////////m_vPos += Velocity;
            //var newVelocity = _physicsData.Velocity + (_physicsData.ExternalVelocity) + _physicsData.InputVelocity;
            //var newVelocity = _physicsData.Velocity + (_physicsData.ExternalVelocity);
            //_physicsData.Velocity = newVelocity;

            //if (newVelocity.Length() > 2f)
            //{
            //    int i = 1;
            //}
            //_physicsData.

            var velocity2 = GetVelocity3().xz();
            //_previousVelocity = velocity;

            //_physicsData.Velocity = velocity;
            // Cap the force added so it does not exceed max velocity
            //var forceTick = force / 50f;
            //var newMaxSpeed = (velocity + forceTick).magnitude;
            //if (newMaxSpeed > _physicsData.MaxSpeed)
            //{
            //    // How much we are over
            //    var adjustForceMagnitude = (newMaxSpeed - _physicsData.MaxSpeed) * 50f;
            //    var drag = Vector2.ClampMagnitude(force, adjustForceMagnitude);
            //    if (_rigidBody3D != null)
            //        _rigidBody3D.AddForce(-drag);
            //    if (_rigidBody2D != null)
            //        _rigidBody2D.AddForce(-drag);
            //}

            //AddForce(force);

            //AddForceLocal(force);
            if (_controller.GetPhysicsData().ApplyGravity)
            {
                // Add Gravity force
                //AddForce(new Vector3(0, _controller.GetPhysicsData().Gravity, 0));
                AddForce(_controller.GetPhysicsData().GravityVector);
                //AddForce(new Vector3(0, -9.8f, 0));
            }


            if (velocity2.sqrMagnitude < float.Epsilon)
            {
                velocity2 = Vector2.zero;
            }

            if (!_applySpeedLimit)
                return;

            float speed = velocity2.magnitude;  // test current object speed
            //var maxS
            if (speed > _controller.GetPhysicsData().MaxSpeed)
            {
                float brakeSpeed = speed - maxSpeed; // calculate the speed decrease
                var velocity3 = velocity2.xz();
                Vector3 normalisedVelocity = velocity3.normalized;
                Vector3 brakeVelocity = normalisedVelocity * brakeSpeed * 50f; // make the brake Vector3 value
                //if (_rigidBody3D != null)
                //    rigidbody.AddForce(-brakeVelocity);  // apply opposing brake force
                //AddForceLocal(transform.InverseTransformDirection(-brakeVelocity));
                AddForce(-brakeVelocity, ForceMode.Force);
                //if (_rigidBody3D != null)
                //_rigidBody3D.drag = brakeSpeed * 50f;
            }
            else
            {
                //if (_rigidBody3D != null)
                //_rigidBody3D.drag = 0f;
            }

            //velocity = Vector2.ClampMagnitude(velocity, _physicsData.MaxSpeed * _physicsData.MaxSpeedMultiplier);
            //SetVelocity(velocity);

            //update the heading if the vehicle has a non zero velocity

            //if (_autoUpdateHeading && velocity != Vector2.zero)
            //{
            //    SetHeading(velocity);
            //}
            //UpdateZ();
        }

        //private void ProcessSteeringBehaviorForce()
        //{
        //    var steeringForce = _steering.Calculate();
        //    Debug.Log($"Steering force: {steeringForce}");
        //    //steeringForce = Vector3.ProjectOnPlane(steeringForce, _controller.GroundNormal);
        //    if (steeringForce.sqrMagnitude < 0.1f)
        //    {
        //        _steeringForce = Vector3.zero;
        //        return;
        //    }
        //    _steeringForce = steeringForce;
        //    var steeringForceLocalSpace = transform.InverseTransformDirection(steeringForce);

        //    var _forwardAmount = steeringForceLocalSpace.z;

        //    var _turnAmount = Mathf.Atan2(steeringForceLocalSpace.x, steeringForceLocalSpace.z);

        //    float turnSpeed = Mathf.Lerp(_controller.GetPhysicsData().StationaryTurnSpeed, _controller.GetPhysicsData().MovingTurnSpeed,
        //        _forwardAmount);
        //    transform.Rotate(0, _turnAmount * turnSpeed * Time.deltaTime, 0);
        //    //Quaternion.rotat

        //    AddForce(steeringForce * _controller.GetPhysicsData().ForceMultiplier);
        //    SetAnimForwardSpeed(transform.forward.magnitude);
        //}

        /// <summary>
        /// Gets called by Update!
        /// </summary>
        private void ProcessSteeringBehaviorForce2()
        {
            var steeringForce = _steering.Calculate();
            steeringForce = Vector3.ProjectOnPlane(steeringForce, _controller.GroundNormal);

            _steeringForce = steeringForce;

            // Apply the turn before leaving so we can process TurnToTarget even when idle
            var forwardAndTurnAmount = CalculateForwardMovementAndTurnAmount(GetVelocity3());
            // Can't rotate while airborn
            if (Controller.GetPhysicsData().AutoTurn && Controller.GetIsGrounded())
                ApplyRotation(forwardAndTurnAmount.forwardAmount, forwardAndTurnAmount.turnAmount);
            //var forwardAndTurnAmount = CalculateForwardMovementAndTurnAmount(steeringForce);

            if (Controller.GetPhysicsData().AutoApplyToAnimator)
            {
                ApplyValuesToAnimator(forwardAndTurnAmount.forwardAmount, forwardAndTurnAmount.sideSpeed, forwardAndTurnAmount.turnAmount);
            }

            if (steeringForce.sqrMagnitude < 0.1f)
            {
                _steeringForce = Vector3.zero;
                return;
            }

            if (Controller.GetPhysicsData().InstantForce)
            {
                SetVelocity2(_steeringForce.xz().normalized * _controller.GetPhysicsData().MaxSpeed);
                //SetVelocity2(_steeringForce.xz().normalized * forwardAndTurnAmount.forwardAmount * _controller.GetPhysicsData().MaxSpeed);
            }
            else
            {
                AddForce(steeringForce * _controller.GetPhysicsData().ForceMultiplier);
                //AddForce(transform.forward * forwardAndTurnAmount.forwardAmount * _controller.GetPhysicsData().ForceMultiplier);
            }
            //SetAnimForwardSpeed(transform.forward.magnitude);
        }

        public (float forwardAmount, float sideSpeed, float turnAmount) CalculateForwardMovementAndTurnAmount(Vector3 force)
        {
            var localMove = transform.InverseTransformDirection(force);
            //localMove = Vector3.ProjectOnPlane(localMove, Controller.GroundNormal);
            // Forward amount is always based on the force
            // No walking backwards, character must turn.
            var forwardAmount = Mathf.Max(0f, localMove.z);
            float turnAmount = 0f;
            var turnMode = Controller.GetPhysicsData().TurnMode;
            if (turnMode == PhysicsData.TurnModeEnum.TowardsSteeringForce)
            {
                turnAmount = Mathf.Atan2(localMove.x, localMove.z);
            }
            else if (turnMode == PhysicsData.TurnModeEnum.TowardsTarget)
            {
                var vectorToTarget = _componentRepository.Target.transform.position.xz() - GetFeetWorldPosition2();
                var angle = Vector2.SignedAngle(transform.forward.xz(), vectorToTarget);
                turnAmount = Mathf.Deg2Rad * -angle;
            }
            else if (turnMode == PhysicsData.TurnModeEnum.TowardsSteeringBehaviorTarget)
            {
                var vectorToSteeringTarget = _steering.Target.xz() - GetFeetWorldPosition2();
                var angle = Vector2.SignedAngle(transform.forward.xz(), vectorToSteeringTarget);
                turnAmount = Mathf.Deg2Rad * -angle;
            }

            return (forwardAmount, localMove.x, turnAmount);
        }

        public void ApplyRotation(float forwardAmount, float turnAmount)
        {
            float turnSpeed = Mathf.Lerp(Controller.GetPhysicsData().StationaryTurnSpeed, Controller.GetPhysicsData().MovingTurnSpeed, forwardAmount);
            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
        }

        private void ApplyValuesToAnimator(float forwardSpeed, float sideSpeed, float turnSpeed)
        {
            _animationComponent.SetForwardSpeed(forwardSpeed);
            _animationComponent.SetTurnSpeed(turnSpeed);
            _animationComponent.SetSideSpeed(sideSpeed);
            if (Controller.GetIsGrounded())
                _animationComponent.SetVerticalSpeed(0f);
            else
                _animationComponent.SetVerticalSpeed(GetVelocity3().y);
        }

        public Rigidbody GetRidigbody()
        {
            return _rigidBody3D;
        }

        private void SetForwardSpeed(float speed)
        {
            if (_animator != null)
            {
                if (_animator.applyRootMotion)
                {
                    return;
                }
            }

            _rigidBody3D.AddRelativeForce(new Vector3(0f, 0f, speed));
        }

        private void SetAnimForwardSpeed(float speed)
        {
            if (_animationComponent != null)
                _animationComponent.SetForwardSpeed(speed);
        }

        //private void AddForceLocal(Vector3 force)
        //{
        //    if (_animator != null)
        //    {
        //        _animator.SetFloat(_animForwardSpeedVar, force.magnitude);
        //        if (_animator.applyRootMotion)
        //        {
        //            return;
        //        }
        //    }

        //    _rigidBody3D.AddRelativeForce(force);
        //}

        public override void StartListening()
        {
            base.StartListening();
        }

        public override void StopListening()
        {
            base.StopListening();
        }

        public Vector3 GetWorldPos3()
        {
            var height = _componentRepository.GetHeight();
            return transform.position + new Vector3(0f, height / 2f, 0f);
        }

        public Vector2 GetWorldPos2()
        {
            return transform.position.xz();
        }

        public void SetWorldPos2(Vector2 newPos)
        {
            // Depth needs to stay the same
            this.transform.position = new Vector3(newPos.x, this.transform.position.y, newPos.y);
            //m_vPosition = new_pos;
        }

        public void SetWorldPos3(Vector3 new_pos)
        {
            // Depth needs to stay the same
            this.transform.position = new_pos;
            //m_vPosition = new_pos;
        }

        public PhysicsData GetPhysicsData()
        {
            return _controller.GetPhysicsData();
        }

        public Vector2 GetVelocity2()
        {
            if (_rigidBody3D != null)
                return _rigidBody3D.velocity.xz();
            return Vector2.zero;
        }

        public Vector3 GetVelocity3()
        {
            if (_rigidBody3D != null)
                return _rigidBody3D.velocity;
            return Vector2.zero;
        }

        public void SetVelocity3(Vector3 velocity)
        {
            if (_rigidBody3D != null)
                _rigidBody3D.velocity = velocity;
        }

        public void SetVelocity2(Vector2 velocity)
        {
            if (_rigidBody3D != null)
                _rigidBody3D.velocity = new Vector3(velocity.x, _rigidBody3D.velocity.y, velocity.y);
        }

        //public virtual void AddVelocity3(Vector3 velocity)
        //{ 
        //    if (_rigidBody3D != null)
        //        _rigidBody3D.velocity += velocity;
        //}

        public void AccelerateTo(Vector3 targetVelocity, float maxAccel)
        {
            // Ignore y (gravity)
            //var deltaV = targetVelocity.xz() - _rigidBody3D.velocity.xz();
            //var deltaV = targetVelocity.xz();
            //var accel = deltaV / Time.deltaTime;
            //var accel = deltaV.xz().normalized * _controller.GetPhysicsData().MaxForce;
            var accel = targetVelocity.normalized * _controller.GetPhysicsData().MaxForce;
            //if (accel.sqrMagnitude > maxAccel * maxAccel)
            //    accel = accel.normalized * maxAccel;
            _rigidBody3D.AddForce(accel);
        }

        //public void AddVelocity2(Vector2 velocity)
        //{
        //    if (_rigidBody3D != null)
        //        _rigidBody3D.velocity = velocity;
        //}

        public void AddForce(Vector3 force)
        {
            if (_rigidBody3D != null)
                _rigidBody3D.AddForce(force);
        }

        public void AddForce2(Vector2 force)
        {
            if (_rigidBody3D != null)
                _rigidBody3D.AddForce(force.xz());
        }

        public void AddForce(Vector3 force, ForceMode forceMode)
        {
            if (_rigidBody3D != null)
                _rigidBody3D.AddForce(force, forceMode);
        }

        public void AddForce2(Vector2 force, ForceMode forceMode)
        {
            if (_rigidBody3D != null)
                _rigidBody3D.AddForce(force.xz(), forceMode);
        }

        public void Jump()
        {
            AddForce(GetPhysicsData().JumpVelocity, ForceMode.VelocityChange);
        }

        public virtual Vector2 GetFeetWorldPosition2()
        {
            //return _controller.GetPhysicsData().Foot.transform.position.xz();
            return transform.position.xz();
        }

        public virtual Vector3 GetFeetWorldPosition3()
        {
            //return _controller.GetPhysicsData().Foot.transform.position;
            return transform.position;
        }

        public virtual void SetFeetWorldPosition2(Vector2 pos)
        {
            //SetWorldPos2(new Vector2(pos.x, pos.y) - (Vector2)_physicsData._footOffset);
            SetWorldPos2(new Vector2(pos.x, pos.y) - _controller.GetPhysicsData().Foot.transform.localPosition.xz());
        }

        public virtual void SetFeetWorldPosition3(Vector3 pos)
        {
            SetWorldPos3((Vector3)pos - _controller.GetPhysicsData().Foot.transform.localPosition);
        }

        public Vector3 GetLocalPos3()
        {
            return transform.localPosition;
        }

        public void Stop()
        {
            SetVelocity3(Vector3.zero);
            //_rigidBody3D?.Sleep();

            //_physicsData.InputVelocity = Vector2.zero;
            //_physicsData.Velocity.SetToZero();
            //if (_physicsAffectors == null)
            //    return;
            //foreach (var affector in _physicsAffectors.Values)
            //for (int i = 0; i < _physicsAffectorsList.Count; i++)
            //{
            //    var affector = _physicsAffectorsList[i];
            //    affector.Stop();
            //    affector.Force = Vector2.zero;
            //}
        }

        public ISteeringBehaviorManager GetSteering()
        {
            return _steering;
        }

        public void SetEnabled(bool enabled)
        {
            _isEnabled = enabled;
        }

        public bool GetEnabled()
        {
            return _isEnabled;
        }

        public void Explode(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier)
        {
            _rigidBody3D.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, upwardsModifier);
        }

        public void EnableCollider(bool enabled)
        {
            var colliders = GetComponents<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                if (!colliders[i].isTrigger)
                    colliders[i].enabled = enabled;
            }
        }

        public void JumpViaDistance(float distance)
        {
            //var maxSpeed = _physicsComponent.GetPhysicsData().MaxSpeed;
            var maxSpeed = _controller.OriginalPhysicsData.MaxSpeed;
            // You jump farther the faster the character is running (there is no force involved, they run at the speed of axisInput * maxSpeed)
            //var jumpDistance = _jumpDistance * _axisInput.magnitude;
            // Get the time it takes to make the leap, based on Max Speed
            var time = distance / maxSpeed;
            // Use a modified version of the displacement foruma to calculate the initial velocity, using time,
            // gravity and displacement
            // Displacement formula can be found on Khan Academy under "Deriving displacement as a function of time, acceleration
            // and initial velicty.
            //var numerator = ((0.5f * _physicsComponent.GetPhysicsData().Gravity * time * time) - _jumpDistance);
            //var initialVelocity = numerator / -time;

            // Derived from Final Velocity = Initial Velocity + a * t, assuming final velocity = -Initial Velocity.
            var initialVelocity = GetPhysicsData().Gravity * time / -2;

            AddForce(new Vector3(0f, initialVelocity, 0f), ForceMode.VelocityChange);
            ////var velocity = _physicsComponent.GetVelocity3();
            //var jumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(_physicsComponent.GetPhysicsData().Gravity) * _jumpHeight);
            ////_physicsComponent.SetVelocity3(velocity);
            //_physicsComponent.AddForce(new Vector3(0f, jumpVelocity, 0f), ForceMode.VelocityChange);
        }

        public void OnDrawGizmos()
        {
            _steering.OnDrawGizmos();
            Gizmos.color = Color.black;
            Gizmos.DrawLine(transform.position, transform.position + _steeringForce);
            Gizmos.DrawSphere(_steering.Target + new Vector3(0, 0.2f, 0), 0.2f);
        }
    }
}
