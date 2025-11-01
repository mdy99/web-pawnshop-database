using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Unity.Jobs;
using UnityEngine.UI;

public class TransmissionManager : MonoBehaviour
{
    public static readonly string serverUrl = "https://192.168.0.1:8080"; // 서버 URL
    public InputField outputArea;

    public static void TransmitGameSessionData()
    {
        GameSessionData sessionData = new GameSessionData();
        string json = JsonUtility.ToJson(sessionData);

    }

    void Start()
    {
        StartCoroutine(FetchJsonValue());
        GameObject.Find("GetButton").GetComponent<Button>().onClick.AddListener(GetData);
    }

    void GetData() => StartCoroutine(FetchJsonValue());

    IEnumerator FetchJsonValue()
    {
        string jsonUrl = "https://jsonplaceholder.typicode.com/todos/1";
        using(UnityWebRequest req  = UnityWebRequest.Get(jsonUrl)) // 여기서는 그냥 선언만 함
        {
            yield return req.SendWebRequest(); // 여기서 실제로 요청을 전송하는 거임
            if(req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("its failed to fetch Json Data");
            }
            else
            {
                outputArea.text = "Loading..."; // 와 로딩을 이렇게 표현하는구나..
                string jsonVal = req.downloadHandler.text;
                MyData myData = JsonUtility.FromJson<MyData>(jsonVal);
                Debug.Log($"userId: {myData.userId}");
                Debug.Log($"Id: {myData.id}");
                Debug.Log($"title: {myData.title}");
                Debug.Log($"completed: {myData.completed}");
            }
        }
    }

    IEnumerator PostData(MyData myData)
    {
        string jsonData = JsonUtility.ToJson(myData);

        string url="https://koreanjson.com/posts/1";
        using(UnityWebRequest req = new UnityWebRequest(url, "POST")) // 임시로 의미없는 "POST"만 채우고 아래서 처리한 뒤 요청 보낼 거임
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

            req.uploadHandler = new UploadHandlerRaw(jsonToSend);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type","application/json");
            yield return req.SendWebRequest();
            if(req.error == null)
            {
                Debug.Log("Post Data Success");
                string jsonVal = req.downloadHandler.text;                
            }
            else
            {
                Debug.LogError("Post Data Error");
            }
        }
    }


    IEnumerator UnityWebRequestGet(){
        string jobId= "41f1cdc2ff58bb5fdc287be0db2a8df3";
        string jobGrowId="df3870efe8e8754011cd12fa03cd275f";
        string url = $"https://api.neople.co.kr/df/skills/{jobId}?jobGrowId={jobGrowId}&apikey=nv4Mmaly0ruI26ElVbnqOcWpqtGoV4nR";
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest(); // 응답이 올 때까지 대기
        if (www.error == null)
        {
            Debug.Log(www.downloadHandler.text);
        }
        else
        {
            Debug.Log("ERROR");
        }
    }
}

public class MyData
{
    public string userId="";
    public string id="";
    public string title="";
    public string completed = "";
}