using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionRegenSkill : NetworkBehaviour
{
    SkillName skillName;
    PlayerController player;
    private TickTimer timerToDestroy;
    float timerApplyRegen;
    TickTimer timerToApplyRegen;
    public float timerDestroy, timeEffect, timerApply;
    public int damage, levelSkill;
    public bool isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen, isDestroyWhenCollider;


    public override void Spawned()
    {
        base.Spawned();
        if (HasStateAuthority && HasInputAuthority)
        {
            timerToDestroy = TickTimer.CreateFromSeconds(Runner, timerDestroy);
            timerToApplyRegen = TickTimer.CreateFromSeconds(Runner, timerApply);
        }
    }
    public void SetUp(SkillName skillName, PlayerController player, float timerApplyDamage, int levelDamage, float timeTrigger,
        Transform parentObject = null
        )
    {
        this.skillName = skillName;
        this.player = player;
        this.timerApply = timerApplyDamage;
        transform.SetParent(parentObject);
        damage = levelDamage;
        timerDestroy = timeTrigger;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (!HasStateAuthority) return;
        if (timerToDestroy.Expired(Runner))
        {
            Destroy(gameObject);
        }
        if (timerToApplyRegen.Expired(Runner))
        {
            if (skillName == SkillName.HealPotion) player.playerStat.currentHealth += damage;
            else player.playerStat.currentMana += damage;
            timerToApplyRegen = TickTimer.CreateFromSeconds(Runner, timerApply);
        }
    }

}
