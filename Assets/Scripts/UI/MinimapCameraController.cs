using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinimapCameraController : MonoBehaviour
{
    public PlayerController player;
    public Camera miniMapCamera;
    public Camera followCamera;   
    public RectTransform uiMinimapRect; 
    public Vector2 minimapSize = new Vector2(200, 200); 

    private float mapWidth = 480;   
    private float mapHeight = 480;
    private void OnEnable()
    {
        StartCoroutine(DelayCheckPlay());
    }
    void Update()
    {
        if (player == null) return;
        Vector2 localPos = uiMinimapRect.anchoredPosition;

        // Chuyển đổi từ tọa độ của UI minimap sang tọa độ thực tế trong game
        float xPos = (localPos.x / minimapSize.x) * mapWidth;
        float zPos = (localPos.y / minimapSize.y) * mapHeight;
        followCamera.transform.position = Quaternion.AngleAxis(player.playerTeam == 0 ? 90 : -90, Vector3.up)
            * new Vector3(xPos, 50, zPos);
        miniMapCamera.transform.rotation = Quaternion.AngleAxis(90, Vector3.right) *
                                 Quaternion.AngleAxis(player.playerTeam == 0 ? -90 : 90, Vector3.forward);
        followCamera.transform.rotation = Quaternion.AngleAxis(90, Vector3.right) *
                         Quaternion.AngleAxis(player.playerTeam == 0 ? -90 : 90, Vector3.forward);
    }
    IEnumerator DelayCheckPlay()
    {
        yield return new WaitForSeconds(0.2f);
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        this.player = player;
    }
    public void BackToPlayer()
    {
        Vector3 posMiniMapInWorld = Quaternion.AngleAxis(player.playerTeam == 0 ? -90 : 90, Vector3.up)
            * new Vector3(player.transform.position.x, 50, player.transform.position.z);
        Vector2 localPos = new Vector2(posMiniMapInWorld.x * minimapSize.x / mapWidth, posMiniMapInWorld.z * minimapSize.y / mapHeight);
        uiMinimapRect.anchoredPosition=localPos;
    }
}
