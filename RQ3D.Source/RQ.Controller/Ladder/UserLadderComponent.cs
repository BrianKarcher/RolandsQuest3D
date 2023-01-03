using Rewired;
using RQ.Base.Extensions;
using RQ.Common.Components;
using RQ.Messaging;
using RQ.Physics;
using UnityEngine;

namespace RQ.Controller.Ladder
{
    [AddComponentMenu("RQ/Components/User Ladder")]
    public class UserLadderComponent : ComponentBase<UserLadderComponent>
    {
        [SerializeField]
        //private float _angle;
        private float _magnitude;

        private PhysicsComponent _physicsComponent;

        private LadderComponent _ladderComponent;

        private void Start()
        {
            if (_physicsComponent == null)
                _physicsComponent = _componentRepository.Components.GetComponent<PhysicsComponent>();
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.tag != "Ladder")
                return;

            //var otherGO = other.attachedRigidbody.gameObject;

            //if (otherGO == null)
            //    return;

            var otherLadderComponent = other.GetComponent<LadderComponent>();

            if (otherLadderComponent == null)
                return;

            _ladderComponent = otherLadderComponent;
            //var otherNormal = otherLadderComponent.Normal;
            var ladderNormal = otherLadderComponent.transform.forward * -1f;
            ladderNormal.Normalize();
            var thisForwardNormal = transform.forward.normalized;

            //var angle = Vector2.SignedAngle(thisForwardNormal.xz(), towardsLadderDirection.xz());
            float dot = Vector2.Dot(thisForwardNormal.xz(), ladderNormal.xz());

            //Debug.Log($"Roland normal: {thisForwardNormal}, ladder normal: {towardsLadderDirection}, Angle = {angle}");
            Debug.Log($"Roland normal: {thisForwardNormal}, ladder normal: {ladderNormal}, Angle = {dot}");
            //if (Mathf.DeltaAngle(angle < -_angle)
            if (dot < -_magnitude)
            {
                Debug.Log("(UserLadderComponent) Message UseLadder sent - normal");
                MessageDispatcher.Instance.DispatchMsg("UseLadder", 0f, _componentRepository.GetId(), _componentRepository.GetId(), null);
            }
            //else if (angle > _angle)
            else if (dot > _magnitude)
            {
                var footPos = _physicsComponent.GetFeetWorldPosition3();
                // Facing same direction of ladder and close to the top of the ladder? Then board it from the top!
                if (Vector3.Distance(footPos, otherLadderComponent.TopOfLadder.transform.position) < 0.5f)
                {
                    Debug.Log("(UserLadderComponent) Message UseLadder sent - foot");
                    MessageDispatcher.Instance.DispatchMsg("UseLadder", 0f, _componentRepository.GetId(), _componentRepository.GetId(), null);
                }
            }
        }

        public LadderComponent GetLadderComponent()
        {
            return _ladderComponent;
        }
    }
}
