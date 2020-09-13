using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ViewMgr : MonoBehaviour
{
    public static ViewMgr Ins = null;

    private const string VIEW_AB_PATH = @"ui/";
    private const int VIEW_CACHE_CAPACITY = 6;

    private const int DefaultLayer = 0;
    private const int TopLayer = 1;
    private const int LayerNum = 2;

    public Camera UICamera { get; private set; }

    public RectTransform CanvasTransfrom;
    public Vector2 CanvasSize = new Vector2(1334, 750);
    private readonly List<Transform> layerTransforms = new List<Transform>();

    private readonly LRU<string, View> mViews = new LRU<string, View>(VIEW_CACHE_CAPACITY);
    private readonly HashSet<string> mLoading = new HashSet<string>();
    private readonly HashSet<string> mVisible = new HashSet<string>();
    private readonly List<string> mStack = new List<string>();

    private string mLastViewName;

    private Type mLoadingView = null;

    // Show Hide Events
    private Utils.StringDelegate mOnShowEvent;

    private Utils.StringDelegate mOnHideEvent;

    private readonly Dictionary<string, Utils.VoidDelegate> mDicOnShowEvent =
        new Dictionary<string, Utils.VoidDelegate>();

    private readonly Dictionary<string, Utils.VoidDelegate> mDicOnHideEvent =
        new Dictionary<string, Utils.VoidDelegate>();


    private void Awake()
    {
        Ins = this;

        CanvasTransfrom = transform as RectTransform;
        UnityEngine.Object.DontDestroyOnLoad(CanvasTransfrom);
        UICamera = UnityEngine.Object.FindObjectOfType<Camera>();
        mViews.onRemoveEntry = OnCacheOverflow;

        for (int i = 0; i < LayerNum; ++i)
        {
            RectTransform layerTrans = new GameObject("Layer_" + i, typeof(RectTransform)).transform as RectTransform;
            layerTrans.SetParent(CanvasTransfrom, false);
            layerTrans.localPosition = Vector3.zero;
            layerTrans.localScale = Vector3.one;
            if (Utils.IsIphoneX())
            {              
                layerTrans.anchorMin = new Vector2(0.5f, 0.5f);
                layerTrans.anchorMax = new Vector2(0.5f, 0.5f);
                layerTrans.pivot = new Vector2(0.5f, 0.5f);
                layerTrans.sizeDelta = new Vector2(2190, 1100);
            }
            if (Utils.IsVivo())
            {
                Debug.LogError("IsVivoFullScreen");
                layerTrans.anchorMin = new Vector2(0.5f, 0.5f);
                layerTrans.anchorMax = new Vector2(0.5f, 0.5f);
                layerTrans.pivot = new Vector2(0.5f, 0.5f);
                layerTrans.sizeDelta = new Vector2(2060, 1080);
            }
            else
            {
                //Debug.LogError("Model="+ SystemInfo.deviceModel);
                layerTrans.anchorMin = Vector2.zero;
                layerTrans.anchorMax = Vector2.one;
                layerTrans.offsetMin = Vector2.zero;
                layerTrans.offsetMax = Vector2.zero;
            }
   
            layerTransforms.Add(layerTrans);
            Canvas canvas = layerTrans.gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = i * 10000;
        }
    }

    private void OnDestroy()
    {
        Ins = null;
    }

    public static void Init(GameObject canvas)
    {
        canvas.AddComponent<ViewMgr>();
    }

    public Transform Canvas
    {
        get { return CanvasTransfrom; }
    }

    public Vector2 CanvasPosToScreenPos(Vector2 pos)
    {
        Vector2 canvasSize = ViewMgr.Ins.CanvasSize;
        pos.x /= canvasSize.x;
        pos.y /= canvasSize.y;
        pos.x *= Utils.ScreenWidth;
        pos.y *= Utils.ScreenHeight;
        return pos;
    }

    public Vector2 ScreenPosToCanvasPos(Vector2 pos)
    {
        Vector2 canvasSize = ViewMgr.Ins.CanvasSize;
        pos.x /= Utils.ScreenWidth;
        pos.y /= Utils.ScreenHeight;
        pos.x *= canvasSize.x;
        pos.y *= canvasSize.y;
        return pos;
    }

    public void addView(View view)
    {
        mViews.Set(view.name, view);
    }

    public T GetView<T>() where T : View
    {
        Type viewType = typeof(T);
        return (T)GetView(viewType.Name);
    }

    public View GetView(string name)
    {
        View view;
        mViews.TryGetValue(name, out view);
        return view;
    }

    public bool IsShow<T>()
    {
        Type viewType = typeof(T);
        return IsShow(viewType.Name);
    }

    public bool IsShow(string viewName)
    {
        return mVisible.Contains(viewName);
    }

    public Coroutine WaitHide<T>()
    {
        Type viewType = typeof(T);
        return StartCoroutine(DoWaitHide(viewType.Name));
    }

    public Coroutine WaitShow<T>()
    {
        Type viewType = typeof(T);
        return StartCoroutine(DoWaitShow(viewType.Name));
    }

    public void ShowView(Type viewType, System.Object param = null, bool hideParent = true)
    {
        string viewName = viewType.Name;
        if (mVisible.Contains(viewName))
            return;

        mVisible.Add(viewName);
        string parentType = mLastViewName;
        StartCoroutine(Load(viewType, () =>
        {
                    if (hideParent && parentType != null)
                    {
                        DoHideView(parentType, viewName);
                    }
                    if (mVisible.Contains(viewName))
                    {
                        mStack.Add(viewName);
                        DoShowView(viewName, null, DefaultLayer, param);
                         mLastViewName = viewName;
                    }
                }));
    }

    public void JumpToView<T>(System.Object param = null, bool hideParent = true)
    {
        HideAll();
        ShowView<T>(param, hideParent);
    }

    public void SetLoadingView<T>()
    {
        Type viewType = typeof(T);
        StartCoroutine(Load(viewType, () => { mLoadingView = viewType; }));
    }

    public void ShowView<T>(System.Object param = null, bool hideParent = true)
    {
        Type viewType = typeof(T);
        ShowView(viewType, param, hideParent);
    }

    public void ShowTopView<T>(System.Object param = null)
    {
        Type viewType = typeof(T);
        ShowTopView(viewType, param);
    }

    public void ShowTopView(Type viewType, System.Object param = null)
    {
        string viewName = viewType.Name;
        if (mVisible.Contains(viewName))
            return;
        mVisible.Add(viewName);
        StartCoroutine(Load(viewType, () =>
                {
                    DoShowView(viewName, null, TopLayer, param);
                }));
    }

    public void HideView(System.Type viewType)
    {
        HideView(viewType.Name);
    }

    public void HideView(string name)
    {
        if (!mVisible.Contains(name))
            return;

        DoHideView(name, null);

        if (mStack.Count > 0 && mStack[mStack.Count - 1] == name)
        {
            mStack.RemoveAt(mStack.Count - 1);

            if (mStack.Count > 0)
            {
                string parent = mStack[mStack.Count - 1];
                if (!IsShow(parent))
                {
                    mVisible.Add(parent);
                    DoShowView(parent, name, DefaultLayer);
                }

                mLastViewName = parent;
            }
        }
    }

    public void HideView<T>() where T : View
    {
        Type viewType = typeof(T);
        HideView(viewType.Name);
    }

    public void HideAll()
    {
        mStack.Clear();
        List<string> mName = new List<string>(mVisible);
        foreach (string s in mName)
        {
            HideView(s);
        }

        mVisible.Clear();
        mLastViewName = null;
    }

    public void Reset()
    {
        HideAll();

        List<View> views = mViews.GetValues();
        foreach (View v in views)
        {
            if(v)
            Destroy(v.name);
        }
        mViews.Clear();
        mLoading.Clear();
        mVisible.Clear();
        mStack.Clear();
        mLastViewName = null;

        System.GC.Collect();
    }

    public void Destroy<T>()
    {
        Type viewType = typeof(T);
        Destroy(viewType.Name);
    }

    public void Destroy(string viewName)
    {
        View view;
        mViews.TryGetValue(viewName, out view);
        if (view)
        {
            view._DoDestroy();
            mViews.Remove(viewName);
        }

        mVisible.Remove(viewName);
    }

    public void AddOnShowEvent(Utils.StringDelegate func)
    {
        mOnShowEvent += func;
    }

    public void RemoveOnShowEvent(Utils.StringDelegate func)
    {
        if (mOnShowEvent != null)
        {
            mOnShowEvent -= func;
        }
    }

    public void AddOnHideEvent(Utils.StringDelegate func)
    {
        mOnHideEvent += func;
    }

    public void RemoveOnHideEvent(Utils.StringDelegate func)
    {
        if (mOnHideEvent != null)
        {
            mOnHideEvent -= func;
        }
    }

    public void AddOnShowEvent<T>(Utils.VoidDelegate func) where T : View
    {
        Type viewType = typeof(T);
        if(mDicOnShowEvent.ContainsKey(viewType.Name))
            mDicOnShowEvent[viewType.Name] += func;
        else
            mDicOnShowEvent[viewType.Name] = func;
    }

    public void AddOnHideEvent<T>(Utils.VoidDelegate func) where T : View
    {
        Type viewType = typeof(T);
        if (mDicOnHideEvent.ContainsKey(viewType.Name))
            mDicOnHideEvent[viewType.Name] += func;
        else
            mDicOnHideEvent[viewType.Name] = func;
    }

    public void RemoveOnShowEvent(string viewName, Utils.VoidDelegate func)
    {
        if (mDicOnShowEvent.ContainsKey(viewName))
        {
            mDicOnShowEvent[viewName] -= func;
        }
    }

    public void RemoveOnHideEvent(string viewName, Utils.VoidDelegate func)
    {
        if (mDicOnHideEvent.ContainsKey(viewName))
        {
            mDicOnHideEvent[viewName] -= func;
        }
    }

    public void SetLayerVisible(int index, bool visible)
    {
        layerTransforms[index].gameObject.SetActive(visible);
    }

    public bool IsScriptView(string view)
    {
        return false;
    }

    public bool mIsPrefab = true;
    private IEnumerator Load(Type viewType, Utils.VoidDelegate onFinished)
    {
        string viewName = viewType.Name;
        View view = null;
        if (mViews.TryGetValue(viewName, out view))
        {
            onFinished();
            yield break;
        }

       if (mLoadingView != null)
            ShowTopView(mLoadingView);

        while (mLoading.Contains(viewName))
        {
            yield return null;

            if (mViews.TryGetValue(viewName, out view))
            {
                onFinished();
                yield break;
            }
        }

        mLoading.Add(viewName);
        GameObject root = null;

        if(mIsPrefab)
        {
            UnityEngine.Object objPrefab = Resources.Load("ui/prefab/" + viewType.Name);
            root = (GameObject)Instantiate(objPrefab);
            yield return new WaitForSeconds(0.1f);
        }
        else
        {
            string relativePath = VIEW_AB_PATH + viewName.ToLower() + ".ab";
            yield return ResMgr.Ins.CreateFromAB(relativePath, viewName, o => root = o as GameObject);
        }
        
        mLoading.Remove(viewName);
        root.name = viewName;
        view = (View)root.AddComponent(viewType);
        mViews.Set(viewName, view);
        root.SetActive(false);
        view._DoInit();
        onFinished();

        if (mLoadingView != null)
            HideView(mLoadingView);
    }

    private IEnumerator DoWaitShow(string viewName)
    {
        while (!IsShow(viewName))
        {
            yield return null;
        }
    }

    private void DoShowView(string viewName, string childView, int layer, System.Object param = null)
    {
        if (!mVisible.Contains(viewName))
        {
            return;
        }
        View view = null;
        if (!mViews.TryGetValue(viewName, out view))
            return;

        if (view.gameObject.activeSelf)
        {
            return;
        }

        Transform trans = view.transform;
        Transform layerTrans = layerTransforms[layer];
        trans.SetParent(layerTrans, false);
        trans.localPosition = Vector3.zero;
        trans.localScale = Vector3.one;
        int index = -1;
        if (mStack.Contains(viewName))
            index = mStack.LastIndexOf(viewName);
        view._SetRenderSort(index < 0 ? 0 : index);
        trans.SetAsLastSibling();
        view.gameObject.SetActive(true);
        view.OpenSortRenderSort(true);
        view._DoShow(childView, param);
        NotifyOnShowEvent(viewName);
    }

    private void DoHideView(string viewName, string childname)
    {
        mVisible.Remove(viewName);
        View view = null;
        if (mViews.TryGetValue(viewName, out view))
        {
            view.gameObject.SetActive(false);
            view._DoHide(childname);
            NotifyOnHideEvent(viewName);
        }
    }

    private IEnumerator DoWaitHide(string viewName)
    {
        while (IsShow(viewName))
        {
            yield return null;
        }
    }

    private void NotifyOnShowEvent(string viewName)
    {
        if (mDicOnShowEvent.ContainsKey(viewName))
        {
            if (mDicOnShowEvent[viewName] != null)
            {
                mDicOnShowEvent[viewName]();
            }
        }
    }

    private void NotifyOnHideEvent(string viewName)
    {
        if (mDicOnHideEvent.ContainsKey(viewName))
        {
            if (mDicOnHideEvent[viewName] != null)
            {
                mDicOnHideEvent[viewName]();
            }
        }
    }

    private bool OnCacheOverflow(string t, View v)
    {
        if (v==null)
        {
            return false;
        }
        if (v.IsShow)
            return false;


        if (v.GetType() == mLoadingView)
            return false;

        if (mStack.Contains(t))
            return false;

        mVisible.Remove(t);
        mLoading.Remove(t);
        v._DoDestroy();
        return true;
    }
}