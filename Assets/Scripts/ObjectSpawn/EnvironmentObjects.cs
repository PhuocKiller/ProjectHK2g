using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentObjects : NetworkBehaviour
{
    PlayerController player;
    [Networked] public int playerTeam { get; set; }
    private List<Collider> collisions = new List<Collider>();
    private TickTimer timerToDestroy;
    TickTimer[] timerToApply;
    public float timerDestroy, timeEffect, timerApply;
    public int damage, levelSkill;
    public bool isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen, isDestroyWhenCollider;
    
    public override void Spawned()
    {
        base.Spawned();
        collisions.Clear();
        if (HasStateAuthority && HasInputAuthority)
        {
            timerToDestroy = TickTimer.CreateFromSeconds(Runner, timerDestroy);
            timerToApply= new TickTimer[10];
            for (int i = 0;i< timerToApply.Length;i++)
            {
                timerToApply[i] = TickTimer.CreateFromSeconds(Runner, timerApply);
            }
        }
    }
    public void SetUp(PlayerController player, int playerTeam, float timerApplyDamage, int levelDamage, bool isPhysicDamage, Transform parentObject = null,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false, float timeTrigger = 0f,
        float timeEffect = 0f, bool isDestroyWhenCollider = false, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        this.player = player;
        this.playerTeam = playerTeam;
        this.timerApply = timerApplyDamage;
        transform.SetParent(parentObject);
        damage = levelDamage;
        this.isPhysicDamage = isPhysicDamage;
        this.isMakeStun = isMakeStun;
        this.isMakeSlow = isMakeSlow;
        this.isMakeSilen = isMakeSilen;
        this.timeEffect = timeEffect;
        this.isDestroyWhenCollider = isDestroyWhenCollider;
        timerDestroy = timeTrigger;
        this.levelSkill = levelSkill;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (HasStateAuthority && timerToDestroy.Expired(Runner))
        {
            if (player.playerType==Player_Types.Viking)
            {
                player.playerStat.isUnstopAble=false;
            }
            Destroy(gameObject);
        }

    }

    private void OnTriggerStay(Collider otherColi)
    {
        if (HasStateAuthority && otherColi.gameObject.layer == 7 ) //Nếu layer là player
        {
            if (!collisions.Contains(otherColi))
            {
                collisions.Add(otherColi);
            }
            int index = -1;
            foreach (var other in collisions)
            {
                index++;
                if (playerTeam != other.gameObject.GetComponent<PlayerController>().playerTeam) //khác team
                {
                    if (other.gameObject.GetComponent<PlayerController>().state != 3
                        && timerToApply[index].Expired(Runner))
                    {
                        other.gameObject.GetComponent<ICanTakeDamage>().ApplyDamage(damage, isPhysicDamage, player,
                    activeInjureAnim: false,
                    counter: (int counterDamage, bool isPhysicDamage) =>
                    {
                        player.ApplyDamage(counterDamage, isPhysicDamage,player);
                    }
                    , isKillPlayer: (int levelHeroKilled, List<PlayerController> playerMakeDamage) => // Nhận exp khi giêt địch ở đây
                    {
                        player.playerStat.currentXP += (int)(100 * Mathf.Lerp(1 / playerMakeDamage.Count, 1, 0.5f) * levelHeroKilled);
                        player.playerScore.killScore += 1;
                        player.playerScore.assistScore -= 1;
                    }
                    , lifeSteal: (int damage) =>
                    {
                        if (player.playerStat.isLifeSteal) player.playerStat.currentHealth += (int)(player.playerStat.lifeSteal * damage);
                    }
                    );
                        timerToApply[index] = TickTimer.CreateFromSeconds(Runner, timerApply);
                    }

                }
                else  //cùng team
                {
                    if (other.gameObject.GetComponent<PlayerController>().state != 3
                        && timerToApply[index].Expired(Runner) &&player.playerType==Player_Types.Sagittarius)
                    {

                        timerToApply[index] = TickTimer.CreateFromSeconds(Runner, timerApply);
                        other.gameObject.GetComponent<PlayerController>().ApplyRegenHealth(damage);
                    }
                }
            } 
        }
    }
    void OnTriggerExit(Collider other)
    {
        // Nếu đối tượng rời khỏi trigger, xóa khỏi danh sách
        if (collisions.Contains(other))
        {
            collisions.Remove(other);
        }
    }
}
