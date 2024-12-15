using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemClickHandler : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    
    public void OnItemClicked()
    {
        
    }
    
    public void OnSelect(BaseEventData eventData)
    {
        if(Singleton<Inventory>.Instance.buyItemPanel.activeInHierarchy)
        {
            GetComponent<Image>().color = Color.green;
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        GetComponent<Image>().color = Color.white;
    }
}
