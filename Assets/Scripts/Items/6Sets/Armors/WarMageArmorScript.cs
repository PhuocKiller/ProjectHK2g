using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarMageArmorScript : InventoryItemBase
{
    public override string Name
    {
        get { return "WarMageArmor"; }
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
        get { return "WarMage Armor\nWhen magicians become too much of an issue, units with this armor will be deployed..\n+ 95 Armor\n+ 1780 Max Health\n+ 17% Magic Resist"; }
    }
}
