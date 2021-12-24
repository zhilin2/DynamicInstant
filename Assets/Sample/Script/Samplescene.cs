using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Samplescene : MonoBehaviour
{
    public Transform content;
    public StaticInitItemView siiv;

    private Transform top;
    private Transform centre;
    private Transform bottom;



    public void Start()
    {
        top = content.Find("Top");
        centre = content.Find("Centre");
        bottom = content.Find("Bottom");

        Transform text = top.Find("Text");

        for (int i = 1; i < 4; i++)
        {
            int index = i;
            siiv.AddItem(text, top, (Transform tran, StaticInitItemView.SIIV s) =>
            {
                tran.GetComponent<Text>().text = "这是顶部固定区域"+ index;
            });
        }
        Transform group1 = (Resources.Load("Group1") as GameObject).transform;

        for (int i = 1; i < 20; i++)
        {
            siiv.AddItem(group1, centre, (Transform tran, StaticInitItemView.SIIV s) =>
            {
                tran.Find("1/text").GetComponent<Text>().text = "" + (((int)(s.data) - 1) * 4 + 1);
                tran.Find("2/text").GetComponent<Text>().text = "" + (((int)(s.data) - 1) * 4 + 2);
                tran.Find("3/text").GetComponent<Text>().text = "" + (((int)(s.data) - 1) * 4 + 3);
                tran.Find("4/text").GetComponent<Text>().text = "" + (((int)(s.data) - 1) * 4 + 4);
            }, i);
        }

        Transform group2 = (Resources.Load("Group2") as GameObject).transform;


        for (int i = 1; i < 50; i++)
        {
            siiv.AddItem(group2, bottom, (Transform tran, StaticInitItemView.SIIV s) =>
            {
            });

        }
        content.GetComponent<GridChildGroup>().ReFlash();

    }
}
