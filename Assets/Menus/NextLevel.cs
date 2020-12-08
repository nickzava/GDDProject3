using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevel : MonoBehaviour
{
	public GameObject[] levels;
	public static int currentLevel;

    // Start is called before the first frame update
    void Start()
    {
		currentLevel = 0;
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
			currentLevel++;
			SetActiveAllChildren(levels[currentLevel].transform, true);
			Camera.main.GetComponent<CameraFollowPlayer>().player = GameObject.FindGameObjectWithTag("Player");
		}
	}



	private void SetActiveAllChildren(Transform transform, bool value)
	{
		transform.gameObject.SetActive(value);
		foreach (Transform child in transform)
		{
			child.gameObject.SetActive(value);

			SetActiveAllChildren(child, value);
		}
	}
}
