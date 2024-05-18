using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu iMainMenu; //This script.
    public Image mainMenuBackground; //Main Menu background image attatched to this object.
    public Color mainMenuBackgroundColour; //Colour attatched to mainMenuBackground so that it can be turned on and off later.
    public Color clear = new Color(1, 1, 1, 0); //This will transition the color along white colours of rather than Color.clear.
    private float transitionDuration = 0.5f; //Transition from background to clear.

    private void Awake()
    {
        SetupVariables();
    }

    void SetupVariables()
    {
        iMainMenu = this;
        mainMenuBackground = GetComponent<Image>();
        mainMenuBackgroundColour = mainMenuBackground.color;
    }

    public void ReturnToMainMenu()
    {
        mainMenuBackground.color = mainMenuBackgroundColour;
        transform.localScale = Vector3.one;
    }

    public void CloseMainMenu()
    {
        StartCoroutine(CloseMainMenuAnimation());
    }

    IEnumerator CloseMainMenuAnimation()
    {
        Image mainMenuBackground = GetComponent<Image>();
        mainMenuBackground.color = mainMenuBackgroundColour;

        float elapsedTime = 0;
        float percentage = 0;

        while (percentage <= 1)
        {
            elapsedTime += Time.deltaTime;
            percentage = elapsedTime / transitionDuration;
            mainMenuBackground.color = Color.Lerp(mainMenuBackgroundColour, clear, percentage);
            yield return null;
        }

        transform.localScale = Vector3.zero;
        yield return null;
    }


}
