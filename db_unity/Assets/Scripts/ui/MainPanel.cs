
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
        gird_x_Cs.OnDrag_X = OnDrag_Prop;
        gird_x_Cs.OnPointerUp_X = OnPointerUp_Prop;
        gird_x_Cs.OnPointerDown_X = OnPointerDown_Prop;

        // player
        PlayerPrefab pp = m_player_prefabs.GetComponent<PlayerPrefab>();
        DragUI playerDrga = pp.grid.GetComponent<DragUI>();
        playerDrga.OnPointerUp_X = OnPointerUp_Player;
        RectTransform playerGridTraf = pp.grid.GetComponent<RectTransform>();
        playerGridTraf.sizeDelta = new Vector2(GridWidthX, GridHightY);
    }

    private IEnumerator playAnimScale()
    {
        Vector2 ddd = new Vector2(0.6f, 0.6f);
        while (true)
        {
            yield return new WaitForSeconds(2);
            yield return AlertPanel.slowScaleTo(m_target_home_image.transform, new Vector3(-1,1,1), 0, 1.5f);
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

        UIGrid m_uiGrid = m_gird.GetComponent<UIGrid>();
        Image girdImage = m_uiGrid.m_color.GetComponent<Image>();
        Grid.xImage = girdImage.sprite;

        // Ŀ���յ㶯��
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

        //��ʾĿ���
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

    private int useGrid_X_Count = 0;

    public void OnPointerUp_Prop(PointerEventData eventData)
    {
        Vector3 worldPosition = eventData.pointerCurrentRaycast.worldPosition;
        Vector3 localPos = m_gird.transform.InverseTransformPoint(worldPosition);
        Vector2Int int2 = getGridByXY(localPos);
        // �ı���ɫ
        Grid grid = battle.getGrid(int2.x, int2.y);
        Debug.Log("OnPointerUp_Prop" + ",worldPosition=" + eventData.pointerCurrentRaycast.worldPosition + ",int2=" + int2 + ",grid=" + grid);
        if (grid != null)
        {
            // �������Լ�����
            if (battle.player.getGrid() == grid)
            {
                AlertPanel.Show("���ܸ��Լ�ʹ��");
                return;
            }
            // �������յ�
            if (battle.getGrid(battle.target) == grid)
            {
                AlertPanel.Show("�޷�ʹ��");
            }
            // �����ǣ�����
            if (grid.color == ColorUtils.X)
            {
                AlertPanel.Show("�Ѿ��ǣ�����");
            }

            grid.color = ColorUtils.X;
            updateGridUi(grid);

            useGrid_X_Count++;
            SetLabelText(m_grid_x_count, useGrid_X_Count);

        }
    }

    public void OnPointerUp_Player(PointerEventData eventData)
    {
        Vector3 worldPosition = eventData.pointerCurrentRaycast.worldPosition;
        Vector3 localPos = m_gird.transform.InverseTransformPoint(worldPosition);
        Vector2Int int2 = getGridByXY(localPos);
        Grid grid = battle.getGrid(int2.x, int2.y);
        if (grid != null)
        {
            //������
            if (!grid.isBorder(battle.player.x, battle.player.y))
            {
                AlertPanel.Show("ֻ�������ڸ���");
                return;
            }
            //�ж��Ƿ�ʤ��
            if (grid.x == battle.target.x && grid.y == battle.target.y)
            {
                AlertPanel.Show("ʤ����");
                return;
            }
            // ֻ������ɫ��ͬ�͡���������
            if (grid.color != ColorUtils.X)
            {
                Grid playerGrid = battle.getGrid(battle.player.x, battle.player.y);
                if (playerGrid != null && playerGrid.color != grid.color)
                {
                    AlertPanel.Show("ֻ������ɫ��ͬ�͡���������");
                    return;
                }
            }
            if (grid.color == ColorUtils.X)
            {
                int index = Utils.Random(0, ColorUtils.List_RBG.Count);
                grid.color = ColorUtils.List_RBG[index];
            }
            battle.player.x = grid.x;
            battle.player.y = grid.y;
            battle.player.color = grid.color;

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
        useGrid_X_Count = 0;
        SetLabelText(m_grid_x_count, useGrid_X_Count);
    }

    protected override void onDestroy()
    {
        Debug.Log("MainPanel.onDestroy");
    }
}
