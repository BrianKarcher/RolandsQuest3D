using System;
using System.Collections.Generic;
using System.Text;
using RQ.Common.Container;
using UnityEngine;

namespace RQ.Controller.Damage
{
    /// <summary>
    /// Sent to an entity to tell them that they have been damaged, by who, damage position, etc.
    /// </summary>
    public class DamageEntityInfo
    {
        public float DamageAmount { get; set; }
        //public string DamagedBy { get; set; }
        //public CollisionActionType CollisionDamageType { get; set; }
        //[NonSerialized]
        public IEntity DamagedByEntity { get; set; }
        
        public Vector3 HitPosition { get; set; }
        public string Tag { get; set; }
        // TODO: Move into StateInfo
        public bool IsDamaged { get; set; }
        public Collider MyCollider { get; set; }
        //public string SkillUsed { get; set; }

        public void CopyFrom(DamageEntityInfo from)
        {
            this.DamageAmount = from.DamageAmount;
            this.DamagedByEntity = from.DamagedByEntity;
            this.HitPosition = from.HitPosition;
            this.Tag = from.Tag;
            this.IsDamaged = from.IsDamaged;
            this.MyCollider = from.MyCollider;
        }
    }
}
