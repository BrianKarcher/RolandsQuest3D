using RQ.Common.Components;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RQ.Controller
{
    [AddComponentMenu("RQ/Components/Animation Component")]
    public class AnimationComponent : ComponentBase<AnimationComponent>
    {
        [SerializeField] private string _runAnim;
        [SerializeField] private string _turnSpeedAnim;
        [SerializeField] private string _forwardSpeedAnim;
        [SerializeField] private string _verticalSpeedAnim;
        [SerializeField] private string _isBlockingAnim;
        [SerializeField] private string[] _attackAnim;
        [SerializeField]
        private string _sideSpeedAnim;
        //public string SideSpeedAnim => _sideSpeedAnim;
        [SerializeField]
        private string _strafeAnim;
        public string StrafeAnim => _strafeAnim;

        [SerializeField] private string _crouchAnimTrigger;

        private Animator _animator;

        [SerializeField]
        private bool _ikActive = false;
        public bool IkActive { get => _ikActive; set => _ikActive = value; }

        [SerializeField]
        private Transform _rightHandObj = null;
        public Transform RightHandObj { get => _rightHandObj; set => _rightHandObj = value; }

        [SerializeField]
        private Transform _lookObj = null;
        public Transform LookObj { get => _lookObj; set => _lookObj = value; }

        private Dictionary<string, LayerWeightLerpData> LayerWeights;

        private class LayerWeightLerpData
        {
            public float Current;
            public float Final;
            public float Time;
            public int IndexInAnimator;
        }

        protected override void Awake()
        {
            base.Awake();
            LayerWeights = new Dictionary<string, LayerWeightLerpData>();
            _animator = GetComponent<Animator>();
        }

        private HashSet<string> LayerWeightLerpsToDelete = new HashSet<string>();
        private void Update()
        {
            // Clear does not do a reinstantiation of the interal list
            LayerWeightLerpsToDelete.Clear();
            foreach (var layerWeight in LayerWeights)
            {
                layerWeight.Value.Current = Mathf.MoveTowards(layerWeight.Value.Current, layerWeight.Value.Final, (1 / layerWeight.Value.Time) * Time.deltaTime);
                var current = layerWeight.Value.Current;

                if (Mathf.Approximately(layerWeight.Value.Current, layerWeight.Value.Final))
                {
                    current = layerWeight.Value.Final;
                    LayerWeightLerpsToDelete.Add(layerWeight.Key);
                }
                _animator.SetLayerWeight(layerWeight.Value.IndexInAnimator, current);
            }
            foreach (var key in LayerWeightLerpsToDelete)
            {
                LayerWeights.Remove(key);
            }
        }

        //a callback for calculating IK
        void OnAnimatorIK()
        {
            if (_animator)
            {

                //if the IK is active, set the position and rotation directly to the goal. 
                if (_ikActive)
                {

                    // Set the look target position, if one has been assigned
                    if (_lookObj != null)
                    {
                        _animator.SetLookAtWeight(1);
                        _animator.SetLookAtPosition(_lookObj.position);
                    }

                    // Set the right hand target position and rotation, if one has been assigned
                    if (_rightHandObj != null)
                    {
                        _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                        //_animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                        _animator.SetIKPosition(AvatarIKGoal.RightHand, _rightHandObj.position);
                        //_animator.SetIKRotation(AvatarIKGoal.RightHand, _rightHandObj.rotation);
                    }

                }

                //if the IK is not active, set the position and rotation of the hand and head back to the original position
                else
                {
                    _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                    _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                    _animator.SetLookAtWeight(0);
                }
            }
        }

        public void AddLayerWeightLerp(string name, float final, float time)
        {
            int index = _animator.GetLayerIndex(name);
            float current = _animator.GetLayerWeight(index);

            LayerWeights[name] = new LayerWeightLerpData()
            {
                IndexInAnimator = index,
                Current = current,
                Final = final,
                Time = time
            };
        }

        public float GetCurrentLayerWeight(string name)
        {
            int index = _animator.GetLayerIndex(name);
            return _animator.GetLayerWeight(index);
        }

        public void SetTurnSpeed(float speed)
        {
            _animator.SetFloat(_turnSpeedAnim, speed);
        }

        public void SetSideSpeed(float speed)
        {
            _animator.SetFloat(_sideSpeedAnim, speed);
        }

        public void SetForwardSpeed(float speed)
        {
            _animator.SetFloat(_forwardSpeedAnim, speed);
        }

        public void SetVerticalSpeed(float speed)
        {
            _animator.SetFloat(_verticalSpeedAnim, speed);
        }

        public void SetIsBlocking(bool blocking) => _animator.SetBool(_isBlockingAnim, blocking);

        public bool GetIsBlocking() => _animator.GetBool(_isBlockingAnim);

        public void SetBool(string anim, bool value)
        {
            Debug.Log($"Setting anim bool {anim} to {value}");
            _animator.SetBool(anim, value);
        }

        public bool GetBool(string anim) => _animator.GetBool(anim);

        public void SetTrigger(string anim)
        {
            Debug.Log($"Setting Anim Trigger {anim}");
            _animator.SetTrigger(anim);
        }

        public void SetAttackTrigger(int num) => _animator.SetTrigger(_attackAnim[num]);
    }
}
