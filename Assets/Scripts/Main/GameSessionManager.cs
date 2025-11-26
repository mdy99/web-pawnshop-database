using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using UnityEditor;

public class GameSessionManager : MonoBehaviour
{
    [Header("디버그 확인용")]
    [SerializeField] private int dayCount = -1;
    [SerializeField] private long money= -1;
    [SerializeField] private long personalDebt = -1;
    [SerializeField] private long pawnshopDebt = -1;
    [SerializeField] private int unlockedShowcaseCount = 8; // 이제 와서 수정은 늦었다 8로 고정시켜
    [SerializeField] private string nickname="";
    [SerializeField] private string shopName="";
    

    [Header("연결용")]
    [SerializeField] private GameObject nickAndShopPanel;
    [SerializeField] private TMP_Text nickNameInput;
    [SerializeField] private TMP_Text shopNameInput;
    
    [SerializeField] private TMP_Text nickShopDayCountOutput;
    [SerializeField] private TMP_Text goldOutput;
    [SerializeField] private TMP_Text personalDebtOutput;
    [SerializeField] private TMP_Text shopDebtOutput;
    [SerializeField] private ItemActionManager itemActionManager;
    [SerializeField] private GameObject GameEndObjs;


    public void PopupGameEndObjs(WorldRecordData worldRecordData, string gameOverDescription)
    {
        string gameEndTitleText;
        if(worldRecordData.gameEndDayCount < 0) // 음수면은 게임오버
        {
            gameEndTitleText = "게임 오버!";
        }
        else
        {
            gameEndTitleText = "게임 클리어!!!";
        }
        GameEndObjs.transform.GetChild(2).GetComponent<TMP_Text>().text = gameEndTitleText;
        GameEndObjs.transform.GetChild(3).GetComponent<TMP_Text>().text = 
                gameOverDescription;
        GameObject worldRecord = GameEndObjs.transform.GetChild(4).gameObject;
        // Nickname
        worldRecord.transform.GetChild(1).GetComponent<TMP_Text>().text
            = $"#{worldRecordData.playerId}";
        worldRecord.transform.GetChild(6).GetComponent<TMP_Text>().text
            = worldRecordData.nickname;
        // Shopname
        worldRecord.transform.GetChild(7).GetComponent<TMP_Text>().text
            = worldRecordData.pawnshopName;
        // DayCount
        worldRecord.transform.GetChild(8).GetComponent<TMP_Text>().text
            = $"{string.Format("{0:#,0}",worldRecordData.gameEndDayCount)} 일";
        // Date
        worldRecord.transform.GetChild(9).GetComponent<TMP_Text>().text
            = worldRecordData.gameEndDate;
        // 게임 오버 화면 띄우기
        GameEndObjs.SetActive(true);
    }

    public int GetLeftMoney()
    {
        return (int)money;
    }
    public void SetLeftMoney(int leftMoney)
    {
        money = leftMoney;
        goldOutput.text =$"{string.Format("{0:#,0}",leftMoney)} G";
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() // 시작할 때 게임 세션 받아오기
    {
        RequestForGameSession();
    }

    private void RequestForGameSession()
    {
        if(SingletonManager.Instance.HasGameSession == "Y")
        { // 게임 세션이 이미 있음. 게임 세션 불러오기
            // 받는 창 켜있으면 끄기
            nickAndShopPanel.SetActive(false);
            RequestLatestGameSession();
        }
        else if (SingletonManager.Instance.HasGameSession == "N")
        { // 게임 세션 생성하기
            // 닉네임 받는 창 띄우기
            nickAndShopPanel.SetActive(true);
        }
    }

    public void OnSubmitButtonClicked()
    {
        string nickText = nickNameInput.text;
        string shopText = shopNameInput.text; 
        // 닉이랑 숍 이름 잘 작성했는지 검사하기
        if(nickText.Length > 10)
        {
            ConfirmPopuper.Instance.PopupCheckPanel("10자 이하의 이름을 입력하세요");
            return;
        }
        
        if(shopText.Length > 10)
        {
            ConfirmPopuper.Instance.PopupCheckPanel("10자 이하의 가게 이름을 입력하세요");
            return;
        }
        nickname = nickText;
        shopName = shopText;
        RequestNewGameSession();
    }

    // 새 게임 데이터 요구하기
    private void RequestNewGameSession()
    {
        // 데이터 요청
        NewGameSessionRequest requestData = new NewGameSessionRequest();
        requestData.nickname = nickNameInput.text;
        requestData.shopName = shopNameInput.text;
        // 데이터 요청
        // GameSessionData responseData = TransmissionManager.Instance.RequestToServer<NewGameSessionRequest,GameSessionData>(RequestType.NEW_SESSION, requestData);
        
        // 테스트용 데이터 사용 <<<<<<<<<<<<<<<<<<<<<<
        TextAsset jsonFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Mocks/3newGameSession.json", typeof(TextAsset));
        GameSessionData responseData =JsonUtility.FromJson<GameSessionData>(jsonFile.text);
        responseData.nickname = nickNameInput.text;
        responseData.shopName = shopNameInput.text;
        
        // 받아온 데이터로 업데이트 하기
        SetUIData(responseData);
        // 이제 닉 받는 패널 끄기
        nickAndShopPanel.SetActive(false); 
    }
    private void RequestLatestGameSession()
    {
        // 데이터 요청
        // GameSessionData responseData = TransmissionManager.Instance.RequestToServer<int,GameSessionData>(RequestType.LATEST_SESSION, 0); // 보낼 데이터가 없음

        // 테스트용 데이터 사용 <<<<<<<<<<<<<<<<<<<<<<
        TextAsset jsonFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Mocks/4latestGameSession.json", typeof(TextAsset));
        GameSessionData responseData =JsonUtility.FromJson<GameSessionData>(jsonFile.text);

        // TODO: 받은 데이터로 보이는 것들 세팅       
        SetUIData(responseData);
    }

    private void SetUIData(GameSessionData responseData)
    {
        dayCount = responseData.dayCount;
        money= responseData.money;
        personalDebt = responseData.personalDebt;
        pawnshopDebt = responseData.pawnshopDebt;
        unlockedShowcaseCount = responseData.unlockedShowcaseCount;
        nickname = responseData.nickname;
        shopName = responseData.shopName;   

        // 닉네임 및 돈
        nickShopDayCountOutput.text = $"{responseData.nickname}의\n{responseData.shopName}\n{responseData.dayCount}일차";
        goldOutput.text = $"{string.Format("{0:#,0}",responseData.money)} G";
        personalDebtOutput.text = $"{string.Format("{0:#,0}",responseData.personalDebt)} G";
        shopDebtOutput.text = $"{string.Format("{0:#,0}",responseData.pawnshopDebt)} G";
        // 빚 ui 업데이트
        itemActionManager.UpdateDebtValue((int)personalDebt, (int)pawnshopDebt);
    }

    public void SetNextDayUI(DayNextData dayNextData)
    {
        // 닉네임 및 돈
        nickShopDayCountOutput.text = $"{nickname}의\n{shopName}\n{dayNextData.dayCount}일차";
        goldOutput.text = $"{string.Format("{0:#,0}",dayNextData.leftMoney)} G";
        personalDebtOutput.text = $"{string.Format("{0:#,0}",dayNextData.personalDebt)} G";
        shopDebtOutput.text = $"{string.Format("{0:#,0}",dayNextData.pawnshopDebt)} G";
        // 빚 ui 업데이트
        itemActionManager.UpdateDebtValue((int)personalDebt, (int)pawnshopDebt);
        
    }
}
