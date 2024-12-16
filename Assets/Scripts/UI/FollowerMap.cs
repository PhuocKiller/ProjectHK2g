using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class FollowerMap: MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform uiMinimapRect; // Đối tượng RectTransform của UI Con
    private RectTransform MapRectTransform; // Đối tượng RectTransform của UI Cha
    private Canvas canvas;
    public Camera followCamera;
    public CinemachineFreeLook freelookCamera;
    private void Start()
    {
        uiMinimapRect = GetComponent<RectTransform>();
        MapRectTransform = uiMinimapRect.parent.GetComponent<RectTransform>();
        canvas = transform.root.GetComponent<Canvas>();
    }

    // Hàm này sẽ được gọi khi bắt đầu kéo
    public void OnBeginDrag(PointerEventData eventData)
    {
        followCamera.enabled = true;
        freelookCamera.enabled = false;
    }

    // Hàm này sẽ được gọi trong khi kéo
    public void OnDrag(PointerEventData eventData)
    {
        // Lấy vị trí chuột trong không gian canvas
        Vector2 localPointerPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle
            (MapRectTransform, eventData.position, eventData.pressEventCamera, out localPointerPosition);
        
        // Tính toán vị trí mới của UI Con và đảm bảo nó không vượt ra ngoài phạm vi của UI Cha
        Vector2 clampedPosition = new Vector2(
            Mathf.Clamp(localPointerPosition.x, -(MapRectTransform.rect.width - uiMinimapRect.rect.width) * 0.5f, (MapRectTransform.rect.width - uiMinimapRect.rect.width)*0.5f),
            Mathf.Clamp(localPointerPosition.y, -(MapRectTransform.rect.height - uiMinimapRect.rect.height) * 0.5f, (MapRectTransform.rect.height - uiMinimapRect.rect.height)*0.5f)
        );
        transform.position = MapRectTransform.transform.TransformPoint(clampedPosition);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        FindObjectOfType<MinimapCameraController>().BackToPlayer();
        followCamera.enabled = false;
        freelookCamera.enabled = true;
    }
}
