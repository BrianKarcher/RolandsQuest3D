using RQ.Base.Item;
using RQ.Base.Skill;
using RQ.Common.Components;
using RQ.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RQ.Controller.Inventory
{
    [AddComponentMenu("RQ/Components/Inventory")]
    public class InventoryComponent : ComponentBase<InventoryComponent>
    {
        private long _addItemId;

        [SerializeField]
        private InventoryData _inventoryData;

        protected override void Awake()
        {
            base.Awake();
            //ItemsByType = new Dictionary<ItemTypeEnum, ItemDesc>();
        }

        public override void StartListening()
        {
            base.StartListening();
            _addItemId = MessageDispatcher.Instance.StartListening("AddItem", _componentRepository.GetId(), (data) =>
            {
                var item = (ItemDesc)data.ExtraInfo;
                Debug.Log($"Adding item {item.ItemConfig.name}, qty {item.Qty}");                
                AddItem(item);
            });
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher.Instance.StopListening("AddItem", _componentRepository.GetId(), _addItemId);
        }

        public void AddItem(ItemDesc item)
        {
            var itemConfigAndCount = _inventoryData.Items.FirstOrDefault(i => i.ItemConfig == item.ItemConfig);
            // Prevent duplication of Items, just adjust the Qty if the item is already in the inventory
            //ItemDesc itemToUpdate = null;
            if (itemConfigAndCount == null)
            {
                itemConfigAndCount = item.Clone();
                _inventoryData.Add(itemConfigAndCount);
                //Items.Add(itemConfigAndCount);
            }
            else
            {
                //itemConfigAndCount = itemConfigAndCount;
                itemConfigAndCount.Qty += item.Qty;
            }
            
            MessageDispatcher.Instance.DispatchMsg("ItemAdded", 0, null, _componentRepository.GetId(), itemConfigAndCount);
        }

        public void ToggleMold(bool isAsc)
        {

        }
    }
}
