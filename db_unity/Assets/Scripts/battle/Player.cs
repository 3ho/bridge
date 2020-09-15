using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player
{
    public Battle battle;
    public int x;
    public int y;

    public int color;

    public Player(Battle battle, int x, int y)
    {
        this.battle = battle;
        this.x = x;
        this.y = y;
    }

    public Grid getGrid()
    {
        return battle.getGrid(x, y);
    }
}

