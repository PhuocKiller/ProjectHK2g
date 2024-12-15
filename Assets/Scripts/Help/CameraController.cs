using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    CinemachineFreeLook freeLookCamera;
    PlayerController player;
    public PlayerController closestEnemyPlayer;
    public float m_SplineCurvature;
    bool isFollowEnemy;
    void Start()
    {
        freeLookCamera = GetComponent<CinemachineFreeLook>();
        Singleton<CinemachineBrain>.Instance.m_ShowDebugText = true;
        CinemachineCore.GetInputAxis = GetAxisCustom;
        
#if UNITY_EDITOR
        freeLookCamera.m_XAxis.m_MaxSpeed = 1000f;
        freeLookCamera.m_YAxis.m_MaxSpeed = 5f;
#elif UNITY_ANDROID || UNITY_IOS
freeLookCamera.m_XAxis.m_MaxSpeed = 150f;
freeLookCamera.m_YAxis.m_MaxSpeed = 1f;
#endif

    }
    private void Update()
    {
     if(isFollowEnemy)
        {
            Vector3 newPosclosestEnemyPlayer = new Vector3(closestEnemyPlayer.transform.position.x, 0, closestEnemyPlayer.transform.position.z);
            Quaternion look=Quaternion.LookRotation(newPosclosestEnemyPlayer - player.transform.position,Vector3.up);
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, look, 15 * Time.deltaTime);
            freeLookCamera.m_XAxis.Value =Mathf.LerpAngle(freeLookCamera.m_XAxis.Value, player.transform.rotation.eulerAngles.y, 15 * Time.deltaTime);
        }   
    }
    public void SetFollowCharacter(Transform characterTransform)
    {
        freeLookCamera.Follow = characterTransform;
        freeLookCamera.LookAt = characterTransform;
        freeLookCamera.m_XAxis.Value = characterTransform.rotation.eulerAngles.y;
        player = characterTransform.gameObject.GetComponent<PlayerController>();
    }
    public void RemoveFollowCharacter()
    {
        freeLookCamera.Follow = null;
        
    }
    public void CameraFollowEnemy()
    {
        isFollowEnemy = true;
        closestEnemyPlayer = player.overlapSphere.closestEnemyPlayer;
    }
    public void CameraUnFollowEnemy()
    {
        isFollowEnemy = false;
        closestEnemyPlayer = null;
    }
    public void MoveCameraUp()
    {
        freeLookCamera.m_YAxis.Value = Mathf.Lerp(freeLookCamera.m_YAxis.Value, 1, 0.5f * Time.deltaTime); //nâng cao camera khi xài skill
    }
    public float GetAxisCustom(string axisName)
    {

#if UNITY_EDITOR
        if (player == null) return 0f;
        if (axisName == "Mouse X")
        {
            if (Input.GetKey("mouse 0") && player.GetCurrentState() == 0
                && Input.mousePosition.x > Screen.width / 2)
            {
                return UnityEngine.Input.GetAxis("Mouse X");

            }
            else
            {
                return 0;
            }
        }
        else if (axisName == "Mouse Y")
        {
            if (Input.GetKey("mouse 0") && player.GetCurrentState() == 0
                && Input.mousePosition.x > Screen.width / 2)
            {
                return UnityEngine.Input.GetAxis("Mouse Y");
            }
            else
            {
                return 0;
            }
        }
        return UnityEngine.Input.GetAxis(axisName);
#elif UNITY_ANDROID || UNITY_IOS
        
        if(player == null ||player.GetCurrentState() != 0) return 0f;
        int touchX; int touchY;
        for (int i = 0; i < Input.touchCount; i++)
        {
        Touch touch = Input.GetTouch(i);
        if (axisName == "Mouse X")
        {
            if (touch.position.x > Screen.width / 2)
            {
                 return touch.deltaPosition.x *0.1f;
            }
            else 
            {
                touchX=0;
            }
        }
        else if (axisName == "Mouse Y")
        {
            if (touch.position.x > Screen.width / 2)
            {
                return touch.deltaPosition.y *0.1f;
            }
            else 
            {
                touchY=0;
            }
        }
        
        }
        return 0;
       
#elif UNITY_STANDALONE
        Debug.Log("Chạy trên PC (Windows, macOS, Linux)");
#else
        Debug.Log("Nền tảng khác");

#endif
    }
}
