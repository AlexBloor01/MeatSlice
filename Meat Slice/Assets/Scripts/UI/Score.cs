using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public static Score iScore;
    public TextMeshProUGUI scoreTextElement;
    public int score;
    public int meatCounter;
    public int cheeseCounter;
    public int vegetableCounter;
    public int tofuCounter;

    void Start()
    {
        SetupVariables();
    }

    void SetupVariables()
    {
        iScore = this;
        if (scoreTextElement == null)
        {
            scoreTextElement = GetComponent<TextMeshProUGUI>();
        }
        ResetScore();
    }

    public void AddScore(FoodType foodType)
    {
        switch (foodType)
        {
            case FoodType.Meat:
                meatCounter++;
                break;

            case FoodType.Cheese:
                cheeseCounter++;
                break;

            case FoodType.Vegetables:
                vegetableCounter++;
                break;

            case FoodType.VeganProtein:
                tofuCounter++;
                break;

            case FoodType.None:
                break;
        }

        score++;
        UpdateScoreText();
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreText();

        meatCounter = 0;
        cheeseCounter = 0;
        vegetableCounter = 0;
        tofuCounter = 0;
    }

    void UpdateScoreText()
    {
        scoreTextElement.text = score.ToString();
    }
}
