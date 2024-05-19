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
    public void RestartGame()
    {
        // if (settingsMenu.isSettingsMenuOpen && gameOver)
        // {
        //     settingsMenu.ResetSettingsMenu();
        //     return;
        // }

        settingsMenu.ResetSettingsMenu();
        gameOver = true;
        gameOver = false;
        Action startAction = MainMenu.iMainMenu.HideMainMenuAnim;
        startAction += levelSetup.SetupLevel;
        StartCoroutine(CloudTransition.iMenuTransition.MenuTransition(startAction, null, levelSetup.StartLevel, 0));
    }

    //Ends the game.
    public void GameOver()
    {
        gameOver = true;
        gameCompleteMenu.OpenGameCompleteMenu();
    }

    //Returns main menu.
    public void ReturnToMenu()
    {
        if (settingsMenu.isSettingsMenuOpen && gameOver)
        {
            settingsMenu.ResetSettingsMenu();
            MainMenu.iMainMenu.UnHideMainMenu();
            return;
        }

        gameOver = true;
        settingsMenu.ResetSettingsMenu();
        StartCoroutine(CloudTransition.iMenuTransition.MenuTransition(MainMenu.iMainMenu.UnHideMainMenuAnim, null, null, 0));
    }
}
