using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHandler : MonoBehaviour
{
    // Holds references to relevant gameobjects
    UnityEngine.UI.Image heartObj;
    Player player;

    [SerializeField]
    Sprite full;
    [SerializeField]
    Sprite hurt;
    [SerializeField]
    Sprite dying;
    [SerializeField]
    Sprite dead;

	private int levelChange = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Fills references
        heartObj = GameObject.Find("Heart1").GetComponent<UnityEngine.UI.Image>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
		if (levelChange != NextLevel.currentLevel)
		{
			player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
			levelChange = NextLevel.currentLevel;
		}
        // Checks health and updates UI
        switch (player.Health)
        {
            case 3:
                heartObj.sprite = full;
                break;
            case 2:
                heartObj.sprite = hurt;
                break;
            case 1:
                heartObj.sprite = dying;
                break;
            case 0:
                heartObj.sprite = dead;
                break;
            default:
                Debug.LogError("HEALTH OUT OF BOUNDS");
                break;
        }
    }
}
