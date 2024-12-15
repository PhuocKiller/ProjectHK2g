using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkNightAttackObjects: NetworkBehaviour
{
    PlayerController player;
    CalculateTriggerEnter trigger;
    private Vector3 direction;
    private List<Collider> collisions = new List<Collider>();
    private TickTimer timer;
    public float timerDespawn, timeEffect;
    public int damage, levelSkill;
    string skillName;
    public bool isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen, isDestroyWhenCollider;
    public override void Spawned()
    {
        base.Spawned();
        collisions.Clear();
        trigger = GetComponent<CalculateTriggerEnter>();
        if (HasStateAuthority && HasInputAuthority)
        {
            timer = TickTimer.CreateFromSeconds(Runner, timerDespawn);
                   }
    }
    public void SetUp(PlayerController player,string skillName, int levelDamage, bool isPhysicDamage, Transform parentObject = null,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false, float timeTrigger = 0f,
        float timeEffect = 0f, bool isDestroyWhenCollider = false, int levelSkill=1)
    {
        this.player = player;
        this.skillName = skillName;
        transform.SetParent(parentObject);
        damage = levelDamage;
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
        if (HasStateAuthority && timer.Expired(Runner))
        {
            Destroy(gameObject);
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (HasStateAuthority)
        {
            if (other.gameObject.layer == 7 && !collisions.Contains(other)
            && other.gameObject.GetComponent<NetworkObject>().HasStateAuthority == false
            && other.gameObject.GetComponent<PlayerController>().state != 3
            && other.gameObject.GetComponent<PlayerController>().playerTeam != player.playerTeam)
            {
                trigger.ControlTriggerPlayer(other, collisions, player,
                CalculateAttackDamage(other.gameObject.GetComponent<PlayerController>()), timeEffect, isPhysicDamage,
                isMakeStun, isMakeSlow, isMakeSilen, isDestroyWhenCollider, Object.InputAuthority);
            }
            if (other.gameObject.layer == 8 && !collisions.Contains(other)
            && other.gameObject.GetComponent<CreepController>().state != 3
            && other.gameObject.GetComponent<CreepController>().playerTeam != player.playerTeam)
            {
                trigger.ControlTriggerPlayer(other, collisions, player, damage, timeEffect, isPhysicDamage,
                isMakeStun, isMakeSlow, isMakeSilen, isDestroyWhenCollider, Object.InputAuthority);
                if (player.playerType == Player_Types.DumbleDore) collisions.Clear();
            }

            if (other.gameObject.layer == 9 && collisions.Count == 0
            && other.gameObject.GetComponent<BuildingController>().state != 3
            && other.gameObject.GetComponent<BuildingController>().playerTeam != player.playerTeam
            && skillName == "NormalAttack")
            {
                trigger.ControlTriggerPlayer(other, collisions, player, damage, 0, true,
                false, false, false, isDestroyWhenCollider, Object.InputAuthority);
            }
        }
    }
    
    int CalculateAttackDamage(PlayerController playerBeingAttack)
    {
        if (skillName=="NormalAttack")
        {
            return damage + (int)(playerBeingAttack.playerStat.currentHealth * 0.015 * ((int)(player.playerStat.level / 3) + 1));
        }
        else if (skillName =="Ultimate")
        {
            return damage + (int)((playerBeingAttack.playerStat.maxHealth- playerBeingAttack.playerStat.currentHealth) * 0.25
                * (int)(player.playerStat.level / 3) );
        }
        else { return 0; }
        
    }
    
}
