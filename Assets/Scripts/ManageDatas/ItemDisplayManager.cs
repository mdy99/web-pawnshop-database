using UnityEngine;
using System.Collections.Generic;
public class ItemDisplayManager : MonoBehaviour
{
    [SerializeField] Dictionary<int, ItemSO> itemDisplayPrefabs = new Dictionary<int, ItemSO>();
    // 전시 위치 별 아이템 정보 매핑

    void Start(){
        // 테스트용 아이템 추가
        ItemSO testItem = ScriptableObject.CreateInstance<ItemSO>();
        testItem.SetItemKey(1);
        testItem.SetItemCatalogKey(1001);
        testItem.SetGrade(Grade.Common);
        testItem.SetFlawEa(0);
        testItem.SetSuspiciousFlawAura(0.0f);
        testItem.SetItemImage();

        AddItemDisplay(0, testItem);
    }

    // public void AddItemToDisplay(JSON itemData, int displayPositionKey)
    // {
    //     ItemSO newItem = ScriptableObject.CreateInstance<ItemSO>();
    //     newItem.SetItemKey(itemData.itemKey);
    //     newItem.SetItemCatalogKey(itemData.itemCatalogKey);
    //     newItem.SetGrade((Grade)itemData.grade);
    //     newItem.SetFlawEa(itemData.flawEa);
    //     newItem.SetSuspiciousFlawAura(itemData.suspiciousFlawAura);
    //     newItem.SetItemImage();

    //     AddItemDisplay(displayPositionKey, newItem);

    // }

    public void AddItemDisplay(int displayPositionKey, ItemSO itemSO)
    {
        itemDisplayPrefabs.Add(displayPositionKey, itemSO);
    }

    public ItemSO GetItemAtPosition(int displayPositionKey)
    {
        itemDisplayPrefabs.TryGetValue(displayPositionKey, out ItemSO itemSO);
        return itemSO;
    }

    public void RemoveItemAtPosition(int displayPositionKey)
    {
        itemDisplayPrefabs.Remove(displayPositionKey);
    }

    public void ClearAllDisplays()
    {
        itemDisplayPrefabs.Clear();
    }

    public Dictionary<int, ItemSO> GetAllItemDisplays()
    {
        return itemDisplayPrefabs;
    }
}
