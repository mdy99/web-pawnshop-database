using UnityEngine;

public class PanelToggler : MonoBehaviour
{
    [SerializeField] private TutorialShower tutorialShower;

    

    public void OnToggleButtonClicked()
    {
        if(gameObject.activeSelf == false)
        {
            tutorialShower.SetIndexZero();
        }
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
