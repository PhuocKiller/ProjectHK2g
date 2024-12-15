using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaPotion : InventoryItemBase
{
    public override string Name
    {
        get { return "ManaPotion"; }
    }
    public override ItemTypes itemTypes
    {
        get { return ItemTypes.Consumable; }
    }
    public override void OnUse(int indexSlot)
    {
        base.OnUse(indexSlot);
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        player.inventory.RemoveItem(this,indexSlot);
    }
    public override int maxStack
    {
        get { return 3; }
    }
    public override string Info
    {
        get { return "The Potion will recover the Mana of player"; }
    }
}
