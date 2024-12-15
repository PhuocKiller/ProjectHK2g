using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemsManager : MonoBehaviour
{
    public PlayerController player;
    GameObject[] itemButton;
    GameObject itemToBuy;
    InventoryItemBase itemToSell;
    [SerializeField] GameObject buyBtn, sellBtn, basicBtn,shieldBtn, armorBtn, weaponBtn,bootBtn;
    int priceItem; int indexItem; int indexSlot;
    NetworkManager networkManager;
    [SerializeField] Transform basicBG,shieldBG,armorBG,weaponBG,bootBG,cantBuyPanel;
    public TextMeshProUGUI priceValue;
    [SerializeField] TextMeshProUGUI itemInfoText;
    
    private void Awake()
    {
        networkManager=FindObjectOfType<NetworkManager>();
        LoadStatItems();
        BasicButton();
    }
    private void OnEnable()
    {
        StartCoroutine(DelayCheckPlay());
        itemToBuy = null;
        priceItem = 0;
        priceValue.text = "0";
        itemInfoText.text = "";
        buyBtn.SetActive(true); sellBtn.SetActive(false);
    }
    private void Update()
    {
        if(player==null) return;
        cantBuyPanel.gameObject.SetActive(!player.playerStat.canBuyItem);
    }
    IEnumerator DelayCheckPlay()
    {
        yield return new WaitForSeconds(0.2f);
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        this.player = player;
    }
    void LoadStatItems()
    {
        LoadStatBasicItems();
        LoadStatShieldItems();
        LoadStatArmorItems();
        LoadStatWeaponItems();
        LoadStatBootItems();
    }
    void LoadStatBasicItems()
    {
        int index = -1;
        foreach (Transform item in basicBG)
        {
            index++;
            Image imageItem = item.GetChild(0).GetComponent<Image>();
            imageItem.sprite = networkManager.basicItems[index].GetComponent<IInventoryItem>().Image;
        }
    }
    void LoadStatShieldItems()
    {
        int index = -1;
        foreach (Transform item in shieldBG)
        {
            index++;
            Image imageItem = item.GetChild(0).GetComponent<Image>();
            imageItem.sprite = networkManager.shieldItems[index].GetComponent<IInventoryItem>().Image;
        }
    }
    void LoadStatArmorItems()
    {
        int index = -1;
        foreach (Transform item in armorBG)
        {
            index++;
            Image imageItem = item.GetChild(0).GetComponent<Image>();
            imageItem.sprite = networkManager.armorItems[index].GetComponent<IInventoryItem>().Image;
        }
    }
    void LoadStatWeaponItems()
    {
        int index = -1;
        foreach (Transform item in weaponBG)
        {
            index++;
            Image imageItem = item.GetChild(0).GetComponent<Image>();
            imageItem.sprite = networkManager.weaponItems[index].GetComponent<IInventoryItem>().Image;
        }
    }
    void LoadStatBootItems()
    {
        int index = -1;
        foreach (Transform item in bootBG)
        {
            index++;
            Image imageItem = item.GetChild(0).GetComponent<Image>();
            imageItem.sprite = networkManager.bootItems[index].GetComponent<IInventoryItem>().Image;
        }
    }
    public void UpdatePrice(Transform thisBtn)
    {
        string parentName=thisBtn.parent.name;
        switch (parentName)
        {
            case "BasicBG": { itemToBuy = networkManager.basicItems[thisBtn.GetSiblingIndex()];  break; }
            case "ShieldBG": { itemToBuy = networkManager.shieldItems[thisBtn.GetSiblingIndex()]; break; }
            case "ArmorBG": { itemToBuy = networkManager.armorItems[thisBtn.GetSiblingIndex()]; break; }
            case "WeaponBG": { itemToBuy = networkManager.weaponItems[thisBtn.GetSiblingIndex()]; break; }
            case "BootBG": { itemToBuy = networkManager.bootItems[thisBtn.GetSiblingIndex()]; break; }
        }
        priceItem = itemToBuy.GetComponent<InventoryItemBase>().Price;
        priceValue.text= priceItem.ToString();
        ShowInfoItem(itemToBuy.GetComponent<InventoryItemBase>());
        buyBtn.SetActive(true); sellBtn.SetActive(false);  
    }

    public void BuyItem()
    {
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        if(!player.playerStat.canBuyItem) return;
        if (player.playerStat.coinsValue< priceItem)
        {

        }
        else
        {
            player.inventory.AddItem(itemToBuy.GetComponent<InventoryItemBase>(), out bool canAdd);
            if (canAdd) player.playerStat.coinsValue -= priceItem;
        }
    }
    public void SellItem()
    {
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        if (!player.playerStat.canBuyItem) return;
        player.inventory.RemoveItem(itemToSell, indexSlot);
        player.playerStat.coinsValue += priceItem;
    }
    public void ShowInfoItem(InventoryItemBase item)
    {
        itemInfoText.text = item.Info;
    }
    public void CheckInfoToSell(InventoryItemBase item, int indexSlot)
    {
        ShowInfoItem(item);
        buyBtn.SetActive(false); sellBtn.SetActive(true);
        itemToSell = item;
        this.indexSlot=indexSlot;
        priceItem = (int)(item.Price * 0.7);
        priceValue.text= priceItem.ToString();
    }
    public void BasicButton()
    {
        basicBtn.GetComponent<Image>().color= Color.green;
        shieldBtn.GetComponent<Image>().color = Color.white;
        armorBtn.GetComponent<Image>().color = Color.white;
        weaponBtn.GetComponent<Image>().color = Color.white;
        bootBtn.GetComponent<Image>().color = Color.white;
    }
    public void ShieldButton()
    {
        basicBtn.GetComponent<Image>().color = Color.white;
        shieldBtn.GetComponent<Image>().color = Color.green;
        armorBtn.GetComponent<Image>().color = Color.white;
        weaponBtn.GetComponent<Image>().color = Color.white;
        bootBtn.GetComponent<Image>().color = Color.white;
    }
    public void ArmorButton()
    {
        basicBtn.GetComponent<Image>().color = Color.white;
        shieldBtn.GetComponent<Image>().color = Color.white;
        armorBtn.GetComponent<Image>().color = Color.green;
        weaponBtn.GetComponent<Image>().color = Color.white;
        bootBtn.GetComponent<Image>().color = Color.white;
    }
    public void WeaponButton()
    {
        basicBtn.GetComponent<Image>().color = Color.white;
        shieldBtn.GetComponent<Image>().color = Color.white;
        armorBtn.GetComponent<Image>().color = Color.white;
        weaponBtn.GetComponent<Image>().color = Color.green;
        bootBtn.GetComponent<Image>().color = Color.white;
    }
    public void BootButton()
    {
        basicBtn.GetComponent<Image>().color = Color.white;
        shieldBtn.GetComponent<Image>().color = Color.white;
        armorBtn.GetComponent<Image>().color = Color.white;
        weaponBtn.GetComponent<Image>().color = Color.white;
        bootBtn.GetComponent<Image>().color = Color.green;
    }
}
