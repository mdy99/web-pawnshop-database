using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager instance = null;

    public static DialogueManager Instance
    {
        get
        {
            if(null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    public Dialogue upperDialogue; // 가장 오래 된 대화
    public Dialogue middleDialogue; // 중간 대화
    public Dialogue lowerDialogue; // 가장 최신 대화

    private Coroutine upperCoroutine;
    private Coroutine middleCoroutine;
    private Coroutine lowerCoroutine;
    
    const float WAITING_TIME=3.0f;

    string testMessage1 ="안녕하세요 이건 메시지입니다.\n메세지를 보냈어요~\n와우~~";
    string testMessage2 ="안녕하세요 이건 두번째 메시지입니다.\n메세지를 보내 보아요 하하\n와우~~\n와우~~";
    string testMessage3 ="안녕하세요 이건 세번째 메시지입니다.";

    void Awake()
    {
        if(null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // StartCoroutine(TestPutMessage(testMessage1,3f));
        // StartCoroutine(TestPutMessage(testMessage2,1f));
        // StartCoroutine(TestPutMessage(testMessage3,5f));
    }

    public void PutDialogue(string newDialogue){
        if (middleDialogue.GetText()!=""){
            if(!upperDialogue.getIsActive()) upperDialogue.SetEnable(true);
            upperDialogue.SetText(middleDialogue.GetText());
            if(upperCoroutine != null)StopCoroutine(upperCoroutine);
            upperCoroutine = StartCoroutine(DisableAfterDelay(upperDialogue));
        }
        if (lowerDialogue.GetText()!=""){
            if(!middleDialogue.getIsActive()) middleDialogue.SetEnable(true);
            middleDialogue.SetText(lowerDialogue.GetText());
            if(middleCoroutine != null)StopCoroutine(middleCoroutine);
            middleCoroutine = StartCoroutine(DisableAfterDelay(middleDialogue));
        }
        if(!lowerDialogue.getIsActive()) lowerDialogue.SetEnable(true);
        lowerDialogue.SetText(newDialogue);
        if(lowerCoroutine != null) StopCoroutine(lowerCoroutine);
        lowerCoroutine = StartCoroutine(DisableAfterDelay(lowerDialogue));

    }
    IEnumerator DisableAfterDelay(Dialogue dialogue){
        yield return new WaitForSeconds(WAITING_TIME);
        dialogue.SetEnable(false);
        dialogue.SetText("");
    }

    public IEnumerator PutMessageWaitTime(string message,float time = 1f){
        yield return new WaitForSeconds(time);
        PutDialogue(message);
    }

}
