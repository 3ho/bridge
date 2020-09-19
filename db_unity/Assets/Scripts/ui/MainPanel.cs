
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

        // 
        RectTransform rootRect = m_mapRoot.GetComponent<RectTransform>();
        Vector2 xy = rootRect.sizeDelta;
        GridWidthX = xy.x / Battle.MapWidth_X;
        GridHightY = xy.y / Battle.MapHeight_Y;
        Debug.Log("sizeDelta=" + xy + ",GridWidthX=" + GridWidthX + ",GridHightY=" + GridHightY);

        // m_gird_x
        DragUI gird_x_Cs = m_gird_x.GetComponent<DragUI>();
        //gird_x_Cs.OnPointerUp_X = OnPointerUp_Prop;
        gird_x_Cs.OnPointerDown_X = OnPointerDown_Prop;

        // player
        PlayerPrefab pp = m_player_prefabs.GetComponent<PlayerPrefab>();
        DragUI playerDrga = pp.grid.GetComponent<DragUI>();
        playerDrga.OnPointerUp_X = OnPointerUp_Player;
        RectTransform playerGridTraf = pp.grid.GetComponent<RectTransform>();
        playerGridTraf.sizeDelta = new Vector2(GridWidthX, GridHightY);

        //m_select_change
        DragUI mSelectChange = m_select_change.GetComponent<DragUI>();
        mSelectChange.OnPointerDown_X = OnPointerDown_Select;

        // image
        UIGrid m_uiGrid = m_gird.GetComponent<UIGrid>();
        Image xImage = m_uiGrid.m_color.GetComponent<Image>();
        Grid.xImage = xImage.sprite;

        // move player
        DragUI moveDragUi = m_move_player.GetComponent<DragUI>();
        moveDragUi.OnPointerUp_X = OnPointerUp_Move;
        moveDragUi.OnPointerDown_X = OnPointerDown_Move;
    }

    private IEnumerator playAnimScale()
    {
        Vector2 ddd = new Vector2(0.6f, 0.6f);
        while (true)
        {
            yield return new WaitForSeconds(2);
            yield return AlertPanel.slowScaleTo(m_target_home_image.transform, new Vector3(-1, 1, 1), 0, 1.5f);
            yield return new WaitForSeconds(2);
            yield return AlertPanel.slowScaleTo(m_target_home_image.transform, new Vector3(1, 1, 1), 0, 1.5f);
            //m_target_home_image.transform.localScale = Vector3.one;
            //yield return AlertPanel.slowScaleTo(m_target_home_image.transform, ddd, 0, 1.5f);
            //yield return new WaitForSeconds(2);

            //yield return new WaitForSeconds(2);
            //yield return AlertPanel.slowScaleTo(m_target_home_image.transform, ddd, 0, 0.3f);
            //yield return AlertPanel.slowScaleTo(m_target_home_image.transform, Vector3.one, 0, 0.3f);
        }

    }

    protected override void onShow(System.Object param = null, string childView = null)
    {
        showLogin();

        // 目标终点动画
        StopCoroutine("playAnimScale");
        StartCoroutine(playAnimScale());
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

        updatePlayerUI();

        //显示目标点
        Vector2 homePos = getPosByXY(battle.target.x, battle.target.y);
        m_target_home.transform.localPosition = homePos;
        RectTransform tempTf = m_target_home.GetComponent<RectTransform>();
        tempTf.sizeDelta = new Vector2(GridWidthX, GridHightY);
    }

    private void updatePlayerUI()
    {
        PlayerPrefab pp = m_player_prefabs.GetComponent<PlayerPrefab>();
        pp.updateUI(battle.player.color);
        Vector2 position = getPosByXY(battle.player.x, battle.player.y);
        pp.grid.transform.localPosition = position;
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
            case ColorUtils.X:
                newImage.color = Color.white;
                newImage.sprite = Grid.xImage;
                break;
            case ColorUtils.None:
                newImage.color = Color.white;
                newImage.sprite = null;
                break;
        }
    }

    public void OnPointerDown_Select(PointerEventData eventData)
    {
        m_select_change.SetActive(false);
        OnPointerUp_Prop(eventData);
    }


    public void OnPointerUp_Move(PointerEventData eventData)
    {
        if (lastPlayerMoveStartPos != null)
        {
            Vector2 offset = eventData.position - lastPlayerMoveStartPos;
            if (offset.sqrMagnitude < 500)
                return;
            float angle = Utils.GetOrientation(offset.x, offset.y);
            Debug.Log("OnPointerUp_Move" + ",offset=" + offset + ",sqrMagnitude=" + offset.sqrMagnitude + ",angle=" + angle);
            Vector2Int girdPos = new Vector2Int(battle.player.x, battle.player.y);
            if (angle < 45 || angle > 315)
            {
                //up
                girdPos += new Vector2Int(0, 1);
            }
            else if (angle < 135)
            {
                // right
                girdPos += new Vector2Int(1, 0);
            }
            else if (angle < 225)
            {
                //down
                girdPos += new Vector2Int(0, -1);
            }
            else
            {
                //left
                girdPos += new Vector2Int(-1, 0);
            }
            moveGird(battle.getGrid(girdPos.x, girdPos.y));
        }
    }

    private Vector2 lastPlayerMoveStartPos;
    public void OnPointerDown_Move(PointerEventData eventData)
    {
        lastPlayerMoveStartPos = eventData.position;
    }

    public Grid getGridByEventData(PointerEventData eventData)
    {
        Vector3 worldPosition = eventData.pointerCurrentRaycast.worldPosition;
        Vector3 localPos = m_gird.transform.InverseTransformPoint(worldPosition);
        Vector2Int int2 = getGridByXY(localPos);
        // 改变颜色
        return battle.getGrid(int2.x, int2.y);
    }


    public void OnPointerDown_Prop(PointerEventData eventData)
    {
        m_select_change.SetActive(true);
        Debug.Log("OnPointerDown_Prop" + ",position=" + eventData.position + ",pressPosition=" + eventData.pressPosition + ",worldPosition=" + eventData.pointerCurrentRaycast.worldPosition);
    }

    private int useGrid_X_Count = 0;

    public void OnPointerUp_Prop(PointerEventData eventData)
    {
        // 改变颜色
        Grid grid = getGridByEventData(eventData);
        Debug.Log("OnPointerUp_Prop" + ",worldPosition=" + eventData.pointerCurrentRaycast.worldPosition + ",grid=" + grid);
        if (grid != null)
        {
            // 不能是自己格子
            if (battle.player.getGrid() == grid)
            {
                AlertPanel.Show("不能给自己使用");
                return;
            }
            // 不能是终点
            if (battle.getGrid(battle.target) == grid)
            {
                AlertPanel.Show("无法使用");
            }
            // 不能是？格子
            if (grid.color == ColorUtils.X)
            {
                AlertPanel.Show("已经是？格子");
            }

            grid.color = ColorUtils.X;
            updateGridUi(grid);

            useGrid_X_Count++;
            SetLabelText(m_grid_x_count, useGrid_X_Count);

        }
    }

    public void OnPointerUp_Player(PointerEventData eventData)
    {
        Grid grid = getGridByEventData(eventData);
        moveGird(grid);
    }

    private void moveGird(Grid grid)
    {
        if (grid != null)
        {
            //不相邻
            if (!grid.isBorder(battle.player.x, battle.player.y))
            {
                AlertPanel.Show("只能走相邻格子");
                return;
            }
            //判断是否胜利
            if (grid.x == battle.target.x && grid.y == battle.target.y)
            {
                m_battle_end.SetActive(true);
                return;
            }
            // 只能走颜色相同和【？】格子
            Grid playerGrid = battle.player.getGrid();
            if (grid.color != ColorUtils.X && grid.color != ColorUtils.None)
            {
                if (battle.player.color != grid.color)
                {
                    AlertPanel.Show("只能走颜色相同和【？】格子");
                    return;
                }
            }
            // 标记已走过的格子
            if (playerGrid != null && playerGrid.color != ColorUtils.None)
            {
                //playerGrid.color = ColorUtils.None;
                //updateGridUi(playerGrid);
            }

            if (grid.color == ColorUtils.X)
            {

                // 随机变成周围的颜色，除了玩家和？
                List<Grid> aroudG = battle.getAroundGrid(grid.x, grid.y);
                aroudG.RemoveAll((obj) =>
                {
                    if (obj == playerGrid || !ColorUtils.List_RBG.Contains(obj.color))
                        return true;
                    return false;
                });
                if (aroudG.Count > 0)
                {
                    int index = Utils.Random(0, aroudG.Count);
                    grid.color = aroudG[index].color;
                }
                else
                {
                    int index = Utils.Random(0, ColorUtils.List_RBG.Count);
                    grid.color = ColorUtils.List_RBG[index];
                }
            }
            battle.player.x = grid.x;
            battle.player.y = grid.y;
            if (grid.color != ColorUtils.None)
            {
                battle.player.color = grid.color;
            }

            updateGridUi(grid);
            updatePlayerUI();
        }
    }

    private Vector2 offsetPos = new Vector2(-0, -0);

    private Vector2 getPosByXY(int X, int Y)
    {
        float x = getXByGrid(X);
        float y = getYByGrid(Y);
        return new Vector2(x, y);
    }

    private Vector2 getPosByGrid(Grid grid)
    {
        return getPosByXY(grid.x, grid.y);
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
        resetBattle();
        m_login.SetActive(false);
        m_battle.SetActive(true);
    }

    private void resetBattle()
    {
        m_select_change.SetActive(false);
        m_battle_end.SetActive(false);
        useGrid_X_Count = 0;
        SetLabelText(m_grid_x_count, useGrid_X_Count);
    }

    protected override void onDestroy()
    {
        Debug.Log("MainPanel.onDestroy");
    }
}
