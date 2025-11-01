using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Dialogue : MonoBehaviour
{
    TMP_Text dialogue;
    
    
    private void Awake() {
        dialogue = GetComponentInChildren<TMP_Text>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogue.text = "";
        SetEnable(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool getIsActive(){
        return gameObject.activeSelf;
    }

    public void SetEnable(bool set){
        gameObject.SetActive(set);
    }

    public bool boo(){
        return isActiveAndEnabled;
    }

    public void SetText(string text){
        // Enable 시킴
        dialogue.text = text;
    }
    public string GetText(){
        return dialogue.text;
    }
    IEnumerator WaitForSecondsAndFadeout(){
        float fadedTime = 0.5f;
        float elapsedTime= 0f;
        // 3초 간 대기
        yield return new WaitForSeconds(3.0f);
        // 페이드아웃
        while(elapsedTime < fadedTime){
            dialogue.alpha = Mathf.Lerp(1.0f,0.0f,elapsedTime/fadedTime);
            elapsedTime += Time.deltaTime;
        }

        yield break;
    }
}
