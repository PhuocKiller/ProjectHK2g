using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Dumbledore : PlayerController
{
    TickTimer timerSkill2;

    public override void Spawned()
    {base.Spawned();}

    public override void NormalAttack(NetworkObject VFXEffect, int levelDamage, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false, float timeTrigger = 0f,
        float TimeEffect = 0f)
    {
       
        base.NormalAttack(VFXEffect, levelDamage, isPhysicDamage, timeTrigger: timeTrigger);
        Runner.Spawn(VFXEffect.gameObject, normalAttackTransform.transform.position, normalAttackTransform.rotation, inputAuthority: Object.InputAuthority
     , onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
     {
        obj.GetComponent<AttackObjects>().SetUpPlayer(this, playerStat.damage, isPhysicDamage, null,
             isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect, true);
         obj.GetComponent<AttackObjects>().SetDirection(transform.forward);
     }
                        );
        StartCoroutine(DelaySpawnAttack(VFXEffect, levelDamage, isPhysicDamage, timeTrigger: timeTrigger));
    }
    IEnumerator DelaySpawnAttack(NetworkObject VFXEffect, int levelDamage, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false, float timeTrigger = 0f,
        float TimeEffect = 0f)
    {
        yield return new WaitForSeconds(0.5f);
    }
    public override void Skill_1(NetworkObject VFXEffect, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        base.Skill_1(VFXEffect, levelDamage, manaCost, isPhysicDamage, isMakeStun: isMakeStun,
            TimeEffect: TimeEffect,timeTrigger: timeTrigger);
        NetworkObject obj = Runner.Spawn(VFXEffect.gameObject, posMouseUp, skill_1Transform.rotation, Object.InputAuthority,
            onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
            {
                obj.GetComponent<AttackObjects>().SetUpPlayer(this,  levelDamage, isPhysicDamage,null,
                 isMakeStun, isMakeSlow, isMakeSilen,timeTrigger,TimeEffect,false,levelSkill);
                StartCoroutine(DelaySkill_1_Collider(obj));
            });
    }
    IEnumerator DelaySkill_1_Collider(NetworkObject obj)
    {
        yield return new WaitForSeconds(0.7f);
        obj.GetComponent<BoxCollider>().enabled = true;
    }
    public override void Skill_2(NetworkObject VFXEffect, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        base.Skill_2(VFXEffect, levelDamage, manaCost, isPhysicDamage, timeTrigger: timeTrigger);
        SkillRPC(3, levelDamage, manaCost, isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect,levelSkill);
    }
    public override void Ultimate(NetworkObject VFXEffect, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        base.Ultimate(VFXEffect, levelDamage, manaCost, isPhysicDamage, timeTrigger: timeTrigger);
        StartCoroutine(DelayUltimate(VFXEffect, levelDamage, isPhysicDamage,
            isMakeStun, isMakeSlow, isMakeSilen,timeTrigger, TimeEffect,posMouseUp));


    }
    IEnumerator DelayUltimate(NetworkObject VFXEffect, int levelDamage, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        yield return new WaitForSeconds(0f);
        NetworkObject obj = Runner.Spawn(VFXEffect.gameObject, posMouseUp, ultimateTransform.rotation, Object.InputAuthority,
            onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
            {
                obj.GetComponent<AttackObjects>().SetUpPlayer(this, levelDamage, isPhysicDamage, null,
                 isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect);
            });
        StartCoroutine(DelayActiveUltimateCollider(obj));
    }
    IEnumerator DelayActiveUltimateCollider(NetworkObject obj)
    {
        if (obj != null)
        {
            yield return new WaitForSeconds(0f);
            obj.GetComponent<BoxCollider>().enabled = true;
            StartCoroutine(DelayDeactiveUltimateCollider(obj));
        }
        
    }
    IEnumerator DelayDeactiveUltimateCollider(NetworkObject obj)
    {
        if (obj != null)
        {
            yield return new WaitForSeconds(0.8f);
            obj.GetComponent<BoxCollider>().enabled = false;
            StartCoroutine(DelayActiveUltimateCollider(obj));
        }
           
    }
}

