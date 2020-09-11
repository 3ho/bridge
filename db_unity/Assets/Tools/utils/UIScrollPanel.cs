using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScrollPanel : MonoBehaviour
{
    public delegate void FillCell(GameObject cell, int index);

    public delegate void CleanCell(GameObject cell);

    private bool mHorizontal = false;
    private Rect mRect;
    private RectTransform mContentTransform;
    private GameObject mCell;
    private Vector2 mCellInitPos;
    private List<string> m_cellsName = new List<string> { "m_cell", "m_cell_0", "m_cell_1", "m_cell_2", "m_cell_3", "m_cell_4", "m_cell_5" };

    private FillCell mFillFunc = null;
    private CleanCell mCleanFunc = null;
    private readonly SortedDictionary<int, GameObject> mIndex2Cell = new SortedDictionary<int, GameObject>();
    private readonly LinkedList<GameObject> mCellObjCache = new LinkedList<GameObject>();

    public Vector2 CellSize = new Vector2(100f, 100f);
    private int mCellMax = 0;
    public int rows = 1;
    public int cols = 1;
    private int mLineNum = 0;
    private int mLastLineBegin = -1, mLastLineEnd = -1;

    public bool addTestCell = false;
    public int addTestCellNum = 5;
    public float AddContentHeight = 0f;

    private void Awake()
    {
        AdjustResolution();
        mHorizontal = GetComponent<ScrollRect>().horizontal;
        mRect = (transform as RectTransform).rect;
        mContentTransform = transform.Find("content") as RectTransform;
        if (mContentTransform == null)
        {
            Debug.LogError("[UIScrollPanel]must have a content gameobject");
            return;
        }
        Transform trans = null;
        for (int i = 0, max = m_cellsName.Count; i < max; i++)
        {
            string cellName = m_cellsName[i];
            //trans = mContentTransform.Find("m_cell");
            trans = mContentTransform.Find(cellName);
            if (trans != null)
            {
                break;
            }
        }

        if (trans == null)
        {
            Debug.LogError("[UIScrollPanel]must have a m_cell gameobject");
            return;
        }

        mCell = trans.gameObject;
        mCellInitPos = trans.localPosition;
        mCellObjCache.AddLast(mCell);

#if UNITY_EDITOR

        if (addTestCell)
        {
            SetCellNum(addTestCellNum);
        }

#endif
       
    }

    private float _offsetY = 0;
    private void AdjustResolution()
    {
       
        float designWidth = 1920;//开发时分辨率宽
        float designHeight = 1080;//开发时分辨率高
        if (Screen.width>1920)
        {
            return;
        }
        float scaleRate = designWidth / (float)Utils.ScreenWidth;
       // Debug.Log("------------>"+ scaleRate);
        _offsetY = (float)Utils.ScreenHeight * scaleRate- designHeight;
        //Debug.Log("------------>" + _offsetY);
        RectTransform rectTransform = this.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y + _offsetY);
        //Debug.Log("------------>" + rectTransform.sizeDelta.x+" "+ rectTransform.sizeDelta.y);
        RectTransform contentRectTransform = this.GetComponent<ScrollRect>().content.GetComponent<RectTransform>();
        contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, contentRectTransform.sizeDelta.y + _offsetY);
        //Debug.Log("------------>" + contentRectTransform.sizeDelta.x + " " + contentRectTransform.sizeDelta.y);
    } 

    public void Reset(int cellNum, FillCell fillFunc, CleanCell cleanFunc = null)
    {
        Clear();
        SetCellNum(cellNum);
        mFillFunc = fillFunc;
        mCleanFunc = cleanFunc;
        mLastLineBegin = mLastLineEnd = -1;
        ResetDragPostion();
        Update();
    }

    public void ResetNoPos(int cellNum, FillCell fillFunc, CleanCell cleanFunc = null)
    {
        SetCellNum(cellNum);
        mFillFunc = fillFunc;
        mCleanFunc = cleanFunc;
        mLastLineBegin = mLastLineEnd = -1;
        Update();
    }

    public void ResetNoPosClear(int cellNum, FillCell fillFunc, CleanCell cleanFunc = null)
    {
        Clear();
        ResetNoPos(cellNum,fillFunc,cleanFunc);
        int start, end;
        CalVisibleRange(out start, out end);
        if (getCountPerRowOrCol() * end >= cellNum)
        {
            ResetEnd();
        }
    }

    public void ResetEnd()
    {
        mContentTransform.anchoredPosition = new Vector2(mContentTransform.anchoredPosition.x, mContentTransform.sizeDelta.y - mRect.height);
    }

    //移动到最后
    //public void ResetEnd_OutExpo()
    //{
    //    //mContentTransform.anchoredPosition
    //    Vector2 pos = new Vector2(mContentTransform.anchoredPosition.x, mContentTransform.sizeDelta.y - mRect.height);
    //    Tween tweener = mContentTransform.DoAnchoredPosition(pos, 0.5f);
    //    tweener.SetEase(Ease.OutExpo);
    //}

    public void ResetDragPostion()
    {
        Vector2 pos = mContentTransform.anchoredPosition;
        if (mHorizontal)
        {
            pos.x = 0;
        }
        else
        {
            pos.y = 0;
        }

        mContentTransform.anchoredPosition = pos;
    }

    public void UpdateAllCell(FillCell func)
    {
        foreach (KeyValuePair<int, GameObject> pair in mIndex2Cell)
        {
            try
            {
                func(pair.Value, pair.Key);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    public void UpdateCell(int index, FillCell func)
    {
        GameObject cell = null;
        if (mIndex2Cell.TryGetValue(index, out cell))
        {
            try
            {
                func(cell, index);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    public void Clear()
    {
        mCellMax = 0;
        mLineNum = 0;

        foreach (KeyValuePair<int, GameObject> pair in mIndex2Cell)
        {
            GameObject cell = pair.Value;
            mCellObjCache.AddLast(cell);
            if (mCleanFunc != null)
            {
                try
                {
                    mCleanFunc(cell);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            SetActiveBetter(cell, false);
        }

        mIndex2Cell.Clear();
    }

    private void Rerange()
    {
        if (mCellMax <= 0)
            return;

        int lineBegin = 0, lineEnd = 0;
        CalVisibleRange(out lineBegin, out lineEnd);

        if ((mLastLineBegin == lineBegin) && (mLastLineEnd == lineEnd))
            return;
        int count = getCountPerRowOrCol();

        {
            for (int line = mLastLineBegin; line <= mLastLineEnd; ++line)
            {
                if (line >= lineBegin && line <= lineEnd)
                    continue;

                for (int i = 0; i < count; ++i)
                {
                    int index = line * count + i;
                    GameObject cell;
                    if (!mIndex2Cell.TryGetValue(index, out cell))
                        continue;

                    mCellObjCache.AddLast(cell);
                    mIndex2Cell.Remove(index);

                    if (mCleanFunc != null)
                    {
                        try
                        {
                            mCleanFunc(cell);
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                    SetActiveBetter(cell, false);
                }
            }
        }

        {
            for (int line = lineBegin; line <= lineEnd; ++line)
            {
                if (line >= mLastLineBegin && line <= mLastLineEnd)
                    continue;

                for (int i = 0; i < count; ++i)
                {
                    int index = line * count + i;

                    GameObject cell;
                    mIndex2Cell.TryGetValue(index, out cell);
                    if (index >= mCellMax)
                    {
                        if (cell != null)
                        {
                            mCellObjCache.AddLast(cell);
                            mIndex2Cell.Remove(index);

                            if (mCleanFunc != null)
                            {
                                try
                                {
                                    mCleanFunc(cell);
                                }
                                catch (Exception e)
                                {
                                    Debug.LogException(e);
                                }
                            }
                            SetActiveBetter(cell, false);
                        }
                    }
                    else
                    {
                        if (cell == null)
                        {
                            cell = MakeCell();
                            mIndex2Cell.Add(index, cell);
                        }

                        cell.transform.SetParent(mContentTransform, false);
                        SetActiveBetter(cell, true);

                        if (mFillFunc != null)
                        {
                            try
                            {
                                mFillFunc(cell, index);
                            }
                            catch (Exception e)
                            {
                                Debug.LogException(e);
                            }
                        }
                    }
                }
            }
        }
        mLastLineBegin = lineBegin;
        mLastLineEnd = lineEnd;
        RangeCells();
    }

    private void SetActiveBetter(GameObject go, bool trueFalse)
    {
        if (go && trueFalse != go.activeSelf)
            go.SetActive(trueFalse);
    }

    public void MoveTo(int index)
    {
        Vector2 pos = mContentTransform.anchoredPosition;
        if (mHorizontal)
        {
            pos.x = CellSize.x * index;
        }
        else
        {
            pos.y = CellSize.y * index;
        }
        mContentTransform.anchoredPosition = pos;
    }

    public void MoveTo(int index, float offset)
    {
        Vector2 pos = mContentTransform.anchoredPosition;
        if (mHorizontal)
        {
            pos.x = CellSize.x * index;
        }
        else
        {
            int temp = (index / cols) - 1;
            if (temp >= 0)
            {
                pos.y = CellSize.y * temp + offset;
            }
        }
    }

    private void SetCellNum(int num)
    {
        //Debug.Log("----------->" + num + "  " + mCellMax);
        if (mCellMax != num)
        {
            mCellMax = num;

            if (mCellMax < 0)
            {
                mCellMax = 0;
            }

            if (mCellMax == 0)
            {
                Clear();
            }

            int count = getCountPerRowOrCol();
            mLineNum = (mCellMax - 1) / count + 1;
            if (mHorizontal)
            {
                mContentTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CellSize.x * mLineNum + mCellInitPos.x);
            }
            else
            {
                mContentTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, CellSize.y * mLineNum - mCellInitPos.y + AddContentHeight);
            }
        }
    }

    private void RangeCells()
    {
        if (mHorizontal)
        {
            foreach (KeyValuePair<int, GameObject> pair in mIndex2Cell)
            {
                GameObject cell = pair.Value;
                RectTransform t = cell.transform as RectTransform;
                Vector2 pos = t.anchoredPosition;
                pos.x = mCellInitPos.x + pair.Key * CellSize.x;
                t.anchoredPosition = pos;
            }
        }
        else
        {
            foreach (KeyValuePair<int, GameObject> pair in mIndex2Cell)
            {
                int row = pair.Key / cols;
                int col = pair.Key % cols;
                GameObject cell = pair.Value;
                RectTransform t = cell.transform as RectTransform;
                Vector2 pos = t.anchoredPosition;
                pos.x = mCellInitPos.x + col * CellSize.x;
                pos.y = mCellInitPos.y - row * CellSize.y;
                t.anchoredPosition = pos;
            }
        }
    }

    private GameObject MakeCell()
    {
        GameObject go;
        if (mCellObjCache.Count > 0)
        {
            go = mCellObjCache.First.Value;
            mCellObjCache.RemoveFirst();
            return go;
        }

        go = Instantiate(mCell) as GameObject;
        return go;
    }

    private void Update()
    {
        Rerange();
    }

    private void CalVisibleRange(out int lineBegin, out int lineEnd)
    {
        Vector3 contentPos = mContentTransform.anchoredPosition;
        if (mHorizontal)
        {
            lineBegin = ((int)(-mCellInitPos.x - contentPos.x - 1)) / ((int)(CellSize.x));
            lineEnd = ((int)(-mCellInitPos.x - contentPos.x + mRect.width - 1)) / ((int)(CellSize.x));
        }
        else
        {
            lineBegin = ((int)(mCellInitPos.y + contentPos.y - 1)) / ((int)(CellSize.y));
            lineEnd = ((int)(mCellInitPos.y + contentPos.y + mRect.height - 1)) / ((int)(CellSize.y));
            //Debug.Log("---------------->"+ mCellInitPos.x+" "+ mCellInitPos.y+"   " + contentPos.x + " " + contentPos.y+"   "+ mRect.width+" "+ mRect.height+"    "+ lineBegin+" "+ lineEnd);
        }

        if (lineBegin < 0)
            lineBegin = 0;

        if (lineBegin >= mLineNum)
            lineBegin = mLineNum - 1;

        if (lineEnd >= mLineNum)
            lineEnd = mLineNum - 1;

        if (lineEnd < 0)
            lineEnd = 0;
    }

    private int getCountPerRowOrCol()
    {
        return mHorizontal ? rows : cols;
    }
}