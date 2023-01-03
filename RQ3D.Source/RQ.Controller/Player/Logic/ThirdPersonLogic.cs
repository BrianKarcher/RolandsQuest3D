using RQ.Messaging;
using RQ.Physics;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RQ.Controller.Player
{
    public class ThirdPersonMovementLogic : IPlayerMovementLogic
    {
        private PlayerController _playerController;
        // TODO: Remove this
        [SerializeField]
        private Vector3 _axisInput;
        // TODO Unserialize this!
        [SerializeField]
        private float _forwardAmount;
        // TODO Unserialize this!
        [SerializeField]
        private float _turnAmount;
        private PhysicsComponent _physicsComponent;
        private AnimationComponent _animationComponent;

        public ThirdPersonMovementLogic(PlayerController playerController)
        {
            _playerController = playerController;
            _physicsComponent = playerController.GetComponent<PhysicsComponent>();
            _animationComponent = playerController.GetComponentRepository().GetComponent<AnimationComponent>();
        }

        public void Setup()
        {

        }

        public void ProcessInput(MovementInputData inputData)
        {


        }
    }
}
