using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager iGameManager; //Reference to self.
    public CloudTransition cloudTransition; //Reference to CloudTransition in scene.
    public LevelSetup levelSetup; //Reference to LevelSetup in scene.
    public static bool playedBefore = false; //Has game been played before?
    public static bool gameOver = false; //Has the game Finished?
    public static bool slowMode = true; //Slow game when within 10
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
    }

    //Restarts the game.
    public void RestartGame()
    {
        gameOver = false;
        Action startAction = MainMenu.iMainMenu.CloseMainMenu;
        startAction += levelSetup.SetupLevel;

        StartCoroutine(CloudTransition.iMenuTransition.MenuTransition(startAction, null, null, 0));
    }

    //Ends the game.
    public void GameOver()
    {
        gameOver = true;
        StartCoroutine(CloudTransition.iMenuTransition.MenuTransition(MainMenu.iMainMenu.ReturnToMainMenu, null, null, 0));
    }

    //Returns main menu.
    public void ReturnToMenu()
    {
        GameOver();
        levelSetup.ResetHolder();
        levelSetup.ResetChoppingboard();
    }

}
