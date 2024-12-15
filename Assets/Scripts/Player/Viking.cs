using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viking : PlayerController
{
    TickTimer timerSkill2;
    [SerializeField] Transform axe;
    
    public override void Spawned()
    {
        base.Spawned();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        buffsFromPassive.lifeSteal = 0.2f * (playerStat.maxHealth - playerStat.currentHealth) / playerStat.maxHealth;
        buffsFromPassive.attackSpeed =(int)(100f * (playerStat.maxHealth - playerStat.currentHealth) / playerStat.maxHealth);
    }

    public override void NormalAttack(NetworkObject VFXEffect, int levelDamage, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false, float timeTrigger = 0f,
        float TimeEffect = 0f)
    {
        base.NormalAttack(VFXEffect, levelDamage, isPhysicDamage, timeTrigger: timeTrigger);
        StartCoroutine(DelaySpawnAttack(VFXEffect, levelDamage, isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen,
          timeTrigger, TimeEffect));
    }
    IEnumerator DelaySpawnAttack(NetworkObject VFXEffect, int levelDamage, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false, float timeTrigger = 0f,
        float TimeEffect = 0f)
    {
        yield return new WaitForSeconds(0.5f * 100f / playerStat.attackSpeed);
        Runner.Spawn(VFXEffect.gameObject, normalAttackTransform.transform.position, normalAttackTransform.rotation, inputAuthority: Object.InputAuthority
     , onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
     {
         obj.GetComponent<AttackObjects>().SetUpPlayer(this, playerStat.b_damage, isPhysicDamage, normalAttackTransform,
             isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect);
     }
                        );
    }

    public override void Skill_1(NetworkObject VFXEffect, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        base.Skill_1(VFXEffect, levelDamage, manaCost, isPhysicDamage, timeTrigger: timeTrigger);
        StartCoroutine(DelaySpawnSkill_1(VFXEffect, levelDamage, isPhysicDamage,
            isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect, posMouseUp));
        
    }
    IEnumerator DelaySpawnSkill_1(NetworkObject VFXEffect, int levelDamage, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        yield return new WaitForSeconds(0.8f);
        DeActiveAxeRPC();
        NetworkObject obj = Runner.Spawn(VFXEffect.gameObject, skill_1Transform.position, skill_1Transform.rotation, Object.InputAuthority,
            onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
            {
                obj.GetComponent<AxeAttackObject>().SetUp(this, levelDamage, isPhysicDamage, null,
                 isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect, posMouseUp);
                obj.GetComponent<AxeAttackObject>().SetDirection(transform.forward);
            });
    }
   

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void ActiveAxeRPC()
    {
        axe.gameObject.SetActive(true);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void DeActiveAxeRPC()
    {
        axe.gameObject.SetActive(false);
    }
    public override void Skill_2(NetworkObject VFXEffect, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        base.Skill_2(VFXEffect, levelDamage, manaCost, isPhysicDamage, timeTrigger: timeTrigger);
        SkillRPC
         (6, levelDamage, manaCost, isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect, levelSkill);
        playerStat.isLifeSteal = true;
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
        NetworkObject obj = Runner.Spawn(VFXEffect.gameObject, skill_2Transform.position, Quaternion.identity, Object.InputAuthority,
            onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
            {
                obj.GetComponent<EnvironmentObjects>().SetUp(this, playerTeam, 0.05f, levelDamage, isPhysicDamage, skill_2Transform,
                 isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect);
                obj.GetComponent<BuffsOfPlayer>().levelSkill = levelSkill;
            });
        playerStat.isUnstopAble = true;
    }
    
}

