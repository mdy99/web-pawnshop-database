using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class ConfirmPopuper : MonoBehaviour
{
    private static ConfirmPopuper instance = null;

    [SerializeField] GameObject popupUI=null;
    [SerializeField] Canvas mainCanvas=null;

    private GameObject popupInstance;

    public void PopupCheckPanel(string log, Action action = null)
    {
        popupUI.transform.GetChild(3).GetComponent<TMP_Text>().text = log;
        popupInstance=Instantiate(popupUI, mainCanvas.transform);
        popupInstance.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(CloseCheckPanel);
        if(action != null)
        {
            popupInstance.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(()=>action.Invoke());
        }
    }


    void OnEnable()
    {
        SceneManager.sceneLoaded += SetMainCanvas;
    }

    void SetMainCanvas(Scene scene, LoadSceneMode mode)
    {
        if((mainCanvas = AYellowpaper.SerializedCollections.SingletonManager.Instance.getMainCanvas()) == null)
        {
            Debug.LogError("can't get mainCanvas by SingletonManager. check ConfirmPopuper");            
        }
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= SetMainCanvas;
    }

    void CloseCheckPanel()
    {
        Destroy(popupInstance);
    }

        void Awake()
    {
        if(null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        // if((popupUI = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/CHECK_POPUP.prefab", typeof(GameObject)))==null)
        if((popupUI = Resources.Load<GameObject>("Prefabs/CHECK_POPUP"))==null)
        {
            Debug.LogError("there's no CHECK_POPUP.prefab");
        }
        if((mainCanvas = AYellowpaper.SerializedCollections.SingletonManager.Instance.getMainCanvas()) == null)
        {
            Debug.LogError("can't get mainCanvas by SingletonManager. check ConfirmPopuper");            
        }

        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static ConfirmPopuper Instance
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
}
