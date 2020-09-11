using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventListener : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public delegate void VoidDelegate(GameObject go);

    public static PointerEventData pointEventData;
    public string SoundName;

    public VoidDelegate onClick;
    public VoidDelegate onDown;
    public VoidDelegate onUp;
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onBeginDrag;
    public VoidDelegate onDrag;
    public VoidDelegate onEndDrag;
    public object parameter;
    public static UIEventListener Get(GameObject go, string soundName = "")
    {
        UIEventListener listener = go.GetComponent<UIEventListener>();
        if (listener == null)
            listener = go.AddComponent<UIEventListener>();
        listener.SoundName = soundName;
        return listener;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        pointEventData = eventData;
        if (onClick != null) onClick(gameObject);
      //  if (string.IsNullOrEmpty(SoundName))
     //       AudioManager.Ins.Play2D("ui_dianji");
        else if (SoundName == "no")
        {

        }
        else
        {
      //      AudioManager.Ins.Play2D(SoundName);
        }
        eventData.Reset();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointEventData = eventData;
        if (onDown != null) onDown(gameObject);
        eventData.Reset();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointEventData = eventData;
        if (onUp != null) onUp(gameObject);
        eventData.Reset();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointEventData = eventData;
        if (onEnter != null) onEnter(gameObject);
        eventData.Reset();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointEventData = eventData;
        if (onExit != null) onExit(gameObject);
        eventData.Reset();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        pointEventData = eventData;
        if (onBeginDrag != null) onBeginDrag(gameObject);
        //eventData.Reset();
    }

    public void OnDrag(PointerEventData eventData)
    {
        pointEventData = eventData;
        if (onDrag != null) onDrag(gameObject);
        eventData.Reset();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        pointEventData = eventData;
        if (onEndDrag != null) onEndDrag(gameObject);
        eventData.Reset();
    }
}