
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Main : MonoBehaviour
{
    public static int ScreenWidth_X;
    public static int ScreenHeight_Y;

    private void Awake()
    {
        SetResolution();
        Utils.Init(ScreenWidth_X, ScreenHeight_Y);
        Utils.SetCanvasMatchWidthOrHeight(gameObject);

        //online socket init();
        ResMgr.Init();
        GameConst.Init();
        ViewMgr.Init(gameObject);
    }

    private IEnumerator Start()
    {
        ViewMgr.Ins.CanvasSize = ViewMgr.Ins.CanvasTransfrom.sizeDelta;
        GameObject eventSystem = GameObject.Find("EventSystem");
        if (eventSystem != null)
        {
            EventSystem e = eventSystem.GetComponent<EventSystem>();
            if (e != null)
            {
                e.pixelDragThreshold = 5;
            }
        }

        //cfg
        yield return StartCoroutine(LoadSomeCfg());
        // start game
        ViewMgr.Ins.ShowView<LoginPanel>();
    }

    private IEnumerator LoadSomeCfg()
    {
        yield return new WaitForSeconds(1);
    }

    private void Update()
    {
        //online socket Update();
    }

    //高分屏修改分辨率
    private void SetResolution()
    {
        if (Screen.dpi > 500)
        {
            ScreenWidth_X = Screen.width / 2;
            ScreenHeight_Y = Screen.height / 2;
            Screen.SetResolution(ScreenWidth_X, ScreenHeight_Y, true);
        }
        else
        {
            ScreenWidth_X = Screen.width;
            ScreenHeight_Y = Screen.height;
        }
        Debug.Log("ScreenSize" + "ScreenWidth_X=" + ScreenWidth_X + ",ScreenHeight_Y=" + ScreenHeight_Y);
    }
}