using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SWarglaives : InventoryItemBase
{
    public override string Name
    {
        get { return "SWarglaives"; }
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
        get { return "Golden Eagle Bow\nOnce owned by an S Rank Adventurer, a shame she retired.\n+ 65 Damage\n+ 30% Attack Speed\n+ 5% Crit Chance"; }
    }
}

