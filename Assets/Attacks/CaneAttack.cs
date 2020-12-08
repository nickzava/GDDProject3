using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaneAttack : BasicAttack
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


    protected virtual void OnTriggerEnter(Collider other)
    {
        // If hit object is player, do damage
        if (other.GetComponentInParent<Player>() != null)
        {
            // Reduce health here other.GetComponentInParent<Player>().
        }
        else if (other.gameObject.CompareTag("enemy"))
        {
            other.gameObject.GetComponent<EnemyBase>().health -= 1;
            other.GetComponent<EnemyBase>().Stun(.2f);
            Vector3 forceDirection = other.transform.position - transform.position;
            //increase force as hitbox grows
            other.attachedRigidbody.AddForce(
                new Vector3(forceDirection.x, forceDirection.y).normalized * 1000f);
        }
    }
}
