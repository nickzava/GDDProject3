using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
	private GameObject mainMenuUI;
	private GameObject creditsUI;

	private GameObject pauseUI;
	private bool paused = false;
	public bool pausable = true;
	private GameObject deathUI;
	private GameObject winUI;

	public GameObject[] levels;

	private GameObject player;

	private void Awake()
	{
		if (SceneManager.GetActiveScene().name == "_MenuScene")
		{
			mainMenuUI = GameObject.Find("MainMenuUI");
			creditsUI = GameObject.Find("CreditsUI");
		}
		else if (SceneManager.GetActiveScene().name == "_Main")
		{ 
			pauseUI = GameObject.Find("PauseMenu");
			deathUI = GameObject.Find("DeathMenu");
			winUI = GameObject.Find("WinMenu");
			player = GameObject.FindGameObjectWithTag("Player");

			for (int i = 0; i < levels.Length; i++)
			{
				if (i != NextLevel.currentLevel)
				{
					levels[i].SetActive(false);
				}
				else
				{
					levels[i].SetActive(true);
				}
			}
		}
	}

	// Start is called before the first frame update
	void Start()
    {
		if (SceneManager.GetActiveScene().name == "_MenuScene")
		{
			creditsUI.SetActive(false);
		}
		else if (SceneManager.GetActiveScene().name == "_Main")
		{
			pauseUI.SetActive(false);
			deathUI.SetActive(false);
			winUI.SetActive(false);

			//for (int i = 1; i < levels.Length; i++)
			//{
			//	levels[i].SetActive(false);
			//}
		}
	}

    // Update is called once per frame
    void Update()
    {
		if (pausable)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (paused)
					UnPause();
				else
					PauseGame();
			}
		}
    }

	//enters main scene
	public void StartGame()
	{
		SceneManager.LoadScene("_Main", LoadSceneMode.Single);
		NextLevel.currentLevel = 0;
		for (int i = 1; i < levels.Length; i++)
		{
			levels[i].SetActive(false);
		}
		Time.timeScale = 1;
		//player.GetComponent<Player>().paused = false;
		paused = false;
	}

	public void RestartLevel()
	{
		SceneManager.LoadScene("_Main", LoadSceneMode.Single);
		for (int i = 0; i < levels.Length; i++)
		{
			if (i != NextLevel.currentLevel)
			{
				levels[i].SetActive(false);
			}
			else
			{
				levels[i].SetActive(true);
			}
		}
		Time.timeScale = 1;
		paused = false;
	}

	//opens credit UI
	public void OpenCredits()
	{
		creditsUI.SetActive(true);
		mainMenuUI.SetActive(false);
	}

	//closes credit UI
	public void ReturnToMainMenu()
	{
		mainMenuUI.SetActive(true);
		creditsUI.SetActive(false);
	}

	//exits main scene, returns to menu
	public void ExitToMainMenu()
	{
		SceneManager.LoadScene("_MenuScene", LoadSceneMode.Single);
	}

	//exits game completely
	public void QuitGame()
	{
		Application.Quit();
	}

	//pauses game, brings up pause menu
	public void PauseGame()
	{
		pauseUI.SetActive(true);
		Time.timeScale = 0;
		player.GetComponent<Player>().paused = true;
		paused = true;
	}

	//unpauses game, closes pause menu
	public void UnPause()
	{
		pauseUI.SetActive(false);
		Time.timeScale = 1;
		player.GetComponent<Player>().paused = false;
		paused = false;
	}

	public void PlayerDeathUI()
	{
		deathUI.SetActive(true);
		Time.timeScale = 0;
		player.GetComponent<Player>().paused = true;
		pausable = false;
	}
}
