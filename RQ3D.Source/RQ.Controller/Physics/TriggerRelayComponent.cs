using RQ.Common.Components;
using UnityEngine;

namespace RQ.Controller.Physics
{
    [AddComponentMenu("RQ/Components/Trigger Relay")]
    public class TriggerRelayComponent : ComponentBase<TriggerRelayComponent>
    {
        private void OnTriggerEnter(Collider other)
        {
            _componentRepository.SendMessage("OnTriggerEnter", other, SendMessageOptions.DontRequireReceiver);
            //SendMessage();
        }

        private void OnTriggerExit(Collider other)
        {
            _componentRepository.SendMessage("OnTriggerExit", other, SendMessageOptions.DontRequireReceiver);
            //SendMessage();
        }
    }
}
