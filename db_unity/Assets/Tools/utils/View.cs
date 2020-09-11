using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public abstract class View : MonoBehaviour
{
    protected virtual void onInit()
    {
    }

    protected virtual void onShow(System.Object param = null, string childView = null)
    {
    }

    protected virtual void onHide(string childView = null)
    {
    }

    protected virtual void onDestroy()
    {
    }

    public bool IsShow
    {
        get { return gameObject.activeSelf; }
    }

    public void _DoInit()
    {
        try
        {
           onInit();
        }
        catch (Exception e)
        {
            Debug.LogError("[view]init view " + gameObject.name + " error");
            Debug.LogError(e);
        }
    }

    public void _DoShow(string childView, System.Object param = null)
    {
        try
        {
           onShow(param, childView);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Debug.LogError("[view]show view " + gameObject.name + " error.");
        }
    }

    public virtual void _SetRenderSort(int RendingSort)
    {
        Canvas canvas = this.GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError(canvas.gameObject.name + "canvas is null");
            return;
        }
        Canvas parentCanvas = transform.parent.GetComponent<Canvas>();
        canvas.pixelPerfect = false;
        canvas.overrideSorting = true;
        canvas.sortingOrder = RendingSort * 10 + parentCanvas.sortingOrder;
    }

    public virtual void OpenSortRenderSort(bool value)
    {
        Canvas canvas = this.GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError(canvas.gameObject.name + "canvas is null");
            return;
        }
        canvas.overrideSorting = true;
    }

    public void _DoHide(string childName)
    {
        try
        {
            onHide(childName);
        }
        catch (Exception e)
        {
            Debug.LogError("[view]hide view " + gameObject.name + " error.");
            Debug.LogError(e);
        }
    }

    public void _DoDestroy()
    {
        if (IsShow)
        {
            Hide();
        }

        try
        {
            onDestroy();
        }
        catch (Exception e)
        {
            Debug.LogError("[view]destroy view " + gameObject.name + " error.");
            Debug.LogError(e);
        }

        DestroyImmediate(gameObject);
    }

    public void Hide()
    {
        ViewMgr.Ins.HideView(GetType().Name);
    }

    public void Destroy()
    {
        ViewMgr.Ins.Destroy(GetType().Name);
    }

    // utils
    public static GameObject GetRelativePathGameObject(GameObject go, GameObject parent, GameObject child)
    {
        if (go == parent)
            return child;

        return go.transform.Find(Utils.GetRelativePath(parent, child)).gameObject;
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

    public static void BindOnClick(GameObject go, UIEventListener.VoidDelegate func)
    {
        UIEventListener.Get(go).onClick = func;
    }

    public static string GetLabelText(GameObject go)
    {
        Text label = go.GetComponent<Text>();
        if (!label)
            return "";
        return label == null ? null : label.text;
    }

    public static string GetButtonText(GameObject go)
    {
        Text label = go.GetComponentInChildren<Text>();
        if (!label)
        {
            return "";
        }
        return label == null ? null : label.text;
    }

    public static void SetButtonText(GameObject go, string text)
    {
        Text label = go.GetComponentInChildren<Text>();
        if (!label)
        {
            return;
        }
        label.text = text;
    }

    public static int GetDropDown(GameObject go)
    {
        Dropdown dropdown = go.GetComponent<Dropdown>();
        return dropdown == null ? 0 : dropdown.value;
    }

    public static void SetDropDown(GameObject go, int value)
    {
        Dropdown dropdown = go.GetComponent<Dropdown>();
        if (!dropdown)
        {
            return;
        }
        dropdown.value = value;
    }

    public static void DropDownAddItem(GameObject go, string value)
    {
        Dropdown dropdown = go.GetComponent<Dropdown>();
        if (!dropdown)
        {
            return;
        }
        Dropdown.OptionData op = new Dropdown.OptionData();
        op.text = value;
        dropdown.options.Add(op);
    }

    public static void SetLabelText(GameObject go, System.Object text,bool isTranslate = true)
    {
        if (go == null)
            return;
        Text label = go.GetComponent<Text>();
        if (!label)
        {
            Debug.LogError(go.name + "not Have Text");
            return;
        }
        //string content = isTranslate ? MultiLanguageMgr.Ins.GetLanguage(text.ToString()) : text.ToString();
        //if (label.text == content)
        //    return;
        //label.text = content;
        label.text = text.ToString();
    }

    public static void SetLabelText(Text label, System.Object text, bool isTranslate = true)
    {
        if (label == null)
            return;
        //string content = isTranslate ? MultiLanguageMgr.Ins.GetLanguage(text.ToString()) : text.ToString();
        //if (label.text == content)
        //    return;
        //label.text = content;
        label.text = text.ToString();
    }

    public static void SetLabelText(GameObject go, System.Object text, Color color)
    {
        Text label = go.GetComponent<Text>();
        if (!label)
            return;
        //label.text = MultiLanguageMgr.Ins.GetLanguage(text.ToString());
        label.text = text.ToString();
        label.color = color;
    }

    public static void SetLabelColor(GameObject go, Color color)
    {
        Text label = go.GetComponent<Text>();
        if (!label)
            return;
        label.color = color;
    }

    public static void SetLabelColor(Text _text, Color color)
    {
        if (!_text)
            return;
        _text.color = color;
    }

    public static void SetSlider(GameObject go, float progress)
    {
        Slider slider = go.GetComponent<Slider>();
        if (!slider)
            return;
        slider.value = progress;
    }

    public static void SetSlider(Slider slider, float progress)
    {
        if (!slider)
            return;
        slider.value = progress;
    }

    public static void SetSlider(GameObject go, float cur, float max)
    {
        Slider slider = go.GetComponent<Slider>();
        if (!slider)
            return;
        if (max == 0)
            slider.value = 1;
        else
            slider.value = cur / max;
    }

    public static void SetCheckbox(GameObject go, bool bChecked)
    {
        Toggle checkbox = go.GetComponent<Toggle>();
        if (!checkbox)
            return;
        checkbox.isOn = bChecked;
    }

    public static bool IsCheckboxChecked(GameObject go)
    {
        Toggle toggle = go.GetComponent<Toggle>();
        if (!toggle)
            return false;
        return toggle.isOn;
    }

    public static void SetInput(GameObject go, string value)
    {
        InputField inputField = go.GetComponent<InputField>();
        if (inputField)
        {
            inputField.text = value;
        }
    }

    public static string GetInputText(GameObject go)
    {
        InputField inputFiled = go.GetComponent<InputField>();
        if (inputFiled)
        {
            return inputFiled.text;
        }
        return "";
    }

    public static void SetInputText(GameObject go, string text)
    {
        InputField inputFiled = go.GetComponent<InputField>();
        if (inputFiled)
        {
            inputFiled.text = text;
        }
    }

    public static void SetProgressBar(GameObject go, float progress)
    {
        Image image = go.GetComponent<Image>();
        image.fillAmount = progress;
    }

    public static void SetImageColor(GameObject go, Color color)
    {
        Image image = go.GetComponent<Image>();
        image.color = color;
    }

    public static void SetImageColor(Image image, Color color)
    {
        if (image!=null)
        {
            image.color = color;
        }
       
    }

    public static void SetSpriteCommon(string abPath, string name, GameObject go, bool isSetNativeSize = false)//一个ab对应多张图片
    {
        Image image = go.GetComponent<Image>();
        if (image == null)
        {
            return;
        }
        ImageName imgName = image.gameObject.GetComponent<ImageName>();
        if (imgName == null)
        {
            imgName = image.gameObject.AddComponent<ImageName>();
        }

        imgName.imageName = name;

        if (image.sprite != null && image.sprite.name == name)
        {
            if (image.enabled == false)
            {
                image.enabled = true;
            }
            return;
        }

        if (string.IsNullOrEmpty(name))
        {
            ResMgr.Ins.CleanRef(image.gameObject);
            image.sprite = null;
        }
        else
        {
            image.enabled = false;
            ResMgr.Ins.LoadAssetFromAB<Sprite>(abPath, name, image.gameObject, o =>
                {
                    if (image == null)
                    {
                        DestroyImmediate(o, true);
                        return;
                    }
                    if (o.name == imgName.imageName)
                    {
                        image.sprite = o as Sprite;
                        if (isSetNativeSize)
                            image.SetNativeSize();
                    }
                    image.enabled = true;
                });
        }
    }

    public static void SetSpriteABCommon(string abPath, GameObject go, bool isSetNativeSize = false)//一个ab对应一张图片
    {
        Image image = go.GetComponent<Image>();
        if (image == null)
        {
            return;
        }
        ImageName imgName = image.gameObject.GetComponent<ImageName>();
        if (imgName == null)
        {
            imgName = image.gameObject.AddComponent<ImageName>();
        }
        imgName.imageName = abPath;
        if (image.sprite != null)
        {
            if (abPath.EndsWith(image.sprite.name))
            {
                if (image.enabled == false)
                {
                    image.enabled = true;
                }
                return;
            }
        }
        if (string.IsNullOrEmpty(abPath))
        {
            image.sprite = null;
            ResMgr.Ins.CleanRef(image.gameObject);
        }
        else
        {
            image.enabled = false;
            ResMgr.Ins.LoadAssetFromAB<Sprite>(abPath, null, image.gameObject, o =>
                {
                    if (image == null)
                    {
                        DestroyImmediate(o,true);
                        return;
                    }
                    if (abPath == imgName.imageName)
                    {
                        image.sprite = o as Sprite;
                        if (isSetNativeSize)
                            image.SetNativeSize();
                    }
                    image.enabled = true;
                });
        }
    }

    public static void SetQualitySprite(GameObject go, string name)
    {
        SetSpriteABCommon("icon/pinzhi/" + name, go, false);
    }

    public static void SetShopTagSprite(GameObject go, string name)
    {
        SetSpriteCommon("icon/shop.ab",name,go);
    }
    
    public static void SetBigHeadSprite(GameObject go, string abPath)
    {
        SetSpriteABCommon("bigherohead/" + abPath + ".ab", go);
    }

    public static void SetFragSprite(GameObject go, string abPath)
    {
        SetSpriteCommon("icon/chip.ab", abPath, go);
    }
    private static Dictionary<string, Sprite> headDic = new Dictionary<string, Sprite>();
    public static Dictionary<GameObject, long> headGos = new Dictionary<GameObject, long>();

    public static void Attach(GameObject go, long id)
    {
        if (headGos.ContainsKey(go))
            headGos[go] = id;
        else
        {
            headGos.Add(go, id);
        }
    }

    //默认头像Sprite
    private static Sprite HeadDefaultSpt;

    public static long GetId(GameObject go)
    {
        return headGos.ContainsKey(go) ? headGos[go] : 0;
    }

//     public static void SetHeadSprite(GameObject go, long roleid, int headId, string headVision)
//     {
//         if (headId < 0)
//         {
//             if (headId == cfg.Consts.HEAD_SERVER_PHOTO)
//             {
//                 string headname = headVision;
//                 if (headDic.ContainsKey(headname))
//                 {
//                     SetSprite(go, headDic[headname]);
//                     return;
//                 }
//                 if (HeadDefaultSpt == null)
//                 {
//                     cfg.HeadCfg headcfg = cfg.HeadCfg.Get(cfg.Consts.HEAD_DEFAULT);
//                     ResMgr.Ins.LoadAssetFromAB<Sprite>(headcfg.resPath, null, go, o =>
//                     {
//                         HeadDefaultSpt = o as Sprite;
//                     });
//                 }
//                 else
//                 {
//                     SetSprite(go, HeadDefaultSpt);
//                 }
//                 Attach(go, roleid);
//                 ResMgr.Ins.GetHeadFromServer(roleid, headVision, o =>
//                 {
//                     long attid = GetId(go);
//                     if (attid == roleid)
//                     {
//                         Texture2D texture2D = o;
//                         Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, 100, 100), Vector2.zero);
//                         sprite.name = "server" + roleid;
//                         string headTempName = headVision;
//                         if (!headDic.ContainsKey(headTempName))
//                             headDic.Add(headTempName, sprite);
//                         SetSprite(go, sprite);
//                     }
//                 });
//             }
//             return;
//         }
//         SetHeadSprite(go, headId);
//     }
// 
//     public static void SetHeadFrameSprite(GameObject go, int headFrameId)
//     {
//         cfg.HeadFrameCfg headframecfg = cfg.HeadFrameCfg.Get(headFrameId == 0 ? headFrameId + 1 : headFrameId);
//         if (headframecfg == null) return;
//         SetSpriteAsync(go, headframecfg.resPath);
//     }

    public static void SetHeadSprite(GameObject go, string name)
    {
        //SetSpriteCommon("icon/head.ab", name, go);
        SetSpriteAsync(go, name);
    }

    public static void SetProductSprite(GameObject go, string name, bool isSetNativeSize = false)
    {
        SetSpriteCommon("icon/product.ab", name, go, isSetNativeSize);
    }

    public static void SetCommonSprite(GameObject go, string name)
    {
        SetSpriteCommon("icon/common1.ab", name, go);
    }

    //服务器保存的照片头像
    public static void SetPhotoHead(GameObject go, string name)
    {
    }

    public static void SetSpriteAlpha(GameObject go, float value)
    {
        Image image = go.GetComponent<Image>();
        if (!image)
            return;
        image.color = new Color(image.color.r, image.color.g, image.color.b, value);
    }

    public static float GetUIWidth(GameObject go)
    {
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        if (!rectTransform)
            return 0f;
        return rectTransform.rect.width;
    }

    public static float GetTextWidth(Transform go)
    {
        Text text = go.GetComponent<Text>();
        if (!text)
            return 0f;
        return text.preferredWidth;
    }

    public static void SetBattleLevelSprite(GameObject go, string iconName)
    {
        SetSpriteAsync(go, "icon/grading/" + iconName);
    }

    public static void SetBattleGunSprite(GameObject go, string iconName)
    {
        SetSpriteABCommon( "icon/" + iconName,go,true);
    }
    public static void SetItemSprite(GameObject go, string name, bool isSetNativeSize = false)
    {
        SetSpriteABCommon("icon/" + name, go, isSetNativeSize);
    }

    public static void SetQrCode(GameObject go)
    {
            string packageName = "";
            bool find = false;
#if UNITY_ANDROID
            //try
            //{
            //    packageName = AndroidSDKInterface.Instance.GetPackageName();
            //}catch(Exception e)
            //{
            //}

            //foreach (cfg.QrCodeCfg qrCodeCfg in cfg.QrCodeCfg.GetAll().Values)
            //{
            //    if (qrCodeCfg.packageName.Equals(packageName))
            //    {
            //        find = true;
            //        SetTexture(go, "texture/" + qrCodeCfg.icon);
            //        break;
            //    }
            //}
#endif

#if UNITY_IPHONE
            packageName = "com.hero.ca.ios";
            foreach (cfg.QrCodeCfg qrCodeCfg in cfg.QrCodeCfg.GetAll().Values)
            {
                if (qrCodeCfg.packageName.Equals(packageName))
                {
                    find = true;
                    SetTexture(go, "texture/" + qrCodeCfg.icon);
                    break;
                }
            }
#endif
            go.SetActive(find);
        
    }

    public static void SetSpriteAsync(GameObject go, string abPath,bool isSetNativeSize =false)
    {
        SetSpriteABCommon(abPath, go,isSetNativeSize);
    }

    public static void SetSprite(GameObject go, Sprite sprite)
    {
        Image image = go.GetComponent<Image>();
        if (!image)
            return;
        image.enabled = true;
        image.sprite = sprite;
    }

    public static void SetTexture(GameObject go, string abPath)
    {
        RawImage rawImage = go.GetComponent<RawImage>();
        if (rawImage == null)
            return;

        if (string.IsNullOrEmpty(abPath))
        {
            rawImage.texture = null;
            ResMgr.Ins.CleanRef(go);
        }
        else
        {
            rawImage.enabled = false;
            ResMgr.Ins.LoadAssetFromAB<Texture>(abPath + ".ab", null, go, o =>
            {
                if (rawImage==null)
                {
                    DestroyImmediate(o,true);
                    return;
                }
                rawImage.texture = o as Texture;
                rawImage.enabled = true;
            });
        }
    }

    public static void SetTexture(GameObject go, Texture tex)
    {
        RawImage rawImage = go.GetComponent<RawImage>();
        if (rawImage == null)
            return;
        rawImage.texture = tex;
    }

    public static void SetTextureAsync(GameObject go, string abPath, string name)
    {
        RawImage rawImage = go.GetComponent<RawImage>();
        if (rawImage == null)
            return;

        if (string.IsNullOrEmpty(name))
        {
            rawImage.texture = null;
            ResMgr.Ins.CleanRef(go);
        }
        else
        {
            ResMgr.Ins.LoadAssetFromAB<Texture2D>(abPath, name, go, o => rawImage.texture = o as Texture2D);
        }
    }

    //public static void DoHideAnim(GameObject obj, GameObject canClickBtn = null)
    //{
    //    if (canClickBtn != null)
    //    {
    //        canClickBtn.SetActive(false);
    //    }
    //    obj.transform.DOScale(new Vector3(0.4f, 0.4f, 0.4f), 0.5f);
    //    obj.transform.DOLocalJump(new Vector3(-900f, 0f), 100, 4, 2f, false).OnComplete(() =>
    //        {
    //            obj.SetActive(false);
    //            obj.transform.localPosition = new Vector3(0f, 0f, 0f);
    //            obj.transform.localScale = new Vector3(1, 1, 1);
    //            if (canClickBtn != null)
    //            {
    //                canClickBtn.SetActive(true);
    //            }
    //        });
    //}

    //public static void DoEnterAnim(GameObject obj)
    //{
    //    obj.transform.localPosition = new Vector3(900f, 0, 0);
    //    obj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    //    obj.SetActive(true);
    //    obj.transform.DOScale(new Vector3(1f, 1f, 1f), 1f);
    //    obj.transform.DOLocalJump(new Vector3(0f, 0f), 100, 4, 1.5f, false);
    //}

    //public static void DoEnterSaleAnim(GameObject go)
    //{
    //    go.SetActive(true);
    //    go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    //    go.transform.DOScale(new Vector3(1, 1, 1), 0.4f);
    //}

    //public static void DoDownToUpAnim(GameObject go)
    //{
    //    go.transform.localPosition = new Vector3(0, 1320f, 0);
    //    go.transform.DOLocalMoveY(0, 0.4f);
    //}

    //public static void DoUpToDownAnim(GameObject go)
    //{
    //    go.transform.localPosition = new Vector3(0, -1320f, 0);
    //    go.transform.DOLocalMoveY(0, 0.4f);
    //}

    //public static void DoLeftToRightAnim(GameObject go)
    //{
    //    go.transform.localPosition = new Vector3(-900f, 0, 0);
    //    go.transform.DOLocalMoveX(0, 0.4f);
    //}

    //public static void DoRightToLeftAnim(GameObject go)
    //{
    //    go.transform.localPosition = new Vector3(900, 0, 0);
    //    go.SetActive(true);
    //    go.transform.DOLocalMoveX(0, 0.4f);
    //}

    //public static void DoFanzhuanAnim(GameObject go)
    //{
    //    go.transform.DOFlip();
    //}

    public IEnumerator DoMatchingAnim(GameObject go)
    {
        List<string> fonts = new List<string>() { "", ".", "..", "..." };
        int i = 0;
        while (true)
        {
            SetLabelText(go, fonts[i]);
            i++;
            yield return new WaitForSeconds(0.5f);
            if (i > 3)
            {
                i = 0;
            }
        }
    }

    public IEnumerator DoLoadingAnim(GameObject go)
    {
        List<string> fonts = new List<string>() { "Loading", "Loading.", "Loading..", "Loading..." };
        int i = 0;
        while (true)
        {
            SetLabelText(go, fonts[i]);
            i++;
            yield return new WaitForSeconds(0.5f);
            if (i > 3)
            {
                i = 0;
            }
        }
    }

    protected virtual void Awake()
    {
        //MultiLanguageMgr.Ins.SetMultiLanguage(gameObject);
    }
}