using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

public class ResMgr : MonoBehaviour
{
    public delegate void ABCallback(AssetBundle ab);

    public delegate void TextCallback(string text);
    public delegate void Texture2DCallback(Texture2D texture);
    public delegate void SpriteCallback(Sprite texture);
    public static ResMgr Ins;

    private const int AssetBundleCacheSize = 5;
    private string mStreamingAssetPathForWWW = "";
    private readonly LRU<string, ABInfo> mAssetBundles = new LRU<string, ABInfo>(AssetBundleCacheSize);
    private readonly HashSet<string> mLoading = new HashSet<string>();

    public static void Init()
    {
        GameObject go = new GameObject("ResMgr");
        go.hideFlags = HideFlags.HideAndDontSave;
        Ins = go.AddComponent<ResMgr>();
    }

    private void Awake()
    {
        mStreamingAssetPathForWWW = Utils.GetStreamingAssetPathForWWW("");
        mAssetBundles.onRemoveEntry = OnCacheOverflow;
    }

    private void OnDestroy()
    {
        Ins = null;
    }

    public Coroutine LoadAB(string abPath, ABCallback callback = null, bool canAutoUnload = true)
    {
        return StartCoroutine(_LoadAB(abPath, callback, canAutoUnload, false));
    }

    public Coroutine LoadAssetFromAB<T>(string abPath, string resName, GameObject go, Utils.UnityObjectDelegate callback = null)
    {
        return StartCoroutine(_LoadAssetFromAB(abPath, resName, go, typeof(T), callback));
    }

    public Coroutine CreateFromAB(string abPath, string resName, Utils.GameObjectDelegate callback = null)
    {
        return StartCoroutine(_CreateFromAB(abPath, resName, callback));
    }

    public void CleanRef(GameObject go)
    {
        ABRef abRef = go.GetComponent<ABRef>();
        if (abRef != null)
        {
            Destroy(abRef);
        }
    }

    public void CleanCahce()
    {
        List<ABInfo> abs = mAssetBundles.GetValues();
        foreach (ABInfo ab in abs)
        {
            if (ab.canUnload && ab.refCount <= 0 && !mLoading.Contains(ab.path))
            {
                ab.ab.Unload(true);
                mAssetBundles.Remove(ab.path);

#if UNITY_EDITOR
                Debug.Log("[ResMgr]Unload " + ab.path);
#endif
            }
        }

        System.GC.Collect();
    }


    public void CleanUp()
    {
        StopAllCoroutines();
        mLoading.Clear();
        List<ABInfo> abs = mAssetBundles.GetValues();
        foreach (ABInfo ab in abs)
        {
            if (ab.canUnload)
            {
                ab.ab.Unload(true);
                mAssetBundles.Remove(ab.path);

#if UNITY_EDITOR
                Debug.Log("[ResMgr]Unload " + ab.path);
#endif
            }
        }
        System.GC.Collect();
    }

    public string Dump()
    {
        StringBuilder sb = new StringBuilder();

        List<ABInfo> abs = mAssetBundles.GetValues();
        foreach (ABInfo info in abs)
        {
            sb.Append("{");
            sb.Append(info.path);
            sb.Append(":");
            sb.Append(info.refCount);
            sb.Append("}");
        }

        sb.Append("loading{");
        foreach (string path in mLoading)
        {
            sb.Append(path);
            sb.Append(",");
        }

        sb.Append("}");

        return sb.ToString();
    }

    private IEnumerator _LoadAB(string abPath, ABCallback callback, bool canUnload, bool addRef)
    {
        ABInfo abInfo;
        if (mAssetBundles.TryGetValue(abPath, out abInfo))
        {
            abInfo.canUnload = (abInfo.canUnload && canUnload);

            if (addRef)
            {
                ++abInfo.refCount;
            }

            if (callback != null)
            {

                callback(abInfo.ab);
            }

            yield break;
        }

        if (mLoading.Contains(abPath))
        {
            while (mLoading.Contains(abPath))
            {
                yield return null;
            }

            if (mAssetBundles.TryGetValue(abPath, out abInfo))
            {
                abInfo.canUnload = abInfo.canUnload && canUnload;

                if (addRef)
                {
                    ++abInfo.refCount;
                }

                if (callback != null)
                {
                    callback(abInfo.ab);
                }
            }
        }
        else
        {
            //异步加载方式
            mLoading.Add(abPath);
            abInfo = new ABInfo();
            abInfo.path = abPath;
            abInfo.canUnload = canUnload;
            if (addRef)
            {
                ++abInfo.refCount;
            }

            AssetBundleCreateRequest assetRequest = AssetBundle.LoadFromFileAsync(Utils.GetFilePath(abPath));
            yield return assetRequest;

            mLoading.Remove(abPath);
            if (assetRequest.isDone && assetRequest.assetBundle != null)
            {
                abInfo.ab = assetRequest.assetBundle;
            }
            else
            {
                Debug.LogError("ResMgr load " + abPath + " failed.");
                yield break;
            }

            mAssetBundles.Set(abPath, abInfo);
            if (callback != null)
            {
                callback(abInfo.ab);
            }
        }
    }

    //加载本地txt文件的内容
    public Coroutine LoadTextLocal(string abPath, TextCallback callback)
    {
        string path = File.Exists(Utils.GetPersistentPath(GameConst.ResRootPath + abPath))
            ? Utils.GetPersistentPathForWWW(GameConst.ResRootPath + abPath)
            : Utils.GetStreamingAssetPathForWWW(abPath);
        return StartCoroutine(_LoadText(path, callback));
    }

    //加载本地txt文件的内容
    public Coroutine LoadTextServer(string abPath, TextCallback callback)
    {
        return StartCoroutine(_LoadText(abPath, callback));
    }

    //加载txt文件的内容
    private IEnumerator _LoadText(string abPath, TextCallback callback)
    {

        WWW www = new WWW(abPath);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            if (callback != null)
            {
                callback(www.text);
            }
        }
        else
        {
            callback(string.Empty);
            Debug.LogError("ResMgr load " + abPath + " failed:" + www.error);
        }
    }

    private void GetTextureFromServer(string abPath, int vision, Texture2DCallback callback)
    {
        WWW www = WWW.LoadFromCacheOrDownload(abPath, vision);
        if (www.texture != null)
        {
            callback(www.texture);
        }
    }

    private IEnumerator _LoadAssetFromAB(string abPath, string resName, GameObject go, System.Type type, Utils.UnityObjectDelegate callback)
    {
        CorrectPath(ref abPath, ref resName);
        yield return Utils.StartConroutine(_LoadAB(abPath, null, true, true));
        ABInfo abInfo;
        if (!mAssetBundles.TryGetValue(abPath, out abInfo))
            yield break;

        //异步
        AssetBundleRequest assetRequest = abInfo.ab.LoadAssetAsync(resName, type);

        yield return assetRequest;
        --abInfo.refCount;
        UnityEngine.Object asset = null;
        if (assetRequest.isDone)
        {
            asset = assetRequest.asset;
        }

        if (asset == null)
        {
            Debug.LogError("ResMgr load res " + resName + " from " + abPath + " failed");
            yield break;
        }

        if ((go == null) && (asset is GameObject))
        {
            go = asset as GameObject;
        }

        if (go != null)
        {
            ABRef abRef = go.GetComponent<ABRef>();
            if (abRef == null)
            {
                abRef = go.AddComponent<ABRef>();
            }
            abRef.SetABPath(abPath);
        }

        if (callback != null)
        {
            callback(asset);
        }
    }

    private IEnumerator _CreateFromAB(string abPath, string resName, Utils.GameObjectDelegate callback)
    {
        CorrectPath(ref abPath, ref resName);
        yield return Utils.StartConroutine(_LoadAB(abPath, null, true, true));
        ABInfo abInfo;
        if (!mAssetBundles.TryGetValue(abPath, out abInfo))
            yield break;

        //异步
        AssetBundleRequest assetRequeat = abInfo.ab.LoadAssetAsync(resName, typeof(GameObject));
        yield return assetRequeat;
        --abInfo.refCount;
        GameObject go = null;
     
        if (assetRequeat.isDone)
        {
            go = assetRequeat.asset as GameObject;
        }

        if (go == null)
        {
            Debug.LogError("ResMgr create res " + resName + " from " + abPath + " failed:");
            yield break;
        }

        go = Instantiate(go) as GameObject;
        ABRef abRef = go.AddComponent<ABRef>();
        abRef.SetABPath(abPath);
        if (callback != null)
        {
            callback(go);
        }
    }

    private bool OnCacheOverflow(string abPath, ABInfo abInfo)
    {
        if (abInfo == null)
            return false;

        if (!abInfo.canUnload)
            return false;

        if (abInfo.refCount > 0)
            return false;

        if (mLoading.Contains(abPath))
        {
            return false;
        }
        abInfo.ab.Unload(true);
        abInfo.ab = null;

#if UNITY_EDITOR
        Debug.Log("[ResMgr]Unload " + abPath);
#endif
        return true;
    }

    private ABInfo FindABInfo(string abPath)
    {
        ABInfo info;
        mAssetBundles.TryGetValue(abPath, out info);
        return info;
    }

    private string GetAssetBundlePathForWWW(string abPath)
    {
        return mStreamingAssetPathForWWW + abPath;
    }

    private sealed class ABRef : MonoBehaviour
    {
        [SerializeField]
        private string abPath;

        private bool mInited = false;

        private void Awake()
        {
            if (!mInited && !string.IsNullOrEmpty(abPath))
            {
                string path = abPath;
                abPath = "";
                SetABPath(path);
            }

            mInited = true;
        }

        private void OnDestroy()
        {
            SetABPath(null);
        }

        public void SetABPath(string path)
        {
            mInited = true;

            if (abPath == path)
                return;

            if (!string.IsNullOrEmpty(abPath))
            {
                ABInfo abInfo = ResMgr.Ins.FindABInfo(abPath);
                if (abInfo != null)
                {
                    --abInfo.refCount;
                }
            }

            abPath = path;

            if (!string.IsNullOrEmpty(abPath))
            {
                ABInfo abInfo = ResMgr.Ins.FindABInfo(abPath);
                if (abInfo != null)
                {
                    ++abInfo.refCount;
                }
            }
        }
    }

    private sealed class ABInfo
    {
        public string path;
        public AssetBundle ab = null;
        public bool canUnload = true;
        public int refCount = 0;
    }

    private static void CorrectPath(ref string abPath, ref string resName)
    {
        abPath = abPath.ToLower();
        if (!string.IsNullOrEmpty(resName))
            return;

        int begin = abPath.LastIndexOf('/');
        int index = abPath.LastIndexOf(".");
        if (index < 0)
        {
            resName = abPath.Substring(begin + 1);
            abPath = abPath.Substring(0, begin) + ".ab";
            
        }
        else
        {
            resName = abPath.Substring(begin + 1, index - begin - 1);
        }
    }

    private IEnumerator _LoadTexture(string abPath, string folder, string textureName, Texture2DCallback callback)
    {
        WWW www = new WWW(abPath);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            if (callback != null)
            {
                callback(www.texture);
                byte[] bytes = www.bytes;
                string picName = textureName;
                string path = Utils.GetPersistentPath(folder+picName);
                Stream stream = new FileStream(path, FileMode.OpenOrCreate);
                BinaryWriter sw = new BinaryWriter(stream);
                sw.Write(bytes);
                sw.Close();
                stream.Close();
            }
        }
        else
        {
            Debug.LogError("ResMgr load " + abPath + " failed:" + www.error);
        }
    }
}