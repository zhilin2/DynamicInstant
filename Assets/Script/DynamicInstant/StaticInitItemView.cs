using UnityEngine;
using System.Collections.Generic;
using System;
public class StaticInitItemView : MonoBehaviour
{
    public Transform mView;
    public Dictionary<Transform, List<Transform>> tranPools = new Dictionary<Transform, List<Transform>>();
    public List<SIIV> funPools = new List<SIIV>();
    private Bounds rang;

    public delegate void InitFunc(Transform tran, SIIV siiv);

    public bool swith = true;
    public bool isFast = true;
    public bool doOnce = false;
    private SIIVRank checkItems = new SIIVRank();
    public delegate bool Sort(SIIV a, SIIV b);
    bool allHide = false;
    //public RectTransform tran;
    //public Transform parent;
    //[ContextMenu("create")]
    //public void Test()
    //{
    //    for (int i = 0; i < 10; i++)
    //    {
    //        GetItem(tran, parent, (s) => { GameDebug.Log("Init:  " + i.ToString()); });
    //    }
    //}
    [ContextMenu("ReSet")]
    public void ReSet()
    {
        for (int j = 0; j < funPools.Count; j++)
        {
            ClearItem(funPools[j]);
        }
        checkItems.start = 0;
        checkItems.end = 0;
    }
    public void Clear()
    {
        for (int j = funPools.Count - 1; j >= 0; j--)
        {
            funPools[j].node.gameObject.SetActive(false);
            Destroy(funPools[j].node.gameObject);
        }
        funPools.Clear();
        foreach(var one in tranPools)
        {
            for(int i= one.Value.Count-1; i>=0;i--)
            {
                Destroy(one.Value[i].gameObject);
            }
        }
        tranPools.Clear();
        checkItems.start = 0;
        checkItems.end = 0;
    }

    public SIIV AddItem(Transform tran, Transform parent, InitFunc initFun)
    {
        SIIV result = new SIIV(CreateDefultNode(tran as RectTransform, parent), tran, initFun);
        funPools.Add(result);
        return result;
    }

    public SIIV InsetItem(int i, Transform tran, Transform parent, InitFunc initFun)
    {
        SIIV result = new SIIV(CreateDefultNode(tran as RectTransform, parent), tran, initFun);
        funPools.Insert(i, new SIIV(CreateDefultNode(tran as RectTransform, parent), tran, initFun));
        if (i <= checkItems.start)
        {
            for (int j = checkItems.start; j <= checkItems.end; j++)
            {
                ClearItem(funPools[j]);
            }
            checkItems.start = i;
            checkItems.end = i;
        }
        else if (i > checkItems.start && i <= checkItems.end)
        {
            for (int j = i; j <= checkItems.end; j++)
            {
                ClearItem(funPools[j]);
            }
            checkItems.end = i;
        }
        return result;
    }

    public SIIV AddItem(Transform tran, Transform parent, InitFunc initFun, object data)
    {
        SIIV result = new SIIV(CreateDefultNode(tran as RectTransform, parent), tran, initFun, data);
        funPools.Add(result);
        return result;
    }

    public SIIV InsetItem(int i, Transform tran, Transform parent, InitFunc initFun, object data)
    {
        SIIV result = new SIIV(CreateDefultNode(tran as RectTransform, parent), tran, initFun, data);
        funPools.Insert(i, result);
        if (i <= checkItems.start)
        {
            for (int j = checkItems.start; j <= checkItems.end; j++)
            {
                ClearItem(funPools[j]);
            }
            checkItems.start = i;
            checkItems.end = i;
        }
        else if (i > checkItems.start && i <= checkItems.end)
        {
            for (int j = i; j <= checkItems.end; j++)
            {
                ClearItem(funPools[j]);
            }
            checkItems.end = i;
        }
        return result;
    }

    public void RemoveItem(int i)
    {
        if (i <= checkItems.start)
        {
            for (int j = checkItems.start; j <= checkItems.end; j++)
            {

                ClearItem(funPools[j]);
            }
            checkItems.start = i;
            checkItems.end = i;
        }
        else if (i > checkItems.start && i <= checkItems.end)
        {
            for (int j = i; j <= checkItems.end; j++)
            {
                ClearItem(funPools[j]);
            }
            checkItems.end = i - 1;
        }
        Destroy(funPools[i].node.gameObject);
        funPools.RemoveAt(i);
    }
    public void RemoveItemBySIIV(SIIV siiv)
    {
        if (funPools.Contains(siiv))
        {
            RemoveItem(funPools.IndexOf(siiv));
        }
    }

    public void SortItem(Sort comparison)
    {
        for (int i = 0; i < funPools.Count - 1; i++)
            for (int j = i + 1; j < funPools.Count; j++)
            {
                if (funPools[i].node.parent == funPools[i].node.parent && !comparison(funPools[i], funPools[j]))
                {
                    Transform node = funPools[i].node;
                    funPools[i].node = funPools[j].node;
                    funPools[j].node = node;
                    SIIV siiv = funPools[i];
                    funPools[i] = funPools[j];
                    funPools[j] = siiv;
                    ClearItem(funPools[i]);
                    ClearItem(funPools[j]);
                }
            }
        checkItems.start = 0;
        checkItems.end = 0;
    }
    public void SortSIIV(Sort comparison)
    {
        for (int i = 0; i < funPools.Count - 1; i++)
        {
            for (int j = i + 1; j < funPools.Count; j++)
            {
                if (!comparison(funPools[i], funPools[j]))
                {
                    SIIV siiv = funPools[i];
                    funPools[i] = funPools[j];
                    funPools[j] = siiv;
                    ClearItem(funPools[i]);
                    ClearItem(funPools[j]);
                }
            }
        }
        checkItems.start = 0;
        checkItems.end = 0;
    }

    public Transform GetItem(int i)
    {
        return funPools[i].tran;
    }

    public void Start()
    {
        rang = RectTransformUtility.CalculateRelativeRectTransformBounds(mView);
    }
    Bounds bounds;
    public void LateUpdate()
    {
        if (doOnce)
        {
            UpdateOnece();
            swith = false;
            doOnce = false;
        }
        if (swith && funPools.Count > 0)
        {
            if (isFast)
            {
                if (allHide)
                {
                    for (int i = 0; i < funPools.Count; i++)
                    {
                        if (funPools[i].node.gameObject.activeSelf)
                        {
                            allHide = false;
                            checkItems.start = i;
                            return;
                        }
                    }
                    return;
                }
                if (checkItems.start > checkItems.end)
                {
                    checkItems.start = checkItems.end;
                }
                checkItems.start = CalculateItemAtStartView(checkItems.start);
                if (checkItems.start > checkItems.end)
                {
                    checkItems.end = checkItems.start;
                }
                checkItems.end = CalculateItemAtEndView(checkItems.end);
            }
            else
            {
                UpdateOnece();
            }
        }
    }

    void UpdateOnece()
    {
        for (int i = 0; i < funPools.Count; i++)
        {
            FormatState(CalculateAtView(funPools[i]), i);
        }
    }

    void FormatState(bool inView, int i)
    {
        if (inView)
        {
            ReFlash(funPools[i]);
        }
        else
        {
            ClearItem(funPools[i]);
        }
    }

    int CalculateItemAtStartView(int i)
    {
        int result = i;
        if (funPools.Count <= i)
        {
            return i;
        }
        if (!funPools[i].node.gameObject.activeSelf)
        {
            if (i > 0)
            {
                i = i - 1;
                return CalculateItemAtStartView(i);
            }
            else
            {
                allHide = true;
                for (int k = 0; k < funPools.Count; k++)
                {
                    if (funPools[k].node.gameObject.activeSelf)
                    {
                        allHide = false;
                        checkItems.start = k;
                        return k;
                    }
                }
                return 0;
            }
        }
        bool temp = CalculateAtView(funPools[i]);
        FormatState(temp, i);
        if (temp)
        {

            if (i > 0)
            {
                i = i - 1;
                UpCalculate(temp, FormatState, ref i);
                result = i;
            }
        }
        else
        {
            if (i < funPools.Count - 1)
            {
                i = i + 1;
                DownCalculate(temp, FormatState, ref i);
                result = i;
            }
        }
        return result;
    }
    int CalculateItemAtEndView(int i)
    {
        int result = i;
        if (!funPools[i].node.gameObject.activeSelf)
        {
            if (i < funPools.Count - 1)
            {
                i = i + 1;
                return CalculateItemAtEndView(i);
            }
            else
            {
                return 0;
            }
        }
        bool temp = CalculateAtView(funPools[i]);
        FormatState(temp, i);
        if (temp)
        {
            if (i < funPools.Count - 1)
            {
                i = i + 1;
                DownCalculate(temp, FormatState, ref i);
                result = i - 1;
            }
        }
        else
        {
            if (i > 0)
            {
                i = i - 1;
                UpCalculate(temp, FormatState, ref i);
                result = i;
            }
        }
        return result;
    }
    void UpCalculate(bool inView, Action<bool, int> action, ref int i)
    {
        if (!funPools[i].node.gameObject.activeSelf)
        {
            if (i > 0)
            {
                i = i - 1;
                UpCalculate(inView, action, ref i);
            }
            else
            {
                return;
            }
        }
        bool temp = CalculateAtView(funPools[i]);
        action(temp, i);
        if (temp == inView)
        {
            if (i > 0)
            {
                i = i - 1;
                UpCalculate(inView, action, ref i);
            }
        }
    }
    void DownCalculate(bool inView, Action<bool, int> action, ref int i)
    {
        if (!funPools[i].node.gameObject.activeSelf)
        {
            if (i < funPools.Count - 1)
            {
                i = i + 1;
                DownCalculate(inView, action, ref i);
            }
            else
            {
                return;
            }
        }
        bool temp = CalculateAtView(funPools[i]);
        action(temp, i);
        if (temp == inView)
        {
            if (i < funPools.Count - 1)
            {
                i = i + 1;
                DownCalculate(inView, action, ref i);
            }
        }
    }
    bool CalculateAtView(SIIV siiv)
    {
        if (mView && siiv.node)
        {
            bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(mView, siiv.node);
            return (rang.min.x <= bounds.max.x && rang.max.x >= bounds.min.x) && (rang.min.y <= bounds.max.y && rang.max.y >= bounds.min.y);
        }
        return false;
    }

    public Transform ReFlash(SIIV siiv)
    {
        if (!siiv.needFrash)
        {
            return siiv.tran;
        }
        RectTransform tran;
        Transform model = siiv.model;
        if (!tranPools.ContainsKey(model))
        {
            tranPools.Add(model, new List<Transform>());
        }
        if (tranPools[model].Count == 0)
        {
            tran = (Instantiate(model.gameObject) as GameObject).transform as RectTransform;
        }
        else
        {
            tran = tranPools[model][tranPools[model].Count - 1] as RectTransform;
            tranPools[model].RemoveAt(tranPools[model].Count - 1);
        }
        siiv.tran = tran;
        tran.gameObject.SetActive(true);
        tran.SetParent(siiv.node);
        tran.anchorMin = Vector2.zero;
        tran.anchorMax = Vector2.one;
        tran.sizeDelta = Vector2.zero;
        tran.localScale = Vector3.one;
        tran.anchoredPosition3D = Vector3.zero;
        siiv.initFun(tran, siiv);
        siiv.needFrash = false;
        return tran;
    }

    public void ClearItem(SIIV siiv)
    {
        Transform tran = siiv.tran;
        siiv.needFrash = true;
        if (tran)
        {
            if (!tranPools.ContainsKey(siiv.model))
            {
                tranPools.Add(siiv.model, new List<Transform>());
            }
            tran.gameObject.SetActive(false);
            tran.SetParent(transform);
            tranPools[siiv.model].Add(tran);
            siiv.tran = null;
        }
    }

    public Transform CreateDefultNode(RectTransform tran, Transform parent)
    {
        RectTransform node = new GameObject("node", typeof(RectTransform)).transform as RectTransform;
        node.SetParent(parent);
        node.sizeDelta = tran.sizeDelta;
        node.anchoredPosition3D = tran.anchoredPosition3D;
        node.anchorMin = tran.anchorMin;
        node.anchorMax = tran.anchorMax;
        node.pivot = tran.pivot;
        node.localScale = tran.localScale;
        return node;
    }

    public class SIIVRank
    {
        public int start;
        public int end;
    }

    public class SIIV
    {
        public Transform node;
        public Transform model;
        public InitFunc initFun;
        public Transform tran;
        public bool needFrash;
        public object data;
        public SIIV(Transform _node, Transform _model, InitFunc _initFun)
        {
            node = _node;
            model = _model;
            initFun = _initFun;
            needFrash = true;
        }
        public SIIV(Transform _node, Transform _model, InitFunc _initFun, object _data)
        {
            node = _node;
            model = _model;
            initFun = _initFun;
            needFrash = true;
            data = _data;
        }
    }
}
