using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowTextBox : MonoBehaviour
{
	private GameObject tutorialTip;	//the tip UI image and text
	private GameObject player;		//player for pausing
	public string tutorialText;		//the text to put in the tip box
	private Text tipText;

	private void Awake()
	{
		tutorialTip = GameObject.Find("TutorialTipUI");
		tipText = GameObject.Find("TipText").GetComponent<Text>();
		player = GameObject.FindGameObjectWithTag("Player");
	}

	// Start is called before the first frame update
	void Start()
    {
		tutorialTip.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			tutorialTip.SetActive(true);
			tipText.text = tutorialText;
			player.GetComponent<Player>().paused = true;
			Time.timeScale = 0;
		}
	}

	public void CloseTip()
	{
		tutorialTip.SetActive(false);
		Time.timeScale = 1;
		player.GetComponent<Player>().paused = false;
		this.gameObject.SetActive(false);
	}
}
