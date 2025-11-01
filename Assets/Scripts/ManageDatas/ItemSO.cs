using UnityEngine;



[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    #region === Variables ===
    [Header("Item Identification")]
    [SerializeField] private int itemKey;
    [SerializeField] private int itemCatalogKey;

    [Header("Attributes")]
    [SerializeField] private Grade grade;
    [SerializeField] private int flawEa;
    [SerializeField] private float suspiciousFlawAura;

    [Header("Authenticity")]
    [SerializeField] private bool authenticity;
    [SerializeField] private bool isAuthenticityFound;

    [Header("State")]
    [SerializeField] private ItemState itemState;

    [Header("Image")]
    [SerializeField] private Sprite itemImage;
    
    #endregion


    #region === Properties ===
    public int ItemKey => itemKey;
    public int ItemCatalogKey => itemCatalogKey;
    public Grade Grade => grade;
    public int FlawEa => flawEa;
    public float SuspiciousFlawAura => suspiciousFlawAura;
    public ItemState ItemState => itemState;
    #endregion


    #region === Setters ===
    public void SetItemKey(int value) => itemKey = value;
    public void SetItemCatalogKey(int value) => itemCatalogKey = value;
    public void SetGrade(Grade value) => grade = value;
    public void SetFlawEa(int value) => flawEa = value;
    public void SetSuspiciousFlawAura(float value) => suspiciousFlawAura = value;

    private void SetAuthenticity(char value)
    {
        authenticity = (value == 'Y');
    }

    private void SetIsAuthenticityFound(char value)
    {
        isAuthenticityFound = (value == 'Y');
    }
    public void SetItemImage(){
        itemImage = Resources.Load<Sprite>($"IMG_ITEM_CATALOG/{itemCatalogKey}");
        Debug.Log($"Loaded Image for ItemCatalogKey {itemCatalogKey} : {itemImage}");
    }
    #endregion


    #region === Getters (Utility) ===
    private char GetAuthenticityChar()
    {
        return authenticity ? 'Y' : 'N';
    }

    private char GetIsAuthenticityFoundChar()
    {
        Debug.Log("isAuthenticityFound: " + ItemState);
        return isAuthenticityFound ? 'Y' : 'N';
    }
    #endregion
}
