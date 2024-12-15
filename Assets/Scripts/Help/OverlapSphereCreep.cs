using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class OverlapSphereCreep : NetworkBehaviour
{
    CreepController creep;
    public CharacterController closestCharac;
    
    
    public override void Spawned()
    {
        base.Spawned();
        creep = GetComponentInParent<CreepController>();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
       
    }
    public List<CharacterController> CheckAllEnemyAround(float range)
    {   List<CharacterController> allEnemies = new List<CharacterController>();
        allEnemies.Clear();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);
        
        foreach (var hitCollider in hitColliders)
        {
            PlayerController enemyPlayer = hitCollider.gameObject.GetComponent<PlayerController>();
            if(enemyPlayer != null && enemyPlayer.GetComponent<NetworkObject>().IsValid)
            {
                CharacterController characPlayer = enemyPlayer.gameObject.GetComponent<CharacterController>();
                if (enemyPlayer.playerTeam != creep.playerTeam && enemyPlayer.state!=3 && !allEnemies.Contains(characPlayer)
                    && (enemyPlayer.playerStat.isVisible|| enemyPlayer.playerStat.isUnderTower))
                {
                    allEnemies.Add(characPlayer);
                }
            }
            CreepController enemyCreep = hitCollider.gameObject.GetComponent<CreepController>();
            if (enemyCreep != null && enemyCreep.GetComponent<NetworkObject>().IsValid)
            {
                CharacterController characCreep = enemyCreep.gameObject.GetComponent<CharacterController>();
                if (enemyCreep.playerTeam != creep.playerTeam && enemyCreep.state != 3 && !allEnemies.Contains(characCreep))
                {
                    allEnemies.Add(characCreep);
                }
            }
            BuildingController enemyTower = hitCollider.gameObject.GetComponent<BuildingController>();
            if (enemyTower != null && enemyTower.GetComponent<NetworkObject>().IsValid)
            {
                CharacterController characTower = enemyTower.gameObject.GetComponent<CharacterController>();
                if (enemyTower.playerTeam != creep.playerTeam && enemyTower.state != 3 && !allEnemies.Contains(characTower))
                {
                    allEnemies.Add(characTower);
                }
            }
        }
        
        return allEnemies;
    }
    public List<PlayerController> CheckPlayerFollowEnemy(List<CharacterController> checkAllEnemyAround)
    {
        List<PlayerController> listPlayerFollowEnemy = new List<PlayerController>();
        foreach (var enemy in checkAllEnemyAround)
        {
            if(enemy.GetComponent<PlayerController>() != null)
            {
                if(enemy.GetComponent<PlayerController>().playerStat.isFollowEnemy)
                {
                    listPlayerFollowEnemy.Add(enemy.GetComponent<PlayerController>());  
                }
            }
        }
        return listPlayerFollowEnemy;
    }
    public List<PlayerController> CheckPlayerAround()
    {
       List<PlayerController> enemyPlayers = new List<PlayerController>();
        enemyPlayers.Clear();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 25f);

        foreach (var hitCollider in hitColliders)
        {
            PlayerController enemyPlayer = hitCollider.gameObject.GetComponent<PlayerController>();

            if (enemyPlayer != null && enemyPlayer.GetComponent<NetworkObject>().IsValid)
            {
                if (enemyPlayer.playerTeam != creep.playerTeam && !enemyPlayers.Contains(enemyPlayer)
                    && enemyPlayer.state!=3)
                {
                    enemyPlayers.Add(enemyPlayer);
                }
            }
        }
        return enemyPlayers;
    }
    public CharacterController FindClosestCharacterInRadius(List<CharacterController> enemyCharac, Vector3 currentPos)
    {
        return enemyCharac
            .OrderBy(charac => Vector3.Distance(charac.transform.position, currentPos)).FirstOrDefault();
    }
    public PlayerController FindClosestPlayerFollowInRadius(List<PlayerController> playerFollow, Vector3 currentPos)
    {
        return playerFollow
            .OrderBy(player => Vector3.Distance(player.transform.position, currentPos)).FirstOrDefault();
    }
    
}
