using RQ.Base.Item;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RQ.Controller.Inventory
{
    /// <summary>
    /// This gets "owned" by the InventoryComponent, as well as getting saved and loaded with the game
    /// </summary>
    [Serializable]
    public class InventoryData
    {
        public List<ItemDesc> Items;
        /// <summary>
        /// Alternate way of storing the same group of items, grouped by Type. 
        /// This allows us to use the Dictionary to retrieve the items faster if we know the type.
        /// </summary>
        public Dictionary<ItemTypeEnum, List<ItemDesc>> ItemsByType;

        public InventoryData()
        {
            ItemsByType = new Dictionary<ItemTypeEnum, List<ItemDesc>>();
        }

        public void Add(ItemDesc item)
        {
            Items.Add(item);
            ItemsByType.TryGetValue(item.ItemConfig.ItemType, out var items);
            if (items == null)
            {
                items = new List<ItemDesc>();
                ItemsByType.Add(item.ItemConfig.ItemType, items);
            }
            //ItemsByType.Add(item.ItemConfig.ItemType, item);
            items.Add(item);
        }
    }
}
