using UnityEngine;

public class ItemDBTranslator : MonoBehaviour
{
    private static ItemDBTranslator instance = null;

    public static ItemDBTranslator Instance{
        get{
            if(instance == null){
                return null;
            }
            else{
            return instance;
            }
        }
    }

    void Awake(){
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else{
            Destroy(this.gameObject);
        }
    }

    void ConvertItemSoToDB(){

    }

    void ConvertDBToItemSO(){
        
    }
}
