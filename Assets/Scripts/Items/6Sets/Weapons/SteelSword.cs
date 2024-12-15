using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteelSword : InventoryItemBase
{
    public override string Name
    {
        get { return "SteelSword"; }
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
        get { return "Steel Sword\nA basic sword, you'll need it.\n+ 50 Damage\n+ 2.5% Crit Chance"; }
    }
}