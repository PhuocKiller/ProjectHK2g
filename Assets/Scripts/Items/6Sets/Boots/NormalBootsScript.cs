using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBootsScript : InventoryItemBase
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
        get { return "Normal Boots\nOne needs something to protect themselves from the terrain afterall.\n+ 45 MovementSpeed"; }
    }
}
