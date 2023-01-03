using Rewired;
using RQ.Base.Attributes;
using RQ.Base.Extensions;
using RQ.Common.Components;
using RQ.Common.Container;
using RQ.Controller.Player;
using RQ.Physics.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.RQ_Assets.Scripts.Components
{
    [AddComponentMenu("RQ/Components/Lock On Component")]
    public class LockOnComponent : ComponentBase<LockOnComponent>
    {
        [SerializeField]
        private string _targetLockAction;
        [SerializeField]
        private float _clickNextTargetTime = 1f;
        public float ClickNextTargetTime => _clickNextTargetTime;
        [SerializeField]
        private string _cameraLockOnMessage;
        public string CameraLockOnMessage => _cameraLockOnMessage;
        [SerializeField]
        private string _cameraNormalMessage;
        public string CameraNormalMessage => _cameraNormalMessage;
        [SerializeField]
        private string _cameraOverShoulderMessage;
        [SerializeField]
        private string _cameraCenterMessage;
        public string CameraCenterMessage => _cameraCenterMessage;

        [SerializeField]
        private LayerMask _targetLockLayers;
        [SerializeField]
        [Tag]
        private string[] _tags;

        private RQ.Controller.Player.PlayerController _playerController;
        private Rewired.Player _rewiredPlayer;
        private Dictionary<int, GameObject> tempGameObjectDict = new Dictionary<int, GameObject>();        

        [SerializeField]
        private LockOnToggleLogic _lockOnLogic;

        public void Start()
        {
            if (_playerController == null)
                _playerController = _componentRepository.Components.GetComponent<RQ.Controller.Player.PlayerController>();
            if (_rewiredPlayer == null)
                _rewiredPlayer = ReInput.players.GetPlayer(0);

            _lockOnLogic = new LockOnToggleLogic(this, _playerController);
            
            //_camera = entity.Components.GetComponent<ThirdPersonCameraController>();
        }

        public void Update()
        {
            //if (_rewiredPlayer.GetButtonUp(_targetLockAction))
            //{

            //}
            //Debug.Log(_targetLockAction);
            if (_rewiredPlayer.GetButtonDown(_targetLockAction))
            {
                _lockOnLogic.ProcessButtonDown();
            }

            if (_rewiredPlayer.GetButtonUp(_targetLockAction))
            {
                _lockOnLogic.ProcessButtonUp();
            }
        }

        /// <summary>
        /// Finds all the close lock targets, sorted by distance
        /// </summary>
        /// <returns></returns>
        public IList<GameObject> CreateTargetList()
        {
            int layerMask = 0;
            layerMask = _targetLockLayers.value;
            //for (int i = 0; i < _targetLockLayers.)
            //if (!UnityEngine.Physics.SphereCast(transform.position, _targetLockRadius, transform.forward, out var hit,
            //    _targetLockDistance, layerMask))

            //for (int i = 0; i < hitResults.Length; i++)
            //{
            //    hitResults[i] = null;
            //}

            //var hitResults = UnityEngine.Physics.OverlapSphere(transform.position, _targetLockRadius);
            //var hitResults = UnityEngine.Physics.OverlapBox(transform.position,
            //    new Vector3(_targetLockRadius, .1f, _targetLockRadius));

            //DrawBoxCast.DrawBoxCastBox(transform.position,
            //    new Vector3(_targetLockRadius, .1f, _targetLockRadius), Quaternion.identity, transform.forward, _targetLockRadius, Color.red);

            //var hitResults = UnityEngine.Physics.BoxCastAll(transform.position,
            //    new Vector3(_targetLockRadius, .1f, _targetLockRadius), transform.forward, Quaternion.identity, _targetLockRadius);

            var position = _playerController.transform.position + (_playerController.transform.forward *
                _playerController.TargetLockRadius) + new Vector3(0, 1, 0);

            //DrawBoxCast.DrawBox(new Box(position,
            //    new Vector3(_targetLockRadius, 1f, _targetLockRadius)), Color.red, 5f);

            DrawBoxCast.DrawBox(position,
                new Vector3(_playerController.TargetLockRadius, 1f, _playerController.TargetLockRadius), _playerController.transform.rotation, Color.red, 5f);

            var hitResults = UnityEngine.Physics.OverlapBox(position,
                new Vector3(_playerController.TargetLockRadius, 1f, _playerController.TargetLockRadius), _playerController.transform.rotation, layerMask);

            // Order the hitResults by distance from the entity
            hitResults = hitResults.OrderBy(i => Vector2.SqrMagnitude(i.attachedRigidbody.transform.position.xz() - _componentRepository.GetPosition().xz())).ToArray();

            if (hitResults.Length == 0)
            {
                Debug.Log("Could not locate nearest target");
                return null;
            }
            else
            {
                Debug.Log($"Found {hitResults.Length} hit targets");

                //PrintCastHitNames(hitResults);

                //return null;
            }

            //if (UnityEngine.Physics.OverlapSphereNonAlloc(transform.position, _targetLockRadius, hitResults, layerMask) == 0)
            //{
            //    print("Could not locate nearest target");
            //    return null;
            //}

            return FilterCastResults(hitResults);
        }

        private static void PrintCastHitNames(Collider[] hitResults)
        {
            for (int i = 0; i < hitResults.Length; i++)
            {
                if (hitResults[i].attachedRigidbody != null)
                    Debug.Log($"(AutoLock) Hit target {hitResults[i].attachedRigidbody.name}");
            }
        }

        public IEnumerable<GameObject> FilterCastResults(RaycastHit[] hitResults)
        {
            if (hitResults.Length == 0)
                return null;
            // TODO Remove the LINQ statement
            var sortedResults = hitResults.Where(i => i.rigidbody != null).OrderBy(i => i.distance);

            //Array.Sort()
            //Array.Sort(hitResults, PhysicsHelper.RaycastDistanceCompareDel);

            //autoLockableObjects.Clear();
            tempGameObjectDict.Clear();

            //for (int i = 0; i < sortedResults.Count; i++)
            foreach (var hit in sortedResults)
            {
                //var hit = hitResults[i];
                var gameobject = hit.rigidbody.gameObject;
                if (gameobject == null)
                    continue;
                // Don't process the same game object twice
                if (tempGameObjectDict.ContainsKey(gameobject.GetInstanceID()))
                    continue;
                tempGameObjectDict.Add(gameobject.GetInstanceID(), gameobject);
                Debug.Log($"Cast hit target {gameobject.name}");
            }
            //var closest = hitResults[0].attachedRigidbody.GetComponent<SpriteCommonComponent>();
            //var closest = hit.rigidbody.GetComponent<SpriteCommonComponent>();
            //return closest;
            //return hitResults[0].attachedRigidbody.gameObject;
            return tempGameObjectDict.Values;
        }

        public IList<GameObject> FilterCastResults(Collider[] hitResults)
        {
            if (hitResults.Length == 0)
                return null;
            // TODO Remove the LINQ statement
            var sortedResults = hitResults.Where(i => i.attachedRigidbody != null).OrderBy(i => i.ClosestPoint(_playerController.transform.position).sqrMagnitude);

            //Array.Sort()
            //Array.Sort(hitResults, PhysicsHelper.RaycastDistanceCompareDel);

            //autoLockableObjects.Clear();
            tempGameObjectDict.Clear();

            //for (int i = 0; i < sortedResults.Count(); i++)
            foreach (var hit in sortedResults)
            {
                bool hasTag = Array.IndexOf(_tags, hit.tag) > -1;
                if (!hasTag)
                    continue;
                //var hit = hitResults[i];
                var gameobject = hit.attachedRigidbody.gameObject;
                if (gameobject == null)
                    continue;
                // Don't process the same game object twice
                if (tempGameObjectDict.ContainsKey(gameobject.GetInstanceID()))
                    continue;
                tempGameObjectDict.Add(gameobject.GetInstanceID(), gameobject);
                Debug.Log($"Cast hit target {gameobject.name}");
            }
            //var closest = hitResults[0].attachedRigidbody.GetComponent<SpriteCommonComponent>();
            //var closest = hit.rigidbody.GetComponent<SpriteCommonComponent>();
            //return closest;
            //return hitResults[0].attachedRigidbody.gameObject;
            return tempGameObjectDict.Values.ToList();
        }

        //public IEnumerable<GameObject> FilterTargetResults()
        //{
        //    // TODO Remove the LINQ statement
        //    var sortedResults = hitResults.Where(i => i != null && i.attachedRigidbody != null).OrderBy(i => i.ClosestPoint(transform.position).sqrMagnitude);

        //    //Array.Sort()
        //    //Array.Sort(hitResults, PhysicsHelper.RaycastDistanceCompareDel);

        //    //autoLockableObjects.Clear();
        //    tempGameObjectDict.Clear();

        //    //for (int i = 0; i < sortedResults.Count; i++)
        //    foreach (var hit in sortedResults)
        //    {
        //        //var hit = hitResults[i];
        //        var gameobject = hit.attachedRigidbody?.gameObject;
        //        if (gameobject == null)
        //            continue;
        //        // Don't process the same game object twice
        //        if (tempGameObjectDict.ContainsKey(gameobject.GetInstanceID()))
        //            continue;
        //        tempGameObjectDict.Add(gameobject.GetInstanceID(), gameobject);
        //    }
        //    //var closest = hitResults[0].attachedRigidbody.GetComponent<SpriteCommonComponent>();
        //    //var closest = hit.rigidbody.GetComponent<SpriteCommonComponent>();
        //    //return closest;
        //    //return hitResults[0].attachedRigidbody.gameObject;
        //    return tempGameObjectDict.Values;
        //}

        public void SetTargetLockLayers(LayerMask layers)
        {
            _targetLockLayers = layers;
        }

        public void SetTags(string[] tags)
        {
            _tags = tags;
        }

        //public void SetCenterAction(Action centerAction)
        //{
        //    _centerAction = centerAction;
        //}

        //public void SetLockOnAction(Action lockOnAction)
        //{
        //    _lockOn = lockOnAction;
        //}

        public bool IsATargetLocked() => _lockOnLogic.IsATargetLocked();

        public bool IsStrafePressed() => _lockOnLogic.IsStrafePressed();

        //public void LateUpdate()
        //{
        //    if (!_lockOnLogic.IsATargetLocked())
        //        return;
        //    //_componentRepository.Target = _lockGO;
        //    _playerController.LookAtTarget();
        //}

        public void SendCameraNormalMessage()
        {

        }

        public IEntity LockGO => _lockOnLogic.LockGO;
    }
}
