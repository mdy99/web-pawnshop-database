using UnityEngine;
using TMPro;
using AYellowpaper.SerializedCollections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public enum LoginState
{
    LOGIN,
    IN_LOGIN
}

public enum ResponseCode
{
    OK = 200,
    FORBIDDEN = 403,
    NOT_FOUND = 404,
    INTERNAL_SERVER_ERROR = 500
}

public class LoginManager : MonoBehaviour
{
    private static LoginManager instance = null;

    [SerializeField] private GameObject LoginResisterObjs;
    [SerializeField] private GameObject worldRecordPanel;

    [SerializeField] private GameObject inLoginObjs;

    [SerializeField] private GameObject wrldRecordRowPrefab;

    private LoginState loginState = LoginState.LOGIN;

    private string playerIdCache="";
    private TMP_InputField loginIdTMP;
    private TMP_InputField loginPwTMP;
    private TMP_InputField RegisterIdTMP;
    private TMP_InputField RegisterPwTMP;

    void Awake()
    {
        if(null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            #if UNITY_STANDALONE
                Screen.SetResolution(720,1280,false);
                Screen.fullScreen = false;
            #endif
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start() // 게임 시작할 때
    {
        LoadWorldRecord(); // 세계 기록 미리 불러오기
        InitLocalVar(); // 변수 초기화
    }

    private void InitLocalVar()
    {
        loginIdTMP =LoginResisterObjs.transform.GetChild(0).GetChild(1).GetComponent<TMP_InputField>();
        loginPwTMP =LoginResisterObjs.transform.GetChild(0).GetChild(3).GetComponent<TMP_InputField>();
        RegisterIdTMP =LoginResisterObjs.transform.GetChild(1).GetChild(1).GetComponent<TMP_InputField>();
        RegisterPwTMP =LoginResisterObjs.transform.GetChild(1).GetChild(3).GetComponent<TMP_InputField>();
    }

    public void RequestToLogOut(){
        TransmissionManager.Instance.RequestToServer<int, int>(
            RequestType.LOGOUT,
            0,
            (responseCode, responseData) =>
            {
                // 확인용 코드 <<<<<<<<<<<<<<
                // responseCode = 200;

                if((ResponseCode)responseCode == ResponseCode.OK)
                {
                    // // 로그인 초기화
                    // playerIdCache =""; // 캐시 초기화
                    // loginIdTMP.text = "";
                    // loginPwTMP.text = "";
                    // RegisterIdTMP.text = "";
                    // RegisterPwTMP.text = "";
                    // 트랜스미션에 세션토큰 전달
                    TransmissionManager.Instance.SetSessionToken(""); // 초기화

                    // 로그인패널 비활성화, 인로그인패널 활성화
                    loginState =LoginState.LOGIN;
                    inLoginObjs.SetActive(false);
                    LoginResisterObjs.SetActive(true);

                    ConfirmPopuper.Instance?.PopupCheckPanel("로그아웃 되었습니다.");    
                }
                else
                {
                    string errorMessage = "통신 오류: 로그아웃 요청 실패.";
                    TransmissionManager.Instance.OnHandleErrorResponseCode(responseCode, errorMessage);
                }
            }
        );
    }


    public void RequestToLogin()
    {
        // 요청 데이터 전달
        PlayerRegisterLoginRequest requestData = new PlayerRegisterLoginRequest();
        // requestData.playerId = loginIdTMP.text.Substring(0,loginIdTMP.text.Length-1);
        // requestData.password = loginPwTMP.text.Substring(0,loginPwTMP.text.Length-1);
        requestData.playerId = loginIdTMP.text;
        requestData.password = loginPwTMP.text;
        // 서버 요청 코루틴 실행
        TransmissionManager.Instance.RequestToServer<PlayerRegisterLoginRequest,LoginResponse>(
            RequestType.LOGIN,
            requestData,
            (responseCode, responseData) => // 서버 요청 코루틴 끝나면 해당 람다 콜백 함수 실행 됨
            {
                // // 테스트 데이터 <<<<<<<<<<<<<
                // responseData= new LoginResponse(); //
                // responseData.sessionToken = "ang Kimoti"; // <<<<<<<< 확인용 코드
                // responseData.hasGameSession = "Y"; // 기존 게임 불러오기, "N"이면 새 게임 생성에 쓸 변수임

                if(responseData != default)
                {
                    // 로그인 초기화
                    playerIdCache =requestData.playerId; // 아이디는 캐시에 저장
                    loginIdTMP.text = null;
                    loginPwTMP.text = null;
                    RegisterIdTMP.text = null;
                    RegisterPwTMP.text = null;
                    inLoginObjs.transform.GetChild(4).GetComponent<TMP_Text>().text = playerIdCache;
                    // 트랜스미션에 세션토큰 전달
                    TransmissionManager.Instance.SetSessionToken(responseData.sessionToken);
                    // 게임 세션 있는지 전달
                    SingletonManager.Instance.HasGameSession = responseData.hasGameSession;

                    // 로그인패널 비활성화, 인로그인패널 활성화
                    loginState =LoginState.IN_LOGIN;
                    LoginResisterObjs.SetActive(false);
                    inLoginObjs.SetActive(true);

                    string log = "로그인 성공";
                    ConfirmPopuper.Instance?.PopupCheckPanel(log);            
                }
                else if((ResponseCode)responseCode == ResponseCode.OK)
                {
                    // 로그인 실패 처리
                    Debug.Log("로그인 실패");
                    string log = "로그인 실패.\nID와 PW를 잘 확인해보세요";
                    ConfirmPopuper.Instance?.PopupCheckPanel(log);  
                }
                else
                {
                    string errorMessage = "통신 오류: 로그인 요청 실패.";
                    TransmissionManager.Instance.OnHandleErrorResponseCode(responseCode, errorMessage);
                }
            }
        );
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += SetLoginScreen;
    }

    public void ResetToLoginScreen()
    {
        SceneManager.LoadScene("StartScene");
        if(loginState == LoginState.IN_LOGIN)
        { // 스타트 씬에서만 작동
            RequestToLogOut();
        }
        
        // 오브젝트들도 다시 받아오기
        Canvas mainCanvas = FindFirstObjectByType<Canvas>();
        LoginResisterObjs = mainCanvas.transform.GetChild(2).GetChild(1).gameObject;
        inLoginObjs = mainCanvas.transform.GetChild(2).GetChild(2).gameObject;
        worldRecordPanel = mainCanvas.transform.GetChild(3).gameObject;
        // 버튼에도 다시 넣어줘야 함
        LoginResisterObjs.transform.GetChild(0).GetChild(4).GetComponent<Button>().onClick.AddListener(RequestToLogin);
        LoginResisterObjs.transform.GetChild(1).GetChild(4).GetComponent<Button>().onClick.AddListener(RequestToRegister);
        inLoginObjs.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(RequestToLogOut);
        inLoginObjs.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(OpenWorldRecord);
        worldRecordPanel.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(CloseWorldRecord);
        // 세계 기록도 변했을 수 있으니 다시 받아오기
        LoadWorldRecord(); // 세계 기록 미리 불러오기
        // 아이디 최신화. 혹시 모르니
        inLoginObjs.transform.GetChild(4).GetComponent<TMP_Text>().text = playerIdCache;
        // 패널 활성화 세팅
        LoginResisterObjs.SetActive(false);
        inLoginObjs.SetActive(true);
    }

    void SetLoginScreen(Scene scene, LoadSceneMode mode)
    {
        if(SceneManager.GetActiveScene().name != "StartScene")
        { // 스타트 씬에서만 작동
            return;
        }
        if(loginState == LoginState.IN_LOGIN)
        {
            // 오브젝트들도 다시 받아오기
            Canvas mainCanvas = FindFirstObjectByType<Canvas>();
            LoginResisterObjs = mainCanvas.transform.GetChild(2).GetChild(1).gameObject;
            inLoginObjs = mainCanvas.transform.GetChild(2).GetChild(2).gameObject;
            worldRecordPanel = mainCanvas.transform.GetChild(3).gameObject;
            // 버튼에도 다시 넣어줘야 함
            LoginResisterObjs.transform.GetChild(0).GetChild(4).GetComponent<Button>().onClick.AddListener(RequestToLogin);
            LoginResisterObjs.transform.GetChild(1).GetChild(4).GetComponent<Button>().onClick.AddListener(RequestToRegister);
            inLoginObjs.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(RequestToLogOut);
            inLoginObjs.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(OpenWorldRecord);
            worldRecordPanel.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(CloseWorldRecord);
            // 세계 기록도 변했을 수 있으니 다시 받아오기
            LoadWorldRecord(); // 세계 기록 미리 불러오기
            // 아이디 최신화. 혹시 모르니
            inLoginObjs.transform.GetChild(4).GetComponent<TMP_Text>().text = playerIdCache;
            // 패널 활성화 세팅
            LoginResisterObjs.SetActive(false);
            inLoginObjs.SetActive(true);
        }
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= SetLoginScreen;
    }

    public void RequestToRegister()
    {
        PlayerRegisterLoginRequest requestData = new PlayerRegisterLoginRequest();
        requestData.playerId = RegisterIdTMP.text;
        requestData.password = RegisterPwTMP.text;
        // TODO: 해당 default가 성공인지 실패인지 알려면
        TransmissionManager.Instance.RequestToServer<PlayerRegisterLoginRequest,int>(
            RequestType.REGISTER,
            requestData,
            (responseCode, responseData) =>
            {
                // responseCode =200; // <<<<<<<<<<<<<<< 확인용 코드
                if(responseCode == 200)
                {
                    loginState =LoginState.IN_LOGIN;
                    string log = "회원가입 성공";
                    ConfirmPopuper.Instance?.PopupCheckPanel(log);            
                }
                else if(responseCode == 400)
                {
                    string log = "회원가입 실패.\nID가 중복됩니다.";
                    ConfirmPopuper.Instance?.PopupCheckPanel(log);  
                }
                else
                {
                    string errorMessage = "통신 오류: 회원가입 요청 실패.";
                    TransmissionManager.Instance.OnHandleErrorResponseCode(responseCode, errorMessage);
                }
            }
        );
    }
    


    private void LoadWorldRecord()
    {
        TransmissionManager.Instance.RequestToServer<int, WorldRecordResponse>(
            RequestType.WORLD_RECORDS,
            0,
            (responseCode, responseData) =>
            {
                // 테스트 데이터 <<<<<<<<<<<<<<<<<<<<<
                // responseCode = 200;
                // TextAsset jsonFile = Resources.Load<TextAsset>("Mocks/20worldRecords");
                // responseData =JsonUtility.FromJson<WorldRecordResponse>(jsonFile.text);

                // 오류 확인
                if((ResponseCode)responseCode != ResponseCode.OK)
                {
                    string errorMessage = "통신 오류: 세계 기록 요청 실패.";
                    TransmissionManager.Instance.OnHandleErrorResponseCode(responseCode, errorMessage);
                }
                // 정상 동작
                else
                {
                    GameObject conetentObj =  worldRecordPanel.transform.GetChild(2).GetChild(0).GetChild(0).gameObject;
                    
                    for(int i = 0; i < responseData.worldRecords.Count; i++)
                    {
                        GameObject wrldRecordRow=Instantiate(wrldRecordRowPrefab, conetentObj.transform);
                        // Nickname
                        wrldRecordRow.transform.GetChild(1).GetComponent<TMP_Text>().text
                            = $"#{responseData.worldRecords[i].playerId}";
                        wrldRecordRow.transform.GetChild(6).GetComponent<TMP_Text>().text
                            = responseData.worldRecords[i].nickname;
                        // Shopname
                        wrldRecordRow.transform.GetChild(7).GetComponent<TMP_Text>().text
                            = responseData.worldRecords[i].pawnshopName;
                        // DayCount
                        wrldRecordRow.transform.GetChild(8).GetComponent<TMP_Text>().text
                            = $"{string.Format("{0:#,0}",responseData.worldRecords[i].gameEndDayCount)} 일";
                        // Date
                        wrldRecordRow.transform.GetChild(9).GetComponent<TMP_Text>().text
                            = responseData.worldRecords[i].gameEndDate;
                    }                    
                }
            }
        );
    }

    public void OpenWorldRecord()
    {
        worldRecordPanel.SetActive(true);
    }
    public void CloseWorldRecord()
    {
        worldRecordPanel.SetActive(false);
    }

}
