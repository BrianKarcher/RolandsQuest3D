using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RQ.Base.Components;
using RQ.Common.Components;
using RQ.Common.Container;
using RQ.Controller.Damage;
using RQ.Messaging;
using RQ.Physics;
using RQ.Physics.Helpers;
using UnityEngine;

namespace RQ.Controller.Attack
{
    [AddComponentMenu("RQ/Components/Attack Component")]
    public class AttackComponent : ComponentBase<AttackComponent>
    {
        [SerializeField]
        private AttackData _attackData;
        [SerializeField]
        private string _messageToSend;
        [SerializeField]
        private GameObject _hitVFX;

        [SerializeField]
        private AttackParryComponent _parry;

        [SerializeField] private bool _debug;
        [SerializeField] private int _attackAnim = -1;

        private PhysicsComponent _physicsComponent;
        private DamageComponent _damageComponent;
        //private AnimationComponent _animationComponent;
        private DateTime? _lastAttackTime;
        public event Action Attacked;

        private bool _hitDetect;
        private float _attackTime;
        private bool _attackTimerActive;
        private bool _attackComplete;
        public bool AttackComplete => _attackComplete;

        protected override void Awake()
        {
            base.Awake();
            // Parry must be in the same GameObject.
            if (_parry == null)
                _parry = GetComponent<AttackParryComponent>();
            _attackTimerActive = false;
            _attackComplete = false;
        }

        private void Start()
        {
            if (_physicsComponent == null)
                _physicsComponent = _componentRepository.Components.GetComponent<PhysicsComponent>();
            if (_damageComponent == null)
                _damageComponent = _componentRepository.Components.GetComponent<DamageComponent>();
            //if (_animationComponent == null)
            //    _animationComponent = _componentRepository.Components.GetComponent<AnimationComponent>();
        }

        private void Update()
        {
            if (_attackTimerActive && Time.time > _attackTime)
            {
                ProcessAttackNow();
                _attackTimerActive = false;
            }
        }

        public void InitiateAttack()
        {
            _attackComplete = false;
            _attackTimerActive = true;
            _attackTime = Time.time + _attackData.StrikeDelay;
            _parry?.StartTimer();
        }

        public void Stop()
        {
            _attackTimerActive = false;
            _parry?.Stop();
        }

        private readonly HashSet<string> _entitiesHit = new HashSet<string>();
        private RaycastHit[] _itemHits = new RaycastHit[10];
        private Collider[] _itemHitsCol = new Collider[10];
        public virtual void ProcessAttackNow()
        {
            _attackComplete = true;
            Attacked?.Invoke();

            if (_damageComponent == null)
                throw new Exception("Damage component is required for the Attack component in entity " + GetComponentRepository().name);
            //base.ProcessAttack();
            if (_attackData.StopMovingDuringAttack)
            {
                //var theSprite = _entity as ISprite;
                if (_physicsComponent != null)
                {
                    _physicsComponent.Stop();
                    //MessageDispatcher.Instance.DispatchMsg(0f, entity.UniqueId, _physicsComponent.UniqueId,
                    //    Enums.Telegrams.StopMovement, null);
                }
                //_physicsComponent.Stop();
            }

            MessageDispatcher.Instance.DispatchMsg("AttackPerformed", 0f, GetComponentRepository().GetId(), _damageComponent.GetId(), null);
            //var facingDirectionVector = _animationComponent.GetFacingDirectionVector();
            var facingDirectionVector = transform.forward;
            var pos = transform.position;
            int layerMask = _attackData.Layers;
            //int layerMask = 0;
            //if (_attackData.SameLayer)
            //{
            //    var environmentLayerMask = LayerMask.NameToLayer("Environment");
            //    //var layerIndex = _collisionComponent.transform.gameObject.layer;
            //    //layerMask = 1 << layerIndex;
            //    layerMask = environmentLayerMask;
            //    //layerMask = ((LayerMask)).value;
            //}
            //else
            //{
            //    foreach (var masks in _attackData.Layers)
            //    {
            //        layerMask |= masks.Mask;
            //    }
            //    //layerMask = AttackData.Layer.Mask;
            //}

            //var heading = _physicsComponent.GetPhysicsData().Heading;
            //GameDataController.Instance.GetLayerMask(LevelLayer.LevelOne);
            //var itemsHit = Physics2D.BoxCastAll(pos + _offset, _size, _angle, facingDirection, _distance, layerMask/*, GetEntityUI().GetTransform().gameObject.layer*/);
            //IEnumerable<RaycastHit2D> itemsHit = Physics2D.BoxCastAll((Vector2)pos + AttackData.Offset, AttackData.Size, AttackData.Angle, facingDirection, AttackData.Distance);
            var attackSize = (Vector3)_attackData.Size;
            //var facingDirection = _animationComponent.GetFacingDirection();
            //if (facingDirection == Direction.Left || facingDirection == Direction.Right)
            //    attackSize.x = 0.1f;
            //else
            //    attackSize.y = 0.1f;
            //attackSize.z = 0.3f;

            var attackPos = transform.TransformPoint(_attackData.Offset);
            var halfExtent = attackSize / 2f;

            //var worldOffset = transform.TransformPoint(_attackData.Offset);

            //var attackPos = pos + worldOffset;
            //Debug.LogError($"Attacker Pos {attackPos}");

            //_maxDistance = AttackData.Distance;
            //_testDirection = facingDirectionVector;
            //_testPosition = attackPos;
            //_size = attackSize / 2;
            _lastAttackTime = DateTime.Now;

            //_hitTarget = false;

            //var left = attackPos.x - halfExtent.x;
            //var right = attackPos.x + halfExtent.x;
            //var up = attackPos.y + halfExtent.y;
            //var down = attackPos.y - halfExtent.y;
            //var forward = attackPos.z + halfExtent.z;
            //var back = attackPos.z - halfExtent.z;

            //var left = attackPos - transform.right * halfExtent.x;
            //var right = attackPos + transform.right * halfExtent.x;
            //var up = attackPos + transform.up * halfExtent.y;
            //var down = attackPos - transform.up * halfExtent.y;
            //var forward = attackPos + transform.forward * halfExtent.z;
            //var back = attackPos - transform.forward * halfExtent.z;

            //if (_debug)
            //{
            //    var leftUpBack = transform.TransformPoint(_attackData.Offset + new Vector3(-halfExtent.x, halfExtent.y, -halfExtent.z));
            //    var rightUpBack = transform.TransformPoint(_attackData.Offset + new Vector3(halfExtent.x, halfExtent.y, -halfExtent.z));
            //    var rightDownBack = transform.TransformPoint(_attackData.Offset + new Vector3(halfExtent.x, -halfExtent.y, -halfExtent.z));
            //    var leftDownBack = transform.TransformPoint(_attackData.Offset + new Vector3(-halfExtent.x, -halfExtent.y, -halfExtent.z));

            //    var leftUpForward = transform.TransformPoint(_attackData.Offset + new Vector3(-halfExtent.x, halfExtent.y, halfExtent.z));
            //    var rightUpForward = transform.TransformPoint(_attackData.Offset + new Vector3(halfExtent.x, halfExtent.y, halfExtent.z));
            //    var rightDownForward = transform.TransformPoint(_attackData.Offset + new Vector3(halfExtent.x, -halfExtent.y, halfExtent.z));
            //    var leftDownForward = transform.TransformPoint(_attackData.Offset + new Vector3(-halfExtent.x, -halfExtent.y, halfExtent.z));

            //    // Draw box outline of the attack.                
            //    Debug.DrawLine(leftUpBack, leftUpForward, Color.red, 5f);
            //    Debug.DrawLine(leftUpForward, rightUpForward, Color.red, 5f);
            //    Debug.DrawLine(rightUpForward, rightUpBack, Color.red, 5f);
            //    Debug.DrawLine(rightUpBack, leftUpBack, Color.red, 5f);

            //    Debug.DrawLine(leftUpBack, rightUpBack, Color.red, 5f);
            //    Debug.DrawLine(rightUpBack, rightDownBack, Color.red, 5f);
            //    Debug.DrawLine(rightDownBack, leftDownBack, Color.red, 5f);
            //    Debug.DrawLine(leftDownBack, leftUpBack, Color.red, 5f);

            //    //Debug.DrawLine(new Vector3(left, up, back), new Vector3(left, up, forward), Color.red, 5f);
            //    //Debug.DrawLine(new Vector3(left, up, forward), new Vector3(right, up, forward), Color.red, 5f);
            //    //Debug.DrawLine(new Vector3(right, up, forward), new Vector3(right, up, back), Color.red, 5f);
            //    //Debug.DrawLine(new Vector3(right, up, back), new Vector3(left, up, back), Color.red, 5f);

            //    //Debug.DrawLine(new Vector3(left, up, back), new Vector3(right, up, back), Color.red, 5f);
            //    //Debug.DrawLine(new Vector3(right, up, back), new Vector3(right, down, back), Color.red, 5f);
            //    //Debug.DrawLine(new Vector3(right, down, back), new Vector3(left, down, back), Color.red, 5f);
            //    //Debug.DrawLine(new Vector3(left, down, back), new Vector3(left, up, back), Color.red, 5f);
            //}

            _hitDetect = false;

            //int hitCount = UnityEngine.Physics.BoxCastNonAlloc(attackPos, halfExtent, transform.forward, _itemHits, transform.rotation, _attackData.Distance);
            //int hitCount = UnityEngine.Physics.OverlapBoxNonAlloc(attackPos, halfExtent, _itemHits, transform.rotation);
            //var hits = UnityEngine.Physics.OverlapBox(attackPos, halfExtent, transform.rotation);
            DrawBoxCast.DrawBoxCastBox(attackPos, halfExtent, transform.rotation, transform.forward, _attackData.Distance, Color.red, 1f);
            Debug.Log($"Attack pos: {attackPos}, extents: {halfExtent}, forward: {transform.forward}, distance: {_attackData.Distance}");
            //int hitCount = UnityEngine.Physics.BoxCastNonAlloc(attackPos, halfExtent, transform.forward, _itemHits, transform.rotation, _attackData.Distance);

            _itemHits = UnityEngine.Physics.BoxCastAll(attackPos, halfExtent, transform.forward, transform.rotation, _attackData.Distance);

            Debug.Log($"Hit count: {_itemHits.Length}");
            //Debug.Log($"Hit count: {hitCount}");

            //for (int i = hitCount; i < _itemHits.Length; i++)
            //{
            //    _itemHits[i] = RaycastHit.;
            //}
            //return;
            //Array.Sort(_itemHits, PhysicsHelper.RaycastDistanceCompareDel);

            // TODO - Remove LINQ expression, it wastes memory
            _itemHits = _itemHits.OrderBy(i => GetDistanceBetweenRaycastHitAndSelf(i)).ToArray();

            //HashSet<string> entitiesHit = new HashSet<string>();
            //var entitiesHit = ObjectPool.Instance.PullFromPool<HashSet<string>>(ObjectPoolType.HashSetString);
            _entitiesHit.Clear();
            //foreach (var itemHit in _itemHits)
            for (int i = 0; i < _itemHits.Length; i++)
            {
                var itemHit = _itemHits[i];
                // Only do triggers for now.
                if (!itemHit.collider.isTrigger)
                    continue;
                //if (itemHit == null)
                //    continue;

                //_hitTarget = true;
                //_hitDistance = itemHit.distance;

                // This assumes the Rigidbody is on the same game object as the SpriteBaseComponent. Cannot assume!
                //var entity = itemHit.collider.attachedRigidbody.transform.GetComponent<SpriteBaseComponent>();
                //ComponentRepository otherEntity = null;

                //var otherEntity = itemHit.attachedRigidbody?.GetComponent<ComponentRepository>();
                var otherEntity = itemHit.collider?.attachedRigidbody?.GetComponent<ComponentRepository>();
                if (otherEntity == null)
                    continue;

                // It sounds fun, but no, you can't hit yourself
                if (otherEntity.GetId() == _componentRepository.GetId())
                    continue;

                Debug.Log("Hit : " + otherEntity.name);
                //            // Make sure each entity gets the message only once

                //if (_targetTags.Contains(entity.GetTag()))

                //var itemHitTag = itemHit.tag;

                if (_entitiesHit.Contains(otherEntity.GetId()))
                    continue;

                var itemHitTag = itemHit.collider.tag;
                if (_attackData.DeflectTags.Contains(itemHitTag))
                {
                    Debug.LogWarning($"{_componentRepository.name} got Deflected!");
                    MessageDispatcher.Instance.DispatchMsg("Deflected", 0f, _componentRepository.GetId(), _componentRepository.GetId(), itemHit);
                    MessageDispatcher.Instance.DispatchMsg("DeflectedOther", 0f, _componentRepository.GetId(), otherEntity.GetId(), 5);
                    // Can't damage anything during a deflection
                    return;
                }

                var tagFound = Array.IndexOf(_attackData.TargetTags, itemHitTag) > -1;
                if (!tagFound)
                    continue;

                //if (AttackData.TargetTags.Contains(itemHit.collider.tag))

                //var collisionComponent = itemHit.collider.GetComponent<ICollisionComponent>();
                //item.point
                //Debug.LogWarning("Hit " + itemHit.collider.name);
                //Debug.LogError($"Hit - Enemy position {otherEntity.transform.position}");
                //Debug.LogError($"Hit - Enemy Collider position {itemHit.collider.transform.position}");
                //var skillUsed = _skill == null ? null : _skill.UniqueId;

                //var hitPosition = itemHit.ClosestPoint(attackPos);
                var hitPosition = itemHit.point;
                var collider = itemHit.collider;
                Debug.Log($"{_componentRepository.name} inflicting {_attackData.Damage} on {otherEntity.name}");

                CreateHitVFX(itemHit);

                _damageComponent.DamageExternalEntity(otherEntity, collider, _attackData.Damage, hitPosition);
                //_damageComponent.DamageExternalEntity(otherEntity.GetId(), _attackData.Damage,
                //    GetComponentRepository().GetTag(), itemHit.point,
                //    otherCollisionComponent as ICollisionComponent, skillUsed);
                MessageDispatcher.Instance.DispatchMsg(_messageToSend, 0f, _componentRepository.GetId(),
                    otherEntity.GetId(), null);
                _entitiesHit.Add(otherEntity.GetId());
                _hitDetect = true;

            }

            //ObjectPool.Instance.ReleaseToPool(ObjectPoolType.HashSetString, entitiesHit);

            //if (!UnityEngine.Physics.BoxCast(attackPos, attackSize / 2, facingDirectionVector, out var itemHit, Quaternion.identity, AttackData.Distance, layerMask))
            //    return;

            // This assumes the Rigidbody is on the same game object as the SpriteBaseComponent. Cannot assume!
            //var entity = itemHit.collider.attachedRigidbody.transform.GetComponent<SpriteBaseComponent>();
            //if (entity != null)
            //{
            //    //if (_targetTags.Contains(entity.GetTag()))
            //    if (AttackData.TargetTags.Contains(itemHit.collider.tag))
            //    {
            //        var collisionComponent = itemHit.collider.GetComponent<ICollisionComponent>();
            //        //item.point
            //        Debug.LogWarning("Hit " + itemHit.collider.name);
            //        Debug.LogError($"Hit - Enemy position {entity.transform.position}");
            //        Debug.LogError($"Hit - Enemy Collider position {itemHit.collider.transform.position}");
            //        var skillUsed = _skill == null ? null : _skill.UniqueId;
            //        _damageComponent.DamageExternalEntity(entity.UniqueId, AttackData.Damage, GetComponentRepository().GetTag(), itemHit.point, collisionComponent, skillUsed);
            //        MessageDispatcher2.Instance.DispatchMsg(_messageToSend, 0f, _componentRepository.UniqueId, entity.UniqueId, null);
            //    }
            //}

            //IEnumerable<RaycastHit> itemsHit = UnityEngine.Physics.BoxCastAll((Vector2)pos + AttackData.Offset, new Vector3(AttackData.Size.x, AttackData.Size.y, .01f), facingDirection, Quaternion.identity, AttackData.Distance);

            //if (itemsHit == null)
            //    return;

            //foreach (var rayCast in itemsHit)
            //{
            //    Debug.DrawLine((Vector2)pos + AttackData.Offset, rayCast.point, Color.blue, 3f);
            //}
            //// Start with the closest collisions
            //itemsHit = itemsHit.OrderBy(i => i.distance);
            //HashSet<string> entitiesHit = new HashSet<string>();

            //foreach (var item in itemsHit)
            //{
            //    if (item.collider != null && item.collider.attachedRigidbody != null)
            //    {
            //        if (!_collisionComponent.CheckCollisionBasedOnLayer(item.collider))
            //            continue;
            //        var entity = item.collider.attachedRigidbody.transform.GetComponent<SpriteBaseComponent>();
            //        if (entity != null)
            //        {
            //            // Make sure each entity gets the message only once
            //            if (entitiesHit.Contains(entity.UniqueId))
            //                continue;
            //            //if (_targetTags.Contains(entity.GetTag()))
            //            if (AttackData.TargetTags.Contains(item.collider.tag))
            //            {
            //                var collisionComponent = item.collider.GetComponent<ICollisionComponent>();
            //                //item.point
            //                Debug.LogWarning("Hit " + item.collider.name);
            //                var skillUsed = _skill == null ? null : _skill.UniqueId;
            //                _damageComponent.DamageExternalEntity(entity.UniqueId, AttackData.Damage, GetComponentRepository().GetTag(), item.point, collisionComponent, skillUsed);
            //                entitiesHit.Add(entity.UniqueId);
            //                //Debug.Break();
            //            }
            //        }
            //    }
            //}
        }

        private float GetDistanceBetweenRaycastHitAndSelf(RaycastHit raycastHit)
        {
            if (raycastHit.collider == null)
                return 0;
            var closestPoint = raycastHit.collider.ClosestPoint(_componentRepository.GetPosition());
            var fromMeToThem = closestPoint - _componentRepository.GetPosition();
            return fromMeToThem.sqrMagnitude;
        }

        private void CreateHitVFX(RaycastHit raycastHit)
        {
            if (_hitVFX == null)
                return;
            var otherEntity = raycastHit.collider?.attachedRigidbody?.GetComponent<EntityCommonComponent>();
            var dirBetweenEntities = otherEntity.GetPosition() - _componentRepository.GetPosition();
            // Create the VFX a little way ahead of the attacker towards the entity attacked
            var _hitPos = _componentRepository.GetPosition() + (dirBetweenEntities.normalized); // + new Vector3(0f, 1f, 0f);

            // Place the VFX half way between entities.
            //var _hitPos = (_componentRepository.GetPosition() + otherEntity.GetPosition()) / 2;
            var rotateTowardsMe = Quaternion.LookRotation(_componentRepository.GetPosition() - otherEntity.GetPosition());
            GameObject.Instantiate(_hitVFX, _hitPos, rotateTowardsMe);
        }

        public AttackData GetAttackData()
        {
            return _attackData;
        }

        //Draw the BoxCast as a gizmo to show where it currently is testing. Click the Gizmos button to see this
        void OnDrawGizmos()
        {
            if (!_debug)
                return;

            //Check if there has been a hit yet
            if (_hitDetect)
            {
                Gizmos.color = Color.red;
                //Gizmos.matrix = transform.localToWorldMatrix;
                var attackPos = transform.TransformPoint(_attackData.Offset);
                //Draw a Ray forward from GameObject toward the maximum distance
                Gizmos.DrawRay(attackPos, transform.forward * _attackData.Distance);
                //Draw a cube at the maximum distance
                Gizmos.DrawWireCube(attackPos + transform.forward * _attackData.Distance, _attackData.Size);

                ////Draw a Ray forward from GameObject toward the hit
                //Gizmos.DrawRay(transform.position, transform.forward * m_Hit.distance);
                ////Draw a cube that extends to where the hit exists
                //Gizmos.DrawWireCube(transform.position + transform.forward * m_Hit.distance, transform.localScale);
            }
            //If there hasn't been a hit yet, draw the ray at the maximum distance
            else
            {
                Gizmos.color = Color.white;
                //Gizmos.matrix = transform.localToWorldMatrix;
                var attackPos = transform.TransformPoint(_attackData.Offset);
                //Draw a Ray forward from GameObject toward the maximum distance
                //Gizmos.DrawRay(attackPos, transform.forward * _attackData.Distance);
                //Draw a cube at the maximum distance
                //Gizmos.DrawWireCube(attackPos + transform.forward * _attackData.Distance, _attackData.Size);
                Gizmos.DrawWireCube(attackPos, _attackData.Size);
            }
        }
    }
}
