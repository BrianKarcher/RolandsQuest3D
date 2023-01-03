using RQ.Common.Components;
using UnityEngine;

namespace RQ.Controller.Physics
{
    [AddComponentMenu("RQ/Components/Hang Trigger")]
    public class HangTriggerComponent : ComponentBase<HangTriggerComponent>
    {
        [SerializeField] private GameObject _playerPlacePosition;
        public GameObject PlayerPlacePosition => _playerPlacePosition;
    }
}
