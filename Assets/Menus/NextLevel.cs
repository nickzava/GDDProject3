using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevel : MonoBehaviour
{
	public GameObject[] levels;
	public static int currentLevel = 0;
	public GameObject winUI;

	private GameObject player;
	public GameObject tutorialTipUI;

	private void Awake()
	{
		DontDestroyOnLoad(this);
		//winUI = GameObject.Find("WinMenu");
	}

	// Start is called before the first frame update
	void Start()
    {
		player = GameObject.FindGameObjectWithTag("Player");
		//for(int i = 1; i < levels.Length; i++)	//di
		//{
		//	if (i != currentLevel)
		//	{
		//		levels[i].SetActive(false);
		//	}
		//}
	}

    // Update is called once per frame
    void Update()
    {
		
    }

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			SetActiveAllChildren(levels[currentLevel].transform, false);
			tutorialTipUI.SetActive(true);
			currentLevel++;

			if (currentLevel != levels.Length)	//if not on final level
			{
				SetActiveAllChildren(levels[currentLevel].transform, true);
				Camera.main.GetComponent<CameraFollowPlayer>().player = GameObject.FindGameObjectWithTag("Player");
			}
			else			//if on final level
			{
				winUI.SetActive(true);
				Time.timeScale = 0;
				player.GetComponent<Player>().paused = true;
				Camera.main.GetComponent<Buttons>().pausable = false;
			}
		}
	}



	private void SetActiveAllChildren(Transform transform, bool value)
	{
		transform.gameObject.SetActive(value);
		foreach (Transform child in transform)
		{
			SetActiveAllChildren(child, value);
			child.gameObject.SetActive(value);
		}
	}
}
