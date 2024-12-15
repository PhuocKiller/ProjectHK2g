using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSkillManager : NetworkBehaviour
{
    PlayerController player;
    
    private void Awake()
    {
        player = GetComponentInParent<PlayerController>();
    }
    public virtual void UseItemSkill(SkillName skillName, NetworkObject VFXEffect, int levelDamage, int manaCost, bool isPhysicDamage,
         bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
         float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        switch (skillName)
        {
            case SkillName.HealPotion:
                {
                    NetworkObject obj = Runner.Spawn(VFXEffect.gameObject, player.itemSkillTransform.position, player.itemSkillTransform.rotation, Object.InputAuthority,
            onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
            {
                obj.GetComponent<PotionRegenSkill>().SetUp(SkillName.HealPotion,player, 0.05f, levelDamage,
              timeTrigger,player.itemSkillTransform);
            });
                    break;
                }
            case SkillName.ManaPotion:
                {
                    NetworkObject obj = Runner.Spawn(VFXEffect.gameObject, player.itemSkillTransform.position, player.itemSkillTransform.rotation, Object.InputAuthority,
            onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
            {
                obj.GetComponent<PotionRegenSkill>().SetUp(SkillName.ManaPotion, player, 0.1f, levelDamage,
              timeTrigger, player.itemSkillTransform);
            });
                    break;
                }
            case SkillName.AdventurerShield:
                {
                    player.SkillRPC(10, levelDamage, manaCost, isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect);
                    break;
                }
            case SkillName.Invicibility:
                {
                    NetworkObject obj = Runner.Spawn(VFXEffect.gameObject, transform.position, Quaternion.identity, Object.InputAuthority,
            onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
            {
                player.playerStat.isStartFadeInvi = true;
                obj.GetComponent<InviObjects>().SetUp(player, player.playerStat.damage, isPhysicDamage, null,
             isMakeStun, isMakeSlow, isMakeSilen,timeTrigger, TimeEffect);
                obj.GetComponent<BuffsOfPlayer>().levelSkill = levelSkill;
            });
                    player.SetParentRPC(obj.Id);
                   break;
                }
            case SkillName.Blink:
                {
                    if (player.playerStat.isFollowEnemy)
                    {
                       posMouseUp = player.overlapSphere.closestEnemyPlayer.transform.position;
                    }
                    else
                    {
                       posMouseUp = player.transform.position + player.transform.forward * 20;
                    }
                    player.characterControllerPrototype.Move((Vector3)posMouseUp - transform.position);
                    break;
                }
            case SkillName.Hasty:
                {
                    player.SkillRPC
         (11, levelDamage, manaCost, isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect, levelSkill);
                    break;
                }
        }
    }
}
