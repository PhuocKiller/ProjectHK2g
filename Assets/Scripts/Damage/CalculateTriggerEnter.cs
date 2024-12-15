using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateTriggerEnter : MonoBehaviour
{
    public bool isCritPhysic;
    public void ControlTriggerPlayer(Collider other, List<Collider> collisions, PlayerController player, int damage,float timeEffect,
        bool isPhysicDamage, bool isMakeStun, bool isMakeSlow, bool isMakeSilen, bool isDestroyWhenCollider,PlayerRef InputAuthority,int levelSkill=1)
    {
            collisions.Add(other);
            int calculateDamage = Singleton<MechanicDamage>.Instance.GetDamageOfTwoObject(damage, isPhysicDamage, player, other, out bool isCritPhysic);
            other.gameObject.GetComponent<ICanTakeDamage>().ApplyDamage(calculateDamage, isPhysicDamage, player,
                counter: (int counterDamage, bool isPhysicDamage) =>
                {
                    player.ApplyDamage(counterDamage, isPhysicDamage,
                         other.gameObject.GetComponent<PlayerController>());
                }
                , isKillPlayer: (int levelHeroKilled, List<PlayerController> playerMakeDamage) => // Nhận exp khi giêt địch ở đây
                {
                    player.playerStat.currentXP += (int)(100 * Mathf.Lerp(1 / playerMakeDamage.Count, 1, 0.5f) * levelHeroKilled);
                    player.playerScore.killScore += 1;
                    player.playerScore.assistScore -= 1;
                    if (player.playerType==Player_Types.DumbleDore) player.playerStat.currentMana += (int)(player.playerStat.maxMana * 0.2 * levelSkill);
                }
                , isKillCreep: (Vector3 posSpawn, float luckyChance) =>
                {
                    NetworkManager networkManager = FindObjectOfType<NetworkManager>();
                    for (int i = 0;i< networkManager.basicItems.Length;i++)
                    {
                       if(FindObjectOfType<MechanicDamage>().GetChance(luckyChance*networkManager.itemsDropChance[i]))
                        {
                            networkManager.SpawnItemFromCreep(i, posSpawn);
                        }
                    }
                }
                , lifeSteal: (int damage) =>
                {
                    if (player.playerStat.isLifeSteal) player.playerStat.currentHealth += (int)(player.playerStat.lifeSteal * damage);
                }, isCritPhysic: isCritPhysic);

        other.gameObject.GetComponent<ICanTakeDamage>().ApplyEffect(InputAuthority, isMakeStun, isMakeSlow, isMakeSilen,
                TimeEffect: timeEffect, callback: () =>
                {
                    if (isDestroyWhenCollider) Destroy(gameObject);//khi chạm vào địch thì hủy vật thể
                }
                );
        this.isCritPhysic = isCritPhysic;
    }
    public void ControlTriggerCreep(Collider other, List<Collider> collisions,CreepController creep, int damage, float timeEffect,
       bool isPhysicDamage, bool isMakeStun, bool isMakeSlow, bool isMakeSilen, bool isDestroyWhenCollider, PlayerRef InputAuthority, int levelSkill = 1)
    {
        collisions.Add(other);
        other.gameObject.GetComponent<ICanTakeDamage>().ApplyDamage
            (Singleton<MechanicDamage>.Instance.GetDamageOfTwoObject(damage, true, null, other, out bool isCritPhysic),isPhysicDamage, null, activeInjureAnim:false,
            counter: (int counterDamage, bool isPhysicDamage) =>
            {
                creep.ApplyDamage(counterDamage, isPhysicDamage,null);
            }
            , isKillPlayer: (int levelHeroKilled, List<PlayerController> playerMakeDamage) => // Nhận exp khi giêt địch ở đây
            {
               
            }
            , lifeSteal: (int damage) =>
            {
                if (creep.playerStat.isLifeSteal) creep.playerStat.currentHealth += (int)(creep.playerStat.lifeSteal * damage);
            }
            );
       
            other.gameObject.GetComponent<ICanTakeDamage>().ApplyEffect(InputAuthority, isMakeStun, isMakeSlow, isMakeSilen,
            TimeEffect: timeEffect, callback: () =>
            {
                if (isDestroyWhenCollider) Destroy(gameObject);//khi chạm vào địch thì hủy vật thể
            }
            );
    }
    public void ControlTriggerTower(Collider other, List<Collider> collisions, BuildingController tower, int damage, 
       bool isPhysicDamage, bool isDestroyWhenCollider, PlayerRef InputAuthority)
    {
        collisions.Add(other);
        other.gameObject.GetComponent<ICanTakeDamage>().ApplyDamage
            (Singleton<MechanicDamage>.Instance.GetDamageOfTwoObject
            (damage, true, null, other, out bool isCritPhysic), isPhysicDamage, null, activeInjureAnim: false);
        other.gameObject.GetComponent<ICanTakeDamage>().ApplyEffect(InputAuthority, false, false, false,
            TimeEffect:0, callback: () =>
            {
               Destroy(gameObject);//khi chạm vào địch thì hủy vật thể
            }
            );
    }
}
