using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScore : NetworkBehaviour
{
    [Networked] public int killScore { get; set; }
    [Networked] public int assistScore { get; set; }
    public List<PlayerController> playersMakeDamages;

    public override void Spawned()
    {
        base.Spawned();
        playersMakeDamages.Clear();
    }
}

