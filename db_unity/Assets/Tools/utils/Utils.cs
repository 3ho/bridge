using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.UI;
//using VoxelBusters;
//using VoxelBusters.NativePlugins;

public class Utils : MonoBehaviour
{
    private static Utils ins;
    public static readonly System.Random mRandom = new System.Random();

    private const int SECONDS_OF_DAY = 24 * 60 * 60;
    private const int SECONDS_OF_HOUR = 60 * 60;
    private const int SECONDS_OF_MINUTE = 60;

    public static int ScreenWidth = 1920;
    public static int ScreenHeight = 1080;
    public static int OriginalScreenWidth = 1920;
    public static int OriginalScreenHeight = 1080;
    public static int HalfScreenWidth = ScreenWidth / 2;
    public static int HalfScreenHeight = ScreenHeight / 2;
    public static Vector2 ScreenCenter;

    public delegate void Event<T>(T arg);

    public delegate void VoidDelegate();

    public delegate void TypeDelegate(Type type);

    public delegate void BoolDelegate(bool arg);

    public delegate bool ReturnBoolDelegate();

    public delegate void IntDelegate(int arg);

    public delegate void Int2Delegate(int arg1, int arg2);

    public delegate void Vector2Delegate(Vector2 vector2);

    public delegate void Vector3Delegate(Vector3 vector3);

    public delegate void FloatDelegate(float arg);

    public delegate void LongDelegate(long arg);

    public delegate void Long2Delegate(long arg1, long arg2);

    public delegate void StringDelegate(string arg);

    public delegate void ObjectDelegate(System.Object arg);

    public delegate void ObjectArrayDelegate(params System.Object[] args);

    public delegate void UnityObjectDelegate(UnityEngine.Object arg);

    public delegate void GameObjectDelegate(UnityEngine.GameObject arg);

    public delegate void UnityObjectArrayDelegate(UnityEngine.Object[] arg);

    public delegate void DataDelegate<in T>(T arg);

    //public delegate void StartLoad(string settingPath, cfg.CfgManager.OnLoaded onLoaded);

    private static string mPersistentDataPath = "";

    private static readonly DateTime date_1970 = new DateTime(1970, 1, 1);

    public static float GetRadio()
    {
        return OriginalScreenWidth * 1f / OriginalScreenHeight;
    }

    public static void Init(int screenWidth, int screenHeight)
    {
        ScreenWidth = screenWidth;
        ScreenHeight = screenHeight;
        HalfScreenWidth = ScreenWidth / 2;
        HalfScreenHeight = ScreenHeight / 2;

        if (string.IsNullOrEmpty(mPersistentDataPath))
        {
            mPersistentDataPath = Application.persistentDataPath;
            GameObject go = new GameObject("utils");
            go.hideFlags = HideFlags.HideAndDontSave;
            go.AddComponent<Utils>();
            PhoneLevel = IsLowEndPhonefun();
        }

        ScreenCenter = new Vector2(ScreenWidth * 0.5f, ScreenHeight * 0.5f);
    }

    public static void SetPesistentPathFromJava()
    {
        
//#if UNITY_ANDROID
//        try
//        {
//            string persistentPathFromJava = AndroidSDKInterface.Instance.GetStreamAssetsPath();
//            Debug.LogError("java persistent path:" + persistentPathFromJava);
//            if (persistentPathFromJava.Length > 0)
//            {
//                persistentPathFromJava = persistentPathFromJava.Substring(0, persistentPathFromJava.Length - 1);
//            }
//            if (persistentPathFromJava != mPersistentDataPath)
//            {
//                mPersistentDataPath = persistentPathFromJava;
//                Debug.LogError(mPersistentDataPath + " != " + persistentPathFromJava);
//            }
//        }
//        catch (Exception e)
//        {
//            Debug.LogError(e.ToString());
//        }        
//#endif
    }

    public static void SetCanvasMatchWidthOrHeight(GameObject go)
    {
        float radio = Utils.GetRadio();
        float radio1 = Screen.width * 1f / Screen.height;
        CanvasScaler canvasScaler = go.GetComponent<CanvasScaler>();
        canvasScaler.matchWidthOrHeight = radio1 > radio ? 1 : 0;
    }

    public static bool TriggerEvent(VoidDelegate e)
    {
        if (e == null)
            return false;
        e();
        return true;
    }

    public static bool TriggerEvent_Try<T>(Event<T> e, T t)
    {
        try
        {
            if (e != null)
                e(t);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public static Coroutine WaitTrue(ReturnBoolDelegate func)
    {
        return StartConroutine(DoWaitTrue(func));
    }

    private static IEnumerator DoWaitTrue(ReturnBoolDelegate func)
    {
        while (!func())
        {
            yield return null;
        }
    }

    public static bool IsParentTransform(Transform child, Transform parent)
    {
        if (child == parent)
            return true;

        while (child.parent != null)
        {
            if (child.parent == parent)
                return true;
            child = child.parent;
        }

        return false;
    }

    public static bool IsVisible(Bounds bounds, Camera camera)
    {
        Vector3 vExtents = bounds.extents;
        Vector3 vCenter = bounds.center;

        if (IsVisible(bounds.min, camera))
            return true;

        if (IsVisible(bounds.max, camera))
            return true;

        Vector3 vLeftTopFront = vCenter + new Vector3(-vExtents.x, vExtents.y, -vExtents.z);
        Vector3 vLeftTopBack = vCenter + new Vector3(-vExtents.x, vExtents.y, vExtents.z);
        Vector3 vLeftbottomBack = vCenter + new Vector3(-vExtents.x, -vExtents.y, vExtents.z);
        Vector3 vRightTopFront = vCenter + new Vector3(vExtents.x, vExtents.y, -vExtents.z);
        Vector3 vRightbottomFront = vCenter + new Vector3(vExtents.x, -vExtents.y, -vExtents.z);
        Vector3 vRightbottomBack = vCenter + new Vector3(vExtents.x, -vExtents.y, vExtents.z);

        if (IsVisible(vLeftTopFront, camera))
            return true;

        if (IsVisible(vLeftTopBack, camera))
            return true;

        if (IsVisible(vLeftbottomBack, camera))
            return true;

        if (IsVisible(vRightTopFront, camera))
            return true;

        if (IsVisible(vRightbottomFront, camera))
            return true;

        if (IsVisible(vRightbottomBack, camera))
            return true;

        return false;
    }

    public static bool IsVisible(Vector3 vPoint, Camera camera)
    {
        Vector3 vScreenPoint = camera.WorldToScreenPoint(vPoint);
        float fWidth = Screen.width;
        float fHeight = Screen.height;
        if (vScreenPoint.x < 0 || vScreenPoint.x > fWidth)
            return false;
        if (vScreenPoint.y < 0 || vScreenPoint.y > fHeight)
            return false;
        if (vScreenPoint.z < camera.nearClipPlane || vScreenPoint.z > camera.farClipPlane)
            return false;
        return true;
    }

    public static void Clamp<T>(ref T val, T min, T max) where T : System.IComparable
    {
        if (val.CompareTo(min) < 0)
            val = min;
        if (val.CompareTo(max) > 0)
            val = max;
    }

    public class CustomTransform
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 localScale;
    }

    public static T Min<T>(T a, T b) where T : System.IComparable
    {
        return a.CompareTo(b) < 0 ? a : b;
    }

    public static T Max<T>(T a, T b) where T : System.IComparable
    {
        return a.CompareTo(b) < 0 ? b : a;
    }

    public static void Swap<T>(ref T a, ref T b)
    {
        T t = a;
        a = b;
        b = t;
    }

    public static float DistanceFromPointToLine(Vector3 vPoint, Vector3 vLinePoint, Vector3 vLineDir,
        out Vector3 vVertical)
    {
        vLineDir.Normalize();
        Vector3 vPointToLine = vPoint - vLinePoint;
        float fDot = Vector3.Dot(vPointToLine, vLineDir);
        vVertical = vLinePoint + vLineDir * fDot;
        return (vPoint - vVertical).magnitude;
    }

    public static float DistanceFromPointToLine(Vector2 vPoint, Vector2 vLinePoint, Vector2 vLineDir,
        out Vector2 vVertical)
    {
        vLineDir.Normalize();
        Vector2 vPointToLine = vPoint - vLinePoint;
        float fDot = Vector2.Dot(vPointToLine, vLineDir);
        vVertical = vLinePoint + vLineDir * fDot;
        return (vPoint - vVertical).magnitude;
    }

    public static bool LineIntersect(Vector2 vPoint1, Vector2 vDir1, Vector2 vPoint2, Vector2 vDir2,
        out Vector2 vIntersect)
    {
        vIntersect = Vector2.zero;
        vDir1.Normalize();
        vDir2.Normalize();
        if (vDir1 == vDir2)
            return false;
        Vector2 vVertical;
        float fDistance = DistanceFromPointToLine(vPoint2, vPoint1, vDir1, out vVertical);
        Vector2 vVerticalDir = vVertical - vPoint2;
        float fVerticalLen = vVerticalDir.magnitude;
        vVerticalDir.Normalize();
        float fAngle = Quaternion.Angle(Quaternion.LookRotation(vDir2), Quaternion.LookRotation(vVerticalDir));
        if (fAngle > 90)
            vDir2 = -vDir2;
        float fLen = fVerticalLen / Mathf.Cos(fAngle);
        vIntersect = vPoint2 + vDir2 * fLen;
        return true;
    }

    public static void DestroyChildren(GameObject go)
    {
        List<GameObject> children = new List<GameObject>();
        foreach (UnityEngine.Transform t in go.transform)
        {
            t.parent = null;
            children.Add(t.gameObject);
        }

        foreach (GameObject child in children)
        {
            UnityEngine.Object.Destroy(child);
        }
    }

    public static T AddComponentIfNotExist<T>(GameObject go) where T : Component
    {
        T t = go.GetComponent<T>();
        if (t == null)
        {
            t = go.AddComponent<T>();
        }

        return t;
    }

    public static void AddModalCollider(GameObject go)
    {
        BoxCollider bc = go.GetComponent<BoxCollider>();
        if (bc == null)
        {
            bc = go.AddComponent<BoxCollider>();
            bc.size = new Vector3(10000, 10000, 0);
            bc.center = new Vector3(0, 0, 5);
        }
    }

    public static string GetRelativePath(GameObject parent, GameObject child)
    {
        string path = child.name;
        UnityEngine.Transform trans = child.transform.parent;
        while (trans != null && trans != parent.transform)
        {
            path = trans.name + "/" + path;
            trans = trans.parent;
        }

        return path;
    }

    public static Coroutine StartConroutine(IEnumerator routine)
    {
        return ins.StartCoroutine(routine);
    }

    public static void StopConroutine(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            ins.StopCoroutine(coroutine);
        }
    }
    public static void StopConroutine(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            ins.StopCoroutine(name);
        }
    }

    private void Awake()
    {
        ins = this;
    }

#if UNITY_EDITOR || UNITY_STANDALONE

    private void Update()
    {
        ScreenWidth = Screen.width;
        ScreenHeight = Screen.height;
        HalfScreenWidth = ScreenWidth / 2;
        HalfScreenHeight = ScreenHeight / 2;
    }

#endif

    public static Quaternion LookRotation(Vector3 forward, Quaternion defaultRotation)
    {
        if (forward.sqrMagnitude > 0.001f)
            return Quaternion.LookRotation(forward);
        return defaultRotation;
    }

    public static Vector3 GetScreenPosition(Vector3 vWorldPos, Camera worldCamera)
    {
        Vector3 vPos = worldCamera.WorldToScreenPoint(vWorldPos);
        vPos = Camera.main.ScreenToWorldPoint(vPos);
        return vPos;
    }

    public static float GetEffectDuration(GameObject effect, bool ignoreloop = true)
    {
        float duration = 0;
        ParticleSystem particleSystem = effect.GetComponent<ParticleSystem>();
        if (particleSystem)
        {
            if (particleSystem.loop && !ignoreloop)
                return Mathf.Infinity;
            float time = particleSystem.startLifetime + particleSystem.startDelay;
            if (time > duration)
                duration = time;
            if (duration < particleSystem.duration)
                duration = particleSystem.duration;
        }
        Component[] particleSystems = effect.GetComponentsInChildren(typeof(ParticleSystem), true);
        foreach (Component c in particleSystems)
        {
            ParticleSystem ps = c as ParticleSystem;
            if (!ps)
                continue;
            if (ps.loop && !ignoreloop)
                return Mathf.Infinity;
            float time = ps.startLifetime + ps.startDelay;
            if (time > duration)
                duration = time;
            if (duration < ps.duration)
                duration = ps.duration;
        }
        return duration;
    }

    public static string GetTimeString(int iTimeInSecond)
    {
        if (iTimeInSecond <= 0)
        {
            return "00:00:00";
        }

        int iHour = iTimeInSecond / 3600;
        int iMinute = (iTimeInSecond - iHour * 3600) / 60;
        int iSecond = iTimeInSecond % 60;
        return (iHour < 10 ? "0" : "") + iHour + ":" + (iMinute < 10 ? "0" : "") + iMinute + ":" +
               (iSecond < 10 ? "0" : "") + iSecond;
    }

    public static string GetNoHourTimeString(int iTimeInSecond)
    {
        if (iTimeInSecond <= 0)
        {
            return "00:00";
        }

        int iMinute = iTimeInSecond / 60;
        int iSecond = iTimeInSecond % 60;
        return (iMinute < 10 ? "0" : "") + iMinute + ":" +
               (iSecond < 10 ? "0" : "") + iSecond;
    }

    public static IEnumerator CastGameObject(GameObject go, float initScale, float initAlpha, float duration)
    {
        Vector3 oriScale = go.transform.localScale;
        float curScale = initScale;
        float scaleSpeed = (initScale - 1) / duration;
        float lastTime = Time.time;
        while (duration > 0)
        {
            float curTime = Time.time;
            float deltaTime = curTime - lastTime;
            lastTime = curTime;
            duration -= deltaTime;
            curScale -= deltaTime * scaleSpeed;
            if (scaleSpeed > 0 && curScale < 1)
                curScale = 1;
            if (scaleSpeed < 0 && curScale > 1)
                curScale = 1;

            go.transform.localScale = oriScale * curScale;
            yield return true;
        }
        go.transform.localScale = oriScale;
        yield break;
    }

    public static IEnumerator Vibration(GameObject go, float duration, float radiusX, float radiusY, float radiusZ)
    {
        Vector3 initPos = go.transform.localPosition;
        float time = Time.realtimeSinceStartup;
        while (duration > 0)
        {
            float deltaTime = Time.realtimeSinceStartup - time;
            time = Time.realtimeSinceStartup;
            duration -= deltaTime;
            go.transform.localPosition = initPos + new Vector3(UnityEngine.Random.Range(-radiusX, radiusX),
                                             UnityEngine.Random.Range(-radiusY, radiusY),
                                             UnityEngine.Random.Range(-radiusZ, radiusZ));
            yield return new WaitForSeconds(0.01f);
        }
        go.transform.localPosition = initPos;
        yield break;
    }

    public static IEnumerator ScalePingpong(GameObject go, int times, float once_duration, float minScale,
        float maxScale, float attenuation)
    {
        Vector3 initScale = go.transform.localScale;
        if (minScale == maxScale)
            yield break;
        if (maxScale < minScale)
            Swap(ref minScale, ref maxScale);
        if (minScale < 0)
            minScale = 0;
        if (maxScale < 1)
            maxScale = 1;
        if (minScale > 1)
            minScale = 1;
        float speed = (maxScale - minScale) * 2 / once_duration;
        float curScale = 1;
        while (times > 0)
        {
            while (curScale > minScale)
            {
                curScale -= Time.deltaTime * speed;
                Utils.Clamp(ref curScale, minScale, maxScale);
                go.transform.localScale = initScale * curScale;
                yield return true;
            }
            while (curScale < maxScale)
            {
                curScale += Time.deltaTime * speed;
                Utils.Clamp(ref curScale, minScale, maxScale);
                go.transform.localScale = initScale * curScale;
                yield return true;
            }
            if (minScale < 1)
                minScale = 1 - (1 - minScale) * attenuation;
            if (maxScale > 1)
                maxScale = maxScale * attenuation;
            speed = (maxScale - minScale) * 2 / once_duration;
            --times;
        }
        go.transform.localScale = initScale;
        yield break;
    }

    public static IEnumerator RotatePingpong(GameObject go, int times, float once_duration, float angle,
        float attenuation, Vector3 forward)
    {
        if (angle == 0)
            yield break;
        Quaternion initRotation = go.transform.rotation;
        if (Math.Abs(angle) > 360)
            angle -= ((int) (angle / 360)) * 360;
        float minAngle = -angle;
        float maxAngle = angle;
        if (minAngle > maxAngle)
        {
            Swap(ref minAngle, ref maxAngle);
        }
        float speed = angle * 2 * 2 / once_duration;
        float curAngle = 0;
        while (times > 0)
        {
            while (curAngle > minAngle)
            {
                curAngle -= Time.deltaTime * speed;
                if (curAngle < minAngle)
                    curAngle = minAngle;
                go.transform.rotation = initRotation * (Quaternion.Euler(0, 0, curAngle) *
                                                        Quaternion.LookRotation(forward));
                yield return true;
            }
            while (curAngle < maxAngle)
            {
                curAngle += Time.deltaTime * speed;
                if (curAngle > maxAngle)
                    curAngle = maxAngle;
                go.transform.rotation = initRotation * (Quaternion.Euler(0, 0, curAngle) *
                                                        Quaternion.LookRotation(forward));
                yield return true;
            }
            if (minAngle < 0)
                minAngle = minAngle * attenuation;
            if (maxAngle > 0)
                maxAngle = maxAngle * attenuation;
            speed = (maxAngle - minAngle) * 2 / once_duration;
            --times;
        }
        go.transform.rotation = initRotation;
        yield break;
    }

    public static int Random(int min, int max)
    {
        return mRandom.Next(min, max);
    }

    public static float Random(float min, float max)
    {
        return (float) (mRandom.NextDouble() * (max - min)) + min;
    }

    public static IEnumerator Flicker(GameObject[] gameObjects, float duration)
    {
        foreach (GameObject o in gameObjects)
        {
            o.SetActive(true);
        }
        yield return new WaitForSeconds(duration);
        foreach (GameObject o in gameObjects)
        {
            o.SetActive(false);
        }
        yield break;
    }

    public static string ConvertChatJavaTime(int javaTime)
    {
        long timeTick = ((long) javaTime) * 1000;
        long ticks_1970 = date_1970.Ticks;
        long time_ticks = ticks_1970 + timeTick * 10000;
        DateTime dt = new DateTime(time_ticks);
        dt = dt.ToLocalTime();
        DateTime dtNow = DateTime.Now;
        if (dt.Year == dtNow.Year && dt.Month == dtNow.Month && dt.Day == dtNow.Day)
        {
            return string.Format("{0:HH:mm}", dt);
        }
        else
            return string.Format("{0:MM/dd HH:mm}", dt);
    }

    public static string ConvertJavaTime(int javaTime)
    {
        long timeTick = ((long) javaTime) * 1000;
        long ticks_1970 = date_1970.Ticks;
        long time_ticks = ticks_1970 + timeTick * 10000;
        DateTime dt = new DateTime(time_ticks);
        dt = dt.ToLocalTime();
        return string.Format("{0:yyyy/MM/dd HH:mm}", dt);
    }

    public static string ConvertJavaTimeDay(int javaTime)
    {
        long timeTick = ((long) javaTime) * 1000;
        long ticks_1970 = date_1970.Ticks;
        long time_ticks = ticks_1970 + timeTick * 10000;
        DateTime dt = new DateTime(time_ticks);
        dt = dt.ToLocalTime();
        return string.Format("{0:yyyy/MM/dd}", dt);
    }

    public static string ConvertJavaTime(int javaTime, string format)
    {
        long timeTick = ((long) javaTime) * 1000;
        long ticks_1970 = date_1970.Ticks;
        long time_ticks = ticks_1970 + timeTick * 10000;
        DateTime dt = new DateTime(time_ticks);
        dt = dt.ToLocalTime();
        return string.Format(("{0:" + format + "}"), dt);
    }

    public static long GetTicksFromJavaTime(long javaTimeInMilliseconds)
    {
        long timeTick = javaTimeInMilliseconds;
        long ticks_1970 = date_1970.Ticks;
        long time_ticks = ticks_1970 + timeTick * 10000;
        return time_ticks;
    }

    public static void ResetEffect(GameObject go)
    {
        ParticleSystem particleSystem = go.GetComponent<ParticleSystem>();
        if (particleSystem)
        {
            particleSystem.time = 0;
            particleSystem.Clear(true);
            particleSystem.Stop(true);
        }
        Component[] components = go.GetComponentsInChildren(typeof(ParticleSystem), true);
        foreach (Component c in components)
        {
            ParticleSystem ps = c as ParticleSystem;
            ps.Clear(true);
            ps.time = 0;
            ps.Stop(true);
        }

        go.SetActive(false);
    }

    public static IEnumerator FlickEffect(GameObject[] gameObjects, float duration)
    {
        foreach (GameObject o in gameObjects)
        {
            o.SetActive(true);
        }
        yield return new WaitForSeconds(duration);
        foreach (GameObject o in gameObjects)
        {
            ResetEffect(o);
            o.SetActive(false);
        }
        yield break;
    }

    public static IEnumerator FlickGameObject(GameObject[] gameObjects, float duration)
    {
        foreach (GameObject o in gameObjects)
        {
            o.SetActive(true);
        }
        yield return new WaitForSeconds(duration);
        foreach (GameObject o in gameObjects)
        {
            o.SetActive(false);
        }
        yield break;
    }

    public static void Flip(GameObject go, bool keepXPositive, bool keepYPositive, bool keepZPositive)
    {
        Vector3 scale = go.transform.localScale;
        if (keepXPositive == (scale.x < 0))
            scale.x = -scale.x;
        if (keepYPositive == (scale.y < 0))
            scale.y = -scale.y;
        if (keepZPositive == (scale.z < 0))
            scale.z = -scale.z;
        go.transform.localScale = scale;
    }

    public static void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public static void EnableComponent<T>(GameObject go, bool enabled) where T : Behaviour
    {
        if (!go)
            return;
        T t = go.GetComponent<T>();
        if (t)
            t.enabled = enabled;
    }

    public static IEnumerator HidePopupWindow(GameObject go)
    {
        Vector3 oriScale = go.transform.localScale;
        //        TweenScale.Begin(go, 0.13f, oriScale * 1.17f);
        yield return new WaitForSeconds(0.13f);
        //        TweenScale.Begin(go, 0.13f, oriScale * 0.5f);
        yield return new WaitForSeconds(0.13f);
        go.transform.localScale = oriScale;
        go.SetActive(false);
    }

    public static IEnumerator ShowPopupWindow(GameObject go)
    {
        Vector3 oriScale = go.transform.localScale;
        go.transform.localScale = oriScale * 0.5f;
        go.SetActive(true);
        yield return new WaitForEndOfFrame();
        //        TweenScale.Begin(go, 0.182f, oriScale * 1.17f);
        yield return new WaitForSeconds(0.182f);
        //        TweenScale.Begin(go, 0.13f, oriScale);
        yield return new WaitForSeconds(0.13f);
        go.transform.localScale = oriScale;
    }

    public static IEnumerator NumberChange(int current, int target, int times, GameObject label, float duration)
    {
        int step = (target - current) / times;
        for (int i = 0; i < times + 1; i++)
        {
            current += step;
            if (step < 0 && current < target)
                current = target;
            if (step > 0 && current > target)
                current = target;
            //            View.SetLabelText(label, current.ToString());
            float dura = duration - Time.deltaTime;
            if (dura < 0)
                dura = 0;
            yield return new WaitForSeconds(dura);
        }

        if (current != target)
        {
            //            View.SetLabelText(label, target.ToString());
            yield break;
        }
    }

    public static void NormalizeInWorld(GameObject go)
    {
        go.transform.position = Vector3.zero;
        go.transform.rotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
    }

    public static void NormalizeInLocal(GameObject go)
    {
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
    }

    public static void PauseEffect(GameObject effect, bool pause)
    {
        ParticleSystem[] pss = effect.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particleSystem in pss)
        {
            if (pause)
                particleSystem.Pause(true);
            else
                particleSystem.Play(true);
        }
    }

    public static string GetIconImagePath(string name)
    {
        return "res/platform/icon/" + name + ".ab";
    }

    public static string GetStreamingAssetPathForWWW(string relPath)
    {
        if (relPath.Length > 0 && relPath[0] == '/')
            relPath = relPath.Substring(1);
        string path = "";
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                path = "file:///" + Application.dataPath + "/StreamingAssets/" + relPath;
                break;

            case RuntimePlatform.Android:
                path = Application.streamingAssetsPath + "/" + relPath;
                break;

            case RuntimePlatform.OSXEditor:
                path = "file://" + Application.dataPath + "/StreamingAssets/" + relPath;
                break;

            case RuntimePlatform.IPhonePlayer:
                path = "file://" + Application.streamingAssetsPath + "/" + relPath;
                break;
        }

        return path;
    }

    public static string GetPersistentDataPathForWWW(string relPath)
    {
        string path = GetPersistentDataPath(relPath);
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                path = "file:///" + path;
                break;

            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.Android:
            case RuntimePlatform.IPhonePlayer:
                path = "file://" + path;
                break;
        }
        return path;
    }

    public static string GetPersistentDataPath(string relPath)
    {
        if (Application.isMobilePlatform)
        {
            string fileName = null;
            if (relPath != null)
            {
                fileName = relPath.Substring(relPath.LastIndexOf("/") + 1);
            }
            return mPersistentDataPath + "/" + fileName;
        }
        else
        {
            return GetStreamingAssetPath(relPath);
        }
    }

    public static string GetDownLoadPath(string relPath)
    {
        return "res" + mPersistentDataPath + "/" + relPath;
    }

    public static string GetPersistentPath(string relPath)
    {
        if (relPath.Length > 0 && relPath[0] == '/')
            relPath = relPath.Substring(1);
        return mPersistentDataPath + "/" + relPath;
    }

    public static string GetPersistentPathForWWW(string relPath)
    {
        string path = GetPersistentPath(relPath);
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                path = "file:///" + path;
                break;

            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.Android:
            case RuntimePlatform.IPhonePlayer:
                path = "file://" + path;
                break;
        }
        return path;
    }

    public static string GetStreamingAssetPath(string relPath = null)
    {
        if (relPath.Length > 0 && relPath[0] == '/')
            relPath = relPath.Substring(1);
        string path = "";
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                path = Application.dataPath + "/StreamingAssets/" + relPath;
                break;

            case RuntimePlatform.Android:
                path = Application.streamingAssetsPath + "/" + relPath;
                break;

            case RuntimePlatform.OSXEditor:
                path = Application.dataPath + "/StreamingAssets/" + relPath;
                break;

            case RuntimePlatform.OSXPlayer:
                path = Application.dataPath + "/Resources/Data/StreamingAssets/" + relPath;
                break;

            case RuntimePlatform.IPhonePlayer:
                path = Application.streamingAssetsPath + "/" + relPath;
                break;
        }
        return path;
    }

    public static string GetFilePath(string relativePath)
    {
        string path = GetPersistentPath(GameConst.ResRootPath + relativePath);
        if (File.Exists(path))
        {
            return path;
        }

//#if UNITY_ANDROID
//        if (PlatformMgr.Ins.IsHgAndroid)
//        {
//            return GetObbAssetPath(relativePath);
//        }
//#endif
     
       return GetStreamingAssetPath(relativePath);
    }

#if UNITY_ANDROID

    //public static string GetObbAssetPath(string relPath = null)
    //{
    //    string path = "";

    //    if (PlatformMgr.Ins.IsHgAndroid)
    //    {
    //        path = AndroidSDKInterface.Instance.GetObbAssetsPath(relPath);
    //    }

    //    return path;
    //}

#endif

    public static string GetFilePathForWWW(string relativePath)
    {
        string tempPath = GameConst.ResRootPath + relativePath;
        string path = GetPersistentPath(tempPath);
        if (File.Exists(path))
        {
            return GetPersistentPathForWWW(tempPath);
        }

        return GetStreamingAssetPathForWWW(relativePath);
    }

    public static string GetClockTime(int milliseconds)
    {
        int seconds = milliseconds / 1000;
        int hour = seconds / 3600;
        int minute = (seconds - hour * 3600) / 60;
        int second = seconds % 60;
        return (hour < 10 ? "0" : "") + hour + ":" + (minute < 10 ? "0" : "") + minute + ":" +
               (second < 10 ? "0" : "") + second;
    }

    public static void ToggleOn(GameObject[] goes, int index)
    {
        for (var i = 0; i < goes.Length; i++)
            goes[i].SetActive(i == index);
    }

    public static void ToggleOff(GameObject[] goes, int index)
    {
        for (var i = 0; i < goes.Length; i++)
            goes[i].SetActive(i != index);
    }

    public static void ActiveN(GameObject[] goes, int n)
    {
        for (var i = 0; i < goes.Length; i++)
            goes[i].SetActive(i < n);
    }

    public static bool IsLowEndProduct()
    {
#if UNITY_IPHONE

        if (UnityEngine.iOS.Device.generation < UnityEngine.iOS.DeviceGeneration.iPhone5 || SystemInfo.systemMemorySize < 800)
        {
            return true;
        }
#endif

#if UNITY_ANDROID
        if (SystemInfo.systemMemorySize <= 2048)
        {
            return true;
        }

        return false;
#endif
        return false;
    }

    public static void PrinteSystemInfo()
    {
        Debug.LogError("persistentDataPath:" + Application.persistentDataPath);
        Debug.LogError("streampath:" + Application.streamingAssetsPath);
        Debug.LogError("OperatingSystem:" + SystemInfo.operatingSystem);
        Debug.LogError("SystemMemorySize:" + SystemInfo.systemMemorySize);
        Debug.LogError("ProcessorCount:" + SystemInfo.processorCount);
        Debug.LogError("ProcessorType:" + SystemInfo.processorType);
#if UNITY_IPHONE
        Debug.LogError("IPhone Generation:" + SystemInfo.supportsShadows);
#endif
    }

    public static string GetAndroidId()
    {
#if UNITY_ANDROID
        try
        {
            AndroidJavaClass clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject objActivity = clsUnity.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject objResolver = objActivity.Call<AndroidJavaObject>("getContentResolver");
            AndroidJavaClass clsSecure = new AndroidJavaClass("android.provider.Settings$Secure");
            return clsSecure.CallStatic<string>("getString", objResolver, "android_id");
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
#endif
        return "";
    }

    static public T FindInParents<T>(GameObject go) where T : Component
    {
        if (go == null)
            return null;

        T comp = null;
        if (comp == null)
        {
            Transform t = go.transform.parent;

            while (t != null && comp == null)
            {
                comp = t.gameObject.GetComponent<T>();
                t = t.parent;
            }
        }

        return comp;
    }

    static public string FormatDistance(float dis)
    {
        int nDis = (int) (dis * 1000);
        if (nDis < 100)
            return "<100m";
        else if (nDis < 1000)
        {
            return nDis.ToString() + "m";
        }
        else
        {
            string result = String.Format("{0:N2}km", dis);
            return result;
        }
    }
#region
    //调用系统插件
//     public static void PickImageFromAlbum(MediaLibrary.PickImageCompletion pickImageFinished)
//     {
//         // Pick image
//         NPBinding.MediaLibrary.PickImage(eImageSource.ALBUM, 1.0f, pickImageFinished);
//     }
// 
//     public static void ReadContacts(AddressBook.ReadContactsCompletion onReceivingContacts)
//     {
//         NPBinding.AddressBook.ReadContacts(onReceivingContacts);
//     }
// 
//     public static void SendTextMessage(string photoNum, Sharing.SharingCompletion finishedSharing)
//     {
//         if (String.IsNullOrEmpty(photoNum))
//             return;
//         // Create composer
//         MessageShareComposer _composer = new MessageShareComposer();
//         _composer.Body = GetString(94);
//         string[] toRecv = {photoNum};
//         _composer.ToRecipients = toRecv;
//         // Show message composer
//         NPBinding.Sharing.ShowView(_composer, finishedSharing);
//     }
#endregion
    //判断是否是低端机
    public enum PhoneLevelEnum
    {
        None,
        LowEndPhone,
        MiddleEndPhone,
        HighEndPhone,
    }
    public static PhoneLevelEnum PhoneLevel;
    private static PhoneLevelEnum IsLowEndPhonefun()
    {
#if UNITY_ANDROID
        int screenMin = Mathf.Min(Screen.width, Screen.height);
        if (screenMin<1080)
        {
            return PhoneLevelEnum.LowEndPhone;
        }
        else if (SystemInfo.systemMemorySize < 2048)
        {
            return PhoneLevelEnum.MiddleEndPhone;
        }
        
#elif UNITY_IPHONE
        if (SystemInfo.systemMemorySize < 1024)
            return PhoneLevelEnum.LowEndPhone; 
#endif
        return PhoneLevelEnum.HighEndPhone;


    }
    public static GameObject GetChildObjByName(string name, GameObject go)
    {
        Transform[] objs = go.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].name == name)
            {
                return objs[i].gameObject;
            }
        }
        Debug.LogError("not find" + name);
        return null;
    }

    public static void HideAllChild(Transform t)
    {
        Transform[] ts = t.GetComponentsInChildren<Transform>();
        foreach (var g in ts)
        {
            g.gameObject.SetActive(false);
        }
    }

    public static bool IsIphoneX()
    {
        string device = SystemInfo.deviceModel;
        if (device== "iPhone10,3"|| device == "iPhone10,6")
        {
            return true;
        }
        return false;
    }

    private static  string[] m_VivoDeviceName = new string[] { "vivo X21", "vivo X21A", "vivo X21A", "vivo X21UD", "vivo X21UD A", "vivo Y85", "vivo Y85A" };
    public static bool IsVivo()
    {
        string device = SystemInfo.deviceModel;
        foreach (var name in m_VivoDeviceName)
        {
            if (device.Contains(name))
            {
                return true;
            }
        }
        return false;
    }
}