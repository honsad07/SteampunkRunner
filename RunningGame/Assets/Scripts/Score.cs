using System;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI deathScreenScoreText;
    public float score = 0;
    public float newScore = 0;
    public float multiplier = 1;

    private GameTimer timer;

    private void Start()
    {
        timer = FindFirstObjectByType<GameTimer>();
    }

    public void Update()
    {
        newScore = 5000 - 40 * (99.999f - timer.timeRemaining);
        scoreText.text = $"Score: {Mathf.Floor(score)}";
        deathScreenScoreText.text = $"Score: {Mathf.Floor(score)}";
        currentScoreText.text = $"+{Mathf.Floor(newScore)}({Math.Round(multiplier, 2)}x)";
    }
}
