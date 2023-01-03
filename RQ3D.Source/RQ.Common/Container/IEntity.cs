using RQ.Common.Components;
using RQ.Messaging;
using UnityEngine;

namespace RQ.Common.Container
{
    public interface IEntity : IMessagingObject
    {
        void Init();
        void Reset();
        void ReAwaken();
        void Destroy();
        T GetComponent<T>();
        T GetComponentInChildren<T>();
        Coroutine StartCoroutine(string name);
        void StopCoroutine(string name);
        IComponentRegistrar Components { get; }
        Transform transform { get; }
        GameObject gameObject { get; }
        GameObject Target { get; set; }
        Vector3 HomePosition { get; }
        float Height { get; }
        Vector3 GetHeadPosition();
        Vector3 GetPosition();
        Vector3 GetFootPosition();
        bool isActiveAndEnabled { get; }
        int GetInstanceID();
    }
}
