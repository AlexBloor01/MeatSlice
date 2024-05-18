using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public static Score iScore;
    public TextMeshProUGUI scoreTextElement;
    int score;

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
        score = 0;
    }

    public void AddScore()
    {
        score++;
        scoreTextElement.text = score.ToString();
    }

    public void ResetScore()
    {
        score = 0;
        scoreTextElement.text = "0";
    }
}
