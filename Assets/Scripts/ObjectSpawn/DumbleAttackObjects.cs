using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbleAttackObjects : NetworkBehaviour
{
    PlayerController player;
    CalculateTriggerEnter trigger;
    private List<Collider> collisions = new List<Collider>();
    private TickTimer timer;
    public float timerDespawn, timeEffect;
    public int damage, levelSkill;
    public bool isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen, isDestroyWhenCollider;
    public override void Spawned()
    {
        base.Spawned();
        collisions.Clear();
        trigger = GetComponent<CalculateTriggerEnter>();
        if (HasStateAuthority && HasInputAuthority)
        {
            timer = TickTimer.CreateFromSeconds(Runner, timerDespawn);
            player.playerStat.isCounter = true;
        }
    }
    public void SetUp(PlayerController player, int levelDamage, bool isPhysicDamage, Transform parentObject = null,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false, float timeTrigger = 0f,
        float timeEffect = 0f, bool isDestroyWhenCollider = false, int levelSkill = 1)
    {
        this.player = player;
        transform.SetParent(parentObject);
        damage = levelDamage;
        player.playerStat.counterDamage = damage;
        this.isPhysicDamage = isPhysicDamage;
        this.isMakeStun = isMakeStun;
        this.isMakeSlow = isMakeSlow;
        this.isMakeSilen = isMakeSilen;
        this.timeEffect = timeEffect;
        this.isDestroyWhenCollider = isDestroyWhenCollider;
        timerDespawn = timeTrigger;
        this.levelSkill = levelSkill;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (HasStateAuthority && timer.Expired(Runner)
            )
        {
            player.playerStat.isCounter = false;
            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(HasStateAuthority)
        {
            if (other.gameObject.layer == 7 &&  !collisions.Contains(other)
            && other.gameObject.GetComponent<NetworkObject>().HasStateAuthority == false
            && other.gameObject.GetComponent<PlayerController>().state != 3
            && other.gameObject.GetComponent<PlayerController>().playerTeam != player.playerTeam)
            {
                trigger.ControlTriggerPlayer(other, collisions, player, damage, timeEffect, isPhysicDamage,
                isMakeStun, isMakeSlow, isMakeSilen, isDestroyWhenCollider, Object.InputAuthority, levelSkill);
            }
            if (other.gameObject.layer == 8 && !collisions.Contains(other)
            && other.gameObject.GetComponent<CreepController>().state != 3
            && other.gameObject.GetComponent<CreepController>().playerTeam != player.playerTeam)
            {
                trigger.ControlTriggerPlayer(other, collisions, player, damage, timeEffect, isPhysicDamage,
                isMakeStun, isMakeSlow, isMakeSilen, isDestroyWhenCollider, Object.InputAuthority);
                if (player.playerType == Player_Types.DumbleDore) collisions.Clear();
            }
        }
    }
}
