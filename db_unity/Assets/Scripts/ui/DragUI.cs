using UnityEngine;
using UnityEngine.EventSystems;

public class DragUI : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    Vector2 startPos;
    Vector2 pressWorldPos;

    public bool isDragGameObject;

    public Utils.Event<PointerEventData> OnDrag_X;
    public Utils.Event<PointerEventData> OnPointerDown_X;
    public Utils.Event<PointerEventData> OnPointerUp_X;

    public void OnDrag(PointerEventData eventData)
    {
        if(isDragGameObject)
        {
            Vector2 curWorld = eventData.pointerCurrentRaycast.worldPosition;
            Vector3 localPos = transform.InverseTransformPoint(curWorld); //当前
            Vector3 localPosPress = transform.InverseTransformPoint(pressWorldPos);//按下
            Vector2 offset = localPos - localPosPress;
            transform.localPosition = startPos + offset;
        }

        Utils.TriggerEvent_Try(OnDrag_X, eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        startPos = transform.localPosition;
        pressWorldPos = eventData.pointerCurrentRaycast.worldPosition;
        Utils.TriggerEvent_Try(OnPointerDown_X, eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localPosition = startPos;
        Utils.TriggerEvent_Try(OnPointerUp_X, eventData);
    }
}
