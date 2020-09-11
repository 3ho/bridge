using UnityEngine;
using System.Collections;

public sealed class EditorTools {
    public static string GetPath(GameObject go, GameObject root = null)
    {
        string path = go.name;
        Transform parent = go.transform.parent;
        while (parent != null && parent.gameObject != root)
        {
            path = parent.gameObject.name + "/" + path;
            parent = parent.parent;
        }

        return path;
    }
    public static string GetFileName(string path)
    {
        int begin = path.LastIndexOf("/");
        if (begin < 0)
            begin = path.LastIndexOf("\\");
        int end = path.LastIndexOf(".");
        return path.Substring(begin + 1, end - begin - 1);
    }
    //得到文件父文件夹的名字
    public static string GetParentFileName(string path,int parentIndex)
    {
        string[] filenames = path.Split('/');
        return filenames[filenames.Length - parentIndex];
    }
}
