using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballAttack : BasicAttack
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    protected override void Update()
    {
        if (elapsedTime < hitboxTime || hitboxTime == -1f)
        {
            // Updates the properties of the box to follow the source
        }
        else
        {
            Destroy(gameObject);
        }
        elapsedTime += Time.deltaTime;
    }

    // Update is called once per frame

    protected new void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<EnemyBase>() != null)
        {
           
            other.attachedRigidbody.AddForce(Quaternion.Inverse(other.transform.rotation) * new Vector3(1, 1, 0));
            other.gameObject.GetComponent<EnemyBase>().health -= 1;
            
        }
    }
}
