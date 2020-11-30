using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Wind attack now grows over time, doing less damage as it grows
public class WindAttack : BasicAttack
{
    public GameObject particleSystem;
    float initalRad = 0;
    SphereCollider sc;
    const float averageForce = 1000;

    // Start is called before the first frame update
    void Start()
    {
        sc = gameObject.GetComponent<SphereCollider>();
        Instantiate(particleSystem, transform.position, transform.rotation)
            .GetComponent<ParticleSystem>().Play();
        initalRad = sc.radius;
        sc.radius = 0;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        //grow rad to max rad based on elapsed time
        sc.radius = initalRad * elapsedTime / hitboxTime;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("enemy"))
        {
            other.GetComponent<EnemyBase>().Stun(.5f);
            Vector3 forceDirection = other.transform.position - transform.position;
            //increase force as hitbox grows
            other.attachedRigidbody.AddForce(
                new Vector3(forceDirection.x,0,forceDirection.z).normalized 
                * Mathf.Min(averageForce * (hitboxTime - elapsedTime)/hitboxTime , averageForce) );
        }
    }
}
