using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.UI;
public enum BuildingType
{
    Tower,
    BaseHouse
}
public class BuildingController : NetworkBehaviour,ICanTakeDamage
{
    NetworkManager runnerManager;
    public CharacterController targetCharacter;
    GameManager gameManager;
    OverlapSphereTower overlapSphere;
    PlayerScore playerScore;
    Bars hpBar;
    public Transform weapon, shootPosition;
    [Networked] public int playerTeam { get; set; }
    [Networked] public int towerID { get; set; }

    CharacterController buildingController;
    public BuildingType buildingType;
    [Networked] public int state { get; set; }
    
    [Networked]  int currentHealth { get; set; }
    [Networked]  int maxHealth { get; set; }
    [Networked] public int damage { get; set; }
    [Networked] public int defend { get; set; }
    [Networked] public bool isLive { get; set; }
    [Networked] bool isAttack { get; set; }
    [Networked] TickTimer TimeOfAttack { get; set; }
    [Networked]  bool isBeingDestroy { get; set; }
    
    public Mesh[] meshTower;
    public MeshFilter[] meshVisualTower;
    public GameObject[] shootVFX;
    public MeshRenderer sphereRender;
    [SerializeField] Sprite[] collapseImages;
    [SerializeField] Sprite[] startImages;
    public Image minimapImage;
    public override void Spawned()
    {
        base.Spawned();
        runnerManager = FindObjectOfType<NetworkManager>();
        gameManager = FindObjectOfType<GameManager>();
        hpBar = GetComponentInChildren<Bars>();
        playerScore= GetComponentInChildren<PlayerScore>();
        overlapSphere= GetComponentInChildren<OverlapSphereTower>();
        
        buildingController = GetComponent<CharacterController>();
        if(buildingType==BuildingType.Tower)
        {
            minimapImage.sprite = startImages[playerTeam];
            for (int i = 0; i < 3; i++)
            {
                meshVisualTower[i].mesh = meshTower[i + 3 * playerTeam];
            }
            TimeOfAttack = TickTimer.CreateFromSeconds(Runner, 0);
            maxHealth = 5 + gameManager.levelCreep * 50;
        }
        else
        {
            maxHealth = 10 + gameManager.levelCreep * 50;
        }
        currentHealth = maxHealth;
        state = 0;
        minimapImage.gameObject.SetActive(true);
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        hpBar.UpdateBar(currentHealth, maxHealth);
        hpBar.transform.rotation = Quaternion.AngleAxis(Camera.main.transform.rotation.eulerAngles.y, Vector3.up);
        if (state == 3)
        {
            if (isBeingDestroy)
            {
                Material mat1 = meshVisualTower[1].GetComponent<MeshRenderer>().material;
                ControlMaterial(3, mat1, mat1.color.a - 0.5f * Runner.DeltaTime, 3000);
                Material mat2 = meshVisualTower[2].GetComponent<MeshRenderer>().material;
                ControlMaterial(3, mat2, mat2.color.a - 0.5f * Runner.DeltaTime, 3000);
                if (mat1.color.a < 0.1f || mat2.color.a < 0.1f)
                {
                    isBeingDestroy = false;
                    TowerCollapse();
                }
            }
            return;
        }
        defend = buildingType == BuildingType.Tower ? 10 : 15 + Mathf.FloorToInt(gameManager.levelCreep * (buildingType == BuildingType.Tower ? 0.5f : 0.75f));
        damage = 100 + gameManager.levelCreep * 2;
        bool isHaveEnemy = overlapSphere.CheckAllEnemyAround(30).Count > 0;
        if (buildingType == BuildingType.Tower) sphereRender.enabled = isHaveEnemy;

        if (!isHaveEnemy)
        {
            isAttack = false;
        }
        else //có enemy xung quanh
        {
            if (overlapSphere.CheckPlayerFollowEnemy(overlapSphere.CheckAllEnemyAround(30)).Count == 0)// nhưng ko có player follow
            {
                targetCharacter = overlapSphere.FindClosestCharacterInRadius(overlapSphere.CheckAllEnemyAround(30), transform.position);
            }
            else //có player follow
            {
                targetCharacter = overlapSphere.FindClosestPlayerFollowInRadius
                    (overlapSphere.CheckPlayerFollowEnemy(overlapSphere.CheckAllEnemyAround(30)), transform.position)
                    .GetComponent<CharacterController>();
            }
            isAttack = true;
        }
        if (isAttack && TimeOfAttack.Expired(Runner) && HasStateAuthority)
        {
            if (buildingType == BuildingType.Tower)
            {
                NormalAttack();
            }
            TimeOfAttack = TickTimer.CreateFromSeconds(Runner, 1);
        }
        if (buildingType == BuildingType.Tower) CalculateWeaponRotate(isHaveEnemy);
    }
    void CalculateWeaponRotate(bool isHaveEnemy)
    {
        Vector3 targetLookPos = isHaveEnemy ? targetCharacter.transform.position : (runnerManager.spawnPointPlayer[playerTeam == 0 ? 1 : 0].position);
        Quaternion look = Quaternion.LookRotation
           ((targetLookPos - transform.position).normalized);
        weapon.rotation = Quaternion.RotateTowards(weapon.rotation, look, 180 * Runner.DeltaTime);
    }
    public virtual void NormalAttack()
    {
        Runner.Spawn(shootVFX[playerTeam], shootPosition.position, shootPosition.rotation, inputAuthority: Object.InputAuthority
       , onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
         {
            obj.GetComponent<AttackObjectsTower>().SetUpTower(this, damage, null,3f);
            obj.GetComponent<AttackObjectsTower>().SetDirection(targetCharacter);
         });
            
    }
   
    public void ApplyDamage(int damage, bool isPhysicDamage, PlayerController player,
        Action<int, bool> counter = null, Action<int, List<PlayerController>> isKillPlayer = null,
        Action<Vector3, float> isKillCreep = null,
        Action<int> lifeSteal = null, bool activeInjureAnim = true, bool isCritPhysic = false)
    {
        BuildingController[] allBuildings = FindObjectsOfType<BuildingController>();
        var inFrontOfBuildings = allBuildings.Where(s => s.towerID < towerID && s.state != 3 && s.playerTeam == playerTeam).ToArray();
        if (inFrontOfBuildings.Count() > 0) return;
        CalculateHealthRPC(damage, isPhysicDamage, player, activeInjureAnim, isCritPhysic);
    }
    public void ApplyEffect(PlayerRef player, bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float TimeEffect = 0f, Action callback = null)
    {
        callback?.Invoke();
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void CalculateHealthRPC
        (int damage, bool isPhysicDamage, PlayerController player, bool activeInjureAnim = true, bool isCritPhysic = false)
    {
        if (!playerScore.playersMakeDamages.Contains(player))
        {
            playerScore.playersMakeDamages.Add(player);
        }
       
        if (currentHealth  > damage)
        {
          currentHealth -= damage;
        }
        else
        {
            WhenTowerDestroy();
        }
    }
    void WhenTowerDestroy()
    {
        currentHealth = 0;
        isLive = false;
        state = 3;
       
        if (buildingType == BuildingType.Tower)
        {
            minimapImage.sprite = collapseImages[playerTeam];
            PlayerController[] playersInEnemyTeam = FindObjectsOfType<PlayerController>();
            if (playersInEnemyTeam != null)
            {
                foreach (var playerEnemy in playersInEnemyTeam)
                {
                    if (playerEnemy.playerTeam != playerTeam)
                    {
                        CalculateXPWhenKill(playerEnemy);
                        CalculateCoinsWhenKill(playerEnemy);
                    }
                }
            }
        }
           
        GetComponent<CharacterController>().enabled = false;
        HideVisualOfTower();
    }
    public void TowerCollapse()
    {
        meshVisualTower[1].GetComponent<MeshRenderer>().enabled = false;
        meshVisualTower[2].GetComponent<MeshRenderer>().enabled = false;
        if(buildingType==BuildingType.Tower) sphereRender.enabled=false;
        hpBar.gameObject.SetActive(false);
        if(buildingType == BuildingType.BaseHouse)
        {
            Debug.Log("Team" + (playerTeam==0?1:0) +" Win" );
        }
    }
    public void HideVisualOfTower()
    {
        isBeingDestroy = true;
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
    
    void CalculateXPWhenKill(PlayerController playerEnemy)
    {
       playerEnemy.playerStat.GainXPWhenKill(1000);
    }
    void CalculateCoinsWhenKill(PlayerController playerEnemy)
    {
       playerEnemy.playerStat.GainCoinWhenKill(500);
    }
    
}
