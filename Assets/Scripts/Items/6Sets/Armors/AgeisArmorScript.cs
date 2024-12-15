using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgeisArmorScript : InventoryItemBase
{
    public override string Name
    {
        get { return "AgeisArmor"; }
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
        get { return "Ageis Armor\nMade from the body of the Ageis itself!.\n+ 185 Armor\n+ 2180 Max Health\n+ 10% Magic Resist"; }
    }
}