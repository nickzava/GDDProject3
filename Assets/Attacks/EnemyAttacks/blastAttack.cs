using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blastAttack : Projectile
{
    protected void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Projectile hit something");
        //check the type of collision before exploding
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("wall"))
        {
            Hit(collision.gameObject.GetComponent<Player>());
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
        if (directHitTarget)
        {
            //directHitTakesDamage

            directHitTarget.Health -= 1;

        }

        Destroy(gameObject);
    }
}
