using RQ.Base.Manager;
using RQ.Common.Components;
using RQ.Controller.Player;
using System;
using UnityEngine;

namespace RQ.Controller.Component
{
    [AddComponentMenu("RQ/Components/Shooter")]
    public class ShooterComponent : ComponentBase<ShooterComponent>
    {
        [SerializeField]
        private GameObject _projectileCreatePoint;
        public GameObject ProjectileCreatePoint => _projectileCreatePoint;
        //[SerializeField] private PlayerController _playerController;
        //[SerializeField] private GameObject _projectile;
        [SerializeField] private float _speed;

        //protected override void Awake()
        //{
        //    base.Awake();
        //    Debug.Log("Block Component is awake!");
        //}

        public void Shoot()
        {
            //transform.TransformPoint(_projectileCreatePoint);
            GameObject newObject = null;

            var projectilePrefab = GameStateController.Instance.CurrentShard == null ? 
                GameStateController.Instance.CurrentMold.ReferencePrefab : 
                GameStateController.Instance.CurrentShard.ReferencePrefab;
            if (projectilePrefab == null)
            {
                Debug.LogError($"Associated item {GameStateController.Instance.CurrentShard.name} has no prefab reference.");
                return;
            }

            var shooterRotation = transform.rotation;

            CreateMuzzleFlash(_projectileCreatePoint.transform.position, shooterRotation);

            newObject = GameObject.Instantiate(projectilePrefab, _projectileCreatePoint.transform.position, shooterRotation);
            var rigidBody = newObject.GetComponent<Rigidbody>();
            if (rigidBody == null)
                throw new Exception("Could not locate PhysicsComponent or RigidBody!");
            var direction = transform.forward;
            var velocity = direction.normalized * _speed;
            rigidBody.velocity = velocity;
        }

        public void CreateMuzzleFlash(Vector3 pos, Quaternion rotation)
        {
            // Muzzle Flash
            if (GameStateController.Instance.CurrentShard.MuzzleFlash == null)
            {
                Debug.LogError("This shard has no muzzle flash. Please apply one.");
                return;
            }

            GameObject.Instantiate(GameStateController.Instance.CurrentShard.MuzzleFlash, pos, rotation);
        }
    }
}
