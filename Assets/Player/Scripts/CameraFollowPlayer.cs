using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
	public GameObject player;
	public float camDistance = -10.0f;

    // Start is called before the first frame update
    void Start()
    {
		player = GameObject.FindGameObjectWithTag("Player");
	}

    // Update is called once per frame
    void Update()
    {
		if (player != null)
		{
			transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -camDistance);
		}
    }
}
