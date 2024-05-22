using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public bool isSettingsMenuOpen = false; //Behaviour will reverse when bool switched.
    public GameObject settingButton; //Reference to the scenes UI settings button for hiding it.
    float lerpDuration = 1f; // Hide Settings Button Animation lerp duration.


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

        if (isSettingsMenuOpen)
        {
            Menus.HideMenu(MainMenu.iMainMenu.gameObject);
            Menus.HideMenu(Score.iScore.gameObject);
            Menus.UnHideMenu(gameObject);
            Time.timeScale = 0;
        }
        else
        {
            Menus.HideMenu(gameObject);
            Menus.UnHideMenu(Score.iScore.gameObject);
            Time.timeScale = 1;

            //Hiding the main menu depends on if the game is over or not.
            if (GameManager.gameOver == true)
            {
                Menus.UnHideMenu(MainMenu.iMainMenu.gameObject);
            }
        }
    }

    //Animation for hiding the settings button.
    public void HideSettingsButtonAnim()
    {
        StartCoroutine(MovementLibrary.AnimationCurveLerp(settingButton, lerpDuration, Vector3.one, Vector3.zero, false, MovementLibrary.ObjectLerpType.LocalScale, GameManager.iGameManager.btnAnimCurve, null));
    }

    //Animation for unhiding the settings button.
    public void UnhideSettingsButtonAnim()
    {
        StartCoroutine(MovementLibrary.AnimationCurveLerp(settingButton, lerpDuration, Vector3.zero, Vector3.one, false, MovementLibrary.ObjectLerpType.LocalScale, GameManager.iGameManager.btnAnimCurve, null));
    }
}
