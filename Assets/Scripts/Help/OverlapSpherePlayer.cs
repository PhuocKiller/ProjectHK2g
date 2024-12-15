using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class OverlapSpherePlayer : NetworkBehaviour
{
    PlayerController player;
    public PlayerController closestEnemyPlayer;
    RectTransform crossHairFollow, crossHairUnFollow;
    public override void Spawned()
    {
        base.Spawned();
        player=GetComponentInParent<PlayerController>();
        crossHairFollow = FindObjectOfType<UIManager>().crossHairFollow;
        crossHairUnFollow = FindObjectOfType<UIManager>().crossHairUnFollow;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if(HasStateAuthority)
        {
            
            crossHairFollow.gameObject.SetActive(CheckPlayerAround().Count >0&& player.playerStat.isFollowEnemy);
            crossHairUnFollow.gameObject.SetActive(CheckPlayerAround().Count >0&& !player.playerStat.isFollowEnemy);
            //hiện hình crossHair
            if (CheckPlayerAround().Count > 0)
            {
                closestEnemyPlayer = FindClosestObjectInRadius(CheckPlayerAround(), transform.position);
                Vector3 posScreenPoint = Camera.main.WorldToScreenPoint(closestEnemyPlayer.transform.position+Vector3.up*2);
                crossHairFollow.position = posScreenPoint;
                crossHairUnFollow.position = posScreenPoint;
            }
            else 
            { 
                closestEnemyPlayer = null;
                player.playerStat.isFollowEnemy = false;
            }
        }
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
                if (enemyPlayer.playerTeam != player.playerTeam && !enemyPlayers.Contains(enemyPlayer)
                    && enemyPlayer.state != 3 && (enemyPlayer.playerStat.isVisible|| enemyPlayer.playerStat.isUnderTower))
                {
                    enemyPlayers.Add(enemyPlayer);
                }
            }
        }
        return enemyPlayers;
    }
    PlayerController FindClosestObjectInRadius(List<PlayerController> enemyPlayers, Vector3 currentPos)
    {
        return enemyPlayers
            .OrderBy(player => Vector3.Distance(player.transform.position, currentPos)).FirstOrDefault(); 
    }
}
