using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ryan : PlayerController
{
    TickTimer timerSkill2;
    [SerializeField] public Transform effectSkill2;
    [Networked] public int attackTimes { get;set; }
    public override void Spawned()
    {
        base.Spawned();
    }

    public override void NormalAttack(NetworkObject VFXEffect, int levelDamage, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false, float timeTrigger = 0f,
        float TimeEffect = 0f)
    { //ryan ko xài base
        state = 4;
        if (effectSkill2.gameObject.activeInHierarchy)
        {
            AnimatorTriggerRPC("SpecialAttack");
            StartCoroutine(ActiveKatana(false,0.5f*100/playerStat.attackSpeed));
            attackTimes = 0;
            StartCoroutine(DelaySpawnSpecialAttack(VFXEffect, levelDamage, isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen,
          timeTrigger, TimeEffect));
        }
        else
        {
            attackTimes++;
            StartCoroutine(DelaySpawnAttack(VFXEffect, playerStat.damage, isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen,
          timeTrigger, TimeEffect));
            if (attackTimes==3)
            {
                NetworkObject obj = Runner.Spawn(networkObjs.listNetworkObj[7], transform.position, Quaternion.identity, Object.InputAuthority,
            onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
            {
                obj.GetComponent<RyanAttackObjects>().SetUp(this, levelDamage, isPhysicDamage, null,
             isMakeStun, isMakeSlow, isMakeSilen, 15, TimeEffect);
                obj.GetComponent<BuffsOfPlayer>().levelSkill = 1;
            });
                SetParentRPC(obj.Id);
            }
            AnimatorTriggerRPC("Attack");
        }
    }
    IEnumerator DelaySpawnAttack(NetworkObject VFXEffect, int levelDamage, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false, float timeTrigger = 0f,
        float TimeEffect = 0f)
    {
        yield return new WaitForSeconds(0.5f * 100f / playerStat.attackSpeed);
        Runner.Spawn(VFXEffect.gameObject, normalAttackTransform.transform.position, normalAttackTransform.rotation, inputAuthority: Object.InputAuthority
     , onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
     {
         obj.GetComponent<AttackObjects>().SetUpPlayer(this, levelDamage, isPhysicDamage, normalAttackTransform,
             isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect);
     });
    }
    IEnumerator DelaySpawnSpecialAttack(NetworkObject VFXEffect, int levelDamage, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false, float timeTrigger = 0f,
        float TimeEffect = 0f)
    {
        yield return new WaitForSeconds(0.45f * 100f / playerStat.attackSpeed);
        Runner.Spawn(networkObjs.listNetworkObj[8], normalAttackTransform.transform.position, normalAttackTransform.rotation, inputAuthority: Object.InputAuthority
     , onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
     {
         obj.GetComponent<AttackObjects>().SetUpPlayer(this, playerStat.damage, isPhysicDamage, normalAttackTransform,
             true, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect);
     });
        foreach (var buffkatana in FindObjectsOfType<RyanAttackObjects>())
        {
            Destroy(buffkatana.gameObject);
        }
    }

    public override void Skill_1(NetworkObject VFXEffect, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        base.Skill_1(VFXEffect, levelDamage, manaCost, isPhysicDamage, timeTrigger: timeTrigger);
        NetworkObject obj = Runner.Spawn(VFXEffect.gameObject, posMouseUp, Quaternion.identity, Object.InputAuthority,
            onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
            {
                obj.GetComponent<EnvironmentObjects>().SetUp(this, playerTeam, 0.05f, levelDamage, isPhysicDamage, null,
                 isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect);
                obj.GetComponent<BuffsOfPlayer>().levelSkill = levelSkill;
            });
        characterControllerPrototype.Move((Vector3)posMouseUp- transform.position);
    }
    
    public override void Skill_2(NetworkObject VFXEffect, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        base.Skill_2(VFXEffect, levelDamage, manaCost, isPhysicDamage, timeTrigger: timeTrigger);
        NetworkObject obj = Runner.Spawn(VFXEffect.gameObject, transform.position, Quaternion.identity, Object.InputAuthority,
            onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
            {
                obj.GetComponent<RyanAttackObjects>().SetUp(this, levelDamage, isPhysicDamage, null,
             isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect);
                obj.GetComponent<BuffsOfPlayer>().levelSkill = levelSkill;
            });
        SetParentRPC(obj.Id);
    }
    public IEnumerator ActiveKatana(bool isActive, float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay);
        ActiveKatanaRPC(isActive);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void ActiveKatanaRPC(bool isActive)
    {
        effectSkill2.gameObject.SetActive(isActive);
    }
    

    public override void Ultimate(NetworkObject VFXEffect, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        base.Ultimate(VFXEffect, levelDamage, manaCost, isPhysicDamage, timeTrigger: timeTrigger);
        StartCoroutine(DelayUltimate(VFXEffect, levelDamage, isPhysicDamage,
            isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect, posMouseUp, levelSkill));
        playerStat.isStartFadeInvi = true;
        NetworkObject obj = Runner.Spawn(VFXEffect.gameObject, transform.position, Quaternion.identity, Object.InputAuthority,
            onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
            {
                obj.GetComponent<InviObjects>().SetUp(this, playerStat.damage, isPhysicDamage, null,
             isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect);
                obj.GetComponent<BuffsOfPlayer>().levelSkill = levelSkill;
            });
        SetParentRPC(obj.Id);
    }
    IEnumerator DelayUltimate(NetworkObject VFXEffect, int levelDamage, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        yield return new WaitForSeconds(0f);
        
    }
}

