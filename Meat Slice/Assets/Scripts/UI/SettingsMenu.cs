using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    bool isSettingsMenuOpen = false; //Behaviour will reverse when bool switched.
    public GameObject mainMenu; //Reference to mainMenu UI game object to set Active true or false based on isSettingsMenuOpen.

    void Start()
    {
        ResetSettingsMenu();
    }

    //Sets the settings menu to origin.
    public void ResetSettingsMenu()
    {
        isSettingsMenuOpen = false;
        transform.localScale = Vector3.zero;
        mainMenu.SetActive(true);
    }

    //Button controlled open and close of settings menu.
    public void OpenSettingsMenu()
    {
        isSettingsMenuOpen = !isSettingsMenuOpen;
        transform.localScale = isSettingsMenuOpen ? Vector3.one : Vector3.zero;
        Time.timeScale = isSettingsMenuOpen ? 0 : 1;
        mainMenu.SetActive(isSettingsMenuOpen ? false : true);
        Score.iScore.gameObject.SetActive(isSettingsMenuOpen ? false : true);
    }
}
