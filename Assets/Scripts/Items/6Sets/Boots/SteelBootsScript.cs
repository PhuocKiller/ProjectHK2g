using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteelBootsScript : InventoryItemBase
{
    public override string Name
    {
        get { return "SteelBoots"; }
    }
    public override ItemTypes itemTypes
    {
        get { return ItemTypes.ActiveSkill; }
    }
    public override void OnUse(int indexSlot)
    {
        base.OnUse(indexSlot);
    }
    public override string Info
    {
        get { return "Steel Boots\nSo i heard you wants to upgrade your Normal Boots..\n+ 30 Armor\n+ 75 MovementSpeed"; }
    }
}
