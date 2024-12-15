using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteKnightArmor : InventoryItemBase
{
    public override string Name
    {
        get { return "WhiteKnightArmor"; }
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
        get { return "White Knight Armor\nDon by the fearless Paladin zealots to battle.\n+ 125 Armor\n+ 1530 Max Health\n+ 7% Magic Resist\n+ 35 MovementSpeed"; }
    }
}
