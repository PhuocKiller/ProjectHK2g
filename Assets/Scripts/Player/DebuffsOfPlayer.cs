using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffsOfPlayer : NetworkBehaviour
{
    [Networked] public float maxHealth { get; set; }
    [Networked] public float maxMana { get; set; }
    [Networked] public float damage { get; set; }
    [Networked] public int defend { get; set; }
    [Networked] public float magicResistance { get; set; }
    [Networked] public float magicAmpli { get; set; }
    [Networked] public float criticalChance { get; set; }
    [Networked] public float criticalDamage { get; set; }
    [Networked] public int moveSpeed { get; set; }
    [Networked] public int attackSpeed { get; set; }
    [SerializeField] public bool canHeal;

    public override void Spawned()
    {
        base.Spawned();
    }
}

