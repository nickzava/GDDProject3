using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Projectile
{

    public GameObject attack;
    protected override void OnCollisionEnter(Collision collision)
    {
        //check the type of collision before exploding
        
        if(/*collision.collider.CompareTag("enemy") ||*/ collision.collider.CompareTag("floor"))
        {
            Explode(collision.gameObject.GetComponent<EnemyBase>());
        }
    }

    private void Explode(EnemyBase directHitTarget = null)
    {
        GameObject explosion = Instantiate(attack, gameObject.transform);
        //Do bonus damage to direct hits
        explosion.GetComponent<BasicAttack>().init(gameObject, 1, 1, new Vector3(1, 1, 1), 1f);
        if (directHitTarget)
        {
            directHitTarget.health -= 1;           

        }
        //Do fireball things        

        Debug.Log("we in here");
        //destroy the projectile
        Destroy(gameObject);
        
    }
}
