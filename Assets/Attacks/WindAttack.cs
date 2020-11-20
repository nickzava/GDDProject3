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
        
    }

    protected new void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<EnemyBase>() != null)
        {
            other.attachedRigidbody.AddForce(Quaternion.Inverse(other.transform.rotation) * new Vector3(1, 1, 0));
        }
    }
}
