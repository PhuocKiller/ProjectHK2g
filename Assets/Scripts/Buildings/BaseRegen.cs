using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRegen : NetworkBehaviour
{
    [Networked] public int playerTeam { get; set; }
    public List<Collider> collisions = new List<Collider>();
    private TickTimer timerToDestroy;
    TickTimer[] timerToApply { get; set; }
    [Networked] public float timerApply { get; set; }

    public override void Spawned()
    {
        base.Spawned();
        collisions.Clear();
            timerToApply = new TickTimer[10];
            for (int i = 0; i < timerToApply.Length; i++)
            {
                timerToApply[i] = TickTimer.CreateFromSeconds(Runner, timerApply);
            }
    }
    public void SetUp(int playerTeam, float timerApplyDamage)
    {
        this.playerTeam = playerTeam;
        this.timerApply = timerApplyDamage;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
    }

    private void OnTriggerStay(Collider otherColi)
    {
        if (HasStateAuthority && otherColi.gameObject.layer == 7) //Nếu layer là player
        {
            if (!collisions.Contains(otherColi))
            {
                collisions.Add(otherColi);
            }
            int index = -1;
            foreach (var other in collisions)
            {
                PlayerController otherPlayer = other.GetComponent<PlayerController>();
                index++;
                if (playerTeam != otherPlayer.playerTeam) //khác team
                {
                    if (otherPlayer.state != 3
                        && timerToApply[index].Expired(Runner))
                    {
                        other.gameObject.GetComponent<ICanTakeDamage>().ApplyDamage(20, true, null,
                    activeInjureAnim: false);
                        timerToApply[index] = TickTimer.CreateFromSeconds(Runner, timerApply);
                    }
                }
                else  //cùng team
                {
                    if (otherPlayer.state != 3)
                    {
                        if(timerToApply[index].Expired(Runner))
                        {
                            timerToApply[index] = TickTimer.CreateFromSeconds(Runner, timerApply);
                            otherPlayer.ApplyRegenHealth((int)(otherPlayer.playerStat.maxHealth * 0.005f));
                            otherPlayer.ApplyRegenMana((int)(otherPlayer.playerStat.maxMana * 0.015f));
                        }
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
