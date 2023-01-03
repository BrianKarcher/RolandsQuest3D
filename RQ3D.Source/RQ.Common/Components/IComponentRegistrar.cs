using System;
using System.Collections.Generic;
using System.Text;
using RQ.Common.Container;
using RQ.Messaging;

namespace RQ.Common.Components
{
    public interface IComponentRegistrar
    {
        //void RegisterComponentAll<T>(T component) where T : IBaseObject;
        void RegisterComponents<T>(IList<T> component) where T : IComponentBase;
        //void UnregisterComponentAll<T>(T component) where T : IBaseObject;
        void UnregisterComponentById<T>(string uniqueId);
        void RegisterComponent<T>(T component) where T : IComponentBase;
        void UnRegisterComponent<T>(T component) where T : IComponentBase;
        T GetComponent<T>() where T : class, IComponentBase;
        T GetComponent<T>(string name) where T : class, IComponentBase;
        IComponentBase GetComponent(string name);
        IList<IComponentBase> GetComponents();
        //IEnumerable<IBaseObject> GetComponentsAll();
        IList<IComponentBase> GetComponents<T>() where T : class, IComponentBase;
        void SetComponentRepository(IEntity _repo);
    }
}
