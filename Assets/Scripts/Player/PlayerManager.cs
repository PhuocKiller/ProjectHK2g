using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : NetworkBehaviour
{
    NetworkRunner myLocalRunner;
    PlayerController player;
    PlayerController[] playerControllers= new PlayerController[6];
    
    public NetworkRunner GetNetworkRunner() { return this.myLocalRunner; }
    public PlayerController GetPlayer() { return player; }
    public void SetRunner(NetworkRunner runner)
    {
        this.myLocalRunner = runner;
    }
    public void SetPLayer(PlayerController player)
    {
        this.player = player;
    }
    private void Update()
    {
        
    }

    public override void Spawned()
    {
        base.Spawned();
        
    }
    private void Start()
    {
      
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
    }

    public void CheckPlayer(out int? state, out PlayerController player)
    {
        playerControllers= FindObjectsOfType<PlayerController>();
        foreach(var playerController in playerControllers)
        {
            
            if(playerController.Object.InputAuthority.PlayerId== playerController.Runner.LocalPlayer)
            {
                state = playerController.state;
                player = playerController;
                return;
            }
            
        }
        state = null;
        player = null;
    }
    
}
