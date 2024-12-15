using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResistanceBootsScript : InventoryItemBase
{
    public override string Name
    {
        get { return "ResistanceBoots"; }
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
        get { return "Resistance Boots\nMagics.. Everyone needs something to protect themselves against that.\n+ 11% Magic Resist\n+ 75 MovementSpeed"; }
    }
}
