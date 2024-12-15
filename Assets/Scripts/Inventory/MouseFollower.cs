using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;
    public ItemDragHandler itemDragHandler;
    

    public void Awake()
    {
        canvas = transform.root.GetComponent<Canvas>();
      //  Toggle(false);
    }

   
    void Update()
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            Input.mousePosition,
            canvas.worldCamera,
            out position
                );
        transform.position = canvas.transform.TransformPoint(position);
    }
    
    public void Toggle(bool val)
    {
        gameObject.SetActive(val);
    }
}
