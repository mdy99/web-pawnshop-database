using TMPro;
using UnityEngine;

public class TutorialShower : MonoBehaviour
{
    [SerializeField] private TMP_Text tutorialText;
    int currentIndex = 0;
    string[] tutorialTexts = new string[]
    {
        "아앙",
        "기모띠"
    };

    public void SetIndexZero()
    {
        currentIndex = 0;
        tutorialText.text = tutorialTexts[currentIndex];
    }

    public void OnNextButtonClicked()
    {
        if(currentIndex < tutorialTexts.Length-1)
        {
            ++currentIndex;    
        }
        tutorialText.text = tutorialTexts[currentIndex];
    }
    public void OnPrevButtonClicked()
    {
        if(currentIndex > 0)
        {
            --currentIndex;    
        }
        tutorialText.text = tutorialTexts[currentIndex];        
    }
}
