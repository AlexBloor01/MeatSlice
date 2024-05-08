using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainMenu : MonoBehaviour
{
    public static MainMenu iMainMenu;
    [SerializeField] private FoodController foodController;

    private void Awake()
    {
        iMainMenu = this;
    }

    public void ReturnToMainMenu()
    {
        transform.localScale = Vector3.one;
    }

    public IEnumerator ExitMainMenuAnim(Action startGame)
    {
        transform.localScale = Vector3.zero;

        yield return new WaitForSeconds(0.1f);

        startGame();
        yield return null;
    }

}
