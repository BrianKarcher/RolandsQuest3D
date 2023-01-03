using RQ.Base.Item;
using System;

namespace RQ.Base.Item
{
    [Serializable]
    public class ItemDesc
    {
        public ItemConfig ItemConfig;
        public int Qty;

        public ItemDesc Clone()
        {
            return new ItemDesc()
            {
                ItemConfig = this.ItemConfig,
                Qty = this.Qty
            };
        }
    }
}
