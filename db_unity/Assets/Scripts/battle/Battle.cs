using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed class Battle
{
    public const int MapWidth_X = 10;
    public const int MapHeight_Y = 15;

    private readonly Grid[,] mCells = null;

    public Battle()
    {
        mCells = genMapData();
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

    public Grid getGrid(int x,int y)
    {
        return mCells[x, y];
    }
}
