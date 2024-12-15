using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffManager : NetworkBehaviour
{
    PlayerController player;
    CreepController creep;
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
     

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        GetBuffChildren();
    }

    public override void Spawned()
    {
        base.Spawned();
        player=transform.parent.parent.GetComponent<PlayerController>();
        creep = transform.parent.parent.GetComponent<CreepController>();
    }

    public void GetBuffChildren()
    {
        if (HasStateAuthority)
        {
            BuffsOfPlayer[] listBuffsofPlayer = GetComponentsInChildren<BuffsOfPlayer>();

            maxHealth = 0; foreach (var buff in listBuffsofPlayer) maxHealth += buff.maxHealth;
            maxMana = 0; foreach (var buff in listBuffsofPlayer) maxMana += buff.maxMana;
            damage = 0; foreach (var buff in listBuffsofPlayer) damage += buff.damage;
            defend = 0; foreach (var buff in listBuffsofPlayer) defend += buff.defend;
            magicResistance = 0; foreach (var buff in listBuffsofPlayer) magicResistance += buff.magicResistance;
            magicAmpli = 0; foreach (var buff in listBuffsofPlayer) magicAmpli += buff.magicAmpli;
            criticalChance = 0; foreach (var buff in listBuffsofPlayer) criticalChance += buff.criticalChance;
            criticalDamage = 0; foreach (var buff in listBuffsofPlayer) criticalDamage += buff.criticalDamage;
            moveSpeed = 0; foreach (var buff in listBuffsofPlayer) moveSpeed += buff.moveSpeed;
            attackSpeed = 0; foreach (var buff in listBuffsofPlayer) attackSpeed += buff.attackSpeed;
            lifeSteal = 0; foreach (var buff in listBuffsofPlayer) lifeSteal += buff.lifeSteal;
            if (player?.state == 3 || creep?.state==3)
            {
                for (int i = 3; i < listBuffsofPlayer.Length; i++) //i=0 là environment, 1 là passive, 2 là item
                {
                    if(listBuffsofPlayer[i].GetComponent<InventoryItemBase>()!=null)
                    {
                        Destroy(listBuffsofPlayer[i].gameObject);
                    }
                }
            }
        }
    }
}

