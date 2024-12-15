using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkillDirection : NetworkBehaviour
{
    [SerializeField] GameObject ImageParent;
    public float skillDuration = 0.5f; // Thời gian kỹ năng
    Vector3 direction, directionNormalize, fixPosition;
    PlayerController player;
    Vector3? posMouseUp;
    private Rect rightArea;
    private void Start()
    {
        
    }
    
    public void GetMouseUp(out Vector3? posMouseUp)
    {
       // ImageParent.transform.position = fixPosition;
        posMouseUp=this.posMouseUp;
        ImageParent.GetComponentInChildren<Image>().enabled = false;
    }

    public override void Spawned()
    {
        base.Spawned();
        player = ImageParent.transform.parent.parent.GetComponent<PlayerController>();
        rightArea = new Rect(0.5f, 0, 0.5f, 1);  // Vùng tay phải (skill buttons)
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        GetMouse();
    }

    public void GetMouse()
    {
#if UNITY_EDITOR
        CalculateDirection(Input.mousePosition);
#elif UNITY_ANDROID || UNITY_IOS
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            
            if (rightArea.Contains(Camera.main.ScreenToViewportPoint(touch.position)))
            {
                CalculateDirection(touch.position);
            }
        }
#elif UNITY_STANDALONE
        Debug.Log("Chạy trên PC (Windows, macOS, Linux)");
#else
        Debug.Log("Nền tảng khác");

#endif

    }
    void CalculateDirection(Vector3 posMouseTouch)
    {
        if (!rightArea.Contains(Camera.main.ScreenToViewportPoint(posMouseTouch))) return;
        Vector3 mousePosition = posMouseTouch;
        mousePosition.z = Camera.main.nearClipPlane;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        int layer = LayerMask.NameToLayer("Ground");
        LayerMask layerMask = 1 << layer;
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 200, 1 << layer))
        {
            
            Vector3 Position_y0 = new Vector3(transform.position.x, 0, transform.position.z);

            if (player.transform.forward == null) return;
            float t = Vector3.Dot(hitInfo.point - Position_y0, player.transform.forward) / Vector3.Dot(player.transform.forward, player.transform.forward);
            posMouseUp = Position_y0 + t * player.transform.forward;

            direction = (Vector3)posMouseUp - Position_y0;
            if (Vector3.Dot(player.transform.forward, direction) > 0 && direction.magnitude > 2)
            {
                if (direction.magnitude > 20)
                {
                    posMouseUp = Position_y0 + direction.normalized * 20;
                }
               
                ImageParent.transform.rotation = Quaternion.LookRotation(player.transform.forward);

                ImageParent.transform.localScale = new Vector3(1, 1, 0.22f * ((Vector3)posMouseUp - Position_y0).magnitude);
                ImageParent.transform.position = Position_y0 + 0.5f * ((Vector3)posMouseUp - Position_y0);
            }
            else posMouseUp = null;
        }
    }
    public void GetMouseDown()
    {
        ImageParent.GetComponentInChildren<Image>().enabled = true;
    }

}
