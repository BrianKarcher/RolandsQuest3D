using System;
using System.Collections.Generic;
using System.Text;
using RQ.Base.Attributes;
using UnityEngine;

namespace RQ.Controller.Attack
{
    [Serializable]
    public class AttackData
    {
        [SerializeField]
        public float StrikeDelay = 0f;
        [SerializeField]
        public bool StopMovingDuringAttack = true;
        [SerializeField]
        public Vector3 Offset;
        [SerializeField]
        public Vector3 Size;
        [SerializeField]
        public float Angle = 0;
        [SerializeField]
        public float Distance = 0;
        [SerializeField]
        public float Damage = 0;
        [SerializeField]
        [Tag]
        public string[] TargetTags = null;
        [SerializeField]
        [Tag]
        public string[] DeflectTags = null;
        [SerializeField]
        public LayerMask Layers;
        //[SerializeField]
        //public ItemConfig _skill;
        //public AttackDirection AttackDirection;
    }
}
