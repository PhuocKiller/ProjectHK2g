﻿using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class InviManager : NetworkBehaviour
{
    public GameObject[] visuals;
    public PlayerController player;
    public CreepController creep;
    public BuildingController tower;
    public SkinnedMeshRenderer[] skinnedMeshRenderers;
    public MeshRenderer[] meshRenderers;
    public override void Spawned()
    {
        base.Spawned();
        player = GetComponentInParent<PlayerController>();
        creep = GetComponentInParent<CreepController>();
        tower = GetComponent<BuildingController>();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (player)
        {
            if (player.playerStat.isLive)
            {
                // CheckInviVisual(player.playerStat.isVisible);
                //ControlInvi();
                if (player.playerStat.isStartFadeInvi)
                {
                    for (int i = 0; i < meshRenderers.Length; i++)
                    {
                        ControlMaterial(3, meshRenderers[i].material, meshRenderers[i].material.color.a - 0.4f * Runner.DeltaTime, 3000);
                    }
                    for (int i = 0; i < skinnedMeshRenderers.Length; i++)
                    {
                        ControlMaterial(3, skinnedMeshRenderers[i].material, skinnedMeshRenderers[i].material.color.a - 0.4f * Runner.DeltaTime, 3000);
                    }
                    if (meshRenderers[0].material.color.a < 0.4f || skinnedMeshRenderers[0].material.color.a < 0.4f)
                    {
                        player.playerStat.isVisible = false;
                        player.playerStat.isStartFadeInvi = false;
                    }

                }
                else if (player.playerStat.isVisible)
                {
                    BackDefaultMaterial();
                }
                if (player.playerStat.isInSight) CheckInviVisual(player.playerStat.isVisible);
                else CheckInviVisual(false);
            }
        }
        else if (creep)
        {
            
            if (creep.playerStat.isInSight)
            {
                CheckInviVisual(creep.playerStat.isVisible);
            }
            else CheckInviVisual(false);
        }
        else  //Là tower
        {
            if (tower.isInSight) CheckInviVisual(true);
            else CheckInviVisual(false);
        }

        // VisualOfPlayer(player.playerStat.isLive);
    }

    public void CheckInviVisual(bool isVisible)
    {
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController mainPlayer);
        if(player)
        {
            if (player.playerTeam != mainPlayer.playerTeam) //con nào chủ thế của invi thì ko bị ảnh hưởng
            {
                CheckVisual(isVisible);
            }
        }
        else if (creep)
        {
            CheckVisual(isVisible);
        }
        else  //Là tower
        {
           
        }
    }
    void CheckVisual(bool isVisible)
    {
        foreach (var visual in visuals)
        {
            bool canSee;
            if (player)
            {
                if (player.playerStat.isUnderTower) canSee = true;
                else canSee = isVisible;
            }
            else if (creep)
            {
                if (creep.playerStat.isUnderTower) canSee = true;
                else canSee = isVisible;
            }
            else 
            {
                canSee = isVisible;
            }
            
            visual.gameObject.SetActive(canSee);
        }
    }
    public void VisualOfPlayer(bool active)
    {
        foreach (var visual in visuals)
        {
            visual.gameObject.SetActive(active);
        }
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void CharacterControllerActiveRPC(bool isLive)
    {
        player.GetComponent<CharacterController>().enabled = isLive;
    }
    public void ControlMaterial(int modeRender, Material material, float alpha, int renderQueue)
    {
        material.SetFloat("_Mode", 3);
        material.color = new Color(material.color.r, material.color.g, material.color.b, alpha);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = renderQueue;
    }
    public void BackDefaultMaterial()
    {
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            MaterialToDefault(meshRenderers[i].material);
        }
        for (int i = 0; i < skinnedMeshRenderers.Length; i++)
        {
            MaterialToDefault(skinnedMeshRenderers[i].material);
        }
    }
    void MaterialToDefault(Material material)
    {
        material.SetFloat("_Mode", 0); // 0 = Opaque
        material.color = new Color(material.color.r, material.color.g, material.color.b, 1);
        // Thiết lập lại các thuộc tính cho Opaque
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha); // Kết hợp màu sắc
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha); // Kết hợp với alpha
        material.SetInt("_ZWrite", 1); // Cho phép ghi vào Depth buffer
        material.DisableKeyword("_ALPHATEST_ON"); // Tắt chế độ alpha test (cutout)
        material.DisableKeyword("_ALPHABLEND_ON"); // Tắt chế độ alpha blend
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON"); // Tắt chế độ alpha premultiply
        material.renderQueue = -1; // Đảm bảo vật liệu render theo thứ tự mặc định
    }
}
