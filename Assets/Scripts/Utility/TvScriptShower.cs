using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEditor;

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
                            +SwitchAffectedPrice((AffectedPrice)newsDatas[i].affectedPrice)
                            +newsDatas[i].amount+"%";   
        }
        textDescription.GetComponent<SlidingText>().SetFullText(textDesc);
        textEffect.GetComponent<SlidingText>().SetFullText(textEff);
    }

    string SwitchAffectedPrice(AffectedPrice price)
    {
        switch (price)
        {
            case AffectedPrice.AppraisedPrice:
                return "감정가";
            case AffectedPrice.AskingPrice:
                return "최초 제시가";
            case AffectedPrice.PurchasePrice:
                return "구매가";
            case AffectedPrice.SellingPrice:
                return "판매가";
            default: 
                return "없는 가격";
        }
    }

}
