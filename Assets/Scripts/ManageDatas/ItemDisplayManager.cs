using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

namespace AYellowpaper.SerializedCollections
{
public class ItemDisplayManager : MonoBehaviour
{
    [SerializedDictionary("position Number","DisplayObject")] // 얘는 게임 오브젝트 접근용 dict
    public SerializedDictionary<int,GameObject> displayObjectMap = new SerializedDictionary<int, GameObject>();

    [SerializedDictionary("position key","DisplayData")] // 얘는 데이터 저장용 dict
    public SerializedDictionary<int, DisplayedItemData> itemDisplayMap = new SerializedDictionary<int, DisplayedItemData>();
    // 전시 위치 별 아이템 정보 매핑
    [SerializeField] ItemActionManager itemActionManager;

    private ItemCatalogData iData;
    void Start()
    {
        // 실제로는 데이웨이브 매니저 - 통신 매니저에서 수행 할 일이지만 미리 확인을 위해 start에서 실행 중
        TextAsset jsonFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Mocks/6displayItemAll.json", typeof(TextAsset));
        ItemDisplaysWrapData displayWrap =JsonUtility.FromJson<ItemDisplaysWrapData>(jsonFile.text);
        InitDisplayedItem(displayWrap.displays);
    }

    public void InitDisplayedItem(List<DisplayedItemData> displays)
    {  
        for(int i = 0; i < displays.Count; i++)
        {
            itemDisplayMap.Add(displays[i].displayPositionKey,displays[i]);
        }

        // 아이템 오브젝트에 데이터 적용하기
        for(int posKey = 0; posKey < displays.Count; posKey++)
        {
            // 값이 있으면 true와 해당 value를 담아서 반환하고, 없으면 false를 반환함
            if(itemDisplayMap.TryGetValue(posKey, out DisplayedItemData displayedItemData)){
                ActivateDisplayedItem(posKey, displayedItemData);
            }
        }
        itemActionManager.OnItemActionTogClicked(true);
    }

    public void SetItemState(int posKey,ItemState itemState)
    {
        itemDisplayMap[posKey].itemState = itemState;
        ActivateDisplayedItem(posKey, itemDisplayMap[posKey]); // 정보 업데이트
    }

    public void AddDisplayedItem(int posKey, DisplayedItemData dData)
    {
        itemDisplayMap.Add(posKey,dData);
        ActivateDisplayedItem(posKey,dData);
        itemActionManager.SetItemActionPanel();
    }

    public void ActivateDisplayedItem(int posKey, DisplayedItemData dData)
    {
        // 아이템 이미지 채우기
        iData = SingletonManager.Instance?.GetItemCatalog(dData.itemCatalogKey);
        displayObjectMap[posKey].transform.GetChild(1).GetComponent<Image>().sprite=Resources.Load<Sprite>($"IMG_ITEM_CATALOG/{iData.imgId}");
        displayObjectMap[posKey].transform.GetChild(1).GetComponent<Image>();
        switch (dData.itemState)
        {
            case ItemState.OnDisplay:
            case ItemState.Sold:
            case ItemState.Created:
            case ItemState.AfterRestoration:
                displayObjectMap[posKey].transform.GetChild(2).gameObject.SetActive(false);
                break;
            case ItemState.UnderRestoration:
                displayObjectMap[posKey].transform.GetChild(2).gameObject.SetActive(true);
                displayObjectMap[posKey].transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text=
                    "수리 중";
                break;
            case ItemState.OnAuction:
                displayObjectMap[posKey].transform.GetChild(2).gameObject.SetActive(true);
                displayObjectMap[posKey].transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text=
                    "경매 중";
                break;
        }
        // 아이템 무슨 상태인지 채우기

        // 디스플레이 아이템 정보 패널도 채우기
            // 값 채우기
            string foundAuth="";
            switch (dData.foundAuthenticity)
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
                    $"최초 제시가: {dData.askingPrice}\n"+
                    $"구매가: {string.Format("{0:#,0}",dData.purchasePrice)} G\n"+
                    $"감정가: {string.Format("{0:#,0}",dData.appraisedPrice)}\n"+
                    $"구매일: {string.Format("{0:#,0}",dData.boughtDate)}\n"+
                    $"판매자: {dData.sellerName}\n"+
                    $"찾은 흠 개수: {dData.foundFlawEa}\n"+
                    $"찾은 등급: "+dData.foundGrade.ToString()+"\n"+
                    $"찾은 진위 여부: {foundAuth}";

        displayObjectMap[posKey].transform.parent.GetChild(8+posKey).GetChild(1).GetComponent<TMP_Text>().text = displayText;
        
        // 활성화
        displayObjectMap[posKey].SetActive(true);
    }

    public void PopupDisplayedItemInformPanel(int posKey)
    {
        displayObjectMap[posKey].transform.parent.GetChild(8+posKey).gameObject.SetActive(true);        
    }

    public void PopOffInformPanel(int posKey)
    {
        displayObjectMap[posKey].transform.parent.GetChild(8+posKey).gameObject.SetActive(false);
    }

    public Dictionary<int, DisplayedItemData> GetItemDisplayMap()
    {
        return itemDisplayMap;
    }

    public List<DisplayedItemData> GetRestorableItem()
    {
        List<DisplayedItemData> restorableItems = new List<DisplayedItemData>();
        for(int posKey = 0; posKey < 8; posKey++)
        {
            // 값이 있으면 true와 해당 value를 담아서 반환하고, 없으면 false를 반환함
            if(itemDisplayMap.TryGetValue(posKey, out DisplayedItemData displayedItemData)){
                // 복원이 가능한 전시중 아이템이면 넣어( 복원완료가 아님)
                if(displayedItemData.itemState == ItemState.OnDisplay)
                {
                    restorableItems.Add(displayedItemData);
                }
            }
        }
        return restorableItems;
    }




    public List<DisplayedItemData> GetAuctionableItem()
    {
        List<DisplayedItemData> auctionableItems = new List<DisplayedItemData>();
        for(int posKey = 0; posKey < 8; posKey++)
        {
            // 값이 있으면 true와 해당 value를 담아서 반환하고, 없으면 false를 반환함
            if(itemDisplayMap.TryGetValue(posKey, out DisplayedItemData displayedItemData)){
                // 경매가 가능한 아이템(전시 중, 복원 후)
                if(displayedItemData.itemState == ItemState.OnDisplay
                    || displayedItemData.itemState == ItemState.AfterRestoration)
                {
                    auctionableItems.Add(displayedItemData);
                }
            }
        }
        return auctionableItems;
    }

}
}
