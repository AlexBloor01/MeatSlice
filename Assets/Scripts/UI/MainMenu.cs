using System.Collections;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    Coroutine mainMenuAnimation;
    public static MainMenu iMainMenu; //This script.
    private Image mainMenuBackground; //Main Menu background image attatched to this object.
    Color mainMenuBackgroundColour; //Colour attatched to mainMenuBackground so that it can be turned on and off later.
    private Color clear = new Color(1, 1, 1, 0); //This will transition the color along white colours of rather than Color.clear.
    private float transitionDuration = 1f; //Transition from background to clear.
    private float percentDisappearTime = 0.5f;

    private void Awake()
    {
        SetupVariables();
    }

    void SetupVariables()
    {
        iMainMenu = this;
        mainMenuBackground = GetComponent<Image>();
        mainMenuBackgroundColour = mainMenuBackground.color;
        Menus.UnHideMenu(gameObject);
    }

    public void UnHideMainMenuAnim()
    {
        if (mainMenuAnimation != null)
        {
            StopCoroutine(mainMenuAnimation);
        }
        mainMenuAnimation = StartCoroutine(MainMenuAnimation(false));
    }

    public void HideMainMenuAnim()
    {
        if (mainMenuAnimation != null)
        {
            StopCoroutine(mainMenuAnimation);
        }
        mainMenuAnimation = StartCoroutine(MainMenuAnimation(true));
    }

    IEnumerator MainMenuAnimation(bool bigToSmall)
    {
        if (bigToSmall == false) Menus.UnHideMenu(gameObject);

        Image mainMenuBackground = GetComponent<Image>();

        float elapsedTime = 0;
        float percent = 0;

        while (percent < 1)
        {
            elapsedTime += Time.unscaledDeltaTime;
            percent = elapsedTime / transitionDuration;

            if (bigToSmall)
            {
                mainMenuBackground.color = Color.Lerp(mainMenuBackgroundColour, clear, percent);
                if (percent > percentDisappearTime && transform.localScale.x != Vector3.zero.x)
                {
                    Menus.HideMenu(gameObject);
                }
            }
            else
            {
                mainMenuBackground.color = Color.Lerp(clear, mainMenuBackgroundColour, percent);
            }
            yield return null;
        }

        yield return null;
    }


}
