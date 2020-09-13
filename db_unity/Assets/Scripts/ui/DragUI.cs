using UnityEngine;
using UnityEngine.EventSystems;

public class DragUI : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    Vector2 startPos;

    public Utils.Event<PointerEventData> OnDrag_X;
    public Utils.Event<PointerEventData> OnPointerDown_X;
    public Utils.Event<PointerEventData> OnPointerUp_X;

    private void Start()
    {
        startPos = transform.localPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 offset = eventData.position - eventData.pressPosition;
        transform.localPosition = startPos + offset;
        //Debug.Log("OnDrag offset=" + offset + ",localPosition" + transform.localPosition + ",startPos=" + startPos);
        Utils.TriggerEvent_Try(OnDrag_X, eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Utils.TriggerEvent_Try(OnPointerDown_X, eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localPosition = startPos;
        Utils.TriggerEvent_Try(OnPointerUp_X, eventData);
    }
}
