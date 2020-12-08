using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blastAttack : Projectile
{
    protected void OnTriggerEnter(Collider collision)
    {
        //check the type of collision before exploding
        if (collision.gameObject.CompareTag("Player"))
        {
            Hit(collision.gameObject.GetComponent<Player>());
        } else if(collision.gameObject.CompareTag("Untagged"))
        {
            Destroy(gameObject);
        }
        
    }

    protected override void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
    }

    private void Hit(Player directHitTarget = null)
    {
        //Do bonus damage to direct hits
        if (directHitTarget && directHitTarget.invincible == false)
        {
            //directHitTakesDamage

            directHitTarget.Health -= 1;

        }

        Destroy(gameObject);
    }
}
