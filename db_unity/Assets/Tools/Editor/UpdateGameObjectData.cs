using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class UpdateGameObjectData : Editor
{
    // [MenuItem("Tools/UpdateGameObjectData")]
    static void UndateGameObjectData()
    {
        Init();
        ExportAll("Assets/Resources/ui/prefab");
    }

    [MenuItem("Tools/UpdateThisPrefabGameObjectData")]
    static void UpdateThisPrefabGameObjectData()
    {
        Init();
        GameObject[] gos = UnityEditor.Selection.gameObjects;
        foreach (GameObject go in gos)
        {
            if (!go.name.Contains("Panel"))
            {
                Debug.LogError("Error," + go.name + ",is not Panel");
                continue;
            }
            Export(go);
        }
        Debug.Log("UpdateThisPrefabGameObjectData ... finish");
    }

    public static void CleraPrefabScript()
    {
        CleanAll("Assets/Resources/ui/prefab");
    }

    private const string UI_CODE_PATH = @"Assets/Scripts/view/";
    private const string UI_AB_PATH = @"Assets/StreamingAssets/ui";
    private const string UI_PREFAB_PATH = @"Assets/Resources/ui/prefab";

    private static readonly HashSet<string> ControllPrefixes = new HashSet<string>();

    private static void Init()
    {
        ControllPrefixes.Add("m");
        ControllPrefixes.Add("btn");
        ControllPrefixes.Add("ckb");
        ControllPrefixes.Add("pgsb");
        ControllPrefixes.Add("txt");
        ControllPrefixes.Add("inp");
        ControllPrefixes.Add("rdb");
        ControllPrefixes.Add("scp");
        ControllPrefixes.Add("grp");
        ControllPrefixes.Add("list");
    }

    private static void ExportAll(string folderPath)
    {
        DirectoryInfo rootFolder = new DirectoryInfo(folderPath);
        foreach (FileInfo file in rootFolder.GetFiles())
        {
            string fileName = file.FullName;
            if (file.Name.EndsWith(".prefab"))
            {
                GameObject go = AssetDatabase.LoadAssetAtPath(fileName.Substring(fileName.LastIndexOf("Assets")), typeof(GameObject)) as GameObject;
                Export(go);
            }
        }

        foreach (DirectoryInfo folder in rootFolder.GetDirectories())
        {
            ExportAll(folder.FullName);
        }
    }

    private static bool Export(GameObject rootGo)
    {
        rootGo.SetActive(true);
        GameObjectData data;
        DestroyImmediate(rootGo.GetComponent<GameObjectData>(), true);
        if (rootGo.GetComponent<GameObjectData>() == null)
        {
            data = rootGo.AddComponent<GameObjectData>();
        }
        else
        {
            DestroyImmediate(rootGo.GetComponent<GameObjectData>(), true);
            data = rootGo.AddComponent<GameObjectData>();
        }

        GenUICode(rootGo, data);

        //CleanScripts(root);
        //ExportUIAssetBundle(rootGo, rootGo.name);
        //保存prefab
        EditorUtility.SetDirty(rootGo);
        //DestroyImmediate(rootGo);
        return true;
    }

    private static void GenUICode(GameObject root, GameObjectData data)
    {
        List<GameObject> controls = new List<GameObject>();
        List<GameObject> groups = new List<GameObject>();
        Traverse(root, controls, groups, false);
        Dictionary<string, int> usedNames = new Dictionary<string, int>();

        #region 如果data中原先有数据就保留，将新加的数据排在后边
        List<string> gameobjectList = getGameobjectStrListByFlag(UI_CODE_PATH + root.name + ".cs");
        Dictionary<string, GameObject> gameObjectDic = TransListToDic(controls);
        string flag = string.Empty;
        if (gameobjectList.Count > 0)
        {
            for (int i = 0; i < gameobjectList.Count; i++)
            {
                if (gameObjectDic.ContainsKey(gameobjectList[i]))
                {
                    data.GameObjects.Add(gameObjectDic[gameobjectList[i]]);
                    flag += gameobjectList[i] + ",";
                    gameObjectDic.Remove(gameobjectList[i]);
                    //UnityEditor.EditorUtility.DisplayProgressBar(gameobjectList[i], "", (float)i/(float));

                    continue;
                }
            }
        }
        foreach (KeyValuePair<string, GameObject> go in gameObjectDic)
        {
            data.GameObjects.Add(go.Value);
            flag += go.Value.name + ",";
        }
        #endregion
        Generator gen = new Generator();
        //将标记写入文本中
        if (flag.Contains(","))
            flag = flag.Substring(0, flag.LastIndexOf(","));
        gen.Println("//-L-<" + flag + ">");
        gen.Println("using UnityEngine;");
        gen.Println("using UnityEngine.UI;");
        gen.Println("using System.Collections.Generic;");

        gen.Println();
        gen.Println("public partial class " + root.name + " : View");
        gen.Println("{");
        gen.AddIndent();
        foreach (GameObject controlObject in data.GameObjects)
        {
            int times = 1;
            string name = controlObject.name;
            if (usedNames.TryGetValue(name, out times))
            {
                name = name + "_" + times;
                ++times;
            }
            usedNames[controlObject.name] = times;

            GroupName groupName = controlObject.GetComponent<GroupName>();
            if (controlObject.GetComponent<UIGameObjectList>() != null)
            {
                if (controlObject.GetComponent<UIGameObjectList>().objects[0] == null)
                {
                    Debug.LogError("groupName have null===" + controlObject.name + "     rootName====" + root.name);
                }

                GroupName childGroupName = controlObject.GetComponent<UIGameObjectList>().objects[0].GetComponent<GroupName>();
                if (childGroupName != null)
                    gen.Printfln("private List<{0}> {1} = new List<{2}>();", childGroupName.groupName, name + "list", childGroupName.groupName);
                gen.Printfln("private GameObject[] {0};", name);
                gen.Printfln("private GameObject {0}Obj;", name);
            }
            else if (groupName == null)
            {
                gen.Println("private GameObject " + name + ";");
                if (name.Contains("txt_"))
                {
                    gen.Println("private Text " + name + "Text" + ";");
                }
            }
            else
            {
                gen.Printfln("private {0} {1};", groupName.groupName, name);
            }
        }
        gen.Println();
        gen.Println("protected override void Awake()");
        gen.Println("{");
        gen.AddIndent();
        //gen.Println("Debug.Log(\"test Awake \" + GetType().FullName + \" \" + this.GetHashCode());");
        gen.Println("base.Awake();");
        gen.Println();
        gen.Println("GameObjectData data = gameObject.GetComponent<GameObjectData>();");
        usedNames.Clear();
        for (int i = 0, max = data.GameObjects.Count; i < max; i++)
        {
            int times = 1;
            string name = data.GameObjects[i].name;
            if (usedNames.TryGetValue(name, out times))
            {
                name = name + "_" + times;
                ++times;
            }
            usedNames[data.GameObjects[i].name] = times;

            GroupName groupName = data.GameObjects[i].GetComponent<GroupName>();
            if (data.GameObjects[i].GetComponent<UIGameObjectList>() != null)
            {
                UIGameObjectList temp = data.GameObjects[i].GetComponent<UIGameObjectList>();
                GroupName childGroupName = temp.objects[0].GetComponent<GroupName>();
                gen.Printfln("{0} = data.GameObjects[{1}].gameObject.GetComponent<UIGameObjectList>().objects;", name, i.ToString());
                gen.Printfln("{0}Obj = data.GameObjects[{1}].gameObject;", name, i.ToString());
                if (childGroupName != null)
                {
                    gen.Printfln("for (int i = 0; i < {0}.Length; i++)", name);
                    gen.Println("{");
                    gen.Printfln(" {0}.Add(View.AddComponentIfNotExist<{1}>({2}[i].gameObject));", name + "list", childGroupName.groupName, name);
                    gen.Println("}");
                }
            }
            else if (groupName == null)
            {
                gen.Println(name + " =  data.GameObjects[" + i.ToString() + "].gameObject;");
                if (name.Contains("txt_"))
                {
                    gen.Println(name + "Text" + " = " + name + ".GetComponent<Text>();");
                }

            }
            else
            {

                gen.Printfln("{0} = View.AddComponentIfNotExist<{1}>(data.GameObjects[" + i.ToString() + "].gameObject);", name, groupName.groupName);
            }
        }

        gen.Println("ViewMgr.Ins.addView(this);");
        gen.ReduceIndent();
        gen.Println("}");
        gen.Println();

        HashSet<string> groupNames = new HashSet<string>();
        foreach (GameObject group in groups)
        {
            string groupName = group.GetComponent<GroupName>().groupName;
            if (groupNames.Contains(groupName))
                continue;

            GenGroupCode(group);
            groupNames.Add(groupName);
        }

        gen.ReduceIndent();
        gen.Println("}");
        StreamWriter sw = new StreamWriter(UI_CODE_PATH + root.name + ".cs", false);
        sw.Write(gen.GetContent());
        sw.Flush();
        sw.Close();
    }

    private static void Traverse(GameObject go, List<GameObject> controls, List<GameObject> groups, bool inGroup)
    {
        if (go == null)
            return;

        bool grouped = inGroup;
        foreach (Transform child in go.transform)
        {
            grouped = inGroup;
            go = child.gameObject;
            if (grouped)
            {
                if (go.GetComponent<GroupName>() != null)
                    groups.Add(go);
            }
            else
            {
                if (IsControll(go))
                    controls.Add(go);

                if (go.GetComponent<GroupName>() != null)
                {
                    grouped = true;
                    groups.Add(go);
                }
            }

            Traverse(child.gameObject, controls, groups, grouped);
        }
    }

    private static bool IsControll(GameObject go)
    {
        int index = go.name.IndexOf("_");
        if (index <= 0)
            return false;

        string prefix = go.name.Substring(0, index);
        return ControllPrefixes.Contains(prefix);
    }

    private static void GenGroupCode(GameObject root)
    {
        Generator gen = new Generator();
        gen.Println("using UnityEngine;");
        gen.Println("using UnityEngine.UI;");
        gen.Println("using System.Collections.Generic;");
        gen.Println();

        List<GameObject> controls = new List<GameObject>();
        List<GameObject> groups = new List<GameObject>();
        Traverse(root, controls, groups, false);
        controls.Sort(new GameObjectNameCMP());
        string className = root.GetComponent<GroupName>().groupName;
        gen.Println("public partial class " + className + " : MonoBehaviour");
        gen.Println("{");
        gen.AddIndent();

        Dictionary<string, int> usedNames = new Dictionary<string, int>();
        foreach (GameObject controlObject in controls)
        {
            int times = 1;
            string name = controlObject.name;
            if (usedNames.TryGetValue(name, out times))
            {
                name = name + "_" + times;
                ++times;
            }
            usedNames[controlObject.name] = times;
            GroupName groupName = controlObject.GetComponent<GroupName>();
            if (controlObject.GetComponent<UIGameObjectList>() != null)
            {
                if (controlObject.GetComponent<UIGameObjectList>().objects[0] == null)
                {
                    Debug.LogError("GgroupName have null===" + controlObject.name);
                }

                GroupName childGroupName = controlObject.GetComponent<UIGameObjectList>().objects[0].GetComponent<GroupName>();
                if (childGroupName != null)
                    gen.Printfln("public List<{0}> {1} = new List<{2}>();", childGroupName.groupName, name + "list", childGroupName.groupName);

                gen.Printfln("public GameObject[] {0};", name);
                gen.Printfln("public GameObject {0}Obj;", name);
            }
            else if (groupName == null)
            {
                gen.Println("public GameObject " + name + ";");
                if (name.Contains("txt_"))
                {
                    gen.Println("public Text " + name + "Text" + ";");
                }
            }
            else
            {
                gen.Printfln("public {0} {1};", groupName.groupName, name);
            }
        }
        gen.Printfln("public System.Object context;");
        gen.Println();

        gen.Println("private void Awake()");
        gen.Println("{");
        gen.AddIndent();

        usedNames.Clear();
        foreach (GameObject controlObject in controls)
        {
            int times = 1;
            string name = controlObject.name;
            if (usedNames.TryGetValue(name, out times))
            {
                name = name + "_" + times;
                ++times;
            }
            usedNames[controlObject.name] = times;
            GroupName groupName = controlObject.GetComponent<GroupName>();

            if (controlObject.GetComponent<UIGameObjectList>() != null)
            {
                UIGameObjectList temp = controlObject.GetComponent<UIGameObjectList>();
                GroupName childGroupName = temp.objects[0].GetComponent<GroupName>();
                gen.Printfln("{0} = transform.Find(@\"{1}\").gameObject.GetComponent<UIGameObjectList>().objects;", name, EditorTools.GetPath(controlObject, root));
                gen.Printfln("{0}Obj = transform.Find(@\"{1}\").gameObject;", name, EditorTools.GetPath(controlObject, root));
                if (childGroupName != null)
                {
                    gen.Printfln("for (int i = 0; i < {0}.Length; i++)", name);
                    gen.Println("{");
                    gen.Printfln(" {0}.Add(View.AddComponentIfNotExist<{1}>({2}[i].gameObject));", name + "list", childGroupName.groupName, name);
                    gen.Println("}");
                }
            }
            else if (groupName == null)
            {
                gen.Println(name + " =  transform.Find(@\"" + EditorTools.GetPath(controlObject, root) + "\").gameObject;");
                if (name.Contains("txt_"))
                {
                    gen.Println(name + "Text" + " = " + name + ".GetComponent<Text>();");
                }
            }
            else
            {
                gen.Printfln("{0} = View.AddComponentIfNotExist<{2}>(transform.Find(@\"{1}\").gameObject);", name, EditorTools.GetPath(controlObject, root), groupName.groupName);
            }
        }

        gen.ReduceIndent();
        gen.Println("}");

        gen.ReduceIndent();
        gen.Println("}");

        StreamWriter sw = new StreamWriter(UI_CODE_PATH + className + ".cs", false);
        sw.Write(gen.GetContent());
        sw.Flush();
        sw.Close();
    }

    private sealed class GameObjectNameCMP : IComparer<GameObject>
    {
        public int Compare(GameObject x, GameObject y)
        {
            return x.name.CompareTo(y.name);
        }
    }

    #region

    //列表转换成字典，gameobject.name为key
    static Dictionary<string, GameObject> TransListToDic(List<GameObject> gameobjects)
    {
        Dictionary<string, GameObject> gameobjectDic = new Dictionary<string, GameObject>();
        for (int i = 0, max = gameobjects.Count; i < max; i++)
        {
            if (gameobjectDic.ContainsKey(gameobjects[i].name))
            {
                gameobjectDic.Add(gameobjects[i].name + "_" + i, gameobjects[i]);
            }
            else
            {
                gameobjectDic.Add(gameobjects[i].name, gameobjects[i]);
            }
        }
        return gameobjectDic;
    }
    //访问文件返回链表
    static List<string> getGameobjectStrList(string path)
    {
        List<string> gameobjectStrDic = new List<string>();
        if (!File.Exists(path))
            return gameobjectStrDic;
        StreamReader sr = new StreamReader(path, Encoding.Default);
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            if (line.Contains("private"))
            {
                string sStr = "";
                if (line.IndexOf(' ') > 0)
                {
                    sStr = line.Substring(line.LastIndexOf(" "));
                    int index = sStr.IndexOf(";");
                    if (index > 0)
                    {
                        sStr = sStr.Substring(0, index);
                    }
                    gameobjectStrDic.Add(sStr.Trim());
                }
            }
        }
        sr.Close();
        return gameobjectStrDic;
    }
    //访问文件，根据标记//-L-<>返回链表
    static List<string> getGameobjectStrListByFlag(string path)
    {
        string[] gameObjectNames = new string[0];

        if (!File.Exists(path))
            return new List<string>();
        StreamReader sr = new StreamReader(path, Encoding.Default);
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            if (line.Contains("//-L-<"))
            {
                line = line.Substring(line.IndexOf("<"));
                int index = line.IndexOf(">");
                if (index > 0)
                {
                    line = line.Substring(1, index - 1);
                }
                gameObjectNames = line.Split(',');
                break;
            }
        }
        sr.Close();
        List<string> gameobjectStrList = new List<string>(gameObjectNames);
        return gameobjectStrList;
    }

    #endregion

    private static void CleanAll(string folderPath)
    {
        DirectoryInfo rootFolder = new DirectoryInfo(folderPath);
        foreach (FileInfo file in rootFolder.GetFiles())
        {
            string fileName = file.FullName;
            if (file.Name.EndsWith(".prefab"))
            {
                Debug.LogError("(fileName：" + fileName);

                GameObject go = AssetDatabase.LoadAssetAtPath(fileName.Substring(fileName.LastIndexOf("Assets")), typeof(GameObject)) as GameObject;
                //CleanScripts(go);
            }
        }

        foreach (DirectoryInfo folder in rootFolder.GetDirectories())
        {
            CleanAll(folder.FullName);
        }
    }

    private static void CleanScripts(GameObject go)
    {
        if (go == null)
            return;

        foreach (Transform child in go.transform)
        {
            CleanScripts(child.gameObject);
        }
        //CleanScript<UpdateGameObjectData>(go);
        CleanScript<GroupName>(go);
        //GroupName
    }

    private static void CleanScript<T>(GameObject go) where T : Component
    {
        T t = go.GetComponent<T>();
        while (t != null)
        {
            DestroyImmediate(t);
            t = go.GetComponent<T>();
        }
    }
}