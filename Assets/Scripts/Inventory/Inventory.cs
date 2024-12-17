using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : NetworkBehaviour
{
    private const int SLOTS = 6;
    public IList<InventorySlot> mSlots = new List<InventorySlot>();
    public event EventHandler<InventoryEventArgs> ItemAdded,ItemRemoved,ItemUsed,InventoryUpdate;
    public Action<ItemDragHandler> OnItemClicked, OnItemBeginDrag, OnItemEndDrag, OnRightMouseBtnClick;
    public Action<int, int> OnItemDroppedOn;
    public int indexItemSlot_1, indexItemSlot_2;
    InventoryItemBase item;
    public GameObject buyItemPanel;
    [SerializeField] Transform inventoryPanel;
    [Networked, Capacity(6)] public NetworkArray<int> indexSlot_Item {  get; }
    [Networked, Capacity(6)] public NetworkArray<int> indexSlot_Count { get; }

    public override void Spawned()
    {
        base.Spawned();
        if(HasStateAuthority)
        {
            SetupInventory();
            for (int i = 0;i<indexSlot_Item.Length;i++)
            {
                indexSlot_Item.Set(i, -1);
                indexSlot_Count.Set(i, 0);
            }
        }
    }
    public void SetupInventory()
    {
        inventoryPanel = FindObjectOfType<InventoryPanelManager>().transform;
        buyItemPanel = GameObject.Find("BuyItemButton").transform.GetChild(0).gameObject;
    }
    private void Awake()
    {
        
    }
    public Inventory()
    {
        for (int i = 0; i < SLOTS; i++)
        {
            mSlots.Add(new InventorySlot());
            mSlots[i].Id = i;
        }
    }
    public void SwapItem()
    {
        NetworkManager networkManager = FindObjectOfType<NetworkManager>();
        int newIDSlot = -1;
        newIDSlot = mSlots[indexItemSlot_1].Id;
        mSlots[indexItemSlot_1].Id = mSlots[indexItemSlot_2].Id;
        mSlots[indexItemSlot_2].Id = newIDSlot;


        InventorySlot newSlot = new InventorySlot();
        newSlot = mSlots[indexItemSlot_1];
        mSlots[indexItemSlot_1] = mSlots[indexItemSlot_2];
        mSlots[indexItemSlot_2] = newSlot;
        for (int i = 0;i<SLOTS;i++)
        {
            indexSlot_Item.Set(i, networkManager.FindOnlineItemsIndex
                (mSlots[i].FirstItem!=null? mSlots[i].FirstItem.Name: ""));
            indexSlot_Count.Set(i, mSlots[i].Count);
        }
        OnItemDroppedOn?.Invoke(indexItemSlot_1, indexItemSlot_2);
    }
    public void AddItem(InventoryItemBase item, out bool canAdd)
    {
        NetworkManager networkManager = FindObjectOfType<NetworkManager>();
        InventoryItemBase newItem = item.Clone();
        InventorySlot freeSlot = FindStackAble(newItem);
        if (freeSlot == null)
        {
            freeSlot = FindNextEmptySlot();
        }
        if (freeSlot != null)
        {
            
            freeSlot.AddItem(newItem);
            
            if (ItemAdded != null)
            {
                ItemAdded(this, new InventoryEventArgs(newItem));
            }
            indexSlot_Item.Set(freeSlot.Id, networkManager.FindOnlineItemsIndex(newItem.Name));
            indexSlot_Count.Set(freeSlot.Id, freeSlot.Count);
            networkManager.SpawnObjWhenAddItem(networkManager.FindItemBaseOnName(newItem.Name), freeSlot.Id);
            SkillButton btn = inventoryPanel.GetChild(freeSlot.Id).GetComponent<SkillButton>();
            btn.Initialize(newItem.skillName);
            canAdd = true;
        }
        else canAdd = false;
        
    }

    public void RemoveItem(InventoryItemBase item,int indexSlot)
    {
        NetworkManager networkManager = FindObjectOfType<NetworkManager>();
        foreach (InventorySlot slot in mSlots)
        {
            if (slot.Remove(item, indexSlot))
            {
                if (ItemRemoved != null)
                {
                    ItemRemoved(this,new InventoryEventArgs(item));
                    networkManager.DestroyObjWhenRemoveItem(item.Name);
                }
                if(slot.Count==0)
                {
                    SkillButton btn = inventoryPanel.GetChild(slot.Id).GetComponent<SkillButton>();
                    indexSlot_Item.Set(slot.Id, -1);
                  //  btn.Initialize(SkillName.None);
                }
                indexSlot_Count.Set(slot.Id, slot.Count);
                break;
            }
        }
    }
    public void BackUpInventory()
    {
        NetworkManager networkManager = FindObjectOfType<NetworkManager>();
        for (int i = 0; i < SLOTS; i++)
        {
            for (int j = 0;j<indexSlot_Count.Get(i);j++)
            {
                if (indexSlot_Item.Get(i) >= 0)
                {
                    InventoryItemBase item = networkManager.onlineItems[indexSlot_Item.Get(i)].GetComponent<InventoryItemBase>();
                    mSlots[i].AddItem(item);
                    if (ItemAdded != null)
                    {
                        ItemAdded(this, new InventoryEventArgs(item));
                    }
                   // networkManager.SpawnObjWhenAddItem(networkManager.FindItemBaseOnName(item.Name), mSlots[i].Id);
                    SkillButton btn = inventoryPanel.GetChild(mSlots[i].Id).GetComponent<SkillButton>();
                    btn.Initialize(item.skillName);
                }
            }
        }
    }
    internal void UseItemClickInventory(InventoryItemBase item,int indexSlot, out bool canActive) //Use item khi click trực tiếp trong inventory
    {
        if (!buyItemPanel.activeInHierarchy)
        {
            if (ItemUsed != null)
            {
                ItemUsed(this, new InventoryEventArgs(item));
            }
            item.OnUse(indexSlot); //item remove nằm trong đây
            if (InventoryUpdate != null)
            {
                InventoryUpdate(this, new InventoryEventArgs(item));
            }
            canActive = true;
        }
        else
        {
            buyItemPanel.GetComponent<ItemsManager>().CheckInfoToSell(item, indexSlot);
            canActive=false;
        }
    }
   
    private InventorySlot FindStackAble(InventoryItemBase item)
    {
        foreach (InventorySlot slot in mSlots)
        {
            if (slot.IsStackable(item))
            {
                return slot;
            }
        }
        
        return null;
    }
    private InventorySlot FindNextEmptySlot()
    {
        foreach (InventorySlot slot in mSlots)
        {
            if (slot.IsEmpty) return slot;
        }
        return null;
    }
    
    
}
