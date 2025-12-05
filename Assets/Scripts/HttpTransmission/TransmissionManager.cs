using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

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

    public void RequestToServer<T,S>(
        RequestType reqType,
        T requestData,
        Action<int, S> onCompleted // 응답 객체를 콜백으로 전달하기. 응답 다 되면 여기에 값이 콜백으로 갈 것임
    ){
        string routeUrl ="";
        switch(reqType){
            case RequestType.REGISTER: // POST /player/register
                routeUrl = "/player/register";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl,onCompleted));
                break;
            case RequestType.LOGIN: // POST /player/login
                routeUrl = "/player/login";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl,onCompleted));
                break;
            case RequestType.LOGOUT: // POST /player/logout
                routeUrl = "/player/logout";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl,onCompleted));
                break;
            case RequestType.NEW_SESSION: // POST /game-session/new
                routeUrl = "/game-session/new";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl, onCompleted));
                break;
            case RequestType.LATEST_SESSION: // POST /game-session/latest
                routeUrl = "/game-session/latest";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl, onCompleted));
                break;
            case RequestType.INIT_CATALOGS: // GET /catalog/initialData
                routeUrl = "/catalog/initialData";
                StartCoroutine(GetJsonValue<S>(routeUrl, onCompleted));
                break;
            case RequestType.DISPLAY_CUR_ALL: // GET /display/currentAll
                routeUrl = "/display/currentAll";
                StartCoroutine(GetJsonValue<S>(routeUrl, onCompleted));
                break;
            case RequestType.NEWS_CUR: // GET /news/current
                routeUrl = "/news/current";
                StartCoroutine(GetJsonValue<S>(routeUrl, onCompleted));
                break;
            case RequestType.CUS_REVEAL: // PATCH /customer/reveal
                routeUrl = "/customer/reveal";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl, onCompleted, true));
                break;
            case RequestType.GET_ITEM_HINTS: // GET /item/getHints
                routeUrl = "/item/getHints";
                StartCoroutine(GetJsonValue<S>(routeUrl, onCompleted));
                break;
            case RequestType.ITEM_ACTION: // POST /item/action
                routeUrl = "/item/action";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl, onCompleted));
                break;
            case RequestType.ITEM_RESULT: // POST /item/result
                routeUrl = "/item/result";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl, onCompleted));
                break;
            case RequestType.ITEM_SELL_START: // POST /item/sellStart
                routeUrl = "/item/sellStart";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl, onCompleted));
                break;
            case RequestType.ITEM_SELL_CANCEL: // POST /item/sellCancel
                routeUrl = "/item/sellCancel";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl, onCompleted));
                break;
            case RequestType.ITEM_SELL_COMPLETE: // POST /item/sellComplete
                routeUrl = "/item/sellComplete";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl, onCompleted));
                break;
            case RequestType.DAILY_DEALS: // POST /deal/loadOrGenerateDailyDeals
                routeUrl = "/deal/loadOrGenerateDailyDeals";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl, onCompleted));
                break;
            case RequestType.DEAL_ACTION: // POST /deal/action
                routeUrl = "/deal/action";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl, onCompleted));
                break;
            case RequestType.DEAL_COMPLETE: // POST /deal/complete
                routeUrl = "/deal/complete";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl, onCompleted));
                break;
            case RequestType.LOAN_UPDATE: // POST /loan/update
                routeUrl = "/loan/update";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl, onCompleted));
                break;
            case RequestType.DEAL_CANCEL: // POST /deal/cancel
                routeUrl = "/deal/cancel";
                StartCoroutine(PostJsonValue<T,S>(requestData,routeUrl, onCompleted));
                break;
            case RequestType.WORLD_RECORDS: // GET /worldRecords
                routeUrl = "/worldRecords";
                StartCoroutine(GetJsonValue<S>(routeUrl, onCompleted));
                break;
        }
    }

    public void OnHandleErrorResponseCode(int responseCode, string errorMessage)
    {
        if (ConfirmPopuper.Instance == null && loginManager == null) { return; }

        if(responseCode == 401)
        {            
            errorMessage = "세션토큰이 만료되었습니다.\n로그인 화면으로 이동합니다.";
        }
        ConfirmPopuper.Instance.PopupCheckPanel(errorMessage, ()=>{loginManager.ResetToLoginScreen();});
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

            // 결과 응답 코드 확인
            int resCode = (int)req.responseCode;
            // 결과 처리
            if(req.result != UnityWebRequest.Result.Success) // 에러 발생
            {
                Debug.LogError("its failed to fetch Json Data");
                callback(resCode, default);
            }
            else // 성공 시
            {
                // 결과를 담기
                string jsonVal = req.downloadHandler.text;
                S resData = default;
                if (!string.IsNullOrEmpty(jsonVal))
                {
                    resData = JsonUtility.FromJson<S>(jsonVal);
                }
                // 결과값 담아서 주기
                callback(resCode, resData);
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

            int resCode = (int)req.responseCode;
            // 결과 처리
            if(req.result != UnityWebRequest.Result.Success) // 에러 발생
            {
                Debug.LogError("its failed to fetch Json Data");
                callback(resCode, default);
            }
            else // 성공 시
            {
                // 결과를 담기
                string jsonVal = req.downloadHandler.text;
                S resData = default;
                if (!string.IsNullOrEmpty(jsonVal))
                {
                    resData = JsonUtility.FromJson<S>(jsonVal);
                }
                // 결과값 담아서 주기
                callback(resCode, resData);
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