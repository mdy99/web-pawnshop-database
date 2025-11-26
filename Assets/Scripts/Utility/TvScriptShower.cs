using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEditor;
using AYellowpaper.SerializedCollections;

public class TvScriptShower : MonoBehaviour
{
    [SerializeField] private TMP_Text textDescription;
    [SerializeField] private TMP_Text textEffect;
    


    public void SetTvText(List<NewsData> newsDatas)
    {
        string textDesc="";
        string textEff="";
        for(int i = 0; i < newsDatas.Count; i++)
        {
            textDesc = textDesc + "       "+ newsDatas[i].newsDescription;
            textEff=textEff+"       "+newsDatas[i].affectedCategoryName
                            +SingletonManager.Instance.ConvertToAffectedPrice((AffectedPrice)newsDatas[i].affectedPrice)
                            +newsDatas[i].amount+"%";   
        }
        textDescription.GetComponent<SlidingText>().SetFullText(textDesc);
        textEffect.GetComponent<SlidingText>().SetFullText(textEff);
    }

}
