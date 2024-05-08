using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager iGameManager;
    public LevelSetup levelSetup;
    public static bool gameOver = false; //Has the game Finished?
    public static bool slowMode = true; //Slow game when within 10
    public static float slowSpeed = 0.1f; //Speed of slowmo, must be below 1 to go slow.

    private void Awake()
    {
        SetupVariables();
    }

    void SetupVariables()
    {
        iGameManager = this;

        if (levelSetup == null)
        {
            levelSetup = FindObjectOfType<LevelSetup>();
        }
    }

    public void RestartGame()
    {
        gameOver = false;
        Action startingActions = levelSetup.SetupLevel;
        StartCoroutine(MainMenu.iMainMenu.ExitMainMenuAnim(startingActions));
    }

    //Ends the game.
    public static void GameOver()
    {
        gameOver = true;
        MainMenu.iMainMenu.ReturnToMainMenu();
    }


}
