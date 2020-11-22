using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindAttack : BasicAttack
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("enemy"))
        {
            other.attachedRigidbody.AddForce(-(other.transform.forward) * 1000);
        }
    }
}
