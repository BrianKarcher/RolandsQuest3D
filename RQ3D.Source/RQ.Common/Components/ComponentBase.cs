using RQ.Messaging;
using System;
using RQ.Common.Container;
using UnityEngine;

namespace RQ.Common.Components
{
    public class ComponentBase : MessagingObject, IComponentBase
    {
        [SerializeField]
        protected ComponentRepository _componentRepository = null;

        [SerializeField]
        private string componentName;
        public string ComponentName { get { return componentName; } set { componentName = value; } }

        private bool _hasAwakened = false;
        private Action<Telegram> _killDelegate;

        protected bool _hasInited = false;

        protected override void Awake()
        {
            base.Awake();
            _killDelegate = (data) =>
            {
                Destroy();
            };

            _hasAwakened = true;
            Init();
        }

        public override void StartListening()
        {
            base.StartListening();
            if (_componentRepository == null)
                throw new Exception($"No component repository located for {this.name}");
            MessageDispatcher.Instance.StartListening("Kill", _componentRepository.GetId(), _killDelegate);
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher.Instance.StopListening("Kill", _componentRepository.GetId(), -1);
        }

        public virtual void Reset()
        { }

        /// <summary>
        /// This gets called right before a pooled entity is enabled
        /// </summary>
        public virtual void ReAwaken()
        {
            if (!_hasAwakened)
                return;
            // OnEnable causes a State Enter to immediaely fire. Start listening to events prior to this happening to avoid bugs.
            StartListening();
        }

        protected void SetComponentRepository()
        {
            var currentTransform = transform;
            while (_componentRepository == null)
            {
                // Search up the tree to find the component repository. This is a time saver.
                _componentRepository = currentTransform.GetComponent<IEntity>() as ComponentRepository;
                currentTransform = currentTransform.parent;
                // Reached the top of the tree
                if (currentTransform == null)
                    break;
            }
        }

        public virtual void Init()
        {
            if (_hasInited)
                return;

            if (_componentRepository == null)
                SetComponentRepository();

            if (_componentRepository != null)
            {
                var typedComponent = this;
                //if (typedComponent == null)
                //{
                //    Debug.LogError($"Could not convert component of type {this.GetType()} into {typeof(T).Name}");
                //    return;
                //}
                Debug.Log($"Registering {typedComponent.GetType()}");
                _componentRepository.Components.RegisterComponent(typedComponent);
            }
            _hasInited = true;
        }

        public override void Destroy()
        {
            base.Destroy();
            UnregisterComponent();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            UnregisterComponent();
        }

        private void UnregisterComponent()
        {
            if (_componentRepository?.Components == null)
                return;
            _componentRepository.Components.UnRegisterComponent(this);
        }

        public IEntity GetComponentRepository()
        {
            return _componentRepository;
        }
    }

    public class ComponentBase<T> : MessagingObject, IComponentBase where T : class, IComponentBase
    {
        [SerializeField]
        protected ComponentRepository _componentRepository = null;

        [SerializeField]
        private string componentName;
        public string ComponentName { get { return componentName; } set { componentName = value; } }

        private bool _hasAwakened = false;
        private Action<Telegram> _killDelegate;

        protected bool _hasInited = false;

        protected override void Awake()
        {
            base.Awake();
            _killDelegate = (data) =>
            {
                Destroy();
            };

            _hasAwakened = true;
            Init();

        }

        public override void StartListening()
        {
            base.StartListening();
            if (_componentRepository == null)
                throw new Exception($"No component repository located for {this.name}");
            MessageDispatcher.Instance.StartListening("Kill", _componentRepository.GetId(), _killDelegate);
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher.Instance.StopListening("Kill", _componentRepository.GetId(), -1);
        }

        public virtual void Reset()
        { }

        /// <summary>
        /// This gets called right before a pooled entity is enabled
        /// </summary>
        public virtual void ReAwaken()
        {
            if (!_hasAwakened)
                return;
            // OnEnable causes a State Enter to immediaely fire. Start listening to events prior to this happening to avoid bugs.
            StartListening();
        }

        protected void SetComponentRepository()
        {
            var currentTransform = transform;
            while (_componentRepository == null)
            {
                // Search up the tree to find the component repository. This is a time saver.
                _componentRepository = currentTransform.GetComponent<IEntity>() as ComponentRepository;
                currentTransform = currentTransform.parent;
                // Reached the top of the tree
                if (currentTransform == null)
                    break;
            }
        }

        public virtual void Init()
        {
            if (_hasInited)
                return;

            if (_componentRepository == null)
                SetComponentRepository();

            if (_componentRepository != null)
            {
                var typedComponent = this as T;
                if (typedComponent == null)
                {
                    Debug.LogError($"Could not convert component of type {this.GetType()} into {typeof(T).Name}");
                    return;
                }
                _componentRepository.Components.RegisterComponent(typedComponent);
            }
            _hasInited = true;
        }

        public override void Destroy()
        {
            base.Destroy();
            UnregisterComponent();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            UnregisterComponent();
        }

        private void UnregisterComponent()
        {
            if (_componentRepository?.Components == null)
                return;
            _componentRepository.Components.UnRegisterComponent<T>(this as T);
        }

        public IEntity GetComponentRepository()
        {
            return _componentRepository;
        }
    }
}
