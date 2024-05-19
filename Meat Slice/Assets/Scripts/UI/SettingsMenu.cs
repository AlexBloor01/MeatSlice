using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public bool isSettingsMenuOpen = false; //Behaviour will reverse when bool switched.

    //Settings button references.
    public GameObject settingButton; //Reference to the scenes UI settings button for hiding it.
    float btnLerpDuration = 1;
    public AnimationCurve btnAnimCurve;


    void Start()
    {
        ResetSettingsMenu();
    }

    //Sets the settings menu to origin.
    public void ResetSettingsMenu()
    {
        isSettingsMenuOpen = false;
        transform.localScale = Vector3.zero;
    }

    //Button controlled open and close of settings menu.
    public void OpenSettingsMenu()
    {
        isSettingsMenuOpen = !isSettingsMenuOpen;
        transform.localScale = isSettingsMenuOpen ? Vector3.one : Vector3.zero;
        Time.timeScale = isSettingsMenuOpen ? 0 : 1;
        Score.iScore.gameObject.SetActive(isSettingsMenuOpen ? false : true);
        MainMenu.iMainMenu.HideMainMenu();

        if (isSettingsMenuOpen == false)
        {
            if (GameManager.gameOver == false)
            {
                MainMenu.iMainMenu.HideMainMenu();
            }
            else
            {
                MainMenu.iMainMenu.UnHideMainMenu();
            }
        }
    }

    public void ClickedSettingsButton()
    {
        StartCoroutine(MovementLibrary.AnimationCurveLerp(settingButton, btnLerpDuration / 3, Vector3.zero, Vector3.one, false, MovementLibrary.ObjectLerpType.LocalScale, btnAnimCurve, null));
    }

    public void HideSettingsButton()
    {
        StartCoroutine(MovementLibrary.AnimationCurveLerp(settingButton, btnLerpDuration, Vector3.one, Vector3.zero, false, MovementLibrary.ObjectLerpType.LocalScale, btnAnimCurve, null));
    }

    public void UnhideSettingsButton()
    {
        StartCoroutine(MovementLibrary.AnimationCurveLerp(settingButton, btnLerpDuration, Vector3.zero, Vector3.one, false, MovementLibrary.ObjectLerpType.LocalScale, btnAnimCurve, null));
    }
}
