﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public sealed class Grid
{
    public static Sprite xImage;

    public readonly int x;
    public readonly int y;

    public int color;

    public UIGrid uiGrid;

    public Grid(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Grid(int x, int y, int color)
    {
        this.x = x;
        this.y = y;
        this.color = color;
    }
}
