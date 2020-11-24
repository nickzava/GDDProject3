using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePickup : MonoBehaviour
{
	protected GameObject player;

    // Start is called before the first frame update
    protected void Start()
    {
		player = GameObject.FindGameObjectWithTag("Player");
	}

    // Update is called once per frame
    protected void Update()
    {
        
    }

	protected virtual void OnTriggerEnter(Collider other)
	{
		Object.Destroy(this.gameObject);	//to be called after pickup has done it's effect
	}
}
