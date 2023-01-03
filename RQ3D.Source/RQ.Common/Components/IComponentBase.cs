using RQ.Common.Container;
using RQ.Messaging;
using UnityEngine;

namespace RQ.Common.Components
{
    public interface IComponentBase : IMessagingObject
    {
        IEntity GetComponentRepository();
        string ComponentName { get; set; }
        void Reset();
        void ReAwaken();
        void Init();
        void Destroy();
        Transform transform { get; }
        GameObject gameObject { get; }
        //void DeserializeUniqueIds(EntitySerializedData entitySerializedData);
        //void StartListening();
        //void StopListening();
    }
}
