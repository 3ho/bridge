using System;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class GameConst
{
    //public static string[] PreloadAbs = { GameConst.FONT_AB_PATH, GameConst.COMMON_AB_PATH, GameConst.Shader_AB_PATH, GameConst.Texture_Woman_PATH, GameConst.Texture_Man_PATH };
    public static string[] PreloadAbs = { };

    public const string VIEW_AB_PATH = @"ui/";
    public const string AUDIO_AB_PATH = "audio/";

    public const string ServerTxtPath = "gameconfig/server.txt";
    //for Tag
    public const string OtherPlayerColliderTag = "OtherPlayerCollider";
    public const string SelfColliderTag = "SelfCollider";
    public const string CarColliderTag = "Car";
    public const string WeiqiangTag = "Weiqiang";
    
    public const string ResRootPath = "res/";

    public const float CanRushInputValue = 0.917f;
    public const string FeedbackPath = "";
    public const string BasicRole = "role/role.ab";
    public const bool IsOpenActive = true;
    //public static string[] DirectoryPaths ;
    public static void Init()
    {
        //ResMgr.Ins.LoadTextLocal("gameconfig/DirectoryPaths.txt", LoadDirectoryPaths);
    }

    //public static void LoadDirectoryPaths(string directory)
    //{
    //    DirectoryPaths = directory.Split('\n');
    //    for (int i = 0, max = DirectoryPaths.Length; i < max; i++)
    //    {
    //        if (!string.IsNullOrEmpty(DirectoryPaths[i]))
    //            CreatPath(DirectoryPaths[i]);
    //    }
    //}
    public static void CreatPath(string directory)
    {
        string path = Utils.GetPersistentPath(directory);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path.Trim());
        }
    }
  
    //本地版本号文件路径
    public const string LocalVisionPath = "version/version.txt";

    //版本号
    public static string Version = "1.0.0.0";
    public static string OldVersion = "0.0.0.0";

    public static string LocalFileListpath = "version/FileList.txt";

    public static string LocalDatapath;

    public static string CodeVersion = "0.81.0.0";

    /// <summary>
    /// 进入游戏需要最先加载的东西
    /// </summary>
    public const string FONT_AB_PATH = @"fonts/fonts.ab";

    public const string COMMON_AB_PATH = @"icon/common.ab";

    public const string EMOJI_AB_PATH = "icon/emoji.ab";
    public const string Shader_AB_PATH = "shader/shader.ab";
    public const string Texture_Woman_PATH = "texture/woman.ab";
    public const string Texture_Man_PATH = "texture/man.ab";

    //背景音乐
    public const string OUTBATTLEMC = "Bgm_loading";

    public const string INBATTLEMC = "youxineibgm";

    public const string SmallHeadSprite = "";

    public static List<Color> _OwnerMarkColors = new List<Color>();

    //资源路径
    public const string RoleResPath = "role/";
    public const string EffectResPath = "effect/";
    public const string WaterMarkResPath = "other/";

    public static bool IsCheck = false;
    public static bool IsSdkLogin= false;

    //聊天记录保存路径
    public static string ChatCommonRecordPath = "chatcommonrecord.oc";
    public static string ChatRecordPath = "chatrecord.oc";
    public static string ChatVoiceSavePath = "voice";

    //语音上传路径
    public const string VoiceUpPath = "voicedown";

    public const string VoiceDownPath = "voiceup";

    //1970年秒数
    //public const int VoicTick1970 = "voiceup";
    //屏蔽字路径
    public const string SensitiveWordPath = "sensitiveWord/sw.txt";

    
    
    //CurrentTime
    public static string CurrentTime
    {
        get
        {
            var currentTime = System.DateTime.Now;
            string timename = currentTime.Year + "" + currentTime.Month + currentTime.Day + currentTime.Hour +
                              currentTime.Minute + currentTime.Second + currentTime.Millisecond;
            return timename;
        }
    }
    //第一次登陆sdk放bundle协议
    public static bool FirstSdk = false;

    public static List<Color> OwnerMarkColors
    {
        get
        {
            if (_OwnerMarkColors.Count <= 0)
            {
                _OwnerMarkColors.Add(new Color(255f / 255f, 107f / 255f, 107f / 255f));
                _OwnerMarkColors.Add(new Color(255f / 255f, 187f / 255f, 127f / 255f));
                _OwnerMarkColors.Add(new Color(255f / 255f, 255f / 255f, 129f / 255f));
                _OwnerMarkColors.Add(new Color(177f / 255f, 255f / 255f, 125f / 255f));
                _OwnerMarkColors.Add(new Color(148f / 255f, 255f / 255f, 200f / 255f));
                _OwnerMarkColors.Add(new Color(148f / 255f, 230f / 255f, 255f / 255f));
                _OwnerMarkColors.Add(new Color(0f / 255f, 50f / 255f, 255f / 255f));
                _OwnerMarkColors.Add(new Color(148f / 255f, 155f / 255f, 255f / 255f));
                _OwnerMarkColors.Add(new Color(230f / 255f, 168f / 255f, 255f / 255f));
                _OwnerMarkColors.Add(new Color(255f / 255f, 168f / 255f, 199f / 255f));
            }
            return _OwnerMarkColors;
        }
    }

    public static Color HobbySelect = new Color(68f / 255f, 149f / 255f, 150f / 255f);
    public static Color HobbyBg = new Color(109f / 255f, 109f / 255f, 109f / 255f);
    public static Color ShopItemBg = new Color(165f / 255f, 236f / 255f, 116f / 255f);

    public static float[] culcPos(int count, int width)
    {
        int halfwidth = width >> 1;
        float[] pos = new float[count];
        if (count < 2)
            return pos;
        int tempCount = count - 1;
        for (int i = 0; i < count; i++)
        {
            pos[i] = tempCount * halfwidth;
            tempCount -= 2;
        }
        return pos;
    }

    public static Vector3[] culcPos(int count, int width, int height, int rows, int cols)
    {

        Vector3[] pos = new Vector3[count];
        for (int i = 0; i < count; i++)
        {
        }
        return pos;
    }
    public static string GetPhotoPath(long roleid, int vision)
    {
        return string.Empty;
    }

    //public static string GetHeadPath(int headId)
    //{
    //    cfg.HeadCfg headcfg = cfg.HeadCfg.Get(headId);
    //    if (headcfg == null)
    //        return string.Empty;
    //    return headcfg.resPath;
    //}

    //计算两个设备直接距离
    public static float GetDistance(int lat1, int lng1, int lat2, int lng2)
    {
        float radLat1 = rad(lat1);
        float radLat2 = rad(lat2);
        float a = radLat1 - radLat2;
        float b = rad(lng1) - rad(lng2);

        float s = 2 * Mathf.Asin(Mathf.Sqrt(Mathf.Pow(Mathf.Sin(a / 2), 2) +
                          Mathf.Cos(radLat1) * Mathf.Cos(radLat2) * Mathf.Pow(Mathf.Sin(b / 2), 2)));
        s = s * 6378.137f; //地球是有多胖km
        s = Mathf.Round(s * 10000) / 10000;
        return s;
    }

    private static float rad(float d)
    {
        return d * Mathf.PI / (180f * 1000f);
    }

    public static int GetVision(string num)
    {
        if (string.IsNullOrEmpty(num))
            return 0;
        string[] nums = num.Split('_');
        if (nums.Length == 2)
            return int.Parse(nums[1]);
        return 1;
    }


    //public static bool IsRoleIdEven
    //{
    //    get
    //    {
    //        if (RoleMgr.Ins.info != null)
    //            return (RoleMgr.Ins.info.roleId>>12) % 2 == 0;
    //        return false;
    //    }
    //}

}

public class SpritePath
{
    public const string SMALLHEAD = @"EUIFrameABs/icon/smallhead.ab/";
}
