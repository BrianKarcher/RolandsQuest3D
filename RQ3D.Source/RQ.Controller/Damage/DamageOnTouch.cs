using System;
using System.Collections.Generic;
using System.Text;
using RQ.Base.Attributes;
using RQ.Common.Components;
using RQ.Common.Container;
using RQ.Messaging;
using UnityEngine;

namespace RQ.Controller.Damage
{
    [AddComponentMenu("RQ/Components/Damage On Touch")]
    public class DamageOnTouch : ComponentBase<DamageOnTouch>
    {
        [SerializeField]
        [Tag]
        [Tooltip("The tags you damage.")]
        private string[] _tagsYouDamage;

        [SerializeField] private bool _onTrigger;
        [SerializeField] private bool _onCollider;
        [SerializeField] private float _damage;
        [SerializeField] private string _messageToSelf = "DamagedOther";

        private void OnCollisionEnter(Collision other)
        {
            if (!_onCollider)
                return;

            var name = this.name;
            //Debug.Log($"Collison - Other:{other.transform.name} This:{gameObject.name}");

            var tagIndex = Array.IndexOf(_tagsYouDamage, other.gameObject.tag);
            if (tagIndex == -1)
                return;

            ContactPoint contact = other.contacts.Length == 0 ? new ContactPoint() : other.contacts[0];
            //var contact = other.contacts.FirstOrDefault();
            var collisionData = new CollisionData()
            {
                //CollisionComponentUniqueId = id,
                OtherCollider = other.collider,
                OurCollider = contact.thisCollider,
                ThisTag = this.tag,
                HitPosition = contact.point
            };

            ProcessCollision(other.rigidbody?.gameObject, collisionData);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_onTrigger)
                return;

            //var otherGO = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;

            //var tagIndex = Array.IndexOf(_tagsYouDamage, otherGO.tag);
            var tagIndex = Array.IndexOf(_tagsYouDamage, other.tag);
            if (tagIndex == -1)
                return;

            // OnTriggerEnter does not have the exact hit position, so just use the point in between instead
            var hitPosition = (other.transform.position + transform.position) / 2;

            CollisionData collisionData = new CollisionData()
            {
                //CollisionComponentUniqueId = id,
                OtherCollider = other,
                ThisTag = this.tag,
                HitPosition = hitPosition,
                OurCollider = this.gameObject.GetComponent<Collider>()
                //AreWeADamageCollider = _ourCollisionComponent?.GetCollisionData()?.DamageCollider
            };

            ProcessCollision(other.attachedRigidbody?.gameObject, collisionData);

            //MessageDispatcher.Instance.DispatchMsg("ExternalDamage", 0f, null, otherEntity.GetId(), damageData);

            // Change the cube color to green.
            //MeshRenderer meshRend = GetComponent<MeshRenderer>();
            //meshRend.material.color = Color.green;
            //Debug.Log(other.name);
        }

        private void ProcessCollision(GameObject otherEntity, CollisionData collisionData)
        {
            Debug.Log($"(DamageOnTouch) Collison - Other:{otherEntity?.transform.name} This:{gameObject.name}");

            DamageExternalEntity(otherEntity, collisionData, _damage);
        }

        public void DamageExternalEntity(GameObject other, CollisionData collisionData, float damageAmount)
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
            var otherEntity = other?.GetComponent<IEntity>();
            if (otherEntity != null)
            {
                MessageDispatcher.Instance.DispatchMsg("ExternalDamage", 0f, null, otherEntity.GetId(), damageInfo);
            }
            MessageDispatcher.Instance.DispatchMsg(_messageToSelf, 0f, null, _componentRepository.GetId(), null);
        }

        //private bool ProcessCollision(CollisionMessageData data, bool isTrigger)
        //{
        //    var doWeDeliverDamage = data.OurCollisionComponent?.GetCollisionData()?.DamageCollider;
        //    if (doWeDeliverDamage != null && !doWeDeliverDamage.Value)
        //        return false;

        //    if (!_damageData.DamageTargetOnCollision && !_damageData.DamageTargetOnTrigger)
        //        return false;

        //    if (data.OtherCollider == null || data.OtherCollider.attachedRigidbody == null)
        //        return false;

        //    var tags = data.OurCollisionComponent == null
        //        ? _tagsYouDamage
        //            : data.OurCollisionComponent.GetCollisionData().Tags;

        //    var containsTag = Array.IndexOf(tags, data.OtherCollider.tag) > -1;

        //    if (!containsTag)
        //        return false;

        //    var targetEntity = data.OtherCollider.attachedRigidbody.transform.GetComponent<IComponentRepository>();
        //    if (targetEntity == null)
        //        return false;
        //    var targetEntityPhysicsComponent = targetEntity.Components.GetComponent<PhysicsComponent>();
        //    var targetEntityCollisionComponent = data.OtherCollider.GetComponent<CollisionComponent>();

        //    var targetEntityFloorComponent = targetEntity.Components.GetComponent<FloorComponent>();
        //    if (targetEntityCollisionComponent == null || !targetEntityCollisionComponent.ReceivesDamage())
        //        return false;

        //    var damageData = GetDamageData();

        //    var floorComponent = _componentRepository.Components.GetComponent<FloorComponent>();

        //    // All passed? Set _collided
        //    var receiverId = targetEntity.UniqueId;

        //    bool causeDamage = false;
        //    if (damageData.DamageTargetOnCollision && !isTrigger)
        //        causeDamage = true;

        //    if (damageData.DamageTargetOnTrigger && isTrigger)
        //        causeDamage = true;

        //    if (causeDamage)
        //    {
        //        var collisionComponent = data.OtherCollider.GetComponent<ICollisionComponent>();
        //        DamageExternalEntity(receiverId, damageData.DamageOnTouch, this.tag, data.HitPosition, collisionComponent, null);
        //    }

        //    return true;
        //}
    }
}
