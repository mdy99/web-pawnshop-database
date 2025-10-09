using UnityEngine;

public class DBMessenger : MonoBehaviour
{
    private static DBMessenger instance = null;

    public static DBMessenger Instance{
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

    public void SendToDB(){

        // switch(){

        // }

    }

    public void RecvFromDB(){

    }

}
