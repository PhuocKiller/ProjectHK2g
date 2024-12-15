using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SkillSystem/Create Skill Data")]
public class SkillSO : ScriptableObject
{
    public SkillName skillName;
    public SkillTypes skillType;
    public float timerTrigger;
    public float cooldownTime;
    public int[] levelManaCosts;
    public Sprite skillIcon;
    public AudioClip triggerSoundFX;
    public int[] levelDamages;
    public bool isPhysicDamage;
    public bool isMakeStun;
    public bool isMakeSlow;
    public bool isMakeSilen;
    public float timeEffect;
    public NetworkObject VfxEffect;
    public string info;
}

