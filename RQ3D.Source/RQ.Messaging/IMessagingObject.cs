namespace RQ.Messaging
{
    public interface IMessagingObject
    {
        string GetId();
        //void Awake();
        void OnEnable();
        void StartListening();
        void StopListening();
        string tag { get; set; }
        string name { get; set; }
    }
}