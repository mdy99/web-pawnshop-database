using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.Collections;

public class TvScriptShower : MonoBehaviour
{
    [SerializeField] private SlidingText textDescription;
    [SerializeField] private SlidingText textEffect;

    public void SetTvText(List<NewsData> newsDatas)
    {
        Debug.Log(newsDatas);
        string textDesc="";
        string textEff="";
        if(newsDatas != null && newsDatas.Count > 0){
            List<NewsData> news = new List<NewsData>(newsDatas);
            for(int i = 0; i < news.Count; i++)
            {
                textDesc = textDesc + "       "+ news[i].newsDescription;
                textEff=textEff+"       "+news[i].affectedCategoryName
                                +SingletonManager.Instance.ConvertToAffectedPrice((AffectedPrice)newsDatas[i].affectedPrice)
                                +news[i].amount+"%";   
            }
        }
        textDescription.SetFullText(textDesc);
        textEffect.SetFullText(textEff);   
    }

}
