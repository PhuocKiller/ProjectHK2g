using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatHUD : MonoBehaviour
{
    public PlayerController player;
    [SerializeField] TextMeshProUGUI attackTMP, defTMP, attackSpeedTMP, magicAmpliTMP, magicResisTMP, moveSpeedTMP;
    private void OnEnable()
    {
        StartCoroutine(DelayCheckPlay());
    }

    // Update is called once per frame
    void Update()
    {
        if (player==null) return;
        attackTMP.text= player.playerStat.damage.ToString();
        defTMP.text = player.playerStat.defend.ToString();
        attackSpeedTMP.text = player.playerStat.attackSpeed.ToString();
        magicAmpliTMP.text = ((player.playerStat.magicAmpli*100)).ToString() + "%";
        magicResisTMP.text = player.playerStat.magicResistance.ToString();
        moveSpeedTMP.text = player.playerStat.moveSpeed.ToString();
    }
    IEnumerator DelayCheckPlay()
    {
        yield return new WaitForSeconds(0.2f);
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        this.player = player;
    }
    
}
