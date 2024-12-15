using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkNight : PlayerController
{
    public override void Spawned()
    {
        base.Spawned();
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
        yield return new WaitForSeconds(0.5f*100f / playerStat.attackSpeed);
        Runner.Spawn(VFXEffect.gameObject, normalAttackTransform.transform.position, normalAttackTransform.rotation, inputAuthority: Object.InputAuthority
     , onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
     {
         obj.GetComponent<DarkNightAttackObjects>().SetUp(this,"NormalAttack", playerStat.damage, isPhysicDamage, normalAttackTransform,
             isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect);
     });
    }
   
    public override void Skill_1(NetworkObject VFXEffect, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        base.Skill_1(VFXEffect, levelDamage, manaCost, isPhysicDamage, timeTrigger: timeTrigger);
        NetworkObject obj = Runner.Spawn(VFXEffect.gameObject, skill_1Transform.position, skill_1Transform.rotation, Object.InputAuthority,
            onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
            {
                obj.GetComponent<AttackObjects>().SetUpPlayer(this, levelDamage, isPhysicDamage, skill_1Transform,
             isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect);
                StartCoroutine(DelaySkill_1_Collider(obj));
            });
    }
    IEnumerator DelaySkill_1_Collider(NetworkObject obj)
    {
        yield return new WaitForSeconds(0.5f);
        obj.GetComponent<CapsuleCollider>().enabled = true;
    }
    public override void Skill_2(NetworkObject VFXEffect, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        base.Skill_2(VFXEffect, levelDamage, manaCost, isPhysicDamage,timeTrigger: timeTrigger);
        SkillRPC(2, levelDamage, manaCost, isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect);
    }
    public override void Ultimate(NetworkObject VFXEffect, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        base.Ultimate(VFXEffect, levelDamage, manaCost, isPhysicDamage, timeTrigger: timeTrigger);
        StartCoroutine(DelayUltimate(VFXEffect, levelDamage, isPhysicDamage,
            isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect, levelSkill));

        
    }
    IEnumerator DelayUltimate(NetworkObject VFXEffect, int levelDamage, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false, float timeTrigger = 0f, float TimeEffect = 0f, int levelSkill = 1)
    {
        yield return new WaitForSeconds(0.7f);
        NetworkObject obj = Runner.Spawn(VFXEffect.gameObject, ultimateTransform.position, ultimateTransform.rotation, Object.InputAuthority,
            onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
            {
                obj.GetComponent<DarkNightAttackObjects>().SetUp(this,"Ultimate", levelDamage, isPhysicDamage, ultimateTransform,
             isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect,false, levelSkill);
            });
        StartCoroutine(DelayUltimateCollider(obj));
    }
    IEnumerator DelayUltimateCollider(NetworkObject obj)
    {
        yield return new WaitForSeconds(0.7f);
        obj.GetComponent<SphereCollider>().enabled = true;
    }
}

