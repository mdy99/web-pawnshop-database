using UnityEngine;

public class ToggleObjs : MonoBehaviour
{
    public GameObject objToToggle;
    public void Toggle(){
        objToToggle.SetActive(!objToToggle.activeSelf);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
