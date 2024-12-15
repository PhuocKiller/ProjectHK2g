using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenSpellBook : InventoryItemBase
{
    public override string Name
    {
        get { return "QueenSpellbook"; }
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
        get { return "Ice Queen's Spellbook\nThe queen was an Ice Mage all along? Don't ask how I got my hands on this one..\n+ 75 Damage\n+ 100% Magic Amp"; }
    }
}
