using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebuffManager : NetworkBehaviour
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

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        GetDebuffChildren();
    }

    public override void Spawned()
    {
        base.Spawned();
    }

    public void GetDebuffChildren()
    {
        if (HasStateAuthority)
        {
            BuffsOfPlayer[] listBuffsofPlayer = GetComponentsInChildren<BuffsOfPlayer>();

            maxHealth = 0; foreach (var buff in listBuffsofPlayer)
            {
                if (buff.GetComponent<NetworkObject>().IsValid)
                {
                    maxHealth -= buff.maxHealth;
                }
            }

            maxMana = 0; foreach (var buff in listBuffsofPlayer) maxMana -= buff.maxMana;
            damage = 0; foreach (var buff in listBuffsofPlayer) damage -= buff.damage;
            defend = 0; foreach (var buff in listBuffsofPlayer) defend -= buff.defend;
            magicResistance = 0; foreach (var buff in listBuffsofPlayer) magicResistance -= buff.magicResistance;
            magicAmpli = 0; foreach (var buff in listBuffsofPlayer) magicAmpli -= buff.magicAmpli;
            criticalChance = 0; foreach (var buff in listBuffsofPlayer) criticalChance -= buff.criticalChance;
            criticalDamage = 0; foreach (var buff in listBuffsofPlayer) criticalDamage -= buff.criticalDamage;
            moveSpeed = 0; foreach (var buff in listBuffsofPlayer) moveSpeed -= buff.moveSpeed;
            attackSpeed = 0; foreach (var buff in listBuffsofPlayer) attackSpeed -= buff.attackSpeed;
        }
    }
}
