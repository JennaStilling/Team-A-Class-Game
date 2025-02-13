using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;

    public static bool isPaused;

    public GameObject player; // Reference to your player object
    public MonoBehaviour PlayerMovement; // Reference to the script controlling movement (e.g., FirstPersonController)

    void Start()
    {
        pauseMenu.SetActive(false);
        isPaused = false;  // Reset pause state
        Time.timeScale = 1f;  // Ensure game is running at normal speed
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        // Show the cursor
        Cursor.lockState = CursorLockMode.None; // Unlocks the cursor
        Cursor.visible = true; // Makes the cursor visible

        // Disable player movement
        PlayerMovement.enabled = false; // Disables the player movement script
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        // Hide the cursor
        Cursor.lockState = CursorLockMode.Locked; // Locks the cursor back to the center
        Cursor.visible = false; // Hides the cursor

        // Enable player movement
        PlayerMovement.enabled = true; // Enables the player movement script again
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        isPaused = false;
    }

    public void QuitGame()
    {
    #if UNITY_EDITOR
    // If running in the Unity Editor, stop playing the scene
    UnityEditor.EditorApplication.isPlaying = false;
    #else
    // If running as a built application, quit the game
    Application.Quit();
    #endif
    }
}
