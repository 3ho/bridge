using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

public partial class AlertPanel : View
{
    private static AlertPanel _ins;
    private static string recentTip = "";
    private Vector3 _mScale;
    private int timerId;

    protected override void onInit()
    {
        _ins = this;
        _mScale = txt_alertBox.transform.localScale;
        SetLabelText(txt_alertBox, "");
        UIEventListener.Get(m_alert).onClick = OnClick;
    }

    protected override void onShow(System.Object param = null, string childView = null)
    {
        if (param == null)
        {
            SetLabelText(txt_alertBox, recentTip);
        }
        else
        {
            SetLabelText(txt_alertBox, param);
        }
        PlayAnimation();
        Invoke("myHide", 2f);
    }

    protected override void onHide(string childView = null)
    {
        CancelInvoke("myHide");
        recentTip = "";
    }

    public static void Show(string tip = "此功能正在玩命开发，即将开启")
    {
        ViewMgr.Ins.Destroy<AlertPanel>();
        //ViewMgr.Ins.HideView<AlertPanel>();
        ViewMgr.Ins.ShowTopView<AlertPanel>(tip);
    }

    private void OnClick(GameObject go)
    {
        Hide();
        ViewMgr.Ins.Destroy<AlertPanel>();
    }

    private void OnTimeOut(System.Object[] args)
    {
        Hide();
        ViewMgr.Ins.Destroy<AlertPanel>();
    }

    private void PlayAnimation()
    {
        //m_alert.transform.localScale = new Vector3(0.3f, 0.3f);
        //StartCoroutine(dsdsdsds(m_alert.transform, new Vector3(1, 1, 1), 0.3f));
        Vector3 target = m_alert.transform.localPosition + new Vector3(0, 200, 0);
        StartCoroutine(slowPositionTo(m_alert.transform, target, 1f, 1f));
        StartCoroutine(slowAlphaTo(m_alert, 0.2f, 1f, 1f));
        //StartCoroutine(slowScaleTo(m_alert.transform, new Vector3(0.8f, 0.8f), 0.9f, 1f));
    }

    public static IEnumerator slowScaleTo(Transform tf, Vector3 target, float delayTime, float time)
    {
        yield return new WaitForSeconds(delayTime);
        Vector3 start = tf.localScale;
        float startTime = Time.time;
        while (Time.time - startTime < time)
        {
            tf.localScale = start + (Time.time - startTime) / time * (target - start);
            yield return null;
        }
        tf.localScale = target;
    }

    private IEnumerator slowPositionTo(Transform tf, Vector3 target, float delayTime, float time)
    {
        yield return new WaitForSeconds(delayTime);
        Vector3 start = tf.localPosition;
        float startTime = Time.time;
        while (Time.time - startTime < time)
        {
            tf.localPosition = start + (Time.time - startTime) / time * (target - start);
            yield return null;
        }
        tf.localPosition = target;
    }

    private IEnumerator slowAlphaTo(GameObject go, float target, float delayTime, float time)
    {
        yield return new WaitForSeconds(delayTime);
        CanvasGroup cG = go.GetComponent<CanvasGroup>();
        if (cG != null)
        {
            float start = cG.alpha;
            float startTime = Time.time;
            while (Time.time - startTime < time)
            {
                cG.alpha = start + (Time.time - startTime) / time * (target - start);
                yield return null;
            }
            cG.alpha = target;
        }
    }

    private void myHide()
    {
        ViewMgr.Ins.Destroy<AlertPanel>();
        Hide();
    }
}
