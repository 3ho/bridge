
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class MainPanel
{
    private int count = 0;

    protected override void onInit()
    {
        UIEventListener.Get(btn_back).onClick = onBack;
        UIEventListener.Get(btn_start).onClick = onStartBattle;
    }

    protected override void onShow(System.Object param = null, string childView = null)
    {
        showLogin();
    }

    public void Update()
    {

    }

    private void onBack(GameObject go)
    {
        showLogin();
    }

    private void onStartBattle(GameObject go)
    {
        count++;
        SetLabelText(txt_count, count);
        Debug.Log("MainPanel.OnStartGame" + Time.time);

        Battle battle = new Battle();
        SetLabelText(txt_count, battle.mapToString());

        showBattle();

        genMap();
    }

    private void genMap()
    {
        addUIGrid(new Grid(1, 1));
    }

    private void addUIGrid(Grid grid)
    {
        GameObject newGird = Instantiate(m_gird) as GameObject;

        Image newImage = newGird.GetComponent<Image>();
        switch (grid.color)
        {
            case ColorUtils.R:
                newImage.color = Color.red;
                break;
            case ColorUtils.B:
                newImage.color = Color.blue;
                break;
            case ColorUtils.G:
                newImage.color = Color.green;
                break;
            default:
                newImage.color = Color.white;
                break;
        }

        newGird.transform.parent = m_gird.transform.parent;
        newGird.transform.localScale = new Vector3(1, 1, 1);
        grid.uiGrid = newGird;

        newGird.transform.localPosition = getPosByGrid(grid);
        newGird.SetActive(true);
        Debug.Log("grid=" + grid + ",pos=" + getPosByGrid(grid));
    }

    private Vector2 offsetPos = new Vector2(-530, -600);

    private Vector2 getPosByGrid(Grid grid)
    {
        float x = getXByGrid(grid.x);
        float y = getYByGrid(grid.y);
        return new Vector2(x, y);
    }
    private float getXByGrid(int gridX)
    {
        return offsetPos.x + gridX * 100;
    }

    private float getYByGrid(int gridY)
    {
        return offsetPos.y + gridY * 100;
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
