using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegionShield : InventoryItemBase
{
    public override string Name
    {
        get { return "LegionShield"; }
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
        get { return "Legion Shield\nThe first countermeasure against Magics.\nPassive: + 25 Armor\n+1250 Max Health\n+ 5% Magic Resist"; }
    }
}