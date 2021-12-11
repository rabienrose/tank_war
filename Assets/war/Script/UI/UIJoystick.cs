using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIJoystick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public event Action onDragBegin;
    public event Action<Vector2> onDrag;
    public event Action onDragEnd;
    public Transform target;
    public float radius = 50f;
    public Vector2 position;
    private bool isDragging = false;
    private RectTransform thumb;
    void Start(){
        thumb = target.GetComponent<RectTransform>();
    }
    public void OnBeginDrag(PointerEventData data){
        isDragging = true;
        if(onDragBegin != null)
            onDragBegin();
    }
    public void OnDrag(PointerEventData data){
        RectTransform draggingPlane = transform as RectTransform;
        Vector3 mousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, data.position, data.pressEventCamera, out mousePos))
        {
            thumb.position = mousePos;
        }
        float length = target.localPosition.magnitude;
        if (length > radius)
        {
            target.localPosition = Vector3.ClampMagnitude(target.localPosition, radius);
        }
        position = target.localPosition;
        position = position / radius * Mathf.InverseLerp(radius, 2, 1);
    }
    void Update(){
        if(isDragging && onDrag != null)
            onDrag(position);
    }
    public void OnEndDrag(PointerEventData data){
        position = Vector2.zero;
        target.position = transform.position;
        isDragging = false;
        if (onDragEnd != null)
            onDragEnd();
    }
}