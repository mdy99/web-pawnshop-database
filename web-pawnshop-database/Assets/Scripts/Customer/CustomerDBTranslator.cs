using UnityEngine;

public class CustomerDBTranslator : MonoBehaviour
{
    private static CustomerDBTranslator instance = null;

    public static CustomerDBTranslator Instance{
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

    void ConvertCustomerSoToDB(){

    }

    void ConvertDBToCustomerSO(){
        
    }
}
