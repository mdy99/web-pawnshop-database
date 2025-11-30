using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using TMPro;

namespace AYellowpaper.SerializedCollections
{
public class SingletonManager : MonoBehaviour
{
    private static SingletonManager instance = null;

    [SerializeField] private Canvas mainCanvas;

    [SerializedDictionary("itemKey","ItemCatalogData")] // 얘는 데이터 저장용 dict
    public SerializedDictionary<int, ItemCatalogData> itemCatalogMap = new SerializedDictionary<int, ItemCatalogData>();
    [SerializedDictionary("customerKey","CustomerCatalogData")] // 얘는 데이터 저장용 dict
    public SerializedDictionary<int, CustomerCatalogData> customerCatalogMap = new SerializedDictionary<int, CustomerCatalogData>();

    private string hasGameSession; // 머스트 Y or N
    // 만약
    public string HasGameSession
    {
        get 
        {
            return hasGameSession; // 게임세션이 있는지?
        }
        set
        {
            hasGameSession = value; // 만약 게임이 끝나면, 이 정보를 N으로 바꿔야 함
        }
    }


    private CustomerState isCustomerDealState = CustomerState.Deal;

    public CustomerState IsCustomerDealState
    {
        get 
        {
            return isCustomerDealState; // 속성 값을 반환
        }
        set
        {
            isCustomerDealState = value;
        }
    }

    void Awake()
    {
        if(null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            // 나중에 이거 데이웨이브매니저에서 실행해야하는 거 알지?
            // InitialCatalogResponse responseData = TransmissionManager.Instance.RequestToServer<int,InitialCatalogResponse>(RequestType.INIT_CATALOGS, 0);

            // 테스트용 코드 <<<<<<<<<<<<<<<<<<<<<<<<<
            TextAsset jsonFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Mocks/5initialCatalog.json", typeof(TextAsset));
            InitialCatalogResponse responseData =JsonUtility.FromJson<InitialCatalogResponse>(jsonFile.text);
            InitCatalogMaps(responseData);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += SetMainCanvas;
    }

    void SetMainCanvas(Scene scene, LoadSceneMode mode)
    {
        mainCanvas = FindFirstObjectByType<Canvas>();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= SetMainCanvas;
    }

    public static SingletonManager Instance
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


    public Canvas getMainCanvas()
    {
        return mainCanvas;
    }

    public string ConvertToAffectedPrice(AffectedPrice price)
    {
        switch (price)
        {
            case AffectedPrice.AppraisedPrice:
                return "감정가";
            case AffectedPrice.AskingPrice:
                return "최초 제시가";
            case AffectedPrice.PurchasePrice:
                return "구매가";
            case AffectedPrice.SellingPrice:
                return "판매가";
            default: 
                return "없는 가격";
        }
    }

    public void InitCatalogMaps(InitialCatalogResponse catalogs)
    {
        for(int i = 0; i < catalogs.itemCatalogs.Count; i++)
        {
            itemCatalogMap.Add(catalogs.itemCatalogs[i].itemCatalogKey, catalogs.itemCatalogs[i]);
        }
        for(int i = 0; i < catalogs.customerCatalogs.Count; i++)
        {
            customerCatalogMap.Add(catalogs.customerCatalogs[i].customerKey, catalogs.customerCatalogs[i]);
        }
    }

    public ItemCatalogData GetItemCatalog(int itemCatalogKey)
    {
        return itemCatalogMap[itemCatalogKey];
    }
    public CustomerCatalogData GetCustomerCatalog(int customerKey)
    {
        return customerCatalogMap[customerKey];
    }

}
}