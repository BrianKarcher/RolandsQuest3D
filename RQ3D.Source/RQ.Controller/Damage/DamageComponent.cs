using System;
using System.Collections.Generic;
using System.Text;
using RQ.Base.Attributes;
using RQ.Base.Components;
using RQ.Common.Components;
using RQ.Common.Container;
using RQ.Messaging;
using RQ.Physics;
using UnityEngine;

namespace RQ.Controller.Damage
{
    [AddComponentMenu("RQ/Components/Damage Component")]
    public class DamageComponent : ComponentBase<DamageComponent>
    {
        [SerializeField]
        private DamageComponentData _damageComponentData = null;
        private long _externalDamageId; //, _setDamageSourceLocationId;
        private Action<Telegram> _externalDamageDelegate, _setDamageSourceLocationDelegate;
        private DamageEntityInfo DamageInfo { get; set; }
        private PhysicsComponent _physicsComponent;
        private EntityStatsComponent _entityStatsComponent;
        [SerializeField]
        private GameObject _deflectPrefab;
        [Tag]
        [SerializeField]
        private string _deflectTag;        

        protected override void Awake()
        {
            base.Awake();

            if (_externalDamageDelegate == null)
            {
                _externalDamageDelegate = telegram =>
                {
                    // The previous damage has not been reacted to yet. Do not accept more damage until
                    // previous has been reacted to.
                    //if (DamageInfo.IsDamaged)
                    //    return true;

                    var damageInfo = (DamageEntityInfo)telegram.ExtraInfo;
                    if (damageInfo == null)
                    {
                        Debug.LogError("No DamageEntityInfo in ExternalDamage");
                        return;
                    }

                    // TODO Implement Deflections
                    //if (damageInfo.MyCollider != null)
                    //{
                    //    //var myCollisionComponent = damageInfo.MyCollider.GetComponent<CollisionComponent>();
                    //    //if (myCollisionComponent.CollisionData.CurrentlyDeflecting)
                    //    if (damageInfo.MyCollider.tag == _deflectTag)
                    //    {
                    //        var attackerId = damageInfo.DamagedByEntity.GetId();

                    //        var vectorToDamageSource = damageInfo.DamagedByEntity.GetFootPosition() -
                    //       _componentRepository.GetPosition();

                    //        //var vectorToDamageSource = damageInfo.DamageSourceLocation -
                    //        //                           _componentRepository.GetPosition();

                    //        var rotation = Quaternion.LookRotation(vectorToDamageSource);

                    //        //var angle = Vector2.SignedAngle(Vector2.right, vectorToDamageSource);
                    //        //var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    //        if (_deflectPrefab != null)
                    //        {
                    //            var deflectGO = GameObject.Instantiate(_deflectPrefab,
                    //                _componentRepository.transform.position, rotation);
                    //            deflectGO.transform.SetParent(_componentRepository.transform);
                    //        }
                    //        MessageDispatcher.Instance.DispatchMsg("SetDamageSourceLocation", 0f, _componentRepository.GetId(),
                    //            attackerId, _componentRepository.GetFootPosition());
                    //        Debug.Log($"{_componentRepository.name} deflect attack from {damageInfo.DamagedByEntity.name}");
                    //        MessageDispatcher.Instance.DispatchMsg("Deflect", 0f, _componentRepository.GetId(),
                    //            attackerId, null);
                    //        return;
                    //    }
                    //}

                    if (!_damageComponentData.TakesDamage)
                        return;

                    if (damageInfo.MyCollider == null)
                        return;

                    //if (damageInfo.MyCollider != null && !damageInfo.CollisionHit.ReceivesDamage())
                    //    return false;

                    //if (!_damageData.Vulnerable)
                    //{
                    //    MessageDispatcher2.Instance.DispatchMsg("AttackedButInvulnerable", 0f, this.UniqueId,
                    //        _componentRepository.UniqueId, null);
                    //    return false;
                    //}

                    _physicsComponent.Controller.BounceFromLocation = damageInfo.DamagedByEntity.GetFootPosition();

                    Debug.Log($"{this.name} damaged by {damageInfo.DamagedByEntity.name}");

                    DamageSelf(damageInfo);

                    return;
                };
                //_setDamageSourceLocationDelegate = (data) =>
                //{
                //    BounceFromLocation = (Vector3) data.ExtraInfo;
                //};
            }
        }

        private void Start()
        {
            if (_entityStatsComponent == null)
                _entityStatsComponent = _componentRepository.Components.GetComponent<EntityStatsComponent>();
            if (_physicsComponent == null)
                _physicsComponent = _componentRepository.Components.GetComponent<PhysicsComponent>();
        }

        public override void StartListening()
        {
            base.StartListening();
            _externalDamageId =
                MessageDispatcher.Instance.StartListening("ExternalDamage", _componentRepository.GetId(), _externalDamageDelegate);
            //_setDamageSourceLocationId = MessageDispatcher.Instance.StartListening("SetDamageSourceLocation",
            //    _componentRepository.GetId(), _setDamageSourceLocationDelegate);
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher.Instance.StopListening("ExternalDamage", _componentRepository.GetId(), _externalDamageId);
            //MessageDispatcher.Instance.StopListening("SetDamageSourceLocation", _componentRepository.GetId(), _setDamageSourceLocationId);
        }

        private void DamageSelf(DamageEntityInfo damageInfo)
        {
            DamageInfo = new DamageEntityInfo();
            DamageInfo.CopyFrom(damageInfo);
            //DamageInfo.DamageAmount = damageInfo.DamageAmount;
            //Debug.LogError("Damaged " + damageInfo.DamageAmount);
            //DamageInfo.DamagedByEntity = damageInfo.DamagedByEntity;
            //DamageInfo.DamagedBy = damageInfo.DamagedBy;
            //DamageInfo.DamageSourceLocation = damageInfo.DamageSourceLocation;
            //DamageInfo.Tag = damageInfo.Tag;
            DamageInfo.IsDamaged = true;
            //DamageInfo.CollisionDamageType = damageInfo.CollisionDamageType;
            //SendDamageNotification();

            // TODO Create a Hit Effect!!!
            //if (_hitEffect != null)
            //    CreateHitEffect(damageInfo.HitPosition);

            // Report the damage
            if (_damageComponentData.ReportDamage)
            {
                MessageDispatcher.Instance.DispatchMsg(_damageComponentData.ReportDamageMessage, 0f,
                    _componentRepository.GetId(), null, null);
            }

            // No stats? Dead!
            if (_entityStatsComponent == null)
                EntityIsDead();
            else
            {
                _entityStatsComponent.AddHp(-damageInfo.DamageAmount);
            }
            //GameObject.Destroy(_componentRepository.gameObject);
            //_componentRepository.SendMessageToAllButThis(0f, this.UniqueId, Telegrams.Damaged, DamageInfo);

            // Do not retain the damage information, assume it has already been responded to in the state change triggered above
            // If there was no state change then the entity can't be damaged and we don't want to buffer attacks so set to null anyway.
            DamageInfo = null;
        }

        private void EntityIsDead()
        {
            MessageDispatcher.Instance.DispatchMsg("EntityDied", 0f, this.GetId(), _componentRepository.GetId(), null);
        }

        public bool IsCollisionValid(CollisionData data, string[] validTags)
        {
            var hasTag = Array.IndexOf(validTags, data.OtherCollider.tag) > -1;
            if (!hasTag)
                return false;

            if (data.OtherCollider == null || data.OtherCollider.attachedRigidbody == null)
                return false;

            var otherEntity = data.OtherCollider.attachedRigidbody?.GetComponent<EntityCommonComponent>();
            if (otherEntity == null)
                return false;

            return true;
        }

        // Damage an external entity
        public void DamageExternalEntity(IEntity otherEntity, CollisionData collisionData, float damageAmount)
        {
            var damageInfo = new DamageEntityInfo()
            {
                DamageAmount = damageAmount,
                //DamagedBy = _componentRepository.UniqueId,
                DamagedByEntity = _componentRepository,
                //damageInfo.DamagedBy = transform.GetComponent<IComponentRepository>().UniqueId;
                //DamageSourceLocation = this.transform.position,
                HitPosition = collisionData.HitPosition,
                Tag = tag,
                MyCollider = collisionData.OtherCollider, // The external entity's MyCollider is this entity's OtherCollider
                //CollisionDamageType = this._damageData.CollisionDamageType
            };
            //var otherEntity = collisionData.OtherCollider.attachedRigidbody.GetComponent<IEntity>();
            MessageDispatcher.Instance.DispatchMsg("ExternalDamage", 0f, null, otherEntity.GetId(), damageInfo);
            MessageDispatcher.Instance.DispatchMsg("DamagedOther", 0f, null, _componentRepository.GetId(), null);
        }

        public void DamageExternalEntity(IEntity otherEntity, Collider otherCollider, float damageAmount, Vector3 hitPosition)
        {
            Debug.Log($"{_componentRepository.name} Hit {otherEntity.name}");
            var damageInfo = new DamageEntityInfo()
            {
                DamageAmount = damageAmount,
                //DamagedBy = _componentRepository.UniqueId,
                DamagedByEntity = _componentRepository,
                //damageInfo.DamagedBy = transform.GetComponent<IComponentRepository>().UniqueId;
                //DamageSourceLocation = this.transform.position,
                //DamageSourceLocation = _componentRepository.GetPosition(),
                HitPosition = hitPosition,
                Tag = tag,
                MyCollider = otherCollider, // The external entity's MyCollider is this entity's OtherCollider
                //CollisionDamageType = this._damageData.CollisionDamageType
            };

            MessageDispatcher.Instance.DispatchMsg("ExternalDamage", 0f, _componentRepository.GetId(), otherEntity.GetId(), damageInfo);
            MessageDispatcher.Instance.DispatchMsg("DamagedOther", 0f, null, _componentRepository.GetId(), null);
        }

        public DamageComponentData GetDamageComponentData()
        {
            return _damageComponentData;
        }

        public DamageEntityInfo GetDamageInfo()
        {
            return DamageInfo;
        }
    }

    public class CollisionData
    {
        //public Collider2D TriggerCollider { get; set; }
        public Collider OtherCollider { get; set; }
        //public Collision2D CollisionCollider { get; set; }
        public Collider OurCollider { get; set; }
        public string CollisionComponentUniqueId { get; set; }
        public string ThisTag { get; set; }
        //public Vector2D HitPosition { get; set; }
        public Vector3 HitPosition { get; set; }
        //public bool? AreWeADamageCollider { get; set; }
        //public GameObject OurCollisionComponent { get; set; }
    }
}
