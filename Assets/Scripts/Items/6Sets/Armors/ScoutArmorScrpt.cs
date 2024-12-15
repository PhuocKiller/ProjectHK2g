using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutArmorScrpt : InventoryItemBase
{
    public override string Name
    {
        get { return "ScoutArmor"; }
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
        get { return "Scout Armor\nLighter, but made with better materials than traditional Leather Armors.\n+ 35 Armor\n+ 650 Max Health\n+ 25 MovementSpeed"; }
    }
}