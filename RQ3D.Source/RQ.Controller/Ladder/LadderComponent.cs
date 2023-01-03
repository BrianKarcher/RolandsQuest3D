using RQ.Common.Components;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RQ.Controller.Ladder
{
    [AddComponentMenu("RQ/Components/Ladder")]
    public class LadderComponent : ComponentBase<LadderComponent>
    {
        //[SerializeField]
        //private Vector3 _normal;
        //public Vector3 Normal => _normal;

        [SerializeField]
        private float _ladderHeight;
        public float LadderHeight => _ladderHeight;

        [SerializeField]
        private GameObject _topOfLadder;
        public GameObject TopOfLadder => _topOfLadder;

        public float GetTopOfLadderHeight()
        {
            return _topOfLadder.transform.position.y + _ladderHeight;
        }

        public float GetBottomOfLadderHeight()
        {
            return transform.position.y;
        }
    }
}
