using Fusion;
using Fusion.Editor;
using Fusion.Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    public string playerID;
   public NetworkRunner runner;
    public NavMeshSurface navMesh;
    [SerializeField]
    GameObject gameManagerObj, playerManagerObj;
    [SerializeField]
    public GameObject[] players, creeps,buildings, basicItems,shieldItems,armorItems,weaponItems,bootItems, onlineItems;
    public float[] itemsDropChance;
    GameNetworkCallBack gameNetworkCallBack;
    [SerializeField]
    UnityEvent onConnected;
    [SerializeField] public Transform[] spawnPointPlayer, spawnPointCreep, spawnPointTower, spawnPointBase;
    public int playerIndex, playerTeam;
    bool flagLogin;
    

    private void Awake()
    {
        runner = GetComponent<NetworkRunner>();
        gameNetworkCallBack = GetComponent<GameNetworkCallBack>();
    }
   
    private void SpawnPlayer(NetworkRunner m_runner, PlayerRef player)
    {
        bool flag = false;
        foreach (var playerObject in Login.playersGame)
        {
            if (playerObject.Key == m_runner.GetPlayerUserId(player))
            {
                if (player == m_runner.LocalPlayer)
                {
                    playerObject.Value.Object.RequestStateAuthority();
                    playerObject.Value.playerCallBack.CallBackReconect();
                }
                flag = true;
            }
        }
        if (flag == true) return;
        if (player == runner.LocalPlayer && runner.IsSharedModeMasterClient)
        {
            SpawnWhenStartGame(m_runner, player);
        }
        
        if (player == runner.LocalPlayer)
        {
            NetworkObject characterObj = runner.Spawn(players[playerIndex],
                spawnPointPlayer[playerTeam].position +Vector3.right*5*(playerTeam==0?1:-1), spawnPointPlayer[playerTeam].rotation,
                inputAuthority: player,
                onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
                {
                    obj.GetComponent<PlayerController>().playerID= runner.GetPlayerUserId(player);
                    
                    obj.GetComponent<PlayerController>().playerTeam = playerTeam;
                });
        }
    }
    void SpawnWhenStartGame(NetworkRunner m_runner, PlayerRef player)
    {
        runner.Spawn(gameManagerObj, inputAuthority: player);
        runner.Spawn(playerManagerObj, inputAuthority: player);
        for (int i = 0; i < spawnPointTower.Length; i++)
        {
            NetworkObject towerObject = runner.Spawn(buildings[0], spawnPointTower[i].position, spawnPointTower[i].rotation, player,
              onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
              {
                  obj.GetComponent<BuildingController>().playerTeam = i <= 3 ? 0 : 1;

                  obj.GetComponent<BuildingController>().towerID = i <= 2 ? i :(( i==3 ||i==7) ?2: i - 4);
              });
        }
        for (int i = 0; i < spawnPointBase.Length; i++)
        {
            NetworkObject towerObject = runner.Spawn(buildings[i+1], spawnPointBase[i].position, spawnPointBase[i].rotation, player, //building 0 là tower
              onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
              {
                  obj.GetComponent<BuildingController>().playerTeam = i;
                  obj.GetComponent<BuildingController>().towerID = 3 + 3 * i;
              });
        }
        for (int i = 0; i < 2; i++)
        {
            NetworkObject baseRegen = runner.Spawn(buildings[3], spawnPointPlayer[i].position, spawnPointPlayer[i].rotation, player, //building 0 là tower
              onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
              {
                  obj.GetComponent<BaseRegen>().SetUp(i,0.05f);
              });
        }
        // navMesh.BuildNavMesh();
    }
    public void SpawnCreep(PlayerRef player)
    {
        if (!runner.IsSharedModeMasterClient) return;
        SpawnMeleeCreep(player);
        SpawnRangeCreep(player);
    }
    void SpawnMeleeCreep(PlayerRef player)
    {
        for (int i = 0; i < 2; i++)
        {
            
            runner.Spawn(creeps[0], spawnPointCreep[0].position + Vector3.left * 2f * i, spawnPointCreep[0].rotation,
                             inputAuthority: player,
                           onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
                           {
                               obj.GetComponent<CreepController>().playerTeam = 0;
                           });
            runner.Spawn(creeps[0], spawnPointCreep[1].position  + Vector3.right * 2f * i, spawnPointCreep[1].rotation,
                 inputAuthority: player,
               onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
               {
                   obj.GetComponent<CreepController>().playerTeam = 1;
               });
        }
    }
    void SpawnRangeCreep(PlayerRef player)
    {
        runner.Spawn(creeps[1], spawnPointCreep[0].position + Vector3.left * 6, spawnPointCreep[0].rotation,
                                inputAuthority: player,
                              onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
                              {
                                  obj.GetComponent<CreepController>().playerTeam = 0;
                              });
        runner.Spawn(creeps[1], spawnPointCreep[1].position + Vector3.right * 6, spawnPointCreep[1].rotation,
                             inputAuthority: player,
                           onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
                           {
                               obj.GetComponent<CreepController>().playerTeam = 1;
                           });
    }
    public void SpawnObjWhenAddItem(GameObject itemObj, int indexItemSlot)
    {
        NetworkObject item=runner.Spawn(itemObj);
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        player.SetParentItemRPC(item.Id, indexItemSlot);
        MeshRenderer[] allmeshs= item.GetComponentsInChildren<MeshRenderer>();
        if (allmeshs.Length > 0)
        {
            foreach (MeshRenderer mesh in allmeshs)
            {
                Destroy(mesh);
            }
        }
    }
    public void DestroyObjWhenRemoveItem(string name)
    {
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        foreach (Transform item in player.buffFromItemManager)
        {
            if (item.GetComponent<InventoryItemBase>().Name == name)
            {
                Destroy(item.gameObject);
                break;
            }
        }
    }
    public void SpawnItemFromCreep(int indexItem,Vector3 posSpawn)
    {
        NetworkObject item = runner.Spawn(basicItems[indexItem], posSpawn);
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
    }
    public GameObject FindItemBaseOnName(string name)
    {
        for (int i = 0; i <basicItems.Length; i++)
        {
            if (name== basicItems[i].GetComponent<InventoryItemBase>().Name) return basicItems[i];
        }
        for (int i = 0; i < shieldItems.Length; i++)
        {
            if (name == shieldItems[i].GetComponent<InventoryItemBase>().Name) return shieldItems[i];
        }
        for (int i = 0; i < armorItems.Length; i++)
        {
            if (name == armorItems[i].GetComponent<InventoryItemBase>().Name) return armorItems[i];
        }
        for (int i = 0; i < weaponItems.Length; i++)
        {
            if (name == weaponItems[i].GetComponent<InventoryItemBase>().Name) return weaponItems[i];
        }
        for (int i = 0; i < bootItems.Length; i++)
        {
            if (name == bootItems[i].GetComponent<InventoryItemBase>().Name) return bootItems[i];
        }
        return null;
    }
    public int FindOnlineItemsIndex(string name)
    {
        for (int i = 0; i < onlineItems.Length; i++)
        {
            if (name == onlineItems[i].GetComponent<InventoryItemBase>().Name) return i;
        }
        return -1 ;
    }
    public async void OnClickBtn(Button btn)
    {
        if (playerID != "" &&runner!=null)
        {
            btn.interactable = false;
            Singleton<Loading>.Instance.ShowLoading();
            gameNetworkCallBack ??= GetComponent<GameNetworkCallBack>();
            gameNetworkCallBack.OnPlayerJoinRegister(SpawnPlayer);
            await runner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.Shared,
                SessionName = "Begin",
                CustomLobbyName = "VN",
                SceneManager = GetComponent<LoadSceneManager>(),
                AuthValues = new AuthenticationValues()
                {
                    UserId= playerID,
                }
            });
            btn.interactable = true;
            onConnected?.Invoke();
            Singleton<Loading>.Instance.HideLoading();
            btn.gameObject.SetActive(false);
        }
        else
        {
            Singleton<AudioManager>.Instance.PlaySound(Singleton<AudioManager>.Instance.error);
        }
    }
    public void ShutdownRunner()
    {
        if (runner.IsRunning)
        {
            runner.Shutdown(false, forceShutdownProcedure: true);
        }
    }
}
