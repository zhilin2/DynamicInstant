using UnityEngine;
using System.Collections.Generic;
public class GridChildGroup : MonoBehaviour
{
    public int spacing;
    public List<RectTransform> Exclusions = new List<RectTransform>();
    public Axis axis = Axis.Y_axle;
    //最小限度
    public float minLimit = -1;
    //最大限度
    public float maxLimit = -1;
    int childCount;
    RectTransform mTran;
    public Vector2 curPos { get; private set; }
    //调整自己
    public void FlashAdjust()
    {
        mTran = transform as RectTransform;
        childCount = transform.childCount;
        RectTransform tran;
        if (axis == Axis.X_axle)
        {
            curPos = new Vector2(spacing, 0);
            mTran.sizeDelta = new Vector2(0, mTran.sizeDelta.y);
            for (int i = 0; i < childCount; i++)
            {
                tran = mTran.GetChild(i) as RectTransform;
                if (tran != null && tran.gameObject.activeSelf == true)
                {
                    GridChildGroup GCG = tran.GetComponent<GridChildGroup>();
                    if (GCG != null)
                    {
                        GCG.FlashAdjust();
                    }
                    if (!Exclusions.Contains(tran))
                    {
                        tran.anchoredPosition = new Vector2(curPos.x, tran.anchoredPosition.y);
                        curPos += new Vector2(spacing + tran.sizeDelta.x, 0);
                    }
                }
            }
            if (maxLimit > 0 && maxLimit < curPos.x)
            {
                mTran.sizeDelta = new Vector2(maxLimit, mTran.sizeDelta.y);
            }else if(minLimit> 0 && minLimit > curPos.x)
            {
                mTran.sizeDelta = new Vector2(minLimit, mTran.sizeDelta.y);
            }
            else
            {
                mTran.sizeDelta = new Vector2(curPos.x, mTran.sizeDelta.y);
            }
        }
        else
        {
            curPos = new Vector2(0, spacing);
            mTran.sizeDelta = new Vector2(mTran.sizeDelta.x, 0);
            for (int i = 0; i < childCount; i++)
            {
                tran = mTran.GetChild(i) as RectTransform;
                if (tran!=null&& tran.gameObject.activeSelf == true)
                {
                    GridChildGroup GCG = tran.GetComponent<GridChildGroup>();
                    if (GCG != null)
                    {
                        GCG.FlashAdjust();
                    }
                    if (!Exclusions.Contains(tran))
                    {
                        tran.anchoredPosition = new Vector2(tran.anchoredPosition.x, -curPos.y);
                        curPos += new Vector2(spacing, spacing + tran.sizeDelta.y);
                    }
                }
            }
            if (maxLimit > 0 && maxLimit < curPos.y)
            {
                mTran.sizeDelta = new Vector2(mTran.sizeDelta.x, maxLimit);
            }
            else if (minLimit > 0 && minLimit > curPos.y)
            {
                mTran.sizeDelta = new Vector2(mTran.sizeDelta.x, minLimit);
            }
            else
            {
                mTran.sizeDelta = new Vector2(mTran.sizeDelta.x, curPos.y);
            }
        }
    }

    [ContextMenu("ReFlash")]
    public void ReFlash()
    {
        FlashAdjust();
    }

    public enum Axis
    {
        X_axle,
        Y_axle,
    }
}
