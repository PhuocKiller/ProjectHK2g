using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using Fusion;

public class Login : MonoBehaviour
{
    private void Awake()
    {
        playersGame.Clear();
    }
    public static Dictionary<NetworkString<_32>, PlayerController> playersGame = new Dictionary<NetworkString<_32>, PlayerController>();
    public static void AddPlayer(PlayerController player)
    {
        playersGame.TryGetValue(player.playerID, out PlayerController playerData);
        if (playerData == null)
        {
            playersGame.Add(player.playerID, player);
        }
    }
    public static void RemovePlayer(PlayerController player)
    {
        playersGame.TryGetValue(player.playerID, out PlayerController playerData);
        if (playerData == null)
        {
            playersGame.Remove(playerData.playerID);
        }
    }
}
