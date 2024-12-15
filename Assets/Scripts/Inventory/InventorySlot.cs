using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class InventorySlot
{
    public List<InventoryItemBase> mItemStack = new List<InventoryItemBase>();

    public int Id;// { get; }
    public void AddItem(InventoryItemBase item)
    {
        item.Slot = this;
        mItemStack.Add(item);
    }
    public InventoryItemBase FirstItem
    {
        get
        {
            if (IsEmpty) { return null; }
            return mItemStack[0];
        }
    }
    public bool IsStackable(InventoryItemBase item)
    {
        if (IsEmpty || Count>=item.maxStack) return false;
        IInventoryItem first= mItemStack[0];
        if (first.Name== item.Name)
        {
            return true;
        }
        return false;
    }
    public bool IsEmpty
    {
        get { return Count == 0; }
    }
    public int Count
    {
        get { return mItemStack.Count; }
    }
    public bool Remove(InventoryItemBase item,int indexSlot)
    {
        if (IsEmpty) return false ;
        IInventoryItem first=FirstItem;
        if (first.Name == item.Name && Id== indexSlot)
        {
            mItemStack.Remove(item);
            return true;
        }
        return false;
    }


}
