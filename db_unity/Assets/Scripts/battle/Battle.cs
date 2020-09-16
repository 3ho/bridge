using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public sealed class Battle
{
    public const int MapWidth_X = 7;
    public const int MapHeight_Y = 9;

    private readonly Grid[,] mCells = null; //地图数据

    public Player player; // 玩家数据

    public Vector2Int target; // 终点

    public Battle()
    {
        mCells = genMapData();

        int startX = MapWidth_X / 2;
        int startY = 0;
        Grid startGrid = getGrid(startX, startY);
        if (!ColorUtils.List_RBG.Contains(startGrid.color))
        {
            int index = Utils.Random(0, ColorUtils.List_RBG.Count);
            startGrid.color = ColorUtils.List_RBG[index];
        }
        player = new Player(this, startX, startY);
        player.color = startGrid.color;

        target = new Vector2Int(MapWidth_X / 2, MapHeight_Y - 1);
    }

    public Grid[,] genMapData()
    {
        Grid[,] mCells = new Grid[MapWidth_X, MapHeight_Y];
        for (int i = 0; i < MapWidth_X; i++)
        {
            for (int j = 0; j < MapHeight_Y; j++)
            {
                mCells[i, j] = new Grid(i, j);
            }
        }
        // 随机数据
        for (int i = 0; i < MapWidth_X; i++)
        {
            for (int j = 0; j < MapHeight_Y; j++)
            {
                int index = Utils.Random(0, ColorUtils.List_RBGX.Count);
                mCells[i, j].color = ColorUtils.List_RBGX[index];
            }
        }
        return mCells;
    }

    public String mapToString()
    {
        StringBuilder sb = new StringBuilder();
        for (int y = MapHeight_Y - 1; y >= 0; y--)
        {
            for (int x = 0; x < MapWidth_X; x++)
            {
                sb.Append(mCells[x, y].color + " ");
            }
            sb.Append("\n");
        }
        return sb.ToString();
    }

    public bool isOutOfMap(int x, int y)
    {
        return x < 0 || x >= MapWidth_X || y < 0 || y >= MapHeight_Y;
    }

    public Grid getGrid(int x, int y)
    {
        if (isOutOfMap(x, y))
            return null;
        return mCells[x, y];
    }

    public Grid getGrid(Vector2Int int2)
    {
        return getGrid(int2.x, int2.y);
    }
}
