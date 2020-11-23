using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : BasePickup
{
	// Start is called before the first frame update
	void Start()
	{
		base.Start();
	}

	// Update is called once per frame
	void Update()
	{
		base.Update();
	}

	protected override void OnTriggerEnter(Collider other)
	{
		if (player.GetComponent<Player>().Health < 3)
		{
			player.GetComponent<Player>().Health += 1;
		}
	}
}
