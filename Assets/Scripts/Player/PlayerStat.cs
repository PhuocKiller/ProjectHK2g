using System;
using System.Collections;
using System.ComponentModel;
using Cinemachine;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStat : NetworkBehaviour
{
    [SerializeField] PlayerBuffManager playerBuffManager;
    [Networked] public int level { get; set; }
    [Networked] public int levelPoint { get; set; }
    [Networked] public int b_maxHealth { get; set; }
    [Networked] public int currentHealth { get; set; }
    [Networked] public int b_maxMana { get; set; }
    [Networked] public int currentMana { get; set; }
    [Networked] public int b_damage { get; set; }
    [Networked] public int b_defend { get; set; }
    [Networked] public int b_magicResistance { get; set; }
    [Networked] public float b_magicAmpli { get; set; }
    [Networked] public float b_criticalChance { get; set; }
    [Networked] public float b_criticalDamage { get; set; }
    [Networked] public int b_moveSpeed { get; set; }
    [Networked] public int b_attackSpeed { get; set; }
    [Networked] public int counterDamage { get; set; }
    [Networked] public float b_lifeSteal { get; set; }
    [Header("Full Stat")]
    public int maxHealth;
    public int maxMana;
    [Networked] public int maxXP { get; set; }
    [Networked] public int currentXP { get; set; }
    [Networked] public int coinsValue { get; set; }
    public int damage;
    public int defend;
    public float magicResistance;
    public float magicAmpli;
    public float criticalChance;
    public float criticalDamage;
    public int moveSpeed;
    public int attackSpeed;
    public float lifeSteal;
    [Space(1)]
    [Header("Multiple Stat")]
    public int multipleHealth;
    public int multipleMana;
    public int multipleDamage;
    public int multipleDefend;
    public int multipleMagicResistance;
    public float multipleMagicAmpli;
    public float multipleCriticalChance;
    public float multipleCriticalDamage;
    public int multipleMoveSpeed;
    public int multipleAttackSpeed;
    public float multipleLifeSteal;

    PlayerController player;
    CreepController creep;
    [HideInInspector][Networked] public bool isBeingStun { get; set; }
    [HideInInspector][Networked] public bool isBeingSlow { get; set; }
    [HideInInspector][Networked] public bool isBeingSilen { get; set; }
    [HideInInspector][Networked] public bool isLifeSteal { get; set; }
    [Networked] public bool isVisible { get; set; }
    [Networked] public bool isUnderTower { get; set; }
    [Networked] public bool isStartFadeInvi { get; set; }
    [Networked] public bool isUnstopAble { get; set; }
    [Networked] public bool isCounter { get; set; }
    [Networked] public bool isLive { get; set; }
    [Networked] public bool isBeingTele { get; set; }
    [Networked] public bool canBuyItem { get; set; }
    [Networked(OnChanged = nameof(FollowEnemyChange))] public bool isFollowEnemy { get; set; }
    protected static void FollowEnemyChange(Changed<PlayerStat> changed)
    {
        if(changed.Behaviour.HasStateAuthority)
        {
            if (changed.Behaviour.isFollowEnemy == true)
            {
                if(changed.Behaviour.player.overlapSphere.closestEnemyPlayer!=null)
                {
                    FindObjectOfType<CameraController>().CameraFollowEnemy();
                    FindObjectOfType<HudManager>().unFollowBtn.SetActive(true);
                }
                else { changed.Behaviour.isFollowEnemy = false; }
                
            }
            else
            {
                FindObjectOfType<CameraController>().CameraUnFollowEnemy();
                FindObjectOfType<HudManager>().unFollowBtn.GetComponent<Button>().onClick.Invoke();
                FindObjectOfType<HudManager>().unFollowBtn.SetActive(false);
            }
        }
    }
    public override void Spawned()
    {
        base.Spawned();
        player = transform.parent.parent.GetComponent<PlayerController>();
        creep = transform.parent.parent.GetComponent<CreepController>();
        if(player)
        {
            playerBuffManager = player.GetComponentInChildren<PlayerBuffManager>();
        }
        else { playerBuffManager = creep.GetComponentInChildren<PlayerBuffManager>(); }
        
        currentHealth = 1; //tránh bị bằng =0 trong lần đầu tiên cập nhật
        levelPoint = 0;
        UpgradeLevel();
        isVisible = true; isLive = true; isLifeSteal = true;
        coinsValue = 30000;
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        UpdateFullStat();
    }
    public void UpdateBaseStat(int level, int multipleHealth, int multipleMana, int multipleDamage, int multipleDefend,
        int multipleMagicResistance, float multipleMagicAmpli,
        float multipleCriticalChance, float multipleCriticalDamage, int multipleMoveSpeed, int multipleAttackSpeed, float multipleLifeSteal)
    {
        if (creep)
        {
            if(creep.creepType==Creep_Types.Melee)
            {
                
                BaseStatOnType(200, 0, 10, 5, 5, 0, 0, 1, 300, 100, 0, level, multipleHealth, multipleMana, multipleDamage, multipleDefend,
                            multipleMagicResistance, multipleMagicAmpli,
                            multipleCriticalChance, multipleCriticalDamage, multipleMoveSpeed, multipleAttackSpeed, multipleLifeSteal);
            }
            else if (creep.creepType == Creep_Types.Range)
            {
                BaseStatOnType(100, 0, 20, 5, 5, 0, 0, 1, 300, 100, 0, level, multipleHealth, multipleMana, multipleDamage, multipleDefend,
                            multipleMagicResistance, multipleMagicAmpli,
                            multipleCriticalChance, multipleCriticalDamage, multipleMoveSpeed, multipleAttackSpeed, multipleLifeSteal);
            }

        }
       
        else
        {
            BaseStatOnType(500, 100, 50, 5, 5, 0, 0,1, 300, 100, 0, level, multipleHealth, multipleMana, multipleDamage, multipleDefend,
            multipleMagicResistance, multipleMagicAmpli,
            multipleCriticalChance, multipleCriticalDamage, multipleMoveSpeed, multipleAttackSpeed, multipleLifeSteal);
        }
    }
    void BaseStatOnType(int b_maxHealth, int b_maxMana, int b_damage, int b_defend, int b_magicResistance, float b_magicAmpli,
        float b_criticalChance, float b_criticalDamage, int b_moveSpeed, int b_attackSpeed, float b_lifeSteal,
        int level, int multipleHealth, int multipleMana, int multipleDamage, int multipleDefend,
        int multipleMagicResistance, float multipleMagicAmpli,
        float multipleCriticalChance, float multipleCriticalDamage, int multipleMoveSpeed, int multipleAttackSpeed, float multipleLifeSteal)
    {
        this.b_maxHealth = b_maxHealth + (level - 1) * multipleHealth; this.b_maxMana = b_maxMana + (level - 1) * multipleMana;
        if (level >= 12) this.currentXP = 0;
        this.maxXP = 100 + (level - 1) * (level - 1) * 100;
        this.b_damage = b_damage + (level - 1) * multipleDamage; this.b_defend = b_defend + ((level - 1) * multipleDefend);
        this.b_magicResistance = b_magicResistance + (level - 1) * multipleMagicResistance; this.b_magicAmpli = b_magicAmpli + (level - 1) * multipleMagicAmpli;
        this.b_criticalChance = b_criticalChance + (level - 1) * multipleCriticalChance; this.b_criticalDamage = b_criticalDamage + (level - 1) * multipleCriticalDamage;
        this.b_moveSpeed = b_moveSpeed + ((level - 1) * multipleMoveSpeed);
        this.b_attackSpeed = b_attackSpeed + ((level - 1) * multipleAttackSpeed);
        this.b_lifeSteal = b_lifeSteal + ((level - 1) * multipleLifeSteal);
    }
    public void UpgradeLevel()
    {
        if (!creep) level++;
        levelPoint++;
        UpdateBaseStat(level, multipleHealth, multipleMana, multipleDamage, multipleDefend,
            multipleMagicResistance, multipleMagicAmpli,
            multipleCriticalChance, multipleCriticalDamage, multipleMoveSpeed, multipleAttackSpeed, multipleLifeSteal);
        UpdateFullStat();
        currentHealth = maxHealth;
        currentMana = maxMana;
        
    }

    private void UpdateFullStat()
    {
        if (player?.state == 3 ||creep?.state==3) return;
        maxHealth = b_maxHealth + playerBuffManager.maxHealth;
        if(maxHealth<1) maxHealth = 1;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        if(creep)
        {
            DelayCreepDie();
        }
        maxMana = b_maxMana + playerBuffManager.maxMana;
        if(currentMana > maxMana) { currentMana = maxMana; }
        damage = b_damage + playerBuffManager.damage;
        defend = b_defend + playerBuffManager.defend;
        magicResistance = b_magicResistance + playerBuffManager.magicResistance;
        magicAmpli = b_magicAmpli + playerBuffManager.magicAmpli;
        criticalChance = b_criticalChance + playerBuffManager.criticalChance;
        if(criticalChance>1) criticalChance = 1;    
        criticalDamage = b_criticalDamage + playerBuffManager.criticalDamage;
        moveSpeed = b_moveSpeed + playerBuffManager.moveSpeed;
        attackSpeed = b_attackSpeed + playerBuffManager.attackSpeed;
        lifeSteal = b_lifeSteal + playerBuffManager.lifeSteal;
        if (attackSpeed < 25) attackSpeed = 25;
    }
    public void GainXPWhenKill (int XPGain)
    {
        currentXP += XPGain;
        player.playerScore.assistScore += 1;
    }
    public void GainCoinWhenKill(int CoinGain)
    {
        coinsValue += CoinGain;
    }
    public void CalculateBaseStatForCreep()
    {
        if (!creep) return;
        UpdateBaseStat(level, multipleHealth, multipleMana, multipleDamage, multipleDefend,
            multipleMagicResistance, multipleMagicAmpli,
            multipleCriticalChance, multipleCriticalDamage, multipleMoveSpeed, multipleAttackSpeed, multipleLifeSteal);
    }
    IEnumerator DelayCreepDie()
    {
        yield return new WaitForSeconds(2f);
        if(this!=null)
        {
            Destroy(transform.parent.parent.gameObject);
        }
    }
}
