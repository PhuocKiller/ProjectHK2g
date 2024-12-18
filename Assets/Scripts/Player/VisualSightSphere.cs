using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VisualSightSphere : NetworkBehaviour
{
    public PlayerController player;
    public CreepController creep;
    public BuildingController tower;
    [Networked] public int mainTeam { get; set; }
    private void Awake()
    {
        
    }

    public override void Spawned()
    {
        base.Spawned();
        player = GetComponentInParent<PlayerController>();
        creep = GetComponentInParent<CreepController>();
        tower = GetComponentInParent<BuildingController>();
        mainTeam = FindObjectOfType<NetworkManager>().playerTeam;
        if ((player != null ? player.playerTeam : (creep != null ? creep.playerTeam : tower.playerTeam)) != mainTeam)
        {
            CanSeeObjectInSight(false);
        }
    }
    void CanSeeObjectInSight(bool active)
    {
        /*MeshRenderer[] allmeshs = player != null ? player.GetComponentsInChildren<MeshRenderer>()
            : (creep != null ? creep.GetComponentsInChildren<MeshRenderer>() : tower.GetComponentsInChildren<MeshRenderer>());
        if (allmeshs.Length > 0)
        {
            foreach (var mesh in allmeshs)
            {
                mesh.enabled = active;
            }
        }
        SkinnedMeshRenderer[] skinMeshs = player != null ? player.GetComponentsInChildren<SkinnedMeshRenderer>()
            : (creep != null ? creep.GetComponentsInChildren<SkinnedMeshRenderer>() : tower.GetComponentsInChildren<SkinnedMeshRenderer>());
        if (skinMeshs.Length > 0)
        {
            foreach (var skinMesh in skinMeshs)
            {
                skinMesh.enabled = active;
            }
        }*/
        if (player) player.statusCanvas.GetComponent<InviManager>().VisualOfPlayer(active);
        else if (creep) creep.statusCanvas.GetComponent<InviManager>().VisualOfPlayer(active);
        else tower.GetComponent<InviManager>().VisualOfPlayer(active);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (player)
        {
            if (player.playerTeam != mainTeam) return;
            CheckOtherColider(other, true);
        }
        else if (creep)
        {
            if (creep.playerTeam != mainTeam) return;
            CheckOtherColider(other, true);
        }
        else if (tower)
        {
            if (tower.playerTeam != mainTeam) return;
            CheckOtherColider(other, true);
        }
    }
    void CheckOtherColider(Collider other, bool isActive)
    {

        PlayerController otherPlayer = other.GetComponent<PlayerController>();
        if (otherPlayer)
        {
            if(otherPlayer.playerTeam== mainTeam) return;
            otherPlayer.playerStat.ChangeBoolIsInSight(isActive);
        }
        CreepController otherCreep = other.GetComponent<CreepController>();
        if (otherCreep)
        {
            if (otherCreep.playerTeam == mainTeam) return;
            otherCreep.playerStat.ChangeBoolIsInSight(isActive);
        }
        BuildingController otherBuilding = other.GetComponent<BuildingController>();
        if (otherBuilding)
        {
            if (otherBuilding.playerTeam == mainTeam) return;
            otherBuilding.ChangeBoolIsInSight(isActive);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (player)
        {
            if (player.playerTeam != mainTeam) return;
            CheckOtherColider(other, false);
        }
        else if (creep)
        {
            if (creep.playerTeam != mainTeam) return;
            CheckOtherColider(other, false);
        }
        else if (tower)
        {
            if (tower.playerTeam != mainTeam) return;
            CheckOtherColider(other, false);
        }
    }
}
