using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public sealed class ColorUtils
{
    public const int R = 1;
    public const int B = 2;
    public const int G = 4;
    public const int X = 8;  // ？问号，用于随机
    public const int None = 16; // 已经走的格子
    public readonly static List<int> List_RBG = new List<int> { R, B, G };
    public readonly static List<int> List_RBGX = new List<int> { R, B, G, X };
}
