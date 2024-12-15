using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioClip attack_Melee, attack_Range, skill1_Melee, skill1_Range, die, injured,
         levelUp, run, error, buyItem, clickButton, checkSound, pauseGame, unpauseGame;
    public AudioClip bossAttack, bossTele, bossChase, bossSkill, bossDie, wizardAttack;
    public AudioClip[] theme;
    public AudioSource musicSource, soundSource, monsterSource;
    public float musicVolume, soundVolume;
   
    private void Start()
    {
      //  for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
       // {
       //     if (i == SceneManager.GetActiveScene().buildIndex)
        //    {
                musicSource.clip = theme[0];
                musicSource.loop = true;
                musicSource.Play();
        //    }
       // }
    }
    public void PlaySound(AudioClip clip, bool isLoop = false)
    {
        soundSource.clip = clip;
        soundSource.loop = isLoop;
        soundSource.Play();
    }
    public void PlayMonsterSound(AudioClip clip, bool isLoop = false)
    {
        monsterSource.clip = clip;
        monsterSource.loop = isLoop;
        monsterSource.Play();
    }
    public void StopSound()
    {
        soundSource.Stop();
    }
}

