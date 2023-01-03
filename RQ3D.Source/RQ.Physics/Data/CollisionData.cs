using RQ.Base.Attributes;
using UnityEngine;

namespace RQ.Physics.Data
{
    public class CollisionData
    {
        //[Tag]
        //public string[] Tags;

        public bool ReceivesDamage = true;
        public bool DamageCollider = false;
        public bool DeflectingCollider = false;
        public bool CurrentlyDeflecting = false;
        public bool IsEnabled = true;
        public Vector3 ColliderOffset { get; set; }
        public Vector3 ColliderSize { get; set; }
        public float ColliderRadius { get; set; }
        public bool IsColliderTrigger { get; set; }
    }
}
