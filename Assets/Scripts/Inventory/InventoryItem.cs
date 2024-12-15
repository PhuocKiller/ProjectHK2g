using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemTypes
{
    Consumable,
    ActiveSkill,
}
public class InventoryItem : MonoBehaviour
{

}
public interface IInventoryItem
{
    ItemTypes itemTypes { get; set; }
    string Name { get; }
    Sprite Image { get; }
    int Price { get; }
    string Info { get; set; }
    void OnPickUp();
    void OnDrop();
    void OnUse(int indexSlot);
    InventorySlot Slot { get; set; }
    int maxStack { get; }
}
public class InventoryEventArgs : EventArgs

{
    public InventoryItemBase Item;
    public InventoryEventArgs(InventoryItemBase item)
    {
        Item = item;
    }
}
