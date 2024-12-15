using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyDutyBootsScript : InventoryItemBase
{
    public override string Name
    {
        get { return "HeavyDutyBoots"; }
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
        get { return "Heavy Duty Boots\nTo protect your legs from most physical attacks.\n+ 75 Armor\n+ 50 MovementSpeed"; }
    }
}
