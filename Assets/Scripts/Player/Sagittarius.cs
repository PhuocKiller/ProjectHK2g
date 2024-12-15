using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class Sagittarius : PlayerController
{
    public override void Spawned()
    { base.Spawned(); }

    public override void NormalAttack(NetworkObject VFXEffect, int levelDamage, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false, float timeTrigger = 0f,
        float TimeEffect = 0f)
    {

        base.NormalAttack(VFXEffect, levelDamage, isPhysicDamage, timeTrigger: timeTrigger);
        
        Runner.Spawn(VFXEffect.gameObject, normalAttackTransform.transform.position, normalAttackTransform.rotation, inputAuthority: Object.InputAuthority
     , onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
     {
         obj.GetComponent<AttackObjects>().SetUpPlayer(this, playerStat.damage, isPhysicDamage, null,
              isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect,true);
         
         obj.GetComponent<AttackObjects>().SetDirection(transform.forward);
     });
    }
  
    public override void Skill_1(NetworkObject VFXEffect, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        base.Skill_1(VFXEffect, levelDamage, manaCost, isPhysicDamage, isMakeStun: isMakeStun,
            TimeEffect: TimeEffect, timeTrigger: timeTrigger);
        if(CheckHitPlayer() && CheckHitPlayer().playerTeam==playerTeam)
        {
            CheckHitPlayer().SkillRPC
         (0, levelDamage, manaCost, isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect);
        }
        else
        {
            SkillRPC(0, levelDamage, manaCost, isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect);
        }
    }

    PlayerController CheckHitPlayer()
    {
        
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
       Physics.Raycast(rayCastTransform.position,transform.forward,out RaycastHit hit, 500, 1<<7);
        if (hit.transform == null) return null;
        else return hit.transform.gameObject.GetComponent<PlayerController>();
    }
    
    public override void Skill_2(NetworkObject VFXEffect, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        base.Skill_2(VFXEffect, levelDamage, manaCost, isPhysicDamage, timeTrigger: timeTrigger);
        if (CheckHitPlayer())
        {
            if(CheckHitPlayer().playerTeam == playerTeam) //cùng team là hồi máu
            {
                CheckHitPlayer().SkillRPC
         (1, levelDamage, manaCost, isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect, levelSkill);
            }
            else //khác team là gây dam
            {
                CheckHitPlayer().SkillRPC
         (9,0, manaCost, isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect, levelSkill);
                CheckHitPlayer().ApplyDamage(levelDamage, isPhysicDamage, this,
                counter: (int counterDamage, bool isPhysicDamage) =>
                {
                    ApplyDamage(counterDamage, isPhysicDamage, CheckHitPlayer());
                }
                , isKillPlayer: (int levelHeroKilled, List<PlayerController> playerMakeDamage) => // Nhận exp khi giêt địch ở đây
                {
                    playerStat.currentXP += (int)(100 * Mathf.Lerp(1 / playerMakeDamage.Count,1,0.5f) * levelHeroKilled);
                    playerScore.killScore += 1;
                    playerScore.assistScore -= 1;
                }
                );
            }
        }
        else
        {
            SkillRPC(1,levelDamage,manaCost,isPhysicDamage,isMakeStun,isMakeSlow, isMakeSilen,timeTrigger, TimeEffect, levelSkill);
        }
    }
    public override void Ultimate(NetworkObject VFXEffect, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        base.Ultimate(VFXEffect, levelDamage, manaCost, isPhysicDamage, timeTrigger: timeTrigger);
        StartCoroutine(DelayUltimate(VFXEffect, levelDamage, isPhysicDamage,
            isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect, posMouseUp, levelSkill));


    }
    IEnumerator DelayUltimate(NetworkObject VFXEffect, int levelDamage, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        yield return new WaitForSeconds(0f);
        NetworkObject obj = Runner.Spawn(VFXEffect.gameObject, posMouseUp, Quaternion.identity, Object.InputAuthority,
            onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
            {
                obj.GetComponent<EnvironmentObjects>().SetUp(this,playerTeam, 0.05f, levelDamage, isPhysicDamage, null,
                 isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect);
                obj.GetComponent<BuffsOfPlayer>().levelSkill = levelSkill;
            });
        
    }
    
}

