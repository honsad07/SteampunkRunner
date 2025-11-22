using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager  : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public bool isPaused = false;

    public void OnPause(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (isPaused) Resume();
        else Pause();
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }
}