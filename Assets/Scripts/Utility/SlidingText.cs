using UnityEngine;
using System.Collections;
using TMPro;
    
public class SlidingText : MonoBehaviour
{
    TMP_Text textComponent;
    private string fullText = "여기는 뉴스 이벤트를 나타내는 텍스트가 들어올 자리입니다.텍스트~~";

    public void SetFullText(string text)
    {
        fullText = text;
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
            for (int i = 0; i <= fullText.Length - displayLength; i++)
            {
                textComponent.text = fullText.Substring(i, displayLength);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}
