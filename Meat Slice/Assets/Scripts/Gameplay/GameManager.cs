using UnityEngine;
using System;
using Palmmedia.ReportGenerator.Core;

public class GameManager : MonoBehaviour
{
    public static GameManager iGameManager; //Reference to self.
    public LevelSetup levelSetup; //Reference to LevelSetup in scene.
    public SettingsMenu settingsMenu; //Reference to SettingsMenu in scene.
    public GameCompleteMenu gameCompleteMenu; //Reference to GameCompleteMenu in scene.
    public static bool playedBefore = false; //Has game been played before?
    public static bool gameOver = true; //Has the game Finished?
    public static bool slowMode = true; //Allow the game to be slowed down. 
    public static float slowSpeed = 0.1f; //Speed of slowmo, must be below 1 to go slow.
    public const int scaleFactor = 10; //Scale of each base object on the x and y axis.


    public AnimationCurve btnAnimCurve;
    float btnLerpDuration = 0.5f;

    private void Awake()
    {
        SetupVariables();
    }

    //Sets up script.
    //Must be played on Awake.
    void SetupVariables()
    {
        iGameManager = this;

        if (levelSetup == null)
        {
            levelSetup = FindObjectOfType<LevelSetup>();
        }

        playedBefore = PlayerPrefs.GetInt("Played_Before") != 0;

        if (playedBefore)
        {
            Debug.Log("Played Before");
        }
        else
        {
            Debug.Log("Not Played Before");
            PlayerPrefs.SetInt("Played_Before", 1);
        }

        gameOver = true;
    }

    //Restarts the game.
    //Access this either through UI Button or script.
    public void RestartGame()
    {
        Time.timeScale = 1;
        settingsMenu.ResetSettingsMenu();
        gameOver = true;
        gameOver = false;
        Action startAction = MainMenu.iMainMenu.HideMainMenuAnim;
        startAction += levelSetup.SetupLevel;
        StartCoroutine(CloudTransition.iMenuTransition.MenuTransition(startAction, null, levelSetup.StartLevel, 0));
    }

    //Ends the game.
    //Access this either through script only.
    public void GameOver()
    {
        gameOver = true;
        gameCompleteMenu.OpenGameCompleteMenu();
    }

    //Returns main menu.
    //Access this either throug UI Button in settings menu.
    public void ReturnToMenu()
    {
        if (settingsMenu.isSettingsMenuOpen && gameOver)
        {
            settingsMenu.ResetSettingsMenu();
            Menus.UnHideMenu(MainMenu.iMainMenu.gameObject);
            return;
        }

        gameOver = true;
        settingsMenu.ResetSettingsMenu();
        StartCoroutine(CloudTransition.iMenuTransition.MenuTransition(MainMenu.iMainMenu.UnHideMainMenuAnim, null, null, 0));
    }

    //Animation for button clicks, place on UI buttons.
    public void ButtonClicked(GameObject obj)
    {
        StartCoroutine(MovementLibrary.AnimationCurveLerp(obj, btnLerpDuration, Vector3.zero, Vector3.one, false, MovementLibrary.ObjectLerpType.LocalScale, btnAnimCurve, null));
    }

    //When button clicked, hide a gameObject. 
    //Use Menus.HideMenu(obj); for unhiding objects in script and this for UI Buttons.
    public void HideMenu(GameObject obj)
    {
        Menus.HideMenu(obj);
    }

    //When button clicked, hide a gameObject. 
    //Use Menus.UnHideMenu(obj); for unhiding objects in script and this for UI Buttons.
    public void UnHideMenu(GameObject obj)
    {
        Menus.UnHideMenu(obj);
    }

    //Exits the game application.
    public void ExitGame()
    {
        Application.Quit();
    }
}
