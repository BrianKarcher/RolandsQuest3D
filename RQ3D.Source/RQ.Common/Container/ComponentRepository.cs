using RQ.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using RQ.Common.Components;
using UnityEngine;

namespace RQ.Common.Container
{
    public class ComponentRepository : MessagingObject, IEntity
    {
        [SerializeField]
        protected bool _addToEntityContainer = true;

        [SerializeField]
        private GameObject _target;
        public GameObject Target { get => _target; set => _target = value; }

        [SerializeField]
        private float _height;

        public float Height => _height;
        
        private CapsuleCollider _capsuleCollider;

        [SerializeField]
        private Vector3 _homePosition;
        public Vector3 HomePosition => _homePosition;

        public bool AddToEntityContainer { get { return _addToEntityContainer; } }
        private IComponentRegistrar _components;
        //private Dictionary<string, List<KeyValuePair<string, Action<Telegram2>>>> _messageRelay;
        //const string _idToCheck = "8da840b9-43f6-4bae-9e63-a3ceee1cb493";

        protected override void Awake()
        {
            base.Awake();
            _homePosition = transform.position;
            _capsuleCollider = GetComponent<CapsuleCollider>();
            //_height = _capsuleCollider == null ? 0f : _capsuleCollider.height;
        }

        [HideInInspector]
        public IComponentRegistrar Components
        {
            get
            {
                if (_components == null)
                {
                    _components = new ComponentRegistrar();
                    //_components.PerformUnregister = (uniqueId => _unregisterComponents.Add(uniqueId));
                    //_components.PerformRegister = (uniqueId, component) =>
                    //    {
                    //        _registerComponents.Add(component);
                    //    };
                }
                return _components;
            }
        }

        public ComponentRepository()
        {
            Components.SetComponentRepository(this);
        }

        public virtual void Init()
        {
            //if (!Application.isPlaying)
            //    return;
            var components = SearchForComponents();
            //Debug.Log(this.name + " Found " + components.Count() + " entities");
            Components.RegisterComponents(components);
            InitComponents(components);
        }

        public void Reset()
        {
            //foreach (var component in Components.GetComponents())
            //{
            for (int i = 0; i < Components.GetComponents().Count; i++)
            {
                var component = Components.GetComponents()[i];
                (component as IComponentBase)?.Reset();
            }
        }

        public void ReAwaken()
        {
            //foreach (var component in Components.GetComponents())
            var components = Components.GetComponents();
            for (int i = 0; i < components.Count; i++)
            {
                (components[i] as IComponentBase)?.ReAwaken();
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (!Application.isPlaying)
                return;
            var name = this.name;
            //if (GetId() == _idToCheck)
            //{
            //    Debug.LogWarning("OnDestroy called on " + _idToCheck);
            //}
            StopListening();
            DestroyComponents();

            if (_addToEntityContainer)
                EntityContainer.Instance.RemoveEntity(this);
        }

        /// <summary>
        /// Unlike OnDestroy, this gets called whether or not the object has awoken
        /// </summary>
        public override void Destroy()
        {
            base.Destroy();
            //if (GetId() == _idToCheck)
            //{
            //    Debug.LogWarning("Destroy called on " + _idToCheck);
            //}

            StopListening();
            DestroyComponents();
            if (this.transform.gameObject != null)
            {
                this.transform.gameObject.SetActive(false);
                GameObject.Destroy(this.transform.gameObject);
            }

            if (_addToEntityContainer)
                EntityContainer.Instance.RemoveEntity(this);
            // OnDestroy does not get called on objects that were never active, so stop listening now
            //            
        }

        private void DestroyComponents()
        {
            var components = Components.GetComponents();
            //Debug.Log(this.name + " Killing " + components.Count + " entities");
            for (int i = 0; i < components.Count; i++)
            {
                var component = components[i];
                if (MessageDispatcher.Instance.IsListening("Kill", component.GetId()))
                    MessageDispatcher.Instance.DispatchMsg("Kill", 0f, this.GetId(), component.GetId(), null);
            }
        }

        public virtual void InitComponents(IEnumerable<IComponentBase> components)
        {
            //Debug.Log(this.name + " Initing " + components.Count() + " entities");
            foreach (var component in components)
            {
                component.Init();
            }
        }

        private IList<IComponentBase> SearchForComponents()
        {
            List<IComponentBase> components = new List<IComponentBase>(20);
            ProcessComponents(components, GetComponentsInChildren<IComponentBase>(true));
            return components;
        }

        private void ProcessComponents(List<IComponentBase> componentList,
            IList<IComponentBase> components)
        {
            if (components == null)
                return;

            for (int i = 0; i < components.Count; i++)
            {
                var component = components[i];
                if (component.GetComponentRepository() == this)
                    componentList.Add(component);
            }
        }

        public float GetHeight()
        {
            return _height;
        }

        public Vector3 GetPosition()
        {
            // transform.position is at the characters feet
            return transform.position + new Vector3(0f, _height / 2f, 0);
        }

        public Vector3 GetHeadPosition()
        {
            return transform.position + new Vector3(0f, _height, 0);
        }

        public Vector3 GetFootPosition()
        {
            return transform.position;
        }
    }

}
