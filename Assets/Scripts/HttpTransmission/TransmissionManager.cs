using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Unity.Jobs;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System;
using UnityEditor.Compilation;
using UnityEngine.SceneManagement;

public enum RequestType{
    REGISTER,           // POST /player/register
    LOGIN,              // POST /player/login
    LOGOUT,             // POST /player/logout
    NEW_SESSION,        // POST /game-session/new
    LATEST_SESSION,     // POST /game-session/latest
    INIT_CATALOGS,      // GET /catalog/initialData
    DISPLAY_CUR_ALL,    // GET /display/currentAll
    NEWS_CUR,           // GET /news/current
    CUS_REVEAL,         // PATCH /customer/reveal
    GET_ITEM_HINTS,     // GET /item/getHints
    ITEM_ACTION,        // POST /item/action
    ITEM_RESULT,        // POST /item/result
    ITEM_SELL_START,    // POST /item/sellStart
    ITEM_SELL_CANCEL, // POST /item/sellCancel
    ITEM_SELL_COMPLETE, // POST /item/sellComplete
    DAILY_DEALS,        // POST /deal/loadOrGenerateDailyDeals
    DEAL_ACTION,        // POST /deal/action
    DEAL_COMPLETE,      // POST /deal/complete
    DEAL_CANCEL,        // POST /deal/cancel
    LOAN_UPDATE,        // POST /loan/update
    WORLD_RECORDS       // GET /worldRecords
}

public class TransmissionManager : MonoBehaviour
{
    private static TransmissionManager instance = null; // 얘 싱글톤임

    public static readonly string serverUrl = "http://localhost:8080"; // 서버 URL
    public static string sessionToken = ""; // 세션 토큰 저장
    
    [SerializeField] private LoginManager loginManager;

    public void SetSessionToken(string sessionTokStr)
    {
        sessionToken = sessionTokStr;
    }

    public S RequestToServer<T,S>(RequestType reqType,T requestData){
        string routeUrl ="";
        int returnedResponseCode = -1;
        S returnData= default;
        switch(reqType){
            case RequestType.REGISTER: // POST /player/register
                routeUrl = "/player/register";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl,
                    (responseCode, responseData) =>
                    {
                        // 얘는 결과값을 안 받으니 리스폰스 코드(200,404 ...)로 반환함
                        returnData = (S)(object)responseCode; // 오브젝트 최상위로 한번 포장하고 제네릭으로 변환
                        returnedResponseCode = responseCode;
                    }));
                break;
            case RequestType.LOGIN: // POST /player/login
                routeUrl = "/player/login";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl,
                    (responseCode, responseData) =>
                    {
                        returnData = responseData;
                        returnedResponseCode = responseCode;
                    }));
                break;
            case RequestType.LOGOUT: // POST /player/logout
                routeUrl = "/player/logout";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl,
                    (responseCode, responseData) =>
                    {
                        // 얘는 결과값을 안 받으니 리스폰스 코드(200,404 ...)로 반환함
                        returnData = (S)(object)responseCode; // 오브젝트 최상위로 한번 포장하고 제네릭으로 변환
                        returnedResponseCode = responseCode;
                    }));
                break;
            case RequestType.NEW_SESSION: // POST /game-session/new
                routeUrl = "/game-session/new";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl,
                    (responseCode, responseData) =>
                    {
                        returnData = responseData;
                        returnedResponseCode = responseCode;
                    }));
                break;
            case RequestType.LATEST_SESSION: // POST /game-session/latest
                routeUrl = "/game-session/latest";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl,
                    (responseCode, responseData) =>
                    {
                        returnData = responseData;
                        returnedResponseCode = responseCode;
                    }));
                break;
            case RequestType.INIT_CATALOGS: // GET /catalog/initialData
                routeUrl = "/catalog/initialData";
                StartCoroutine(GetJsonValue<S>(routeUrl,
                    (responseCode, responseData) =>
                    {
                        returnData = responseData;
                        returnedResponseCode = responseCode;
                    }));
                break;
            case RequestType.DISPLAY_CUR_ALL: // GET /display/currentAll
                routeUrl = "/display/currentAll";
                StartCoroutine(GetJsonValue<S>(routeUrl,
                    (responseCode, responseData) =>
                    {
                        returnData = responseData;
                        returnedResponseCode = responseCode;
                    }));
                break;
            case RequestType.NEWS_CUR: // GET /news/current
                routeUrl = "/news/current";
                StartCoroutine(GetJsonValue<S>(routeUrl,
                    (responseCode, responseData) =>
                    {
                        returnData = responseData;
                        returnedResponseCode = responseCode;
                    }));
                break;
            case RequestType.CUS_REVEAL: // PATCH /customer/reveal
                routeUrl = "/customer/reveal";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl,
                    (responseCode, responseData) =>
                    {
                        returnData = responseData;
                        returnedResponseCode = responseCode;
                    }));
                break;
            case RequestType.GET_ITEM_HINTS: // GET /item/getHints
                routeUrl = "/item/getHints";
                StartCoroutine(GetJsonValue<S>(routeUrl,
                    (responseCode, responseData) =>
                    {
                        returnData = responseData;
                        returnedResponseCode = responseCode;
                    }));
                break;
            case RequestType.ITEM_ACTION: // POST /item/action
                routeUrl = "/item/action";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl,
                    (responseCode, responseData) =>
                    {
                        returnData = responseData;
                        returnedResponseCode = responseCode;
                    }));
                break;
            case RequestType.ITEM_RESULT: // POST /item/result
                routeUrl = "/item/result";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl,
                    (responseCode, responseData) =>
                    {
                        returnData = responseData;
                        returnedResponseCode = responseCode;
                    }));
                break;
            case RequestType.ITEM_SELL_START: // POST /item/sellStart
                routeUrl = "/item/sellStart";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl,
                    (responseCode, responseData) =>
                    {
                        returnData = responseData;
                        returnedResponseCode = responseCode;
                    }));
                break;
            case RequestType.ITEM_SELL_CANCEL: // POST /item/sellCancel
                routeUrl = "/item/sellCancel";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl,
                    (responseCode, responseData) =>
                    {
                        returnData = responseData;
                        returnedResponseCode = responseCode;
                    }));
                break;
            case RequestType.ITEM_SELL_COMPLETE: // POST /item/sellComplete
                routeUrl = "/item/sellComplete";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl,
                    (responseCode, responseData) =>
                    {
                        returnData = responseData;
                        returnedResponseCode = responseCode;
                    }));
                break;
            case RequestType.DAILY_DEALS: // POST /deal/loadOrGenerateDailyDeals
                routeUrl = "/deal/loadOrGenerateDailyDeals";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl,
                    (responseCode, responseData) =>
                    {
                        returnData = responseData;
                        returnedResponseCode = responseCode;
                    }));
                break;
            case RequestType.DEAL_ACTION: // POST /deal/action
                routeUrl = "/deal/action";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl,
                    (responseCode, responseData) =>
                    {
                        returnData = responseData;
                        returnedResponseCode = responseCode;
                    }));
                break;
            case RequestType.DEAL_COMPLETE: // POST /deal/complete
                routeUrl = "/deal/complete";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl,
                    (responseCode, responseData) =>
                    {
                        returnData = responseData;
                        returnedResponseCode = responseCode;
                    }));
                break;
            case RequestType.LOAN_UPDATE: // POST /loan/update
                routeUrl = "/loan/update";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl,
                    (responseCode, responseData) =>
                    {
                        returnData = responseData;
                        returnedResponseCode = responseCode;
                    }));
                break;
            case RequestType.DEAL_CANCEL: // POST /deal/cancel
                routeUrl = "/deal/cancel";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl,
                    (responseCode, responseData) =>
                    {
                        // 얘는 결과값을 안 받으니 리스폰스 코드(200,404 ...)로 반환함
                        returnData = (S)(object)responseCode; // 오브젝트 최상위로 한번 포장하고 제네릭으로 변환
                        returnedResponseCode = responseCode;
                    }));
                break;
            case RequestType.WORLD_RECORDS: // GET /worldRecords
                routeUrl = "/worldRecords";
                StartCoroutine(GetJsonValue<S>(routeUrl,
                    (responseCode, responseData) =>
                    {
                        returnData = responseData;
                        returnedResponseCode = responseCode;
                    }));
                break;
        }
        // returnedResponseCode = 401; // <<<<< 디버깅용 코드
        // 401 Unauthorized 
        if(returnedResponseCode == 401)
        {
            ConfirmPopuper.Instance.
                PopupCheckPanel("세션토큰이 만료되었습니다.\n로그인 화면으로 이동합니다.",
                                                ()=>{loginManager.ResetToLoginScreen();});
        }
        else if(returnedResponseCode != 200)
        {
            // ConfirmPopuper.Instance.
            //     PopupCheckPanel("통신 오류가 발생하였습니다.");
        }

        return returnData;
    }

    // 인자로 전달한 파라미터에 결과값 담아서 줄게
    IEnumerator GetJsonValue<S>(string routeUrl, Action<int,S> callback)
    {
        string jsonUrl = serverUrl+routeUrl; // ex http://local.host/player/register
        using(UnityWebRequest req  = UnityWebRequest.Get(jsonUrl)) // 여기서는 그냥 선언만 함
        {
            // 헤더 설정
            req.SetRequestHeader("Content-Type","application/json"); // json 타입으로 주고 받을게
            if(!string.IsNullOrEmpty(sessionToken)) // 세션 토큰 방어
            {
                req.SetRequestHeader("Authorization", $"Token {sessionToken}"); // 세션 토큰 헤더에 담기
            }
            // 통신 시도
            yield return req.SendWebRequest(); // 여기서 실제로 요청을 전송하는 거임
            if(req.result != UnityWebRequest.Result.Success) // 에러 발생
            {
                Debug.LogError("its failed to fetch Json Data");
            }
            else // 성공 시
            {
                // 결과를 담기
                int resCode = (int)req.responseCode;
                string jsonVal = req.downloadHandler.text;
                // 결과값 담아서 주기
                callback(resCode,JsonUtility.FromJson<S>(jsonVal));
                
            }
        }
    }

    // 전달할 데이터랑 결과값 받을 데이터 인자로 주세요
    IEnumerator PostJsonValue<T,S>(T requestData, string routeUrl, Action<int,S> callback, bool isPatch = false)
    {
        // 보낼 데이터가 없으면 requestData 타입을 int로 설정
        string jsonData = "";
        // 구조체를 JSON 데이터화 하기
        if(requestData is not int)
        {
            jsonData = JsonUtility.ToJson(requestData);            
        }


        // url 설정
        string url= serverUrl+routeUrl;
        // 보내기 시작
        using(UnityWebRequest req = new UnityWebRequest(url, isPatch? "PATCH" : "POST"))
        { // 생성자에서 이걸 보고 판단함. 아니면 아래서 req.method ="PATCH" 이렇게 설정하면 돼
            // json 인코딩
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

            // 바디에 담기
            req.uploadHandler = new UploadHandlerRaw(jsonToSend);
            // 결과물 받을 버퍼 생성
            req.downloadHandler = new DownloadHandlerBuffer();
            // 헤더 설정
            req.SetRequestHeader("Content-Type","application/json"); // json 타입으로 주고 받을게
            if(!string.IsNullOrEmpty(sessionToken)) // 세션 토큰 방어
            {
                req.SetRequestHeader("Authorization", $"Token {sessionToken}"); // 세션 토큰 헤더에 담기
            }
            // 통신 시도
            yield return req.SendWebRequest(); 
            if(req.error == null) // 성공 시
            {
                Debug.Log("Post Data Success");
                // 다운로드 핸들러에서 결과물 받아 사용
                if (req.responseCode == 200 && !string.IsNullOrEmpty(req.downloadHandler.text))
                {
                    int resCode = (int)req.responseCode;
                    string jsonVal = req.downloadHandler.text;
                    // 받은 결과물을 결과물 컨테이너 담아서 종료
                    callback(resCode,JsonUtility.FromJson<S>(jsonVal));
                }
                else{
                    Debug.LogError("Post Data Error Request 400 500 ...");
                }
            }
            else // 실패 시
            {
                Debug.LogError("Post Data Error No Response");
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

    public static TransmissionManager Instance
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
}

public class MyData
{
    public string userId="";
    public string id="";
    public string title="";
    public string completed = "";
}

