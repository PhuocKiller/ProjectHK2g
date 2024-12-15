using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Tesla : PlayerController
{
    public Transform shootGun;
    public Transform EffectShotGun;
    public NetworkObject passiveWhenKillVFX;
    public override void Spawned()
    { base.Spawned(); }

    public override void NormalAttack(NetworkObject VFXEffect, int levelDamage, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false, float timeTrigger = 0f,
        float TimeEffect = 0f)
    {

        base.NormalAttack(VFXEffect, levelDamage, isPhysicDamage, timeTrigger: timeTrigger);
        
        StartCoroutine(DelaySpawnAttack(VFXEffect, levelDamage, isPhysicDamage,
            isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect));
    }
    IEnumerator DelaySpawnAttack(NetworkObject VFXEffect, int levelDamage, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false, float timeTrigger = 0f,
        float TimeEffect = 0f)
    {
        yield return new WaitForSeconds(0.5f*100/playerStat.attackSpeed);
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
        SkillRPC
         (4, levelDamage, manaCost, isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect, levelSkill);

    }
    
    public override void Skill_2(NetworkObject VFXEffect, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        base.Skill_2(VFXEffect, levelDamage, manaCost, isPhysicDamage, timeTrigger: timeTrigger);
        StartCoroutine(DelaySpawnSkill_2(VFXEffect, levelDamage, isPhysicDamage,
            isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect, posMouseUp));
        DeActiveShootGunRPC();
    }
    IEnumerator DelaySpawnSkill_2(NetworkObject VFXEffect, int levelDamage, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        yield return new WaitForSeconds(0f);
        NetworkObject obj = Runner.Spawn(VFXEffect.gameObject, skill_2Transform.position, skill_2Transform.rotation, Object.InputAuthority,
            onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
            {
                obj.GetComponent<GrenadeController>().SetUp(this, levelDamage, isPhysicDamage, null,
                 isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect, posMouseUp);
                obj.GetComponent<GrenadeController>().SetDirection(transform.forward);
            });
        SetParentRPC(obj.Id);
    }
    public void PlayerThrowGrenade()
    {
        skill_2Transform.GetComponentInChildren<GrenadeController>().ThrowGrenade();
    }

    [Rpc(RpcSources.All, RpcTargets.All)] public void ActiveShootGunRPC()
    {
        shootGun.gameObject.SetActive(true);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void DeActiveShootGunRPC()
    {
        shootGun.gameObject.SetActive(false);
    }
    public override void Ultimate(NetworkObject VFXEffect, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        base.Ultimate(VFXEffect, levelDamage, manaCost, isPhysicDamage, timeTrigger: timeTrigger);
        StartCoroutine(DelaySpawnUltimate(VFXEffect, levelDamage, isPhysicDamage,
            isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect, posMouseUp));


    }
    IEnumerator DelaySpawnUltimate(NetworkObject VFXEffect, int levelDamage, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        yield return new WaitForSeconds(0.5f);
        NetworkObject obj = Runner.Spawn(VFXEffect.gameObject, skill_1Transform.position, skill_1Transform.rotation, Object.InputAuthority,
            onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
            {
                obj.GetComponent<AttackObjects>().SetUpPlayer(this, levelDamage, isPhysicDamage, null,
                 isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect);
                obj.GetComponent<AttackObjects>().SetDirection(transform.forward);
            });
    }
   public void PassiveWhenKill()
    {
        EffectShotGun.gameObject.SetActive(true);
        if (HasStateAuthority)
        {
            NetworkObject obj = Runner.Spawn(passiveWhenKillVFX, transform.position, Quaternion.identity, Object.InputAuthority,
            onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
            {
                obj.GetComponent<AttackObjects>().SetUpPlayer(this, 0, true, null,
             false, false, true, 15, 1);
                obj.GetComponent<BuffsOfPlayer>().levelSkill = 1;
            });
            SetParentRPC(obj.Id);
        }
    }
}

