using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {
    public GameObject pauseUI;

    //private bool paused = false;

    void Start()
    {
        //turns off pause menu when game starts
        pauseUI.SetActive(false);
    }

    void Update()
    {
        //turns on and off pause
        if (Input.GetButtonDown("Pause"))
        {
            GameManager.Paused = !GameManager.Paused;
        }

        if (GameManager.Paused)
        {
            pauseUI.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            pauseUI.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void Resume()
    {
		GameManager.Paused = false;
		Time.timeScale = 1;
	}

    public void Restart()
    {
		Time.timeScale = 1;
		Application.LoadLevel(Application.loadedLevel);
    }

    public void MainMenu()
    {
		Time.timeScale = 1;
		Application.LoadLevel(0);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
