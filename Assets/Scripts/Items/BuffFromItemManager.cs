using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffFromItemManager : NetworkBehaviour
{
    BuffsOfPlayer buffsOfPlayer;

    public override void Spawned()
    {
        base.Spawned();
        buffsOfPlayer=GetComponent<BuffsOfPlayer>();
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
    }
    public void GetBuffChildren()
    {
        if (HasStateAuthority)
        {
            BuffsOfPlayer[] listBuffsofItems = GetComponentsInChildren<BuffsOfPlayer>();

            buffsOfPlayer.maxHealth = 0; foreach (var buff in listBuffsofItems) buffsOfPlayer.maxHealth += buff.maxHealth;
            buffsOfPlayer.maxMana = 0; foreach (var buff in listBuffsofItems) buffsOfPlayer.maxMana += buff.maxMana;
            buffsOfPlayer.damage = 0; foreach (var buff in listBuffsofItems) buffsOfPlayer.damage += buff.damage;
            buffsOfPlayer.defend = 0; foreach (var buff in listBuffsofItems) buffsOfPlayer.defend += buff.defend;
            buffsOfPlayer.magicResistance = 0; foreach (var buff in listBuffsofItems) buffsOfPlayer.magicResistance += buff.magicResistance;
            buffsOfPlayer.magicAmpli = 0; foreach (var buff in listBuffsofItems) buffsOfPlayer.magicAmpli += buff.magicAmpli;
            buffsOfPlayer.criticalChance = 0; foreach (var buff in listBuffsofItems) buffsOfPlayer.criticalChance += buff.criticalChance;
            buffsOfPlayer.criticalDamage = 0; foreach (var buff in listBuffsofItems) buffsOfPlayer.criticalDamage += buff.criticalDamage;
            buffsOfPlayer.moveSpeed = 0; foreach (var buff in listBuffsofItems) buffsOfPlayer.moveSpeed += buff.moveSpeed;
            buffsOfPlayer.attackSpeed = 0; foreach (var buff in listBuffsofItems) buffsOfPlayer.attackSpeed += buff.attackSpeed;
            buffsOfPlayer.lifeSteal = 0; foreach (var buff in listBuffsofItems) buffsOfPlayer.lifeSteal += buff.lifeSteal;
           
        }
    }
}
