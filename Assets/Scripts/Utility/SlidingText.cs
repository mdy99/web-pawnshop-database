using UnityEngine;
using System.Collections;
using TMPro;
using System;

public class SlidingText : MonoBehaviour
{
    TMP_Text textComponent;
    private string fullText = "여기는 뉴스 이벤트를 나타내는 텍스트가 들어올 자리입니다.텍스트~~";

    public void SetFullText(string text)
    {
        if(string.IsNullOrEmpty(text))
        {
            fullText = "여기는 뉴스 이벤트를 나타내는 텍스트가 들어올 자리입니다. 뉴스가 없다면 7일차까지 기다려보세요. 7일차마다 뉴스가 배정됩니다.";
        }
        else
        {
            fullText = text + "      ";
        }
    }
    const int displayLength = 8;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textComponent = GetComponent<TMP_Text>();
        textComponent.text = fullText.Substring(0, displayLength);
        StartCoroutine(SlideText());
    }

    IEnumerator SlideText()
    {
        while (true)
        {
            if(fullText.Length < displayLength)
            {
                textComponent.text = fullText;
                yield return new WaitForSeconds(0.2f);
                continue;
            }
            
            for (int i = 0; i <= fullText.Length - displayLength; i++)
            {
                textComponent.text = fullText.Substring(0, displayLength);
                fullText = fullText.Substring(1)+fullText.Substring(0,1);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}
