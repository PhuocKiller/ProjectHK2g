using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectSphere : NetworkBehaviour
{
    BuildingController tower;

    public override void Spawned()
    {
        base.Spawned();
        tower=GetComponentInParent<BuildingController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            if(player.playerTeam!=tower.playerTeam)
            {
                TowerDetectPlayer(player, true);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            if (player.playerTeam != tower.playerTeam)
            {
                TowerDetectPlayer(player, false);
            }
        }
    }
    void TowerDetectPlayer(PlayerController player, bool isDetect)
    {
        TowerDetectPlayerRPC(player, isDetect);
    }
    [Rpc(RpcSources.All, RpcTargets.All)] void TowerDetectPlayerRPC(PlayerController player, bool isDetect)
    {
        player.playerStat.isUnderTower = isDetect;
    }
}
