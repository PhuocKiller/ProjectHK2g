using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCallBackInfomation : MonoBehaviour
{
    PlayerController player;
    private void Awake()
    {
        player = GetComponentInParent<PlayerController>();
    }
    public void CallBackReconect()
    {
        player.skillManager.Initialize();
        player.skillManager.DrawSkill();
        player.joystick = FindObjectOfType<Joystick>();
        player.inventory.SetupInventory();
        player.inventory.BackUpInventory();
    }
}
