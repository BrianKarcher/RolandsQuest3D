using RQ.Base.Config;
using UnityEngine;

namespace RQ.Base.Item
{
    public class ItemConfig : RQBaseConfig
    {
        public ItemTypeEnum ItemType;
        public string DisplayName;
        //public Texture HUDImage;
        public Sprite HUDImageSelected;
        public Sprite HUDImageUnselected;
        public Color HUDColor;
        public float SellCost;
        public float PurchaseCost;
        public GameObject ReferencePrefab;
    }
}
