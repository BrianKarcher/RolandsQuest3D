using UnityEngine;

namespace RQ.Messaging
{
    public class MessagingObject : MonoBehaviour, IMessagingObject
// : BaseObject, IMessagingObject
    {
        private bool _isListening = false;

        protected string _instanceId;

        public virtual string GetId()
        {
            if (_instanceId == null)
                _instanceId = GetInstanceID().ToString();
            return _instanceId;
        }

        protected virtual void Awake()
        {
            _instanceId = GetInstanceID().ToString();
        }

        public virtual void OnEnable()
        {
            if (!Application.isPlaying)
                return;

            if (!_isListening)
            {
                StartListening();
                _isListening = true;
            }
        }

        public virtual void StartListening()
        {

        }

        public virtual void StopListening()
        {

        }

        protected bool IsListening()
        {
            return _isListening;
        }

        public virtual void OnDisable()
        {
            if (!Application.isPlaying)
                return;
            if (_isListening)
            {
                StopListening();
                _isListening = false;
            }
        }

        public virtual void OnDestroy()
        {
            // This is getting destroyed, any messages being sent its way are deprecated
            // Remove them so future object with same Unique Id does not accidently receive them
            MessageDispatcher.Instance.RemoveMessagesForReceiverId(GetId());
        }

        /// <summary>
        /// Unline OnDestroy, this gets called whether or not the object has awoken
        /// </summary>
        public virtual void Destroy()
        {
            if (_isListening)
            {
                StopListening();
                _isListening = false;
            }
        }
    }
}
