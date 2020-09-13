
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

        // m_gird_x
        DragUI gird_x_Cs = m_gird_x.GetComponent<DragUI>();
        gird_x_Cs.OnDrag_X = OnDrag_Prop;
        gird_x_Cs.OnPointerUp_X = OnPointerUp_Prop;
        gird_x_Cs.OnPointerDown_X = OnPointerDown_Prop;
    }

    protected override void onShow(System.Object param = null, string childView = null)
    {
        showLogin();

        UIGrid m_uiGrid = m_gird.GetComponent<UIGrid>();
        Image girdImage = m_uiGrid.m_color.GetComponent<Image>();
        Grid.xImage = girdImage.sprite;
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
                addUIGrid(battle.getGrid(i, j));
            }
        }
    }

    private void addUIGrid(Grid grid)
    {
        GameObject newGird = Instantiate(m_gird.gameObject) as GameObject;
        UIGrid newUIG = newGird.GetComponent<UIGrid>();
        grid.uiGrid = newUIG;

        updateGridUi(grid);

        RectTransform rt = newGird.GetComponent<RectTransform>();
        rt.SetParent(m_gird.transform.parent, false);
        rt.pivot = Vector2.zero;
        rt.anchorMax = rt.anchorMin = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
        rt.localScale = Vector3.one;
        rt.localPosition = getPosByGrid(grid);
        rt.sizeDelta = new Vector2(GridWidthX, GridHightY);

        newGird.SetActive(true);
    }

    private void updateGridUi(Grid grid)
    {
        Image newImage = grid.uiGrid.m_color.GetComponent<Image>();
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
                newImage.color = Color.white;
                newImage.sprite = Grid.xImage;
                break;
        }
    }

    public void OnDrag_Prop(PointerEventData eventData)
    {

    }

    public void OnPointerDown_Prop(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown_Prop" + ",position=" + eventData.position + ",pressPosition=" + eventData.pressPosition + ",worldPosition=" + eventData.pointerCurrentRaycast.worldPosition);
    }

    public void OnPointerUp_Prop(PointerEventData eventData)
    {
        Vector3 worldPosition = eventData.pointerCurrentRaycast.worldPosition;
        Vector3 localPos = m_gird.transform.InverseTransformPoint(worldPosition);
        Vector2Int int2 = getGridByXY(localPos);
        // ¸Ä±äÑÕÉ«
        Grid grid = battle.getGrid(int2.x, int2.y);
        Debug.Log("OnPointerUp_Prop" + ",worldPosition=" + eventData.pointerCurrentRaycast.worldPosition + ",int2=" + int2 + ",grid=" + grid);
        if (grid != null)
        {
            grid.color = ColorUtils.X;
            updateGridUi(grid);
        }
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

    private Vector2Int getGridByXY(Vector2 pos)
    {
        float x = (pos.x - offsetPos.x) / GridWidthX;
        float y = (pos.y - offsetPos.y) / GridHightY;
        if (x < 0)
            x--;
        if (y < 0)
            y--;
        return new Vector2Int((int)x, (int)y);
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
