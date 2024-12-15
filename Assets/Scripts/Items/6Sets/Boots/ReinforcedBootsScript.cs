using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReinforcedBootsScript : InventoryItemBase
{
    public override string Name
    {
        get { return "ReinforcedBoots"; }
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
        get { return "Reinforced Boots\nQuite heavier than your average Boots, but provide great protection.\n+ 150 Max Health\n+ 40 Armor\n+ 25 MovementSpeed"; }
    }
}