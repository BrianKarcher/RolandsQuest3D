using RQ.Base.Extensions;
using RQ.Controller.Camera;
using RQ.Physics;
using UnityEngine;

namespace RQ.Controller.Player
{
    public class OverShoulderMovementLogic : IPlayerMovementLogic
    {
        private PlayerController _playerController;
        private ThirdPersonCameraController _camera;
        // TODO Unserialize this!
        [SerializeField]
        private float _forwardAmount;
        // TODO Unserialize this!
        //[SerializeField]
        private float _turnAmount;
        private PhysicsComponent _physicsComponent;
        private AnimationComponent _animationComponent;

        public OverShoulderMovementLogic(PlayerController playerController)
        {
            _playerController = playerController;
            _camera = UnityEngine.Camera.main.transform.parent.GetComponent<ThirdPersonCameraController>();
            _physicsComponent = playerController.GetComponent<PhysicsComponent>();
            _animationComponent = playerController.GetComponentRepository().GetComponent<AnimationComponent>();
        }


    }
}
