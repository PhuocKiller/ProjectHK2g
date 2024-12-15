using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffsOfPlayer : NetworkBehaviour
{
    [Networked] public int maxHealth { get; set; }
    [Networked] public int maxMana { get; set; }
    [Networked] public int damage { get; set; }
    [Networked] public int defend { get; set; }
    [Networked] public float magicResistance { get; set; }
    [Networked] public float magicAmpli { get; set; }
    [Networked] public float criticalChance { get; set; }
    [Networked] public float criticalDamage { get; set; }
    [Networked] public int moveSpeed { get; set; }
    [Networked] public int attackSpeed { get; set; }
    [Networked] public float lifeSteal { get; set; }
    [Networked] public int levelSkill { get; set; }
    [SerializeField] public bool canHeal;

    public override void Spawned()
    {
        base.Spawned();
        BuffsBaseOnLevel(levelSkill);
    }
    public void BuffsBaseOnLevel(int level)
    {
        maxHealth = level * maxHealth;
        maxMana = level * maxMana;
        damage = level * damage;
        defend = level * defend;
        magicResistance = level * magicResistance;
        magicAmpli = level * magicAmpli;
        criticalChance = level * criticalChance;
        criticalDamage = level * criticalDamage;
        moveSpeed = level * moveSpeed;
        attackSpeed = level * attackSpeed;
        lifeSteal= level * lifeSteal;
    }
}
