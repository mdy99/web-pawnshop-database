using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor;
using TMPro;
using Unity.VisualScripting;
using System.Net.Http;

public enum GradeActionType
{
    RARE,
    UNIQUE,
    LEGENDARY
}
public enum FlawActionType
{
    LOW,
    MIDDLE,
    HIGH
}


namespace AYellowpaper.SerializedCollections
{
public class DealManager : MonoBehaviour
{
    [SerializeField] private GameObject dealObjs;
    [SerializeField] private GameObject deseBlackFilter;
    [SerializeField] private GameObject trayItemObj;
    [SerializeField] private GameObject customerObj;
    [SerializeField] private GameObject customerInformPanel;

    [SerializeField] private GameObject sellObjs;

    [SerializeField] private GameObject itemDisplayManager;


    [SerializedDictionary("order","DealData")] // 얘는 데이터 저장용 dict
    public SerializedDictionary<int, DealData> dailyDealsMap = new SerializedDictionary<int, DealData>();

    private int flawChangedPurchasePrice=-999;
    private int flawChangedAppraisedPrice=-999;
    private int authChangedPurchasePrice=-999;
    private int authChangedAppraisedPrice=-999;
    private int gradeChangedPurchasePrice=-999;
    private int gradeChangedAppraisedPrice=-999;
    private DealData currentDealData;

    private GameObject currentFlawObjs;
    private GameObject currentAuthObjs;
    private GameObject currentGradeObjs;
    private GameObject currentItemHintObjs;
    private GameObject currentCusHintObjs;
    private GameObject currentTotalPriceObjs;
    
    private Dictionary<int,DisplayedItemData> displaysMap;
    private SellStartRequest currentSellItem=new SellStartRequest();
    
    private int currentDealIndex =-1;
    
    [SerializeField] private GameObject gradeTogGroup;
    [SerializeField] private GameObject flawTogGroup;
    [SerializeField] private GameObject gradeActionButton;
    [SerializeField] private GameObject flawActionButton;

    private GradeActionType currentGradeActionType = GradeActionType.RARE;
    private FlawActionType currnetFlawActionType = FlawActionType.LOW;

    [SerializeField] private GameSessionManager gameSessionManager;


    void Start()
    {
        dealObjs.transform.GetChild(3).GetChild(0).GetComponent<Button>().onClick.AddListener(DecideDeal);
        dealObjs.transform.GetChild(4).GetChild(0).GetComponent<Button>().onClick.AddListener(DenyDeal);
        displaysMap = itemDisplayManager.GetComponent<ItemDisplayManager>().GetItemDisplayMap();

        // 테스트용 코드
        TextAsset jsonFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Mocks/10generateDailyDeals.json", typeof(TextAsset));
        DailyDealsWrapData dailyDealsWrapData =JsonUtility.FromJson<DailyDealsWrapData>(jsonFile.text);
        InitDailyDeals(dailyDealsWrapData.dailyDeals);
        //SetNextDaySetting();
    }

    public void InitDailyDeals(List<DealData> dailyDeals)
    {
        for(int i = 0; i < 3; i++)
        {
            dailyDealsMap.Add(i,dailyDeals[i]);
        }
        currentDealIndex = 0;
        SetDealPanelToNewData(dailyDealsMap[currentDealIndex]); // 최초 거래로 설정해둠
    }

    public void SetDealPanelToNewData(DealData dData)
    {
        // 트레이 위 아이템 이미지 활성화
        trayItemObj.SetActive(true);
        // 고객 설정
        customerObj.SetActive(true);
        currentDealData = dData;
        CustomerCatalogData cData =SingletonManager.Instance?.GetCustomerCatalog(dData.customerKey);
        customerObj.GetComponent<Image>().sprite = Resources.Load<Sprite>($"IMG_CUSTOMER/{cData.imgId}");
        // 고객 패널 설정
        customerInformPanel.transform.GetChild(1).GetComponent<TMP_Text>().text
            = $"{cData.customerName}\n[{cData.favoriteCategoryName}]";

        /* 거래/판매 분기점 */
        int posKey =-1;
        if ((posKey=DecideNextIsDeal())== -1) // 거래 세팅
        // int posKey =1; // 판매 확인용 코드
        // if(false)
        {
            // 아이템 설정
            ItemCatalogData iData = SingletonManager.Instance?.GetItemCatalog(dData.itemCatalogKey);
            trayItemObj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>($"IMG_ITEM_CATALOG/{iData.imgId}");
            // 아이템 패널 설정
            trayItemObj.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text
                = $"{iData.itemCatalogName}\n[{iData.categoryName}]";
            // 거래 패널 할당
            currentFlawObjs = dealObjs.transform.GetChild(0).GetChild(3).gameObject;
            currentAuthObjs = dealObjs.transform.GetChild(0).GetChild(4).gameObject;
            currentGradeObjs = dealObjs.transform.GetChild(0).GetChild(2).gameObject;
            currentItemHintObjs = dealObjs.transform.GetChild(0).GetChild(1).gameObject;
            currentCusHintObjs = dealObjs.transform.GetChild(2).gameObject;
            currentTotalPriceObjs = dealObjs.transform.GetChild(1).gameObject;

            // 거래 패널 데이터 할당
            UpdateChangedFlawPrice(currentDealData.foundFlawEa);
            UpdateChangedAuthPrice((Authenticity)currentDealData.foundAuthenticity);
            UpdateChangedGradePrice((Grade)currentDealData.foundGrade);
            UpdateTotalPrice(dData.purchasePrice, dData.appraisedPrice,dData.askingPrice);
            // 토글 초기화
            OnGradeLevelToggleClicked();
            OnFlawLevelToggleClicked();
            // 아이템/고객 힌트 처리
            InitAlreadyRevealedCustomerHint();
            ActivateAllItemHintButton();
        }
        else // 판매 세팅
        {
            SetSellPanel(posKey);
        }
    }

    public void OnGradeLevelToggleClicked()
    {
        // 현재 토글 상태 변경하기
        if(gradeTogGroup.transform.GetChild(0).GetComponent<Toggle>().isOn == true)
        { // rare
            gradeActionButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "20G";
            currentGradeActionType = GradeActionType.RARE;
        }
        else if(gradeTogGroup.transform.GetChild(1).GetComponent<Toggle>().isOn == true)
        { // unique
            gradeActionButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "30G";            
            currentGradeActionType = GradeActionType.UNIQUE;
        }
        else
        { // legendary
            gradeActionButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "50G";
            currentGradeActionType = GradeActionType.LEGENDARY;
        }
    }

    public void OnFlawLevelToggleClicked()
    {
        // TODO: 현재 토글 상태 변경하기
        if(flawTogGroup.transform.GetChild(0).GetComponent<Toggle>().isOn == true)
        { // 하급
            flawActionButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "20G";
            currnetFlawActionType = FlawActionType.LOW;
        }
        else if(flawTogGroup.transform.GetChild(1).GetComponent<Toggle>().isOn == true)
        { // 중급
            flawActionButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "60G";            
            currnetFlawActionType = FlawActionType.MIDDLE;
        }
        else
        { // 고급
            flawActionButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "100G";
            currnetFlawActionType = FlawActionType.HIGH;
        }
    }

    public void OnAppraiseGrade()
    {
      // 요청하고 결과 받기
        DealActionRequest requestData = new DealActionRequest();
        requestData.drcKey = dailyDealsMap[currentDealIndex].drcKey;
        requestData.actionType = "APPRAISE";
        switch (currentGradeActionType)
        {
            case GradeActionType.RARE:
                requestData.actionLevel = 1;
                break;
            case GradeActionType.UNIQUE:
                requestData.actionLevel = 2;
                break;
            case GradeActionType.LEGENDARY:
                requestData.actionLevel = 3;
                break;
        }
        // 실제 데이터 요청
        // DealActionResponse responseData =TransmissionManager.Instance.RequestToServer<DealActionRequest,DealActionResponse>(RequestType.DEAL_ACTION,requestData);
        
        // 테스트용 데이터 <<<<<<<<<<<<<<<<<
        TextAsset jsonFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Mocks/11dealAction_grade.json", typeof(TextAsset));
        DealActionResponse responseData =JsonUtility.FromJson<DealActionResponse>(jsonFile.text);

        /* 업데이트 */
        // 총 구매가 & 감정가
        UpdateTotalPrice(responseData.totalPurchasePrice, responseData.totalAppraisedPrice);
        // 구매가 변동값
        currentGradeObjs.transform.GetChild(3).GetChild(1).GetComponent<TMP_Text>().text 
                =$"{string.Format("{0:#,0}",responseData.changedPurchasedPriceByAction)} G";
        // 감정가 변동값
        currentGradeObjs.transform.GetChild(4).GetChild(1).GetComponent<TMP_Text>().text 
                =$"{string.Format("{0:#,0}",responseData.changedAppraisedPriceByAction)} G";
        // 찾은 등급값 표시
        currentGradeObjs.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text 
                = ConvertGradeToString((Grade)responseData.foundGrade);
        // 남은 돈 표시
        gameSessionManager.SetLeftMoney(responseData.leftMoney); 
    }

    public void OnFindFlaw()
    {
        // 요청하고 결과 받기
        DealActionRequest requestData = new DealActionRequest();
        requestData.drcKey = dailyDealsMap[currentDealIndex].drcKey;
        requestData.actionType = "FINDFLAW";
        switch (currnetFlawActionType)
        {
            case FlawActionType.LOW:
                requestData.actionLevel = 1;
                break;
            case FlawActionType.MIDDLE:
                requestData.actionLevel = 2;
                break;
            case FlawActionType.HIGH:
                requestData.actionLevel = 3;
                break;
        }
        // 실제 데이터 요청
        // DealActionResponse responseData =TransmissionManager.Instance.RequestToServer<DealActionRequest,DealActionResponse>(RequestType.DEAL_ACTION,requestData);
        
        // 테스트용 데이터 <<<<<<<<<<<<<<<<<
        TextAsset jsonFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Mocks/11dealAction_flaw.json", typeof(TextAsset));
        DealActionResponse responseData =JsonUtility.FromJson<DealActionResponse>(jsonFile.text);

        /* 업데이트 */
        // 총 구매가 & 감정가
        UpdateTotalPrice(responseData.totalPurchasePrice, responseData.totalAppraisedPrice);
        // 구매가 변동값
        currentFlawObjs.transform.GetChild(3).GetChild(1).GetComponent<TMP_Text>().text 
                =$"{string.Format("{0:#,0}",responseData.changedPurchasedPriceByAction)} G";
        // 감정가 변동값
        currentFlawObjs.transform.GetChild(4).GetChild(1).GetComponent<TMP_Text>().text 
                =$"{string.Format("{0:#,0}",responseData.changedAppraisedPriceByAction)} G";
        // 찾은 등급값 표시
        currentFlawObjs.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text 
                = $"{responseData.foundFlawEa}개";
        // 남은 돈 표시
        gameSessionManager.SetLeftMoney(responseData.leftMoney); 
    }

    public void OnFindAuth()
    {
        // 요청하고 결과 받기
        DealActionRequest requestData = new DealActionRequest();
        requestData.drcKey = dailyDealsMap[currentDealIndex].drcKey;
        requestData.actionType = "AUTHCHECK";
        requestData.actionLevel = 0;

        // 실제 데이터 요청
        // DealActionResponse responseData =TransmissionManager.Instance.RequestToServer<DealActionRequest,DealActionResponse>(RequestType.DEAL_ACTION,requestData);
        
        // 테스트용 데이터 <<<<<<<<<<<<<<<<<
        TextAsset jsonFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Mocks/11dealAction_auth.json", typeof(TextAsset));
        DealActionResponse responseData =JsonUtility.FromJson<DealActionResponse>(jsonFile.text);

        /* 업데이트 */
        // 총 구매가 & 감정가
        UpdateTotalPrice(responseData.totalPurchasePrice, responseData.totalAppraisedPrice);
        // 구매가 변동값
        currentAuthObjs.transform.GetChild(3).GetChild(1).GetComponent<TMP_Text>().text 
                =$"{string.Format("{0:#,0}",responseData.changedPurchasedPriceByAction)} G";
        // 감정가 변동값
        currentAuthObjs.transform.GetChild(4).GetChild(1).GetComponent<TMP_Text>().text 
                =$"{string.Format("{0:#,0}",responseData.changedAppraisedPriceByAction)} G";
        // 찾은 등급값 표시
        currentAuthObjs.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text 
                = ConvertAuthToString((Authenticity)responseData.foundAuthenticity);
        // 남은 돈 표시
        gameSessionManager.SetLeftMoney(responseData.leftMoney); 
    }

    public void OpenItemHint(int posKey)
    {
        // TODO: 아이템 힌트 열람 서버에 요청
        // 받아서 처리 -> 텍스트에 담기
        // 아이템 힌트 버튼 SetActive(false) 때리기
        currentItemHintObjs.transform.GetChild(posKey).GetChild(2).gameObject.SetActive(false);
    }

    public void OpenCustomerHint(int posKey)
    {
        // TODO: 아이템 힌트 열람 서버에 요청
        // 받아서 처리 -> 텍스트에 담기
        // 아이템 힌트 버튼 SetActive(false) 때리기
        currentCusHintObjs.transform.GetChild(3+posKey).GetChild(2).gameObject.SetActive(false);
    }

    private void ActivateAllItemHintButton()
    {
        currentItemHintObjs.transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
        currentItemHintObjs.transform.GetChild(2).GetChild(2).gameObject.SetActive(true);
        currentItemHintObjs.transform.GetChild(3).GetChild(2).gameObject.SetActive(true);
        currentItemHintObjs.transform.GetChild(4).GetChild(2).gameObject.SetActive(true);
        currentItemHintObjs.transform.GetChild(5).GetChild(2).gameObject.SetActive(true);
        currentItemHintObjs.transform.GetChild(6).GetChild(2).gameObject.SetActive(true);
    }

    private void InitAlreadyRevealedCustomerHint()
    {
        currentCusHintObjs.transform.GetChild(3).GetChild(2).gameObject.SetActive(true);
        currentCusHintObjs.transform.GetChild(4).GetChild(2).gameObject.SetActive(true);
        currentCusHintObjs.transform.GetChild(5).GetChild(2).gameObject.SetActive(true);
        
        if (currentDealData.revealedFraud !=-1) // 사기꾼율: 이미 열람되었다면,
        {
            currentCusHintObjs.transform.GetChild(3).GetChild(1).GetComponent<TMP_Text>().text
                = $"{currentDealData.revealedFraud}";
            currentCusHintObjs.transform.GetChild(3).GetChild(2).gameObject.SetActive(false);
        }
        if (currentDealData.revealedWellCollect !=-1) // 잘수집: 이미 열람되었다면,
        {
            currentCusHintObjs.transform.GetChild(4).GetChild(1).GetComponent<TMP_Text>().text
                = $"{currentDealData.revealedWellCollect}";
            currentCusHintObjs.transform.GetChild(4).GetChild(2).gameObject.SetActive(false);
        }
        if (currentDealData.revealedClumsy !=-1) // 서투름: 이미 열람되었다면,
        {
            currentCusHintObjs.transform.GetChild(5).GetChild(1).GetComponent<TMP_Text>().text
                = $"{currentDealData.revealedClumsy}";
            currentCusHintObjs.transform.GetChild(5).GetChild(2).gameObject.SetActive(false);            
        }
    }

    private void UpdateTotalPrice(int purPrice, int appPrice, int askPrice=-1)
    {
        if(askPrice !=-1){
            currentTotalPriceObjs.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text 
                =$"{string.Format("{0:#,0}",askPrice)} G";
        }
        currentTotalPriceObjs.transform.GetChild(1).GetChild(1).GetComponent<TMP_Text>().text
                =$"{string.Format("{0:#,0}",purPrice)} G";
        currentTotalPriceObjs.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text
                =$"{string.Format("{0:#,0}",appPrice)} G";
    }

    private void UpdateChangedFlawPrice(int flawEA)
    {
        flawChangedPurchasePrice = -1 * (int)(flawEA * (currentDealData.askingPrice*0.05f)); // 흠개수 *5% 하락
        flawChangedAppraisedPrice = -1 * (int)(flawEA * (currentDealData.askingPrice*0.05f)); // 흠개수 *5% 하락        
        
        currentFlawObjs.transform.GetChild(3).GetChild(1).GetComponent<TMP_Text>().text
                =$"{string.Format("{0:#,0}",flawChangedPurchasePrice)} G";
        currentFlawObjs.transform.GetChild(4).GetChild(1).GetComponent<TMP_Text>().text
                =$"{string.Format("{0:#,0}",flawChangedAppraisedPrice)} G";
        currentFlawObjs.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text
                = $"{string.Format("{0:#,0}",flawEA)} 개";
    }
    private void UpdateChangedAuthPrice(Authenticity auth)
        {
            string strFeed="";
            switch (auth)
            {
                case Authenticity.Real:
                    authChangedPurchasePrice = 0; // 변동사항 없음
                    authChangedAppraisedPrice = 0; // 변동사항 없음
                    strFeed = "진품";
                    break;
                case Authenticity.Fake:
                    authChangedPurchasePrice = -1 * (int)(currentDealData.askingPrice*0.5f); // 구매가 50% 하락
                    authChangedAppraisedPrice = -1 * (int)(currentDealData.askingPrice*0.2f); // 감정가 20% 하락
                    strFeed = "가품";
                    break;
                case Authenticity.Unknown:
                    authChangedPurchasePrice = 0; // 변동사항 없음
                    authChangedAppraisedPrice = 0; // 변동사항 없음
                    strFeed = "미판정";
                    break;
            }
        
        currentAuthObjs.transform.GetChild(3).GetChild(1).GetComponent<TMP_Text>().text
                =$"{string.Format("{0:#,0}",authChangedPurchasePrice)} G";
        currentAuthObjs.transform.GetChild(4).GetChild(1).GetComponent<TMP_Text>().text
                =$"{string.Format("{0:#,0}",authChangedAppraisedPrice)} G";
        currentAuthObjs.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text
                = $"{strFeed}";
        }
    private void UpdateChangedGradePrice(Grade grade)
    {
        string strFeed="";
        switch (grade)
        {
            case Grade.Common:
                strFeed= "일반";
                gradeChangedPurchasePrice = 0; // 변동사항 없음
                gradeChangedAppraisedPrice = 0; // 변동사항 없음
                break;
            case Grade.Rare:
                strFeed= "레어";
                gradeChangedPurchasePrice = 0; // 변동사항 없음
                gradeChangedAppraisedPrice = (int)(currentDealData.askingPrice *1.2f); // 1.2배
                break;
            case Grade.Unique:
                strFeed= "유니크";
                gradeChangedPurchasePrice = 0; // 변동사항 없음
                gradeChangedAppraisedPrice = (int)(currentDealData.askingPrice *1.5f); // 1.5배
                break;
            case Grade.Legendary:
                strFeed= "레전더리";
                gradeChangedPurchasePrice = 0; // 변동사항 없음
                gradeChangedAppraisedPrice = (int)(currentDealData.askingPrice *1.7f); // 1.7배
                break;
        }
    
        currentGradeObjs.transform.GetChild(3).GetChild(1).GetComponent<TMP_Text>().text
                =$"{string.Format("{0:#,0}",gradeChangedPurchasePrice)} G";
        currentGradeObjs.transform.GetChild(4).GetChild(1).GetComponent<TMP_Text>().text
                =$"{string.Format("{0:#,0}",gradeChangedAppraisedPrice)} G";
        currentGradeObjs.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text = strFeed;
    }

    /* SELL */
    private int DecideNextIsDeal()
    {
        List<int> MatchedDisplayPositions= new List<int>(8);
        bool isSellPossible= false;
        // TODO: 싱글톤 고객 상태를 구매/판매 상태로 변경하기
        CustomerCatalogData cData = SingletonManager.Instance.GetCustomerCatalog(currentDealData.customerKey);
        for(int i = 0; i < 8; i++)
        {
            if (displaysMap.ContainsKey(i))
            {
                ItemCatalogData iData=SingletonManager.Instance.GetItemCatalog(displaysMap[i].itemCatalogKey);
                if(cData.favoriteCategoryName==iData.categoryName)
                {// 선호 카테고리 == 카테고리
                    isSellPossible = true;
                    MatchedDisplayPositions.Add(i);
                }
            }
        }

        int rand = Random.Range(0,MatchedDisplayPositions.Count-1);
        if(isSellPossible == true) // 판매 개시
        {
            SingletonManager.Instance.IsCustomerDealState = CustomerState.Sell;
            return MatchedDisplayPositions[rand]; //false는 판매
        }
        else // 거래 개시
        {
            SingletonManager.Instance.IsCustomerDealState = CustomerState.Deal;
            return -1; // -1는 거래. 
        }
    }

    public void SetSellPanel(int posKey)
    {
        // TODO: 요청 데이터 세팅
        currentSellItem.itemKey = displaysMap[posKey].itemKey;
        currentSellItem.customerKey = currentDealData.customerKey;
        
        // 요청하고 결과값 받기 -> 서버 있어야 받을 수 있음
        // SellStartResponse responseData =TransmissionManager.Instance.RequestToServer<SellStartRequest,SellStartResponse>(RequestType.ITEM_SELL_START,requestData);

        // 테스트용 데이터 사용
        TextAsset jsonFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Mocks/16itemSellStart.json", typeof(TextAsset));
        SellStartResponse sellStartResponseData =JsonUtility.FromJson<SellStartResponse>(jsonFile.text);
        //이미지 세팅
        ItemCatalogData iData=SingletonManager.Instance.GetItemCatalog(displaysMap[posKey].itemCatalogKey);
        sellObjs.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().sprite
            = Resources.Load<Sprite>($"IMG_ITEM_CATALOG/{iData.imgId}");
        // 텍스트 세팅
        sellObjs.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text =$"{sellStartResponseData.sellingPrice}";  
    }


    public void DecideToSellItem()
    {
        SellCompleteRequest requestData=null;
        requestData.itemKey = currentSellItem.itemKey;
        requestData.customerKey = currentSellItem.customerKey;
        // TODO: 서버에 전달하기 sellComplete
        // SellCompleteResponse responseData =TransmissionManager.Instance.RequestToServer<SellCompleteRequest,SellCompleteResponse>(RequestType.ITEM_SELL_COMPLETE,requestData);

        // 테스트용 데이터 사용
        TextAsset jsonFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Mocks/17sellComplete.json", typeof(TextAsset));
        SellCompleteResponse sellCompleteResponseData =JsonUtility.FromJson<SellCompleteResponse>(jsonFile.text);

//         {
//   "earnedAmount": "+2400",
//   "leftMoney": 55000,
//   "displayedPositionKey": 3
// }
        // TODO: 디스플레이 아이템에서 삭제
        // TODO: 돈 업데이트
        // TODO: 팝업창 띄우기

        // 판매 오브젝트 끄기
        deseBlackFilter.SetActive(false);
        sellObjs.SetActive(false);
        // 
    }

    public void DenyToSellItem()
    {
        // TODO: 서버에 전달하기 Deny sell
        // TODO: 고객 상태 -> 거래 상태로 바꾸기
        SingletonManager.Instance.IsCustomerDealState = CustomerState.Deal;
        // TODO: 거래 패널 세팅하기
        SetDealPanelToNewData(dailyDealsMap[currentDealIndex]);
        // 판매 오브젝트 끄기
        deseBlackFilter.SetActive(false);
        sellObjs.SetActive(false);
        //
    }




    // TODO: 서버에 요청 주는 파트들
    // 1. 아이템 힌트 열람 -> 서버 -> json 형식으로 받아서 텍스트에 내용 다 채운 다음에 버튼 지우기
    // 2. 흠 찾기 -> 서버 요청 주고 결과를 Update흠찾기, Update 토탈 로 처리하면 될 듯
    // 3. 진위 여부 -> 서버 요청 주고 결과를 Update진위여부, Update 토탈 로 처리하면 될 듯
    // 4. 감정 -> 서버 요청 주고 결과를 Update등급, Update 토탈 로 처리하면 될 듯
    // 5. 거래 성공 -> 서버에 전달하고 다음 고객, 아이템, 거래패널로 업데이트하기
    // 6. 거래 실패 -> 서버에 전달하고 다음 고객, 아이템, 거래패널로 업데이트하기


    private string ConvertAuthToString(Authenticity authenticity)
    {
        string strFeed = "";
        switch (authenticity)
        {
            case Authenticity.Real:
                strFeed= "진품";
                break;
            case Authenticity.Fake:
                strFeed= "가품";
                break;
            case Authenticity.Unknown:
                strFeed= "모름";
                break;
        }
        return strFeed;
    }
    private string ConvertGradeToString(Grade grade)
    {
        string strFeed = "";
        switch (grade)
        {
            case Grade.Common:
                strFeed= "일반";
                break;
            case Grade.Rare:
                strFeed= "레어";
                break;
            case Grade.Unique:
                strFeed= "유니크";
                break;
            case Grade.Legendary:
                strFeed= "레전더리";
                break;
        }
        return strFeed;
    }


    private void DecideDeal()
    {
        // TODO: 서버에 전달하기
        // 딜 오브젝트 끄기
        deseBlackFilter.SetActive(false);
        dealObjs.SetActive(false);
        // 
        currentDealIndex++; // 다음 거래로 이동
        if(currentDealIndex > 2)
        {
            SetNextDaySetting();
        }
        else {
            SetDealPanelToNewData(dailyDealsMap[currentDealIndex]);
        }
    }

    private void DenyDeal()
    {
        // TODO: 서버에 전달하기
        // 딜 오브젝트 끄기
        deseBlackFilter.SetActive(false);
        dealObjs.SetActive(false);
        
        currentDealIndex++; // 다음 거래로 이동
        if(currentDealIndex > 2)
        { // 3개 거래 완료 했으면,
            // TODO: 팝업창 띄우고 그냥 확인 누르면 정산 시켜
            SetNextDaySetting();
        }
        else {
            SetDealPanelToNewData(dailyDealsMap[currentDealIndex]);
        }
    }
    
    // 정산 창 띄우기
    private void StartSettleMoney()
    {
        // TODO: 정산 창 띄우기
        // 정산 창에는 
    }

    private void StartNextDay()
    {
        
    }

    private void SetNextDaySetting()
    {
        // TODO: 다음 날로 넘어가는 확인 팝업창 띄우기
        string text ="오늘 모든 거래를 마쳤습니다.\n 다음 날로 넘어가기";
        ConfirmPopuper.Instance.PopupCheckPanel(text,StartSettleMoney);
        // TODO: 고객 비활성화
        customerObj.SetActive(false);
        // TODO: 아이템 비활성화
        trayItemObj.SetActive(false);
    }

    public void PopupItemInform()
    {
        trayItemObj.transform.GetChild(2).gameObject.SetActive(true);
    }

    public void PopoffItemInform()
    {
        trayItemObj.transform.GetChild(2).gameObject.SetActive(false);        
    }
    public void PopupCustomerInform()
    {
        customerInformPanel.SetActive(true);
    }

    public void PopoffCustomerInform()
    {
        customerInformPanel.SetActive(false);        
    }

}
}