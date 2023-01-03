using RQ.Common.Container;
using RQ.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RQ.Base.Components
{
    [AddComponentMenu("RQ/Components/Entity Common")]
    [Serializable]
    public class EntityCommonComponent : ComponentRepository
    {
        [SerializeField]
        private EntityCommonComponent _parentRepo;
        private HashSet<EntityCommonComponent> _childSpriteBaseComponents = new HashSet<EntityCommonComponent>();

        //[HideInInspector]
        //public bool JustLoaded { get; set; }

        private Action<Telegram> _killDelegate;

        protected override void Awake()
        {
            if (!Application.isPlaying)
            {
                // Make sure basic setup is performed on the object like setting the RqName
                // and Unique Id Registration
                base.Awake();
                return;
            }

            base.Awake();
            _killDelegate = (data) =>
            {
                Destroy();
            };

            if (_addToEntityContainer)
                EntityContainer.Instance.AddEntity(this);
        }

        public void Start()
        {
            // We can safely say this was no longer just loaded from a Save file
            //JustLoaded = false;
            Target = EntityContainer.Instance.GetMainCharacter()?.gameObject;
        }

        /// <summary>
        /// Call this manually since Awake does not get called if an object is not enabled in the scene
        /// </summary>
        public override void Init()
        {
            base.Init();
            //Debug.Log("Init called on " + this.name);
            if (_parentRepo != null)
                _parentRepo.AddChildSpriteBaseComponent(this);
        }

        public override void StartListening()
        {
            base.StartListening();
            MessageDispatcher.Instance.StartListening("Kill", this.GetInstanceID().ToString(), _killDelegate);
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher.Instance.StopListening("Kill", this.GetInstanceID().ToString(), -1);
        }

        //public override bool HandleMessage(Telegram msg)
        //{
        //    if (base.HandleMessage(msg))
        //        return true;

        //    switch (msg.Msg)
        //    {
        //        case Telegrams.Serialize:
        //            var serializeData = msg.ExtraInfo as EntitySerializedData;
        //            this.Serialize(serializeData);
        //            break;
        //        case Telegrams.Deserialize:
        //            var deserializeData = msg.ExtraInfo as EntitySerializedData;
        //            this.Deserialize(deserializeData);
        //            break;
        //        case Telegrams.Kill:
        //            Destroy();
        //            break;
        //        case Telegrams.GetPos:
        //            msg.Act((Vector2D)GetPos());
        //            break;
        //        case Telegrams.GetWorldPos:
        //            msg.Act((Vector2D)GetWorldPos());
        //            break;
        //            //case Telegrams.GetClosestObjectFromSuppliedList:
        //            //    var list = msg.ExtraInfo as IEnumerable<string>;
        //            //    var closestObject = GetClosestObject(list);
        //            //    msg.Act(closestObject);
        //            //    return true;
        //    }

        //    return false;
        //}

        //public Vector3 GetPos()
        //{
        //    return this.transform.localPosition;
        //}

        //public Vector3 GetWorldPos()
        //{
        //    return this.transform.position;
        //}

        public virtual Transform Instantiate(Transform original, Vector3 position, Quaternion rotation)
        {
            return (Transform)GameObject.Instantiate(original, position, rotation);
        }

        public void AddChildSpriteBaseComponent(EntityCommonComponent spriteBaseComponent)
        {
            if (!_childSpriteBaseComponents.Contains(spriteBaseComponent))
                _childSpriteBaseComponents.Add(spriteBaseComponent);
        }

        public override void Destroy()
        {
            if (_childSpriteBaseComponents != null)
            {
                foreach (var child in _childSpriteBaseComponents)
                {
                    child.Destroy();
                }
            }
            base.Destroy();

        }
    }
}
