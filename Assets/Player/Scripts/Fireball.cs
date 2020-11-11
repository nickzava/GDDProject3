using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Projectile
{
    protected override void OnCollisionEnter(Collision collision)
    {
        //check the type of collision before exploding
        if(collision.collider.CompareTag("enemy") || collision.collider.CompareTag("floor"))
        {
            Explode(collision.gameObject.GetComponent<EnemyBase>());
        }
    }

    private void Explode(EnemyBase directHitTarget = null)
    {
        //Do bonus damage to direct hits
        if(directHitTarget)
            //directHitTakesDamage

        //Do fireball things


        //destroy the projectile
        Destroy(gameObject);
    }
}
