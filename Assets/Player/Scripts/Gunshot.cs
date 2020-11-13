using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gunshot : Projectile
{
    protected override void OnCollisionEnter(Collision collision)
    {
        //check the type of collision before exploding
        if (collision.collider.CompareTag("enemy") || collision.collider.CompareTag("floor"))
        {
            Hit(collision.gameObject.GetComponent<EnemyBase>());
        }
    }

    private void Hit(EnemyBase directHitTarget = null)
    {
        //Do bonus damage to direct hits
        if (directHitTarget)
        {
            //directHitTakesDamage

            //Do fireball things


            //destroy the projectile
            Destroy(gameObject);
        }
    }
}

