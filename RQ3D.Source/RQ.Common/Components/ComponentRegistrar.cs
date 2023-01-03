using System;
using System.Collections.Generic;
using RQ.Common.Container;
using RQ.Messaging;

namespace RQ.Common.Components
{
    public class ComponentRegistrar : IComponentRegistrar
    {
        // TODO Make more components have names so we can make this a Dictionary?
        private List<IComponentBase> _entityComponents;

        private Dictionary<Type, List<IComponentBase>> _dictEntityComponents;
        public Action<string> PerformUnregister;
        public Action<string, IComponentBase> PerformRegister;
        private IEntity _repo;

        public ComponentRegistrar()
        {
            _entityComponents = new List<IComponentBase>(10);
            _dictEntityComponents = new Dictionary<Type, List<IComponentBase>>();
        }

        public void RegisterComponents<T>(IList<T> components) where T : IComponentBase
        {
            foreach (var component in components)
                RegisterComponent(component);
        }

        public void RegisterComponent<T>(T component) where T : IComponentBase
        {
            var componentType = component.GetType();

            if (!_dictEntityComponents.TryGetValue(componentType, out var dictComponents))
            {
                dictComponents = new List<IComponentBase>(3);
                _dictEntityComponents.Add(componentType, dictComponents);
            }
            // Don't allow duplicates
            foreach (var dupCheckComponent in dictComponents)
            {
                if (dupCheckComponent.GetId() == component.GetId())
                    return;
            }
            dictComponents.Add(component);
            _entityComponents.Add(component);
        }

        public void UnRegisterComponent<T>(T component) where T : IComponentBase
        {
            if (component == null)
                throw new Exception("Component is null in UnRegisterComponent");
            if (!_dictEntityComponents.TryGetValue(typeof(T), out var components))
                return;

            _entityComponents.Remove(component);
            _dictEntityComponents[typeof(T)].Remove(component);
        }

        public void UnregisterComponentById<T>(string uniqueId)
        {
            IComponentBase componentToRemove = null;

            if (!_dictEntityComponents.TryGetValue(typeof(T), out var components))
                return;
            foreach (var component in components)
            {
                if (component.GetId() == uniqueId)
                {
                    componentToRemove = component;
                    break;
                }
            }

            if (componentToRemove != null)
            {
                _entityComponents.Remove(componentToRemove);
                components.Remove(componentToRemove);
            }

        }

        public T GetComponent<T>() where T : class, IComponentBase
        {
            return GetComponent<T>(string.Empty);
        }

        public T GetComponent<T>(string name) where T : class, IComponentBase
        {
            if (name == null)
                name = string.Empty;

            if (!_dictEntityComponents.TryGetValue(typeof(T), out var components))
            {
                components = _entityComponents;
            }

            foreach (var component in components)
            {
                if (component is T && component.ComponentName == name)
                {
                    return component as T;
                }
            }

            return null;
        }

        public IComponentBase GetComponent(string name)
        {
            if (name == null)
                name = string.Empty;

            foreach (var component in _entityComponents)
            {
                if (component.ComponentName == name)
                {
                    return component;
                }
            }

            return null;
        }

        public IList<IComponentBase> GetComponents()
        {
            return _entityComponents;
        }

        /// <summary>
        /// Casting a list as a new type creates a new list. We are returning IBaseObject instead so no new list is created.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IList<IComponentBase> GetComponents<T>() where T : class, IComponentBase
        {
            if (!_dictEntityComponents.TryGetValue(typeof(T), out var components))
            {
                components = new List<IComponentBase>();
                foreach (var component in _entityComponents)
                {
                    if (component is T)
                    {
                        components.Add(component);
                    }
                }
                return components;
            }

            return components;
        }

        public void SetComponentRepository(IEntity repo)
        {
            _repo = repo;
        }
    }

}
