using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RQ.Controller.Damage
{
    [Serializable]
    public class DamageComponentData
    {
        //public bool DamageTargetOnCollision = true;
        //public bool DamageTargetOnTrigger = false;
        //public float DamageOnTouch = 1f;
        public bool TakesDamage = true;

        [SerializeField]
        private float _damagedBounceForce = 1f;
        public float DamagedBounceForce => _damagedBounceForce;
        [SerializeField]
        private bool _reportDamage = true;
        public bool ReportDamage => _reportDamage;
        [SerializeField]
        private string _reportDamageMessage = "EnemyDamaged";
        public string ReportDamageMessage => _reportDamageMessage;

        [SerializeField]
        private float _damageDrag = 1f;
        public float DamageDrag => _damageDrag;
        //public bool TakesSkillDamage = true;
        //public bool Vulnerable = true;
        //public CollisionActionType CollisionDamageType = CollisionActionType.Normal;
        [HideInInspector]
        public HashSet<string> EntityDeathNotification { get; set; }
        //public string AttackedBySkill { get; set; }
        public int AttackCount { get; set; }

        //public List<string> DamageNotify { get; set; }
        //public DamageData()
        //{
        //    DamageNotify = new List<string>();
        //}
    }
}
