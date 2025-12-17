using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public float timeRemaining = 99f;
    public TextMeshProUGUI timerText;
    public bool isRunning = true;
    public bool isDead = false;

    public GameObject loseScreenPanel;

    private Score scoreScript; 

    private void Start()
    {
        scoreScript = FindFirstObjectByType<Score>();
    }

    void Update()
    {
        if (!isRunning) return;
    
        timeRemaining -= Time.deltaTime;
    
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            isRunning = false;
            
            HandleLose(); 
        }
    
        DisplayTimer();
    }
    
    private void HandleLose()
    {
        isDead = true;
        if (loseScreenPanel != null)
        {
            loseScreenPanel.SetActive(true);
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        scoreScript.newScore = 0;
    }

    private void DisplayTimer()
    {
        int seconds = Mathf.FloorToInt(timeRemaining);
        int hundredths = Mathf.FloorToInt((timeRemaining - seconds) * 100);

        timerText.text = $"{seconds:00}:{hundredths:00}";
    }
    
    public void ResetTimer(float newTime)
    {
        timeRemaining = newTime;
        isRunning = false;
        DisplayTimer();
    }
}