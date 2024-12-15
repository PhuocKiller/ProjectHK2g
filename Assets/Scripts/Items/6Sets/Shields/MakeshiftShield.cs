using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeshiftShield : InventoryItemBase
{
    public override string Name
    {
        get { return "MakeshiftShield"; }
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
        get { return "Makeshift Shield\nA custom made shield for the magic swordmens.\n+ 25 Armor\n+ 900 Max Health\n+ 75 Max Mana\n+ 2.5% Magic Resist"; }
    }
}
