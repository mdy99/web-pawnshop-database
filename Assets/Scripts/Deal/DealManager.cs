using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

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

    [SerializeField] private ItemDisplayManager itemDisplayManager;


    [SerializeField] private List<DealData> dailyDealsList = new List<DealData>();

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
    private SellCompleteRequest currentSellItem=new SellCompleteRequest();
    
    private int currentDealIndex =-1;
    
    [SerializeField] private GameObject gradeTogGroup;
    [SerializeField] private GameObject flawTogGroup;
    [SerializeField] private GameObject gradeActionButton;
    [SerializeField] private GameObject flawActionButton;

    private GradeActionType currentGradeActionType = GradeActionType.RARE;
    private FlawActionType currnetFlawActionType = FlawActionType.LOW;

    [SerializeField] private GameSessionManager gameSessionManager;
    [SerializeField] private GameObject FinalizeObjs;

    private DealCompleteResponse dealDecideActionResponseData = null;
    [SerializeField] private TvScriptShower tvScriptShower;
    [SerializeField] private GameObject itemResultObjs;

    private List<ActionItemData> actionItemDatas;
    private int actionItemIndex = -1;
    private ItemActionResultResponse currentItemActionResponseData;

    string[] dealSuccessDialogues = new string[]
    {"<고객> 썩 맘에 드는 가격은\n아니지만, 알겠어요",
        "<고객> 좋은 거래\n감사합니다 *^^*",
        "<고객> 거래 감사합니다.",
        "<고객> 이걸 이 가격까지\n후릴 줄이야..",
        "<고객> 다음에 쿨거하면\n좀 더 쳐주시나요?",
        };
    string[] denyDealDialogueList = new string[]
    {
        "거래를 아주 매몰차게 거부하였습니다.",
        "당신은 고객을 벌레 보듯이 쳐다보며 내쫓았습니다.",
        "당신은 고객이 가져온 물건을 가게 바깥에 내동댕이치며 나가라고 소리쳤습니다.",
        "거래를 정상적으로 거부하였습니다."
    };
    string[] denyDealDialogues = new string[]
    {
        "<고객> 에라이 나도\n여기서 안 팔아~",
        "<고객> 알겠습니다..\n아쉽네요",
        "<고객> 다음에는 좋은 거래가\n있길 바랍니다"
    };

    string[] sellCompleteDialogues = new string[]
    {
        "<고객> 좋은 거래 감사합니다 *^^*",
        "<고객> 이 가격이면\n합당하네요~",
        "<고객> 경매로 팔면 더 받으실텐데\n이 가격이면 너무 좋네요~"
    };

    private bool isFlawLevelTogClicked = false;
    private bool isGradeTogClicked = false;

    void Start()
    {
        // 전시장 아이템 불러오기
        displaysMap = itemDisplayManager.GetItemDisplayMap();
        StartToday();
    }

    private void StartToday()
    {
        DialogueManager.Instance.PutDialogue("<속마음> 날이 시작된다..");
        // 오늘의 뉴스 이벤트 불러오기
        RequestDailyNews();
        // 오늘 복원/경매 완료 된 것들 불러오기
        RequestDailyCompletedItemAction();
        // 오늘 거래 불러오기
        RequestDailyDeals();
    }

    // 경매/복원 완료된 것들 가져오기
    private void RequestDailyCompletedItemAction()
    {
        /* 데이터 가져오기 */ 
        // 실제 데이터 받는 코드        
        // ItemActionResultResponse responseData =TransmissionManager.Instance.RequestToServer<int,ItemActionResultResponse>(RequestType.ITEM_RESULT,0);
        // 테스트용 코드 <<<<<<<<<<<<<<<<<<
        TextAsset jsonFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Mocks/15itemResult.json", typeof(TextAsset));
        ItemActionResultResponse responseData =JsonUtility.FromJson<ItemActionResultResponse>(jsonFile.text);
        /* 복원/경매 완료 된 것들 보여주기 하나씩 하나씩 */

        // 경매/복원 완료 된 거 없으면 종료
        if(responseData.actionResults == null)
        {
            return;
        }
        // 전역 actionDataList 세팅
        actionItemDatas = responseData.actionResults;
        // 전역 인덱스 =0
        actionItemIndex = 0; // 인덱스 시작
        // 현재 경매/복원 완료 요청 데이터 답변 데이터 전역 세팅
        currentItemActionResponseData = responseData;
        // 아이템 결과 패널 데이터 세팅하기
        UpdateItemResultData();
        // 아이템 결과 패널 띄우기
        itemResultObjs.SetActive(true);
        DialogueManager.Instance.PutDialogue("<속마음> 벌써 처리가 되었군..");
    }

    private void UpdateItemResultData()
    {
        ItemCatalogData iData = SingletonManager.Instance.GetItemCatalog(actionItemDatas[actionItemIndex].itemCatalogKey);
        // 아이템 이미지
        itemResultObjs.transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite
         = Resources.Load<Sprite>($"IMG_ITEM_CATALOG/{iData.imgId}");
        // 아이템 이름
        itemResultObjs.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text
        = iData.itemCatalogName;
        // 아이템 상태
        itemResultObjs.transform.GetChild(2).GetChild(2).GetComponent<TMP_Text>().text
        = ConvertItemState((ItemState)actionItemDatas[actionItemIndex].itemState);
        // 경매낙찰금/복원비용
        itemResultObjs.transform.GetChild(2).GetChild(3).GetComponent<TMP_Text>().text
        = ((ItemState)actionItemDatas[actionItemIndex].itemState == ItemState.AfterRestoration)
            ?"복원 비용":"경매 낙찰금";
        // 얼마 gold
        itemResultObjs.transform.GetChild(2).GetChild(4).GetComponent<TMP_Text>().text
        = $"{string.Format("{0:#,0}",actionItemDatas[actionItemIndex].deltaMoney)}G";        
        // 최종 감정가 얼마 gold
        itemResultObjs.transform.GetChild(2).GetChild(5).GetComponent<TMP_Text>().text
        = $"{string.Format("{0:#,0}",actionItemDatas[actionItemIndex].appraisedPrice)}G";
    }

    public void OnItemResultConfirmButtonClicked()
    {
        // 해당 아이템이 복원 완료인 경우
        if((ItemState)actionItemDatas[actionItemIndex].itemState == ItemState.AfterRestoration)
        {
            // 해당 전시장 아이템 상태 업데이트하기
            itemDisplayManager.UpdateDisplayedItem(actionItemDatas[actionItemIndex]);
        }
        // 해당 아이템이 경매 완료인 경우(판매됨)
        else if((ItemState)actionItemDatas[actionItemIndex].itemState == ItemState.Sold)
        {
            // 전시장에서 삭제하기
            itemDisplayManager.RemoveDisplayedItem(actionItemDatas[actionItemIndex].displayedPositionKey);
        }
        // 게임세션.돈에서 차감/더하기
        gameSessionManager.
            SetLeftMoney(gameSessionManager.GetLeftMoney()
                        +actionItemDatas[actionItemIndex].deltaMoney); // 변화량만큼 보이는 거 임시 업데이트. 나중에 한번에 최종 돈으로 업데이트할 거야 
        // 인덱스 증가
        actionItemIndex += 1;
        // 모든 경매/복원 완료 아이템 다 처리함?
        if(actionItemDatas.Count <= actionItemIndex) // 실제 경매/복원 완료된 아이템 인덱스를 넘으면 안 됨
        { // 모든 경매/복원 완료 아이템 다 처리함.
            // 없으면, 아이템 결과 패널 끄기
            itemResultObjs.SetActive(false);
            // 최종 돈 업데이트하기
            gameSessionManager.SetLeftMoney(currentItemActionResponseData.leftMoney);
            // 만약 모든 경매/복원 완료 아이템 다 처리하고 게임 오버 됐으면,
            if(currentItemActionResponseData.isGameOvered == "Y")
            {
                // 게임오버 창 띄우기
                gameSessionManager.PopupGameEndObjs(currentItemActionResponseData.worldRecord,
                                    "복원 비용을 못 낸다고? 넌 안 되겠다ㅋㅋ");
                DialogueManager.Instance.PutDialogue("<속마음> 아뿔싸 돈을 남겨두는\n습관을 가질 걸..");
            }
        }
        else
        { // 다음 경매/복원 완료 아이템 세팅
            UpdateItemResultData();
            DialogueManager.Instance.PutDialogue("<속마음> 벌써 처리가 되었군..");
        }
   }

    IEnumerator PutNewsDialogue(NewsWrapData responseData)
    {
        for(int i = 0; i < responseData.newsList.Count; ++i)
        {
            DialogueManager.Instance.PutDialogue
            ($"<뉴스> {responseData.newsList[i].affectedCategoryName}의 {SingletonManager.Instance.ConvertToAffectedPrice((AffectedPrice)responseData.newsList[i].affectedPrice)}가 {responseData.newsList[i].amount}%만큼 영향을 받겠습니다.\n");
            yield return new WaitForSeconds(2.0f);
        }
        
    }

    // 오늘의 뉴스 이벤트 불러오기
    private void RequestDailyNews()
    {
        // 실제 데이터 받는 코드        
        // NewsWrapData responseData =TransmissionManager.Instance.RequestToServer<int,NewsWrapData>(RequestType.NEWS_CUR,0);
        // 테스트용 코드 <<<<<<<<<<<<<<<<<<
        TextAsset jsonFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Mocks/7newsCurrent.json", typeof(TextAsset));
        NewsWrapData responseData =JsonUtility.FromJson<NewsWrapData>(jsonFile.text);
        StartCoroutine(PutNewsDialogue(responseData));
        // tv에 표시하기
        tvScriptShower.SetTvText(responseData.newsList);
    }

    private void RequestDailyDeals()
    {
        // 실제 데이터 받는 코드        
        // DailyDealsWrapData responseData =TransmissionManager.Instance.RequestToServer<int,DailyDealsWrapData>(RequestType.DAILY_DEALS,0);
        // 테스트용 코드 <<<<<<<<<<<<<<<<<<
        TextAsset jsonFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Mocks/10generateDailyDeals.json", typeof(TextAsset));
        DailyDealsWrapData dailyDealsWrapData =JsonUtility.FromJson<DailyDealsWrapData>(jsonFile.text);
        // 받은 데이터로 오늘 데이터 세팅
        InitDailyDeals(dailyDealsWrapData.dailyDeals);
    }

    public void InitDailyDeals(List<DealData> dailyDeals)
    {
        for(int i = 0; i < dailyDeals.Count; i++)
        {
            dailyDealsList.Add(dailyDeals[i]);
        }
        currentDealIndex = 0;
        SetDealPanelToNewData(dailyDealsList[currentDealIndex]); // 최초 거래로 설정해둠
    }

    public void SetDealPanelToNewData(DealData dData, bool isOverToSell= false)
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
        if (DecideNextIsDeal()== -1 || isOverToSell == true) // 거래 세팅. 판매하고 왔으면, 거래로 진행해
        // int posKey =1; // 판매 확인용 코드
        //if(false)
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
            // 다이얼로그
            DialogueManager.Instance.PutDialogue("<고객> 좋은 거래 물품을\n가져왔어요 *^^*");
        }
        else // 판매 세팅
        {
            SetSellPanel();
        }
    }

    public void OnGradeLevelToggleClicked()
    {
        if (isGradeTogClicked)
        {
            isGradeTogClicked = false;
            // 현재 토글 상태 변경하기
            if(gradeTogGroup.transform.GetChild(0).GetComponent<Toggle>().isOn == true)
            { // rare
                gradeActionButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "20G";
                currentGradeActionType = GradeActionType.RARE;
                DialogueManager.Instance.PutDialogue("<속마음> 레어 등급까지도 찾을 수 있겠어..\n실제 등급이 그정도라면..");
            }
            else if(gradeTogGroup.transform.GetChild(1).GetComponent<Toggle>().isOn == true)
            { // unique
                gradeActionButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "30G";            
                currentGradeActionType = GradeActionType.UNIQUE;
                DialogueManager.Instance.PutDialogue("<속마음> 유니크 등급까지도 찾을 수 있겠어..\n실제 등급이 그정도라면..");
            }
            else
            { // legendary
                gradeActionButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "50G";
                currentGradeActionType = GradeActionType.LEGENDARY;
                DialogueManager.Instance.PutDialogue("<속마음> 레전더리까지도 찾을 수 있겠어..\n실제 등급이 그정도라면..");
            }            
        }
        else
        {
            isGradeTogClicked = true;
        }
    }

    public void OnFlawLevelToggleClicked()
    {
        if (isFlawLevelTogClicked)
        {
            isFlawLevelTogClicked = false;
            // TODO: 현재 토글 상태 변경하기
            if(flawTogGroup.transform.GetChild(0).GetComponent<Toggle>().isOn == true)
            { // 하급
                flawActionButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "20G";
                currnetFlawActionType = FlawActionType.LOW;
                DialogueManager.Instance.PutDialogue("<속마음> 1개 정도는 찾을 수 있겠어..\n실제 흠 개수가 그정도 있다면..");
            }
            else if(flawTogGroup.transform.GetChild(1).GetComponent<Toggle>().isOn == true)
            { // 중급
                flawActionButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "60G";            
                currnetFlawActionType = FlawActionType.MIDDLE;
                DialogueManager.Instance.PutDialogue("<속마음> 4개 정도는 찾을 수 있겠어..\n실제 흠 개수가 그정도 있다면..");
            }
            else
            { // 고급
                flawActionButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "100G";
                currnetFlawActionType = FlawActionType.HIGH;
                DialogueManager.Instance.PutDialogue("<속마음> 7개 정도는 찾을 수 있겠어..\n실제 흠 개수가 그정도 있다면..");
            }            
        }
        else
        {
            isFlawLevelTogClicked = true;
        }
    }

    public void OnAppraiseGrade()
    {
      // 요청하고 결과 받기
        DealActionRequest requestData = new DealActionRequest();
        requestData.drcKey = dailyDealsList[currentDealIndex].drcKey;
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
        // 다이얼로그
        DialogueManager.Instance.
            PutDialogue($"<속마음> 등급 감정으로 {ConvertGradeToString((Grade)responseData.foundGrade)}라는 것을 알아냈다.\n좀 더 높을 수도 있지 않을까..?");
    }

    public void OnFindFlaw()
    {
        // 요청하고 결과 받기
        DealActionRequest requestData = new DealActionRequest();
        requestData.drcKey = dailyDealsList[currentDealIndex].drcKey;
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
        // 다이얼로그
        DialogueManager.Instance.
            PutDialogue($"<속마음> 흠 찾기로 {responseData.foundFlawEa}개를 찾아냈다.\n좀 더 찾을 수도 있지 않을까?");
    }

    public void OnFindAuth()
    {
        // 요청하고 결과 받기
        DealActionRequest requestData = new DealActionRequest();
        requestData.drcKey = dailyDealsList[currentDealIndex].drcKey;
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
        // 다이얼로그
        string message;
        if(responseData.foundAuthenticity == 0) // 가품
        {
            message = $"<속마음> 등급 감정으로 {ConvertAuthToString((Authenticity)responseData.foundAuthenticity)}라는 것을 알아냈다.\n 복원할 때 알아냈으면 엄청난 손해를 볼 뻔 했어..";
        }
        else // 진품
        {
            message = $"<속마음> 등급 감정으로 {ConvertAuthToString((Authenticity)responseData.foundAuthenticity)}라는 것을 알아냈다.\n";            
        }
        // 다이얼로그
        DialogueManager.Instance.PutDialogue(message);
    }

    public void OpenItemHint(int posKey)
    {
        // 아이템 힌트 요청 데이터 세팅
        ItemHintRequest requestData = new ItemHintRequest();
        requestData.itemKey = currentDealData.itemKey;

        // 실제 데이터 요청
        //ItemHintResponse responseData =TransmissionManager.Instance.RequestToServer<ItemHintRequest,ItemHintResponse>(RequestType.GET_ITEM_HINTS,requestData);
        
        // 테스트용 데이터 <<<<<<<<<<<<<<<<<
        TextAsset jsonFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Mocks/9itemHind.json", typeof(TextAsset));
        ItemHintResponse responseData =JsonUtility.FromJson<ItemHintResponse>(jsonFile.text);

        /* 받아서 처리 -> 텍스트에 담기 */
        // 힌트 이름
        currentItemHintObjs.transform.GetChild(posKey).GetChild(0).GetComponent<TMP_Text>().text = responseData.hintName;
        // 힌트 값
        currentItemHintObjs.transform.GetChild(posKey).GetChild(1).GetComponent<TMP_Text>().text = responseData.hintValue;
        // 남은 돈 표시
        gameSessionManager.SetLeftMoney(responseData.leftMoney); 

        // 다이얼로그
        DialogueManager.Instance.PutDialogue($"<속마음>{responseData.hintName}는 {responseData.hintValue}정도군.. ");
        // 아이템 힌트 버튼 SetActive(false) 때리기
        currentItemHintObjs.transform.GetChild(posKey).GetChild(2).gameObject.SetActive(false);
    }

    public void OpenCustomerHint(int posKey)
    {
        // 아이템 힌트 요청 데이터 세팅
        RevealCustomerRequest requestData = new RevealCustomerRequest();
        requestData.customerKey = currentDealData.customerKey;
        switch (posKey)
        {
            case 0:
                requestData.attribute = "FRAUD";
                break;
            case 1:
                requestData.attribute = "WELL_COLLECT";
                break;
            case 2:
                requestData.attribute = "CLUMSY";
                break;
        }

        // 실제 데이터 요청
        // RevealCustomerResponse responseData =TransmissionManager.Instance.RequestToServer<RevealCustomerRequest,RevealCustomerResponse>(RequestType.CUS_REVEAL,requestData);
        
        // 테스트용 데이터 <<<<<<<<<<<<<<<<<
        TextAsset jsonFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Mocks/8revealCustomer.json", typeof(TextAsset));
        RevealCustomerResponse responseData =JsonUtility.FromJson<RevealCustomerResponse>(jsonFile.text);

        /* 받아서 처리 -> 텍스트에 담기 */
        // 힌트 이름
        currentCusHintObjs.transform.GetChild(3+posKey).GetChild(0).GetComponent<TMP_Text>().text
            = ConvertCustomerAttributeToString(responseData.attribute);
        // 힌트 값
        currentCusHintObjs.transform.GetChild(3+posKey).GetChild(1).GetComponent<TMP_Text>().text
            = $"{responseData.value*100}%";
        // 남은 돈 표시
        gameSessionManager.SetLeftMoney(responseData.leftMoney); 
        // 다이얼로그
        DialogueManager.Instance.PutDialogue($"<속마음>{ConvertCustomerAttributeToString(responseData.attribute)}는 {responseData.value*100}%정도군.. ");
        // 고객 힌트 버튼 SetActive(false) 때리기
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
            // 힌트 이름
            currentCusHintObjs.transform.GetChild(3).GetChild(0).GetComponent<TMP_Text>().text
                = ConvertCustomerAttributeToString("FRAUD");
            // 힌트 값
            currentCusHintObjs.transform.GetChild(3).GetChild(1).GetComponent<TMP_Text>().text
                = $"{currentDealData.revealedFraud*100}%";
            // 고객 힌트 버튼 SetActive(false) 때리기
            currentCusHintObjs.transform.GetChild(3).GetChild(2).gameObject.SetActive(false);        
        }
        if (currentDealData.revealedWellCollect !=-1) // 잘수집: 이미 열람되었다면,
        {
            // 힌트 이름
            currentCusHintObjs.transform.GetChild(4).GetChild(0).GetComponent<TMP_Text>().text
                = ConvertCustomerAttributeToString("WELL_COLLECT");
            // 힌트 값
            currentCusHintObjs.transform.GetChild(4).GetChild(1).GetComponent<TMP_Text>().text
                = $"{currentDealData.revealedWellCollect*100}%";
            currentCusHintObjs.transform.GetChild(4).GetChild(2).gameObject.SetActive(false);
        }
        if (currentDealData.revealedClumsy !=-1) // 서투름: 이미 열람되었다면,
        {
            // 힌트 이름
            currentCusHintObjs.transform.GetChild(5).GetChild(0).GetComponent<TMP_Text>().text
                = ConvertCustomerAttributeToString("CLUMSY");
            // 힌트 값
            currentCusHintObjs.transform.GetChild(5).GetChild(1).GetComponent<TMP_Text>().text
                = $"{currentDealData.revealedClumsy*100}%";
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
        // 싱글톤 고객 상태를 구매/판매 상태로 변경하기
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

    public void SetSellPanel()
    {
        // TODO: 요청 데이터 세팅
        SellStartRequest requestData = new SellStartRequest();
        requestData.customerKey = currentDealData.customerKey;
        
        // 요청하고 결과값 받기 -> 서버 있어야 받을 수 있음
        // SellStartResponse responseData =TransmissionManager.Instance.RequestToServer<SellStartRequest,SellStartResponse>(RequestType.ITEM_SELL_START,requestData);

        // 테스트용 데이터 사용 <<<<<<<<<<<<<<<
        TextAsset jsonFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Mocks/16itemSellStart.json", typeof(TextAsset));
        SellStartResponse responseData =JsonUtility.FromJson<SellStartResponse>(jsonFile.text);
        // 아이템 키를 받았을테니까 여기서 
        int posKey = -1;
        for(int i = 0; i < displaysMap.Count; ++i)
        {
            if(displaysMap[i].itemKey == responseData.itemKey)
                {
                    posKey = i;
                    break;
                }
        }
        if(posKey == -1)
        {
            ConfirmPopuper.Instance.PopupCheckPanel("해당 전시장 위치에 아이템이 없습니다.");
        }
        // posKey = 0; // 디버깅용 코드 <<<
        // 전역 현재 판매 데이터 세팅
        currentSellItem.customerKey = currentDealData.customerKey;
        currentSellItem.itemKey = responseData.itemKey;
        //이미지 세팅
        ItemCatalogData iData=SingletonManager.Instance.GetItemCatalog(displaysMap[posKey].itemCatalogKey);
        sellObjs.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().sprite
            = Resources.Load<Sprite>($"IMG_ITEM_CATALOG/{iData.imgId}");
        // 텍스트 세팅
        sellObjs.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text =$"{responseData.sellingPrice}";  
        // 다이얼로그
        DialogueManager.Instance.PutDialogue("<고객> 이 아이템의 테마가 저랑 잘 어울리는 것 같아요.\n 구매하고 싶네요");
    }


    public void OnDecideToSellItem()
    {
        // 서버에 전달하기 sellComplete
        // SellCompleteResponse responseData =TransmissionManager.Instance.RequestToServer<SellCompleteRequest,SellCompleteResponse>(RequestType.ITEM_SELL_COMPLETE,currentSellItem);

        // 테스트용 데이터 사용 <<<<<<<<<<<<<<<<<<
        TextAsset jsonFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Mocks/17sellComplete.json", typeof(TextAsset));
        SellCompleteResponse responseData =JsonUtility.FromJson<SellCompleteResponse>(jsonFile.text);

        // 디스플레이 아이템에서 삭제
        itemDisplayManager.RemoveDisplayedItem(responseData.displayedPositionKey);
        // 돈 업데이트
        gameSessionManager.SetLeftMoney(responseData.leftMoney); 
        // 팝업창 띄우기
        ConfirmPopuper.Instance.
            PopupCheckPanel
                ($"고객 직접 판매를 통해 {string.Format("{0:#,0}",responseData.earnedAmount)} G를 획득하였습니다!");
        DialogueManager.Instance.PutDialogue(sellCompleteDialogues[Random.Range(0,sellCompleteDialogues.Length)]);
        // 판매 오브젝트 끄기
        deseBlackFilter.SetActive(false);
        sellObjs.SetActive(false);
    }

    public void OnDenyToSellItem()
    {
        // 판매 거부를 서버에다 전달하기
        SellCancelRequest requestData = new SellCancelRequest();
        requestData.itemKey = currentSellItem.itemKey;
        requestData.customerKey = currentSellItem.customerKey;

        // 서버에 전달
        TransmissionManager.Instance.RequestToServer<SellCancelRequest, int>(RequestType.ITEM_SELL_CANCEL, requestData);

        // 고객 상태 -> 거래 상태로 바꾸기
        SingletonManager.Instance.IsCustomerDealState = CustomerState.Deal;
        // 거래 패널 세팅하기
        SetDealPanelToNewData(dailyDealsList[currentDealIndex], true);
        // 판매 오브젝트 끄기
        deseBlackFilter.SetActive(false);
        sellObjs.SetActive(false);
    }

    private string ConvertCustomerAttributeToString(string attribute)
    {
        string strFeed = "";
        switch (attribute)
        {
            case "FRAUD":
                strFeed= "사기꾼 기질";
                break;
            case "WELL_COLLECT":
                strFeed= "수집가 기질";
                break;
            case "CLUMSY":
                strFeed= "대충 정리함";
                break;
        }
        return strFeed;
    }
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


    public void OnDecideDeal()
    {
        // 서버에 요청할 데이터 세팅하기
        DealCompleteRequest requestData =  new DealCompleteRequest();
        requestData.itemKey = currentDealData.itemKey;
        requestData.drcKey = currentDealData.drcKey;
        // 서버에 요청하고 데이터 받아오기
        // DealCompleteResponse responseData =TransmissionManager.Instance.RequestToServer<DealCompleteRequest,DealCompleteResponse>(RequestType.DEAL_COMPLETE,requestData);

        // 테스트용 데이터 사용
        TextAsset jsonFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Mocks/12dealComplete.json", typeof(TextAsset));
        DealCompleteResponse responseData =JsonUtility.FromJson<DealCompleteResponse>(jsonFile.text);

        dealDecideActionResponseData = responseData; // 전역변수에 저장해서 다같이 봐

        // 거래 성공 시
        if(responseData.dealSuccess =="Y")
        {
            // 거래 성공에 대한 클라 업데이트하기
            // 전시장 아이템 추가
            // 거래 다음으로 넘어가기 위해서 여기 주석처리하면 됨 <<<<<<<<<<<<<<<<<<<<
            itemDisplayManager.AddDisplayedItem(responseData.displayedItem.displayPositionKey, responseData.displayedItem);
            // 남은 돈 표시
            gameSessionManager.SetLeftMoney(responseData.leftMoney); 

            // 거래 창 끄기
            deseBlackFilter.SetActive(false); 
            dealObjs.SetActive(false);
            // 거래 성공 확인 팝업창 띄우기
            ConfirmPopuper.Instance.PopupCheckPanel("거래에 성공하였습니다!");
            // 다이얼로그
            DialogueManager.Instance.PutDialogue(dealSuccessDialogues[Random.Range(0,dealSuccessDialogues.Length)]);
            // 다음 거래로 이동
            currentDealIndex++; 
            // 다음 행동 정하기
            if(currentDealIndex >= dailyDealsList.Count || responseData.isDayNext == "Y") // 모든 거래 다 함
            {// 오늘 거래 다했는지 체크
                PopupFinalizeObjs(); // 정산화면 띄우기
            }
            else { // 다음 거래 준비
                SetDealPanelToNewData(dailyDealsList[currentDealIndex]);
            }        
        }
        else // 거래 성사 실패 시(돈 부족 등)
        {
            // 거래 실패 확인 팝업창 띄우기
            ConfirmPopuper.Instance.PopupCheckPanel("거래에 실패하였습니다.\n남은 돈을 확인해보세요");   
            DialogueManager.Instance.PutDialogue("돈이 없다구요?");
        } 
    }

    private void PopupFinalizeObjs()
    {
        // 정산 화면 데이터 세팅하기
        FinalizeObjs.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text
          =$"{string.Format("{0:#,0}",dealDecideActionResponseData.dayFinalize.startMoney)}G";
        FinalizeObjs.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text
          =$"{string.Format("{0:#,0}",dealDecideActionResponseData.dayFinalize.todayEndMoney)}G";
        FinalizeObjs.transform.GetChild(2).GetChild(2).GetComponent<TMP_Text>().text
          =$"{string.Format("{0:#,0}",dealDecideActionResponseData.dayFinalize.interest)}G";
        FinalizeObjs.transform.GetChild(2).GetChild(3).GetComponent<TMP_Text>().text
          =$"{string.Format("{0:#,0}",dealDecideActionResponseData.dayFinalize.weeklyInterest)}G";
        FinalizeObjs.transform.GetChild(2).GetChild(4).GetComponent<TMP_Text>().text
          =$"{string.Format("{0:#,0}",dealDecideActionResponseData.dayFinalize.finalMoney)}G";
        // 정산 화면 띄우기
        DialogueManager.Instance.PutDialogue("<속마음> 정산의 시간이다..");
        FinalizeObjs.SetActive(true);
    }

    public void OnPushFinalizeConfirmButton()
    {// 확인 버튼 누름
        // 정산화면 비활성화
        FinalizeObjs.SetActive(false);

        // 게임오버 됐는지 체크
        if(dealDecideActionResponseData.isGameOvered == "Y")
        { // 오버했다면, 오버 화면 띄우기
            // 게임 오버 화면 데이터 세팅하기
            gameSessionManager.PopupGameEndObjs(dealDecideActionResponseData.worldRecord, 
            "상환할 이자를 내지 못해 사채업자들에게 끌려갔습니다...");
            DialogueManager.Instance.PutDialogue("<주인> 안돼!!");
        }
        else
        {// 오버하지 않았다면, 다음 날 세팅    
            SetNextDaySetting();
        }
    }

    public void OnGameEndButtonClicked()
    {
        // 시작 씬으로 이동
        SceneManager.LoadScene("StartScene");
    }

    public void OnDenyDeal()
    {
        // 서버에 요청할 데이터 세팅하기
        DealCompleteRequest requestData =  new DealCompleteRequest();
        requestData.itemKey = currentDealData.itemKey;
        requestData.drcKey = currentDealData.drcKey;
        // 서버에 요청하고 데이터 받아오기
        // DealCompleteResponse responseData =TransmissionManager.Instance.RequestToServer<DealCompleteRequest,DealCompleteResponse>(RequestType.DEAL_CANCEL,requestData);

        // 테스트용 데이터 사용
        TextAsset jsonFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Mocks/12dealCancelLeftDeal.json", typeof(TextAsset));
        DealCompleteResponse responseData =JsonUtility.FromJson<DealCompleteResponse>(jsonFile.text);
        
        dealDecideActionResponseData = responseData; // 전역변수에 저장해서 다같이 봐

        // 거래 창 끄기
        deseBlackFilter.SetActive(false); 
        dealObjs.SetActive(false);
        // 거래 거부 성공 확인 팝업창 띄우기
        ConfirmPopuper.Instance.PopupCheckPanel(denyDealDialogueList[Random.Range(0,denyDealDialogueList.Length)]);
        DialogueManager.Instance.PutDialogue(denyDealDialogues[Random.Range(0,denyDealDialogues.Length)]);
        // 다음 거래로 이동
        currentDealIndex++; 
        // 다음 행동 정하기
        if(currentDealIndex >= dailyDealsList.Count || responseData.isDayNext == "Y") // 모든 거래 다 함
        {// 오늘 거래 다했는지 체크
            PopupFinalizeObjs(); // 정산화면 띄우기
        }
        else { // 다음 거래 준비
            SetDealPanelToNewData(dailyDealsList[currentDealIndex]);
        }        
    }

    private void SetNextDaySetting()
    {
        // 다음 날로 넘어가는 확인 팝업창 띄우기
        string text ="다음 날로 넘어가기";
        ConfirmPopuper.Instance.PopupCheckPanel(text);
        // 고객 비활성화
        customerObj.SetActive(false);
        // 아이템 비활성화
        trayItemObj.SetActive(false);
        // 다음날 UI 세팅
        gameSessionManager.SetNextDayUI(dealDecideActionResponseData.dayNext);
        // 다음 날 시작
        StartToday();
    }

    private string ConvertItemState(ItemState state)
    {
        switch (state)
        {
            case ItemState.Created:
                return "생성됨";
            case ItemState.OnDisplay:
                return "전시 중";
            case ItemState.UnderRestoration:
                return "복원 중";
            case ItemState.OnAuction:
                return "경매 중";
            case ItemState.AfterRestoration:
                return "복원됨";
        }
        return "";
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