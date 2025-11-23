using UnityEngine;
using System.Linq;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class ItemActionManager : MonoBehaviour
{
    [SerializeField] private GameObject itemObjs; // 아이템 복원 오브젝트
    [SerializeField] private GameObject itemActionTog;
    [SerializeField] private GameObject itemListTog;
    [SerializeField] private ItemDisplayManager displayManager;
    
    List<DisplayedItemData> actionItemList;
    private DisplayedItemData currentClickedItem;

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
        }
        else
        {
            requestData.actionType = "auction";
        }
        Debug.Log(requestData.actionType);
        requestData.itemKey = currentClickedItem.itemKey;
        Debug.Log(requestData.itemKey);
        // ItemActionResponse responseData = TransmissionManager.Instance.RequestToServer<ItemActionRequest,ItemActionResponse>(RequestType.ITEM_ACTION,requestData);
        //요청하고 결과값 받기 -> 서버 있어야 받을 수 있음
        // 테스트 데이터 <<<<<<<<<<<<<<
        TextAsset jsonFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Mocks/14itemAction.json", typeof(TextAsset));
        ItemActionResponse responseData =JsonUtility.FromJson<ItemActionResponse>(jsonFile.text);

        Debug.Log(responseData);
        // 디스플레이 아이템 매니저도 업데이트
        displayManager.SetItemState(currentClickedItem.displayPositionKey, responseData.itemState);
        
        // 여기 아이템액션 매니저의 창도 업데이트
        OnItemActionTogClicked(itemActionTog.transform.GetChild(0).GetComponent<Toggle>().isOn);
    }

    public void SetItemActionPanel()
    {
        OnItemActionTogClicked(itemActionTog.transform.GetChild(0).GetComponent<Toggle>().isOn);
    }

    public void OnItemActionTogClicked(bool isOn)
    {
        if (isOn)
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

            string displayText =$"{iData.itemCatalogName}: [{iData.categoryName}]\n\n"+ // 아이템 이름
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

            string displayText =$"{iData.itemCatalogName}: [{iData.categoryName}]\n\n"+ // 아이템 이름
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

}
