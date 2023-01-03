using RQ.Common.Components;
using UnityEngine;

namespace RQ.Controller.Damage
{
    [AddComponentMenu("RQ/Components/Collision Component")]
    public class CollisionComponent : ComponentBase<CollisionComponent>
    {
        [SerializeField]
        private RQ.Physics.Data.CollisionData _collisionData = null;
        public RQ.Physics.Data.CollisionData CollisionData => _collisionData;

    }
}
