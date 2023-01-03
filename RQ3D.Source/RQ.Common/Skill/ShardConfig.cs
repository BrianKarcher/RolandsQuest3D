using RQ.Base.Config;
using RQ.Base.Item;
using UnityEngine;

namespace RQ.Base.Skill
{
    public class ShardConfig : ItemConfig
    {
        [SerializeField]
        private GameObject _muzzleFlash;
        public GameObject MuzzleFlash => _muzzleFlash;
    }
}
