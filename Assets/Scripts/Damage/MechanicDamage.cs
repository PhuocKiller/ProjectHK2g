using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechanicDamage : MonoBehaviour
{
    public float deltaDamage = 0.03f;
    public bool isCritPhysicDamage;
    public int GetDamageOfTwoObject(int damage, bool isPhysicDamage, PlayerController playerAttack, Collider ObjectBeAttack, out bool isCritPhysic)
    {
        if (ObjectBeAttack.GetComponent<PlayerController>() != null)
        {
            int baseDamage = (int)(damage * Random.Range(0.95f, 1.05f) *
          (1 - (deltaDamage * (isPhysicDamage ? ObjectBeAttack.GetComponent<PlayerController>().playerStat.defend : ObjectBeAttack.GetComponent<PlayerController>().playerStat.magicResistance)
          / (1 + deltaDamage * ObjectBeAttack.GetComponent<PlayerController>().playerStat.defend))));
            CheckCritPhysicDamage(playerAttack, isPhysicDamage, out float increaseDamage);
            isCritPhysic = isCritPhysicDamage;
            return (int)(baseDamage * increaseDamage);
        }
        else if (ObjectBeAttack.GetComponent<CreepController>() != null)
        {
            int baseDamage = (int)(damage * Random.Range(0.95f, 1.05f) *
        (1 - (deltaDamage * (isPhysicDamage ? ObjectBeAttack.GetComponent<CreepController>().playerStat.defend : ObjectBeAttack.GetComponent<CreepController>().playerStat.magicResistance)
        / (1 + deltaDamage * ObjectBeAttack.GetComponent<CreepController>().playerStat.defend))));
            CheckCritPhysicDamage(playerAttack, isPhysicDamage, out float increaseDamage);
            isCritPhysic = isCritPhysicDamage;
            return (int)(baseDamage * increaseDamage);
        }

        else if (ObjectBeAttack.GetComponent<BuildingController>() != null)
        {
            if (!isPhysicDamage)
            {
                isCritPhysic = isCritPhysicDamage;
                return 0;
            }
            else
            {
                isCritPhysic = isCritPhysicDamage;
                return (int)(damage * Random.Range(0.95f, 1.05f) *
          (1 - (deltaDamage * (ObjectBeAttack.GetComponent<BuildingController>().defend)
          / (1 + deltaDamage * ObjectBeAttack.GetComponent<BuildingController>().defend))));
            }

        }
        else
        {
            isCritPhysic = isCritPhysicDamage;
            return (int)(damage * Random.Range(0.95f, 1.05f));
        }

    }
    void CheckCritPhysicDamage(PlayerController playerAttack, bool isPhysicDamage, out float increaseDamage)
    {
        if (playerAttack)
        {
            if (isPhysicDamage)
            {
                increaseDamage = IncreasePhysicDamage(playerAttack, out bool isCrited);
                isCritPhysicDamage = isCrited;
            }
            else
            {
                increaseDamage = IncreaseMagicDamage(playerAttack);
                isCritPhysicDamage = false;
            }
        }
        else
        {
            increaseDamage = 1;
            isCritPhysicDamage = false;
        }
    }
    public float IncreasePhysicDamage(PlayerController playerAttack, out bool isCrited)
    {
        if (GetChance(playerAttack.playerStat.criticalChance))
        {
            isCrited = true;
            return playerAttack.playerStat.criticalDamage;
        }
        else
        {
            isCrited = false; return 1;
        }
    }
    public float IncreaseMagicDamage(PlayerController playerAttack)
    {
        return (1 + playerAttack.playerStat.magicAmpli);
    }
    public bool GetChance(float chance)
    {
        float r = Random.Range(1f, 100f);
        if (r / 100 < chance)
        {
            return true;
        }
        else return false;
    }
}
