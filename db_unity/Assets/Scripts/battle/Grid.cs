﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed class Grid
{
    public readonly int x;
    public readonly int y;

    public int color;

    public Grid(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}