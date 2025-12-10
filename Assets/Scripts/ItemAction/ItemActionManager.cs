using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public enum DebtGoldUnit
{
    HUNDRED =100,
    FIVE_HUNDRED = 500,
    THOUSAND = 1000,
    TWO_THOUSAND = 20009
}

public enum DebtActionType
{
    LOAN,
    REPAY
}

public class ItemActionManager : MonoBehaviour
{
    [SerializeField] private GameObject itemObjs; // 아이템 복원 오브젝트
    [SerializeField] private GameObject itemActionTog;
    [SerializeField] private GameObject itemListTog;
    [SerializeField] private ItemDisplayManager displayManager;
    [SerializeField] private GameSessionManager gameSessionManager;
    [SerializeField] private ToggleObjs toggleObjsManager;

    [SerializeField] private GameObject personalDebtTogObjs;
    [SerializeField] private GameObject pawnshopDebtTogObjs;
    [SerializeField] private GameObject pawnshopDebtActionTogObjs;
    [SerializeField] private TMP_Text personalDebtText;
    [SerializeField] private TMP_Text pawnshopDebtText;
    
    List<DisplayedItemData> actionItemList;
    private DisplayedItemData currentClickedItem;

    private DebtGoldUnit currentPersonalDebtGoldClickedUnit;
    private DebtGoldUnit currentPawnshopDebtGoldClickedUnit;
    private DebtActionType currentPawnshopDebtActionType;


    public void UpdateDebtValue(int personalDebt = 0, int pawnshopDebt = 0)
    {
        if(personalDebt != 0)
        {
            personalDebtText.text
                = $"{string.Format("{0:#,0}",personalDebt)}G";            
        }
        if(pawnshopDebt != 0)
        {
            pawnshopDebtText.text
                = $"{string.Format("{0:#,0}",pawnshopDebt)}G";
        }
        
    }

    
    // 개인 빚 상환 버튼 눌렸을 때
    public void OnPersonalDebtButtonClicked()
    {
        // 요청 데이터 생성
        LoanUpdateRequest requestData = new LoanUpdateRequest();
        // 해당 설정으로 요청 데이터 세팅
        requestData.debtType = "PERSONAL";
        // 금액 세팅(개인 빚은 무조건 상환)
        requestData.amount = -1 * (int)currentPersonalDebtGoldClickedUnit;
        // 요청 데이터 보내고 반환 데이터 받기
        TransmissionManager.Instance.RequestToServer<LoanUpdateRequest,LoanUpdateResponse>(
            RequestType.LOAN_UPDATE,
            requestData,
            (responseCode, responseData) =>
            {
                // 테스트용 데이터 <<<<<<<<<<<<<<<<<
                // responseCode = 200;
                // TextAsset jsonFile = Resources.Load<TextAsset>("Mocks/18loanUpdatePersonal");
                // responseData =JsonUtility.FromJson<LoanUpdateResponse>(jsonFile.text);
                
                // 통신 오류 체크
                if((ResponseCode)responseCode != ResponseCode.OK)
                {
                    string errorMessage = "통신 오류: 개인 빚 상환 요청 실패.";
                    TransmissionManager.Instance.OnHandleErrorResponseCode(responseCode, errorMessage);
                }
                else
                {
                    /* 결과 데이터 UI 업데이트하기 */
                    // 돈 업데이트
                    Debug.Log($"leftmoney: {responseData.leftMoney}");
                    gameSessionManager.SetLeftMoney(responseData.leftMoney);
                    // 가게빚 UI 업데이트
                    UpdateDebtValue(responseData.leftDebtAmount, 0);
                    // 팝업창 띄우기
                    string log = $"개인 빚 {string.Format("{0:#,0}",(int)currentPersonalDebtGoldClickedUnit)}G을 상환하였습니다.";
                    ConfirmPopuper.Instance.PopupCheckPanel(log);
                    DialogueManager.Instance.PutDialogue("<속마음> 이정도 갚았나..\n 모든 빚을 다 갚으면 모든 목적을 이룬 걸 거야..");
                    // 게임 클리어 됐는지 체크
                    if(responseData.isGameCleared == "Y")
                    { 
                        // 뉴스 패널 끄기
                        toggleObjsManager.TurnOffNewsObjs();

                        // 못 찾은 아이템 리스트 창 띄우기
                        gameSessionManager.PopupNotFoundItemListObjs(responseData.notFoundCategoryList);
                        // 게임 클리어 화면 데이터 세팅하기
                        gameSessionManager.PopupGameEndObjs(responseData.worldRecord, 
                        "모든 빚을 다 상환하여 게임을 클리어하였습니다!!!");
                        DialogueManager.Instance.PutDialogue("<주인> 다 갚았다!!");
                    }   
                }
            }
        );
    }

    // 가게 빚 대출/상환 버튼 눌렸을 때
    public void OnPawnshopDebtButtonClicked()
    {
        // 요청 데이터 생성
        LoanUpdateRequest requestData = new LoanUpdateRequest();
        requestData.debtType = "PAWNSHOP";
        // 대출인지 상환인지 구분 확인
        if(currentPawnshopDebtActionType == DebtActionType.LOAN) // 대출 요청이라면
        {
            // 금액 세팅
            requestData.amount = (int)currentPawnshopDebtGoldClickedUnit;
        }
        else // 상환
        {
            // 금액 세팅
            requestData.amount = -1 * (int)currentPawnshopDebtGoldClickedUnit;
            
        }
        // 요청 데이터 보내고 반환 데이터 받기
        TransmissionManager.Instance.RequestToServer<LoanUpdateRequest,LoanUpdateResponse>(
            RequestType.LOAN_UPDATE,
            requestData,
            (responseCode, responseData) =>
            {
                // 테스트용 데이터 <<<<<<<<<<<<<<<<<
                // responseCode = 200;
                // TextAsset jsonFile = Resources.Load<TextAsset>("Mocks/18loanUpdateClearPawnShop");
                // responseData =JsonUtility.FromJson<LoanUpdateResponse>(jsonFile.text);
                
                // 통신 오류 체크
                if((ResponseCode)responseCode != ResponseCode.OK)
                {
                    string errorMessage = "통신 오류: 가게 빚 처리 요청 실패.";
                    TransmissionManager.Instance.OnHandleErrorResponseCode(responseCode, errorMessage);
                }
                else
                {
                    /* 결과 데이터 UI 업데이트하기 */
                    // 돈 업데이트
                    Debug.Log($"leftmoney: {responseData.leftMoney}");
                    gameSessionManager.SetLeftMoney(responseData.leftMoney);
                    // 가게빚 UI 업데이트
                    UpdateDebtValue(0, responseData.leftDebtAmount);
                    // 팝업창 띄우기
                    string log;
                    if(requestData.amount > 0) // 대출이었다면
                    {
                        log = $"가게 빚 {string.Format("{0:#,0}",(int)currentPawnshopDebtGoldClickedUnit)}G을 대출하였습니다.";        
                        DialogueManager.Instance.PutDialogue("<속마음> 어쩔 수 없다.. 대출을 받자");
                    }
                    else
                    {
                        log = $"가게 빚 {string.Format("{0:#,0}",(int)currentPawnshopDebtGoldClickedUnit)}G을 상환하였습니다.";                    
                        DialogueManager.Instance.PutDialogue("<속마음> 이대로 조금씩 조금씩..");
                    }
                    ConfirmPopuper.Instance.PopupCheckPanel(log);
                    // 게임 클리어 됐는지 체크
                    if(responseData.isGameCleared == "Y")
                    {
                        // 뉴스 패널 끄기
                        toggleObjsManager.TurnOffNewsObjs();
                        // 못 찾은 아이템 리스트 창 띄우기
                        gameSessionManager.PopupNotFoundItemListObjs(responseData.notFoundCategoryList);                       
                        // 게임 클리어 화면 띄우기
                        gameSessionManager.PopupGameEndObjs(responseData.worldRecord, 
                        "모든 빚을 다 상환하여 게임을 클리어하였습니다!!!");
                        DialogueManager.Instance.PutDialogue("<주인> 다 갚았다!!");
                    }
                }
            }
        );
    }

    public void OnPersonalDebtGoldToggleClicked()
    {
        // 전역변수 업데이트
        bool tog2000 = personalDebtTogObjs.transform.GetChild(0).GetComponent<Toggle>().isOn;
        bool tog1000 = personalDebtTogObjs.transform.GetChild(1).GetComponent<Toggle>().isOn;
        bool tog500 = personalDebtTogObjs.transform.GetChild(2).GetComponent<Toggle>().isOn;
        bool tog100 = personalDebtTogObjs.transform.GetChild(3).GetComponent<Toggle>().isOn;
        if(tog2000 == true)
        {
            currentPersonalDebtGoldClickedUnit = DebtGoldUnit.TWO_THOUSAND;
            DialogueManager.Instance.PutDialogue("<속마음> 2,000골드를...");
        }
        else if(tog1000 == true)
        {
            currentPersonalDebtGoldClickedUnit = DebtGoldUnit.THOUSAND;
            DialogueManager.Instance.PutDialogue("<속마음> 1,000골드를...");
        }
        else if(tog500 == true)
        {
            currentPersonalDebtGoldClickedUnit = DebtGoldUnit.FIVE_HUNDRED;
            DialogueManager.Instance.PutDialogue("<속마음> 500골드를...");
        }
        else if(tog100 == true)
        {
            currentPersonalDebtGoldClickedUnit = DebtGoldUnit.HUNDRED;
            DialogueManager.Instance.PutDialogue("<속마음> 100골드를...");
        }
    }
    public void OnPawnshopDebtGoldToggleClicked()
    {
        // 전역변수 업데이트
        bool tog2000 = pawnshopDebtTogObjs.transform.GetChild(0).GetComponent<Toggle>().isOn;
        bool tog1000 = pawnshopDebtTogObjs.transform.GetChild(1).GetComponent<Toggle>().isOn;
        bool tog500 = pawnshopDebtTogObjs.transform.GetChild(2).GetComponent<Toggle>().isOn;
        bool tog100 = pawnshopDebtTogObjs.transform.GetChild(3).GetComponent<Toggle>().isOn;
        if(tog2000 == true)
        {
            currentPawnshopDebtGoldClickedUnit = DebtGoldUnit.TWO_THOUSAND;
            DialogueManager.Instance.PutDialogue("<속마음> 2,000골드를...");
        }
        else if(tog1000 == true)
        {
            currentPawnshopDebtGoldClickedUnit = DebtGoldUnit.THOUSAND;
            DialogueManager.Instance.PutDialogue("<속마음> 1,000골드를...");
        }
        else if(tog500 == true)
        {
            currentPawnshopDebtGoldClickedUnit = DebtGoldUnit.FIVE_HUNDRED;
            DialogueManager.Instance.PutDialogue("<속마음> 500골드를...");
        }
        else if(tog100 == true)
        {
            currentPawnshopDebtGoldClickedUnit = DebtGoldUnit.HUNDRED;
            DialogueManager.Instance.PutDialogue("<속마음> 100골드를...");
        }        
    }
    public void OnPawnshopDebtActionToggleClicked()
    {
        // 전역변수 업데이트
        bool togLoan = pawnshopDebtActionTogObjs.transform.GetChild(0).GetComponent<Toggle>().isOn;
        bool togRepay = pawnshopDebtActionTogObjs.transform.GetChild(1).GetComponent<Toggle>().isOn;
        if(togLoan == true)
        {
            currentPawnshopDebtActionType = DebtActionType.LOAN;
            DialogueManager.Instance.PutDialogue("<속마음> 대출을 할까..");
        }
        else if(togRepay == true)
        {
            currentPawnshopDebtActionType = DebtActionType.REPAY;
            DialogueManager.Instance.PutDialogue("<속마음> 상환을 할까..");
        }        
    }

    public void OnActionItemClicked(bool isOn, int itemIndex){
        if(isOn){
            currentClickedItem = actionItemList[itemIndex];
        }
    }

    public void OnItemActionButtonClicked()
    {
        // 요청 데이터 세팅
        ItemActionRequest requestData = new ItemActionRequest();
        // 만약 복원에 체크 되어있다면
        if (itemActionTog.transform.GetChild(0).GetComponent<Toggle>().isOn)
        {
            requestData.actionType = "restore";
            DialogueManager.Instance.PutDialogue("<속마음> 복원할 수 있는 아이템이...");
        }
        else
        {
            requestData.actionType = "auction";
            DialogueManager.Instance.PutDialogue("<속마음> 경매 나갈 수 있는 아이템이...");
        }
        Debug.Log(requestData.actionType);
        requestData.itemKey = currentClickedItem.itemKey;
        Debug.Log(requestData.itemKey);
        TransmissionManager.Instance.RequestToServer<ItemActionRequest,ItemActionResponse>(
            RequestType.ITEM_ACTION,
            requestData,
            (responseCode, responseData) =>
            {
                // 테스트 데이터 <<<<<<<<<<<<<<
                // responseCode = 200;
                // TextAsset jsonFile = Resources.Load<TextAsset>("Mocks/14itemAction");
                // responseData =JsonUtility.FromJson<ItemActionResponse>(jsonFile.text);
                
                // 통신 오류 체크
                if((ResponseCode)responseCode != ResponseCode.OK)
                {
                    string errorMessage = "통신 오류: 아이템 복원/경매 처리 요청 실패.";
                    TransmissionManager.Instance.OnHandleErrorResponseCode(responseCode, errorMessage);
                }
                else
                {
                    // 디스플레이 아이템 매니저도 업데이트
                    displayManager.SetItemState(currentClickedItem.displayPositionKey, responseData.itemState);
                    
                    // 여기 아이템액션 매니저의 창도 업데이트
                    OnItemActionTogClicked(itemActionTog.transform.GetChild(0).GetComponent<Toggle>().isOn);
                    DialogueManager.Instance.PutDialogue("<속마음> 얼마나 걸릴려나..");                    
                }
            }
        );
    }

    public void SetItemActionPanel()
    {
        OnItemActionTogClicked(itemActionTog.transform.GetChild(0).GetComponent<Toggle>().isOn);
    }

    public void OnItemActionTogClicked(bool isOn)
    {
        if (itemActionTog.transform.GetChild(0).GetComponent<Toggle>().isOn)
        {
            SetRestorableItems();
        }
        else
        {
            SetAuctionableItems();
        }
        if(actionItemList.Count>0){
            currentClickedItem = actionItemList[0];
        }
    }


    public void SetAuctionableItems()
    {
        actionItemList = displayManager.GetAuctionableItem();

        for(int i = 0; i < actionItemList.Count; i++)
        {
            ItemCatalogData iData = SingletonManager.Instance.GetItemCatalog(actionItemList[i].itemCatalogKey);
            itemListTog.transform.GetChild(0+i).GetChild(1).GetChild(0)
                .GetComponent<Image>().sprite = Resources.Load<Sprite>($"IMG_ITEM_CATALOG/{iData.imgId}");
            
            // 디스플레이 아이템 정보 패널도 채우기
            // 값 채우기
            string foundAuth="";
            switch (actionItemList[i].foundAuthenticity)
            {
                case -1:
                    foundAuth = "미발견";
                    break;
                case 0:
                    foundAuth = "가품";
                    break;
                case 1:
                    foundAuth = "진품";
                    break;
            }        

            string displayText =$"{iData.itemCatalogName}: [{iData.categoryName}]\n"+ // 아이템 이름
                        $"상태: {ConvertItemState(actionItemList[i].itemState)}\n"+                       
                        $"최초 제시가: {string.Format("{0:#,0}",actionItemList[i].askingPrice)} G\n"+
                        $"구매가: {string.Format("{0:#,0}",actionItemList[i].purchasePrice)} G\n"+
                        $"감정가: {string.Format("{0:#,0}",actionItemList[i].appraisedPrice)} G\n"+
                        $"구매일: {string.Format("{0:#,0}",actionItemList[i].boughtDate)} G\n"+
                        $"판매자: {actionItemList[i].sellerName}\n"+
                        $"찾은 흠 개수: {actionItemList[i].foundFlawEa}\n"+
                        $"찾은 등급: "+actionItemList[i].foundGrade.ToString()+"\n"+
                        $"찾은 진위 여부: {foundAuth}";

            itemListTog.transform.GetChild(0+i+8).GetChild(1).GetComponent<TMP_Text>().text = displayText;
            itemListTog.transform.GetChild(0+i).gameObject.SetActive(true);
        }
        for(int i = actionItemList.Count; i < 8; i++)
        {
            itemListTog.transform.GetChild(0+i).gameObject.SetActive(false);
        }
    }

    private void SetRestorableItems()
    {
        actionItemList = displayManager.GetRestorableItem();

        for(int i = 0; i < actionItemList.Count; i++)
        {
            ItemCatalogData iData = SingletonManager.Instance.GetItemCatalog(actionItemList[i].itemCatalogKey);
            itemListTog.transform.GetChild(0+i).GetChild(1).GetChild(0)
                .GetComponent<Image>().sprite = Resources.Load<Sprite>($"IMG_ITEM_CATALOG/{iData.imgId}");
            

            // 디스플레이 아이템 정보 패널도 채우기
                // TODO: 값 채우기
                string foundAuth="";
                switch (actionItemList[i].foundAuthenticity)
                {
                    case -1:
                        foundAuth = "미발견";
                        break;
                    case 0:
                        foundAuth = "가품";
                        break;
                    case 1:
                        foundAuth = "진품";
                        break;
                }        

            string displayText =$"{iData.itemCatalogName}: [{iData.categoryName}]\n"+ // 아이템 이름
                        $"상태: {ConvertItemState(actionItemList[i].itemState)}\n"+                       
                        $"최초 제시가: {string.Format("{0:#,0}",actionItemList[i].askingPrice)} G\n"+
                        $"구매가: {string.Format("{0:#,0}",actionItemList[i].purchasePrice)} G\n"+
                        $"감정가: {string.Format("{0:#,0}",actionItemList[i].appraisedPrice)} G\n"+
                        $"구매일: {string.Format("{0:#,0}",actionItemList[i].boughtDate)} G\n"+
                        $"판매자: {actionItemList[i].sellerName}\n"+
                        $"찾은 흠 개수: {actionItemList[i].foundFlawEa}\n"+
                        $"찾은 등급: "+actionItemList[i].foundGrade.ToString()+"\n"+
                        $"찾은 진위 여부: {foundAuth}";

            itemListTog.transform.GetChild(0+i+8).GetChild(1).GetComponent<TMP_Text>().text = displayText;
            itemListTog.transform.GetChild(0+i).gameObject.SetActive(true);
        }
        for(int i = actionItemList.Count; i < 8; i++)
        {
            itemListTog.transform.GetChild(0+i).gameObject.SetActive(false);
        }
    }


    public void PopupDisplayedItemInformPanel(int posKey)
    {
        itemListTog.transform.GetChild(8+posKey).gameObject.SetActive(true);        
    }

    public void PopOffInformPanel(int posKey)
    {
        itemListTog.transform.GetChild(8+posKey).gameObject.SetActive(false);        
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

}
