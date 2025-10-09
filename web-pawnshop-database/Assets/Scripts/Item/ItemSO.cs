using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    ItemCatalogSO itemCatalog;
    string hiddenGrade;
    int hiddenFlow;
    bool authenticity;
}
