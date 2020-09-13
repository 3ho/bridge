
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class MainPanel
{
    private int count = 0;

    private float GridWidthX;
    private float GridHightY;

    protected override void onInit()
    {
        UIEventListener.Get(btn_back).onClick = onBack;
        UIEventListener.Get(btn_start).onClick = onStartBattle;
    }

    protected override void onShow(System.Object param = null, string childView = null)
    {
        showLogin();

        // 
        RectTransform rootRect = m_mapRoot.GetComponent<RectTransform>();
        Vector2 xy = rootRect.sizeDelta;
        GridWidthX = xy.x / Battle.MapWidth_X;
        GridHightY = xy.y / Battle.MapHeight_Y;
        Debug.Log("sizeDelta=" + xy + ",GridWidthX=" + GridWidthX + ",GridHightY=" + GridHightY);
    }

    public void Update()
    {

    }

    private void onBack(GameObject go)
    {
        showLogin();
    }

    private Battle battle;

    private void onStartBattle(GameObject go)
    {
        count++;
        SetLabelText(txt_count, count);
        Debug.Log("MainPanel.OnStartGame" + Time.time);

        battle = new Battle();
        SetLabelText(txt_count, battle.mapToString());

        showBattle();

        genMap();
    }

    private void genMap()
    {
        for (int i = 0; i < Battle.MapWidth_X; i++)
        {
            for (int j = 0; j < Battle.MapHeight_Y; j++)
            {
                addUIGrid(battle.getGrid(i,j));
            }
        }
        //addUIGrid(new Grid(1, 1,ColorUtils.B));
        //addUIGrid(new Grid(2, 3));
        //addUIGrid(new Grid(4, 2, ColorUtils.R));
        //addUIGrid(new Grid(7, 1, ColorUtils.G));
    }

    private void addUIGrid(Grid grid)
    {
        GameObject newGird = Instantiate(m_gird.gameObject) as GameObject;
        grid.uiGrid = newGird;
        UIGrid newUIG = newGird.GetComponent<UIGrid>();
        Image newImage = newUIG.m_color.GetComponent<Image>();
        
        switch (grid.color)
        {
            case ColorUtils.R:
                newImage.color = Color.red;
                newImage.sprite = null;
                break;
            case ColorUtils.B:
                newImage.color = Color.blue;
                newImage.sprite = null;
                break;
            case ColorUtils.G:
                newImage.color = Color.green;
                newImage.sprite = null;
                break;
            default:
                break;
        }
        RectTransform rt = newGird.GetComponent<RectTransform>();
        rt.SetParent(m_gird.transform.parent, false);
        rt.pivot = Vector2.zero;
        rt.anchorMax = rt.anchorMin = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
        rt.localScale = Vector3.one;
        rt.localPosition = getPosByGrid(grid);

        rt.sizeDelta = new Vector2(GridWidthX, GridHightY);

        newGird.SetActive(true);
        Debug.Log("grid=" + grid + ",pos=" + getPosByGrid(grid));
    }

    private Vector2 offsetPos = new Vector2(-0, -0);

    private Vector2 getPosByGrid(Grid grid)
    {
        float x = getXByGrid(grid.x);
        float y = getYByGrid(grid.y);
        return new Vector2(x, y);
    }
    private float getXByGrid(int gridX)
    {
        return offsetPos.x + gridX * GridWidthX;
    }

    private float getYByGrid(int gridY)
    {
        return offsetPos.y + gridY * GridHightY;
    }

    private void showLogin()
    {
        m_login.SetActive(true);
        m_battle.SetActive(false);
    }

    private void showBattle()
    {
        m_login.SetActive(false);
        m_battle.SetActive(true);
    }

    protected override void onDestroy()
    {
        Debug.Log("MainPanel.onDestroy");
    }
}
