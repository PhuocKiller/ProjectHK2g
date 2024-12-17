using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusManager : MonoBehaviour
{
    [SerializeField] Bars hpBarHUD, manaBarHUD, expBarHUD,teleBar;
    [SerializeField] TextMeshProUGUI levelText;
    public PlayerController player;
    [SerializeField] TextMeshProUGUI timeDie;
    [SerializeField] GameObject panelAvaterWhenDie;
    [SerializeField] Sprite[] avaImages;
    [SerializeField] Image avatar;
    private void OnEnable()
    {
        StartCoroutine(DelayCheckPlay());
    }
    private void Update()
    {
        if (player == null) return;
        // Debug.Log(player.playerStat.currentHealth);
        hpBarHUD.UpdateBar(player.playerStat.currentHealth, player.playerStat.maxHealth);
        manaBarHUD.UpdateBar(player.playerStat.currentMana, player.playerStat.maxMana);
        expBarHUD.UpdateBar(player.playerStat.currentXP, player.playerStat.maxXP);
        teleBar.gameObject.SetActive(player.playerStat.isBeingTele);
        teleBar.UpdateBar(player.statusCanvas.currentTeleTimeStatus, 5);
        levelText.text = player.playerStat.level.ToString();
        panelAvaterWhenDie.SetActive(player.state == 3);
        if (player.state == 3)
        {
            timeDie.text = Mathf.FloorToInt((float)player.timeDie.RemainingTime(player.runnerManager.GetComponent<NetworkRunner>())).ToString();
        }
    }
    IEnumerator DelayCheckPlay()
    {
        yield return new WaitForSeconds(0.5f);
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        this.player = player;
        avatar.sprite = avaImages[(int)player.playerType];
    }
}
