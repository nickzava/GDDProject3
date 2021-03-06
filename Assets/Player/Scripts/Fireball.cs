﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Projectile
{

    public GameObject attack;
    protected override void OnCollisionEnter(Collision collision)
    {
        //check the type of collision before exploding
        
        if(collision.collider.CompareTag("enemy") || collision.collider.CompareTag("floor") || collision.collider.CompareTag("wall"))
        {
            Explode(collision.gameObject.GetComponent<EnemyBase>());
        }
    }

    private void Explode(EnemyBase directHitTarget = null)
    {
        

        //Do bonus damage to direct hits
        if (directHitTarget)
        {
            directHitTarget.health -= 2;           

        }
        //Do fireball things        


        Vector3 tempVec = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        
        attack = Instantiate(attack, tempVec, Quaternion.identity);

        attack.GetComponent<FireballAttack>().init(gameObject, 1, 1, 1f);

        //destroy the projectile

        Destroy(gameObject);
    }
}
