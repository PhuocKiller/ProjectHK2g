using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragHandler : MonoBehaviour
{
    private Camera _camera;
    RectTransform _rectTransform;
    Vector2 localPoint;
    [SerializeField] int slotID;
    
    private void Start()
    {
       
    }
    public IInventoryItem Item { get; set; }
    public void OnBeginDrag( )
    {
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        player.inventory.indexItemSlot_1 = transform.parent.parent.GetSiblingIndex();
        player.inventory.OnItemBeginDrag?.Invoke(this);
    }

    public void OnEndDrag()
    {
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        player.inventory.OnItemEndDrag?.Invoke(this);    
    }

    public void OnPointerClick( )
    {
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        player.inventory.OnItemClicked?.Invoke(this);
    }
    
    public void OnDrop()
    {
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        player.inventory.indexItemSlot_2 = transform.parent.parent.GetSiblingIndex();
        player.inventory.SwapItem();
    }
    
   
}
