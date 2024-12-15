using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public class PlayerController : NetworkBehaviour, ICanTakeDamage
{
    ItemSkillManager itemSkillManager;
    public Inventory inventory;
    public SkillManager skillManager;
    public PlayerCallBackInfomation playerCallBack;
    [Networked] public string playerID {  get; set; }
    public Transform buffFromItemManager;
    public NetworkManager runnerManager;
    public GameManager gameManager;
    public OverlapSpherePlayer overlapSphere;
    public Joystick joystick;
    public Vector3 spawnPosPlayer;
    [Networked]  public int playerTeam { get; set; }
    public ListNetworkObject networkObjs;
    public List<Collider> collisionsEnvi = new List<Collider>();
    public BuffsOfPlayer buffsFromEnvi,buffsFromPassive;
    public PlayerStat playerStat;
    public PlayerScore playerScore;
    public StatusCanvas statusCanvas;
    Vector2 moveInput;
    Vector3 moveDirection;
    public CharacterController characterControllerPrototype;
    Animator animator;
    float speed;
    private int targetX, targetY, beforeTarget;
    float previousSpeedX, currentSpeedX, previousSpeedY, currentSpeedY;
    [HideInInspector] public bool isGround;
    [Networked]
    bool isJumping { get; set; }
    [Networked]
    bool isBasicAttackAttack { get; set; }
    [Networked]
    float jumpHeight { get; set; }
    Vector3 velocity;


    // 0 là normal
    // 1 là jump
    // 2 là injured
    // 3 là die
    // 4 là active attack
    // 5 là đang cast skill
    [Networked(OnChanged = nameof(listenState))]
    [SerializeField]
    public int state { get; set; }
    protected static void listenState(Changed<PlayerController> changed)
    {
        
    }
    [SerializeField]
    public Transform jumpTransform,normalAttackTransform, skill_1Transform,
        skill_2Transform, ultimateTransform, rayCastTransform, itemSkillTransform, transformCamera;
    
    [SerializeField] public Player_Types playerType;
    [HideInInspector] public SkillButton[] skillButtons;
    [SerializeField] GameObject[] statusDebuffs;
    
    [SerializeField] [Networked] TickTimer TimeOfStunDebuff { get; set; }
    [SerializeField][Networked] TickTimer TimeOfSlowDebuff { get; set; }
    [SerializeField][Networked] TickTimer TimeOfSilenDebuff { get; set; }
    [Networked] float maxStunTimeStatus { get; set; }
    [Networked] float currentStunTimeStatus { get; set; }
    [Networked] float maxSilenTimeStatus { get; set; }
    [Networked] float currentSilenTimeStatus { get; set; }
    [Networked] float maxSlowTimeStatus { get; set; }
    [Networked] float currentSlowTimeStatus { get; set; }
    private void Awake()
    {
        characterControllerPrototype = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        statusCanvas = GetComponentInChildren<StatusCanvas>();
        overlapSphere = GetComponentInChildren<OverlapSpherePlayer>();
        runnerManager = FindObjectOfType<NetworkManager>();
        buffFromItemManager=GetComponentInChildren<BuffFromItemManager>().transform;
        itemSkillManager=GetComponentInChildren<ItemSkillManager>();
        playerCallBack = GetComponentInChildren<PlayerCallBackInfomation>();
        skillManager=GetComponentInChildren<SkillManager>();
        inventory = GetComponentInChildren<Inventory>();
    }
    public override void Spawned()
    {
        base.Spawned();
        
        if (Object.InputAuthority.PlayerId == Runner.LocalPlayer.PlayerId)
        {
            gameManager = FindObjectOfType<GameManager>();
            spawnPosPlayer = runnerManager.spawnPointPlayer[playerTeam].position + Vector3.right * 5 * (playerTeam == 0 ? 1 : -1);
            Singleton<CameraController>.Instance.SetFollowCharacter(transform);
            Singleton<PlayerManager>.Instance.SetRunner(Runner);
            TimeOfStunDebuff = TickTimer.CreateFromSeconds(Runner,0);
            TimeOfSlowDebuff = TickTimer.CreateFromSeconds(Runner, 0);
            TimeOfSilenDebuff = TickTimer.CreateFromSeconds(Runner, 0);
            UIManagerRegisInven();
        }
        joystick = FindObjectOfType<Joystick>();
        Login.AddPlayer(this);
    }
    public void UIManagerRegisInven()
    {
        FindObjectOfType<UIManager>().RegisterEventInven(inventory);
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
       // CheckPing();
        CalculateCanvas();
        CalculateStatusDebuff();
        if (state != 2) animator.enabled = !playerStat.isBeingStun;
        if (state == 3)
        {
            if (timeDie.RemainingTime(Runner) <= 0 || timeDie.ExpiredOrNotRunning(Runner))
            {
                
                playerStat.isLive = true;
                state = 0;
                playerStat.currentHealth = playerStat.maxHealth;
                playerStat.currentMana=playerStat.maxMana;  
                AnimatorSetBoolRPC("isLive",true);
                statusCanvas.GetComponent<InviManager>().VisualOfPlayer(true);
                playerScore.playersMakeDamages.Clear();
            }
            return;
        }
        if (!playerStat.isBeingStun && (state==0 ||state ==5 ||state==1))
        {
            CalculateMove();
            CalculateJump();
        }
        CalculateEXP();
        animator.SetFloat("AttackSpeed", (float)playerStat.attackSpeed / 100);
        animator.SetFloat("MoveSpeed", (float)playerStat.moveSpeed / 300);
    }
    #region Jump
    private void CalculateJump()
    {
        if (HasStateAuthority)
        {
            if (isJumping&& animator.GetCurrentAnimatorStateInfo(0).IsName("Move"))
            {
                isGround = false;
                isJumping = false;
                velocity += new Vector3(0, 35f, 0);
            }
            if (isGround)
            {
                velocity.y = 0;
                characterControllerPrototype.Move(velocity * Runner.DeltaTime);
            }
            else
            {
                velocity += new Vector3(0, -100f * Runner.DeltaTime, 0);

                characterControllerPrototype.Move(velocity * Runner.DeltaTime);
            }
        }
    }
    
    public void Jump(NetworkObject VFXEffect)
    {
        NoTeleAnyMore();
        isJumping = true;
        AnimatorTriggerRPC("Jump");
        NetworkObject jumpVFX= Runner.Spawn(VFXEffect, jumpTransform.transform.position,
            jumpTransform.rotation, inputAuthority: Object.InputAuthority);
        StartCoroutine(DespawnJumpVFX(jumpVFX));
    }
    IEnumerator DespawnJumpVFX(NetworkObject jumpVFX)
    {
        yield return new WaitForSeconds(0.5f);
        Runner.Despawn(jumpVFX);
    }
    #endregion

    #region "SkillButton"
    public void Teleport(NetworkObject VFXEffect)
    {
        AnimatorSetBoolRPC("isTeleporting",true);
        playerStat.isBeingTele = true;
        statusCanvas.TimeOfTele = TickTimer.CreateFromSeconds(Runner, 5);
        NetworkObject obj= Runner.Spawn(VFXEffect, rayCastTransform.position, Quaternion.identity);
        SetParentTeleObjRPC(obj.Id);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SetParentTeleObjRPC(NetworkId id)
    {
        if (!Runner.TryFindObject(id, out NetworkObject obj)) return;
        obj.transform.SetParent(rayCastTransform);
    }
    public void TeleToBase()
    {
        NoTeleAnyMore();
        characterControllerPrototype.enabled = false;
        SpawnAtStartPos();
        characterControllerPrototype.enabled = true;
        Singleton<CameraController>.Instance.SetFollowCharacter(transform);
    }
    public void NoTeleAnyMore()
    {
        AnimatorSetBoolRPC("isTeleporting", false);
        playerStat.isBeingTele = false;
        if(rayCastTransform.childCount>0) Destroy(rayCastTransform.GetChild(0).gameObject);
    }
    public virtual void NormalAttack(NetworkObject VFXEffect, int levelDamage, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false, float timeTrigger = 0f, float TimeEffect = 0f)
    {
        NoTeleAnyMore();
        AnimatorTriggerRPC("Attack");
        state = 4;
    }
    public virtual void Skill_1(NetworkObject VFXEffect, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        NoTeleAnyMore();
        AnimatorTriggerRPC("Skill_1");
        playerStat.currentMana -= manaCost;
    }

    public virtual void Skill_2(NetworkObject VFXEffect, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp= null,int levelSkill = 1)
    {
        NoTeleAnyMore();
        AnimatorTriggerRPC("Skill_2");
        playerStat.currentMana -= manaCost;
    }
    public virtual void Ultimate(NetworkObject VFXEffect, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        NoTeleAnyMore();
        AnimatorTriggerRPC("Ultimate");
        playerStat.currentMana -= manaCost;
    }
    public virtual void UseItemSkill(SkillName skillName,NetworkObject VFXEffect, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        NoTeleAnyMore();
        AnimatorTriggerRPC("UseItemSkill");
        playerStat.currentMana -= manaCost;
        itemSkillManager.UseItemSkill(skillName, VFXEffect, levelDamage, manaCost, isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen,
        timeTrigger, TimeEffect, posMouseUp, levelSkill);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void AnimatorTriggerRPC(string name)
    {
        animator.SetTrigger(name);
    }
    #endregion
    #region Update
    void Update()
    {
        if (state == 3 || state == 4)
        {
            return;
        }
        
        if (HasStateAuthority)
        {
            
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            
            if (moveInput.magnitude == 0)
            {
                moveInput = new Vector2(joystick.Horizontal, joystick.Vertical).normalized;
            }
        }
        if (isGround)
        {
            velocity = Vector3.zero;
        }
    }
    #endregion
    #region Move
    void CalculateMove()
    {
        if (HasStateAuthority /*&& playerID == Runner.GetPlayerUserId(Object.InputAuthority)*/)
        {

            moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
            CalculateAnimSpeed("MoveX", moveInput.x, true);
            CalculateAnimSpeed("MoveY", moveInput.y, false);
            speed = 2f + Vector2.Dot(moveInput, Vector2.up);
            Quaternion look = Quaternion.AngleAxis(Camera.main.transform.rotation.eulerAngles.y, Vector3.up);
            if (moveDirection.magnitude > 0)
            {
                NoTeleAnyMore();
                if (gameManager.state==GameState.InGame)
                {
                    characterControllerPrototype.Move(look * moveDirection * speed * 0.015f
                * playerStat.moveSpeed * (playerStat.isBeingSlow ? 0.3f : 1f) * Runner.DeltaTime);
                }
                if(gameManager.state == GameState.WaitBeforeStart)
                {
                    Vector3 directionToCenter = transform.position - spawnPosPlayer; // Vector3.zero là vị trí của sphere center
                    if (directionToCenter.magnitude > 15f)
                    {
                        characterControllerPrototype.Move(-directionToCenter.normalized * speed * 0.015f
                * playerStat.moveSpeed*Runner.DeltaTime);
                    }
                    else
                    {
                        characterControllerPrototype.Move(look * moveDirection * speed * 0.015f
                * playerStat.moveSpeed * (playerStat.isBeingSlow ? 0.3f : 1f) * Runner.DeltaTime);
                    }
                }
            
            transform.rotation = Quaternion.RotateTowards(transform.rotation, look, 360 * Runner.DeltaTime);
            }
        
        }
    }
    private void CalculateAnimSpeed(string animationName, float speed, bool isMoveX)
    {
        if (isMoveX)
        {
            currentSpeedX = speed;
        }
        else
        {
            currentSpeedY = speed;
        }

        if (isMoveX && previousSpeedX != currentSpeedX)
        {
            CaculateSmoothAnimation(animationName, true, speed);
        }
        if (!isMoveX && previousSpeedY != currentSpeedY)
        {
            CaculateSmoothAnimation(animationName, false, speed);
        }

        if (isMoveX)
        {
            previousSpeedX = speed;
        }
        else
        {
            previousSpeedY = speed;
        }
    }

    void CaculateSmoothAnimation(string animationName, bool isMoveX, float? Speedtarget = null)
    {
        float time = 0;
        float start = animator.GetFloat(animationName);
        float x = Speedtarget == null ? 2 : 5;
        float targetTime = 1 / x;
        while (time <= targetTime)
        {
            if (Speedtarget != null
                && Speedtarget != (isMoveX ? currentSpeedX : currentSpeedY))
            {
                time = targetTime;
                break;
            }
            float valueRandomSmooth = Mathf.Lerp(start, Speedtarget == null ?
                (isMoveX ? targetX : targetY) : Speedtarget.Value, x * time);
            animator.SetFloat(animationName, valueRandomSmooth);
            time += Runner.DeltaTime;
        }
    }
    void SpawnAtStartPos()
    {
        transform.position = spawnPosPlayer;
        transform.rotation = runnerManager.spawnPointPlayer[playerTeam].rotation;
    }
    #endregion
    
    #region State
    public void SwithCharacterState(int newstate)
    {
        switch (state)
        {
            case 0: { break; }
            case 1: { break; }
            case 2: { break; }
            case 3: { break; }
        }
        switch (newstate)
        {
            case 0: { break; }
            case 1: { break; }
            case 2: { break; }
            case 3:
                {
                    if (HasStateAuthority) Singleton<AudioManager>.Instance.PlaySound(Singleton<AudioManager>.Instance.die);
                    animator.SetTrigger("Die");
                    break;
                }
        }
        state = newstate;
    }
    public int GetCurrentState()
    {
        return state;
    }
    
    #endregion
    public void CheckCamera(PlayerRef player, bool isFollow)
    {
        if (player == Runner.LocalPlayer)
        {
            if (isFollow)
            {
                Singleton<CameraController>.Instance.SetFollowCharacter(transform);
            }
            else
            {
                Singleton<CameraController>.Instance.RemoveFollowCharacter();
            }
        }
    }
    #region Collider
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Ground") && !isGround)
        {
            isGround = true;
        }
    }
    private void OnTriggerStay(Collider otherColi)
    {
        if (!HasStateAuthority) return;
        if (!collisionsEnvi.Contains(otherColi) && otherColi.gameObject.CompareTag("Environment")) //Nếu là environment
        {
          collisionsEnvi.Add(otherColi);
        }
        buffsFromEnvi.maxHealth = 0;
        buffsFromEnvi.maxMana = 0; 
        buffsFromEnvi.damage = 0; 
        buffsFromEnvi.defend = 0;
        buffsFromEnvi.magicResistance = 0;
        buffsFromEnvi.magicAmpli = 0;
        buffsFromEnvi.criticalChance = 0; 
        buffsFromEnvi.criticalDamage = 0; 
        buffsFromEnvi.moveSpeed = 0;
        buffsFromEnvi.attackSpeed = 0;
        foreach (var other in collisionsEnvi)
        {
            if (other != null &&other.GetComponent<NetworkObject>().IsValid)
            {
                if(other.GetComponent<EnvironmentObjects>().playerTeam ==playerTeam)
                {
                    BuffsOfPlayer buffs = other.GetComponent<BuffsOfPlayer>();
                    buffsFromEnvi.maxHealth += buffs.maxHealth;
                    buffsFromEnvi.maxMana += buffs.maxMana ;
                    buffsFromEnvi.damage += buffs.damage ;
                    buffsFromEnvi.defend += buffs.defend ;
                    buffsFromEnvi.magicResistance += buffs.magicResistance ;
                    buffsFromEnvi.magicAmpli += buffs.magicAmpli ;
                    buffsFromEnvi.criticalChance += buffs.criticalChance ;
                    buffsFromEnvi.criticalDamage += buffs.criticalDamage ;
                    buffsFromEnvi.moveSpeed += buffs.moveSpeed ;
                    buffsFromEnvi.attackSpeed += buffs.attackSpeed ;
                }
                else
                {
                    BuffsOfPlayer buffs = other.GetComponent<BuffsOfPlayer>();
                    buffsFromEnvi.maxHealth -= buffs.maxHealth ;
                    buffsFromEnvi.maxMana -= buffs.maxMana ;
                    buffsFromEnvi.damage -= buffs.damage ;
                    buffsFromEnvi.defend -= buffs.defend ;
                    buffsFromEnvi.magicResistance -= buffs.magicResistance ;
                    buffsFromEnvi.magicAmpli -= buffs.magicAmpli ;
                    buffsFromEnvi.criticalChance -= buffs.criticalChance ;
                    buffsFromEnvi.criticalDamage -= buffs.criticalDamage ;
                    buffsFromEnvi.moveSpeed -= buffs.moveSpeed ;
                    buffsFromEnvi.attackSpeed -= buffs.attackSpeed ;
                }
            }
            
        }
    }
    private void OnTriggerExit(Collider otherColi)
    {
        if (collisionsEnvi.Contains(otherColi))
        {
            collisionsEnvi.Remove(otherColi);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!HasStateAuthority) return;
        InventoryItemBase item = other.GetComponent<InventoryItemBase>();
        if (item != null)
        {
            inventory.AddItem(item, out bool canAdd);
            if(canAdd) item.OnPickUp();

        }
    }
    private void OnCollisionEnter(Collision collision)
    {

    }
    #endregion
    #region Apply Damage
    public void ApplyDamage(int damage, bool isPhysicDamage, PlayerController player,
        Action<int,bool> counter = null, Action<int, List<PlayerController>> isKillPlayer = null,
        Action<Vector3, float> isKillCreep = null,
        Action<int> lifeSteal = null,bool activeInjureAnim = true,bool isCritPhysic= false)
    {
        CalculateHealthRPC(damage, isPhysicDamage, player, activeInjureAnim, isCritPhysic);
        if(playerStat.isCounter)
        {
            counter?.Invoke(playerStat.counterDamage, isPhysicDamage);
        }
        
        if (state==3)
        {
            isKillPlayer?.Invoke(playerStat.level, playerScore.playersMakeDamages);
        }
        lifeSteal?.Invoke(damage);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void CalculateHealthRPC
        (int damage, bool isPhysicDamage, PlayerController player, bool activeInjureAnim = true, bool isCritPhysic = false)
    {
        if (!playerScore.playersMakeDamages.Contains(player) &&player!=null)
        {
            playerScore.playersMakeDamages.Add(player);
        }
        if ((playerStat.currentHealth + statusCanvas.GetCurrentDamageAbsorbShield()) > damage)
        {
            if (activeInjureAnim)
            {
                state = 2;
                animator.SetTrigger("Injured");
                if (HasStateAuthority) Singleton<AudioManager>.Instance.PlaySound(Singleton<AudioManager>.Instance.injured);
            }
            if (statusCanvas.GetCurrentDamageAbsorbShield() > 0)
            {
                
                statusCanvas.ReduceDamageAbsoreShield(damage, out int overBalanceDmg);
                playerStat.currentHealth -= overBalanceDmg;
                statusCanvas.PlayerHaveInjure(overBalanceDmg, isCritPhysic);
            }
            else
            {
                playerStat.currentHealth -= damage;
                statusCanvas.PlayerHaveInjure(damage, isCritPhysic);
            }
            
        }
        else
        {
            WhenPlayerDie();
            statusCanvas.PlayerHaveInjure(damage, isCritPhysic);
        }
    }
    void WhenPlayerDie()
    {
        playerStat.currentHealth = 0;
        
        if (overlapSphere!= null)
        {
            foreach (var enemyPlayers in overlapSphere.CheckPlayerAround())
            {
                
                if (!playerScore.playersMakeDamages.Contains(enemyPlayers))
                {
                    playerScore.playersMakeDamages.Add(enemyPlayers);
                }
            }
        }
        
        if (playerScore.playersMakeDamages.Count>0)
        {
            foreach (var playerDamage in playerScore.playersMakeDamages)
            {
                CalculateWhenKill(playerDamage);
            }
        }
        
        timeDie = TickTimer.CreateFromSeconds(Runner,5+ 2 * playerStat.level); //thời gian hồi sinh
        animator.SetBool("isLive", false);
        SwithCharacterState(3);
        playerStat.isBeingStun = false; playerStat.isBeingSlow = false; playerStat.isBeingSilen = false;
        playerStat.isLive = false; playerStat.isFollowEnemy = false;
        StartCoroutine(DelayHideVisualWhenDie());
    }
    void CalculateWhenKill(PlayerController playerDamage)
    {
        playerDamage.playerStat.GainXPWhenKill((int)100*(playerScore.playersMakeDamages.Count+1) * playerStat.level / playerScore.playersMakeDamages.Count);
        playerDamage.playerStat.GainCoinWhenKill((int)100 * (playerScore.playersMakeDamages.Count + 1) * playerStat.level / playerScore.playersMakeDamages.Count);
        playerDamage.GetComponent<Tesla>()?.PassiveWhenKill();
    }
    void CalculateEXP()
    {
        if (playerStat.currentXP >= playerStat.maxXP)
        {
            playerStat.currentXP -= playerStat.maxXP;
            playerStat.UpgradeLevel();
            if (HasStateAuthority) Singleton<AudioManager>.Instance.PlaySound(Singleton<AudioManager>.Instance.levelUp);
        }
    }
    public IEnumerator DelayHideVisualWhenDie ()
    {
        yield return new WaitForSeconds(3f);
        statusCanvas.GetComponent<InviManager>().VisualOfPlayer(playerStat.isLive);
        if (HasStateAuthority)
        {
            SpawnAtStartPos();
            Singleton<CameraController>.Instance.SetFollowCharacter(transform);
        }
    }
    [Rpc(RpcSources.All, RpcTargets.All)] public void AnimatorSetBoolRPC(string aniName, bool isActive)
    {
        animator.SetBool(aniName, isActive);
    }
    [Networked] public TickTimer timeDie {  get; set; }
    #endregion

    #region Apply Effect
    public void ApplyEffect(PlayerRef player,bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float TimeEffect = 0, Action callback = null)
    {
        CalculateEffectRPC(player,isMakeStun,isMakeSlow,isMakeSilen,TimeEffect);
        callback?.Invoke();
    }
    [Rpc(RpcSources.All, RpcTargets.All)] public void CalculateEffectRPC(PlayerRef player, bool isMakeStun = false,
        bool isMakeSlow = false, bool isMakeSilen = false, float TimeEffect = 0)
    {
        if (playerStat.isUnstopAble || !playerStat.isLive) return;
        if(isMakeStun)
        {
            if (TimeOfStunDebuff.RemainingTime(Runner) == null||
                TimeEffect >= (float)TimeOfStunDebuff.RemainingTime(Runner))
            {
                TimeOfStunDebuff = TickTimer.CreateFromSeconds(Runner, TimeEffect);
                maxStunTimeStatus = TimeEffect;
                currentStunTimeStatus = TimeEffect;
            }
            playerStat.isBeingStun = true;
        }
        if (isMakeSilen)
        {
            if (TimeEffect >= (float)TimeOfSilenDebuff.RemainingTime(Runner))
            {
                TimeOfSilenDebuff = TickTimer.CreateFromSeconds(Runner, TimeEffect);
                maxSilenTimeStatus = TimeEffect;
                currentSilenTimeStatus = TimeEffect;
            }
            playerStat.isBeingSilen = true;
        }
        if (isMakeSlow)
        {
            if(TimeEffect>=(float)TimeOfSlowDebuff.RemainingTime(Runner))
            {
                TimeOfSlowDebuff = TickTimer.CreateFromSeconds(Runner, TimeEffect);
                maxSlowTimeStatus = TimeEffect;
                currentSlowTimeStatus = TimeEffect;
            }
            playerStat.isBeingSlow = true;
        }
        
    }
    public void ApplyRegenHealth(int regen)
    {
        CalculateRegenHealthRPC(regen);
    }
    [Rpc(RpcSources.All, RpcTargets.All)] public void CalculateRegenHealthRPC(int regen)
    {
        if (state == 3) return;
        playerStat.currentHealth += regen;
    }
    
    public void ApplyRegenMana(int regen)
    {
        CalculateRegenManaRPC(regen);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void CalculateRegenManaRPC(int regen)
    {
        if (state == 3) return;
        playerStat.currentMana += regen;
    }
    #endregion
    #region Status Canvas
    void CalculateStatusDebuff()
    {
        
        if (HasStateAuthority)
        {
            if (TimeOfStunDebuff.RemainingTime(Runner)>0)
            {
                currentStunTimeStatus = (float)TimeOfStunDebuff.RemainingTime(Runner); 
            }
            else if (TimeOfSilenDebuff.RemainingTime(Runner) > 0)
            {
                currentSilenTimeStatus = (float)TimeOfSilenDebuff.RemainingTime(Runner); 
            }
            else if(TimeOfSlowDebuff.RemainingTime(Runner) > 0)
            {
                currentSlowTimeStatus = (float)TimeOfSlowDebuff.RemainingTime(Runner); 
            }
        }
        
        if (HasStateAuthority && TimeOfStunDebuff.Expired(Runner))
            {
            playerStat.isBeingStun = false;
            } 
        statusDebuffs[0].SetActive(playerStat.isBeingStun);
        if (HasStateAuthority && TimeOfSlowDebuff.Expired(Runner))
        {
            playerStat.isBeingSlow = false;
        }
        statusDebuffs[1].SetActive(playerStat.isBeingSlow);
        if (HasStateAuthority && TimeOfSilenDebuff.Expired(Runner))
        {
            playerStat.isBeingSilen = false;
        }
        statusDebuffs[2].SetActive(playerStat.isBeingSilen);
    }
    void CalculateCanvas()
    {
        statusCanvas.TimeRemainingBar.gameObject.SetActive
            ((TimeOfStunDebuff.RemainingTime(Runner) > 0|| TimeOfSilenDebuff.RemainingTime(Runner) > 0
            || TimeOfSlowDebuff.RemainingTime(Runner) > 0)&& state!=3);

        if (TimeOfStunDebuff.RemainingTime(Runner) > 0)
        {
            statusCanvas.TimeRemainingBar.UpdateBar(currentStunTimeStatus, maxStunTimeStatus);
        }
        else if (TimeOfSilenDebuff.RemainingTime(Runner) > 0)
        {
            statusCanvas.TimeRemainingBar.UpdateBar(currentSilenTimeStatus, maxSilenTimeStatus);
        }
        else if (TimeOfSlowDebuff.RemainingTime(Runner) > 0)
        {
            statusCanvas.TimeRemainingBar.UpdateBar(currentSlowTimeStatus, maxSlowTimeStatus);
        }
        statusCanvas.healthBarPlayer.UpdateBar(playerStat.currentHealth, playerStat.maxHealth);
        statusCanvas.statusBeingTMP.text =
         (playerStat.isBeingStun ? "Stunned " : "") + (playerStat.isBeingSlow ? "Slowed " : "") + (playerStat.isBeingSilen ? "Silened " : "");
        
        //xoay các bar để mọi player nhìn rõ
    }
    #endregion
    public Player_Types GetPlayerTypes()
    {
        return playerType;
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SkillRPC(int objectList, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun, bool isMakeSlow, bool isMakeSilen, float timeTrigger = 0f, float TimeEffect = 0f, int levelSkill = 1)
    {
        if(HasStateAuthority)
        {
            NetworkObject obj = Runner.Spawn(networkObjs.listNetworkObj[objectList], transform.position, transform.rotation,
                       Object.InputAuthority,
          onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
          {
              AttackObjects attObj = obj.GetComponent<AttackObjects>();
              if(attObj)
              {
                  attObj.SetUpPlayer(this, 0, isPhysicDamage, null,
                isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect);
              }
              DumbleAttackObjects dumObj = obj.GetComponent<DumbleAttackObjects>();
              if(dumObj)
              {
                  dumObj.SetUp(this, levelDamage, isPhysicDamage, null,
                isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect);
              }
              Shield newshield = obj.GetComponent<Shield>();
              if(newshield)
              {
                  newshield.maxDamageAbsorb = levelDamage;
                  newshield.currentDamageAbsorb = levelDamage;
              }
              BuffsOfPlayer buff= obj.GetComponent<BuffsOfPlayer>();
              if(buff)
              {
                  buff.levelSkill = levelSkill;
                  if (buff.canHeal)
                  {
                      playerStat.currentHealth += levelDamage;
                      if(playerStat.currentHealth < 0)
                      {
                          state = 3;
                          AnimatorTriggerRPC("Die");
                          if (HasStateAuthority) Singleton<AudioManager>.Instance.PlaySound(Singleton<AudioManager>.Instance.die);
                      }
                  }
              }
          });
            SetParentRPC(obj.Id);
        }
        
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SetParentRPC(NetworkId id)
    {
       if (! Runner.TryFindObject(id, out NetworkObject obj) ) return;
       if(obj.GetComponent<BuffsOfPlayer>() != null )
        {
            obj.transform.SetParent(transform.GetChild(2).GetChild(1));
        }
        if (obj.GetComponent<GrenadeController>() != null && playerType==Player_Types.Tesla)
        {
            obj.transform.SetParent(skill_2Transform);
        }
    }
    [Rpc(RpcSources.All, RpcTargets.All)] public void SetParentItemRPC(NetworkId id, int indexItemSlot)
    {
        if (!Runner.TryFindObject(id, out NetworkObject item)) return;
        item.transform.SetParent(buffFromItemManager.transform);
        item.GetComponent<Collider>().enabled = false;
        MeshRenderer[] meshItems = item.GetComponentsInChildren<MeshRenderer>();
        foreach (var mesh in meshItems)
        {
            mesh.enabled = false;
        }
    }
    void CheckPing()
    {
        if (HasStateAuthority)
        {
            PlayerController[] players = FindObjectsOfType<PlayerController>();
            foreach (var player in players)
            {
                double ms = Runner.GetPlayerRtt(player.Object.InputAuthority);
                Debug.Log(player.Object.InputAuthority.PlayerId + " ping " + ms + " ms");
            }
        }
    }
}

