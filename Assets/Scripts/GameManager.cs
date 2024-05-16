using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //This script manages the game from levels, to score, to camera.
    //TODO: REMAKE FOR BETTER SCENE MANAGER
    public int currentLevel;
    public bool isPaused;
    public GameObject pauseMenu;

    private void Start()
    {
        isPaused = false;
        Unpause();
    }

    //Resets the current level the player is on
    public void Reset()
    {
        SceneManager.LoadScene(currentLevel);
    }

    //Loads a specific level
    public void Load(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    //Returns to the main menu to the player can exit the game.
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void Update()
    {
        //If the player presses the escape button, pause the game
        if (Input.GetKeyDown("escape"))
        {
            //If the game is paused
            if (isPaused)
            {
                //Unpause
                isPaused = false;
                Unpause();
            }
            //If the game is not paused
            else
            {
                //Pause
                isPaused = true;
                Pause();
            }
        }
    }

    void Unpause()
    {
        //Resume time
        Time.timeScale = 1;
        
        //Deactivate the pause menu
        pauseMenu.SetActive(false);
    }

    void Pause()
    {
        //Stop time
        Time.timeScale = 0;
        
        //Activate the pause menu
        pauseMenu.SetActive(true);
    }
}
