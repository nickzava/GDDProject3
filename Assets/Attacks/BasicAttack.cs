using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : MonoBehaviour
{
    // Stats of the attack
    protected int damage;
    protected float dist;
    protected GameObject source;
    protected float hitboxTime;
    protected float elapsedTime;
    // Reference to the hitbox trigger
    Collider hitBox;

    public int Damage
    {
        get
        {
            return damage;
        }
    }    
    

    /// <summary>
    /// Initializer method, called by player/enemy who caused the attack to fill out paramaters
    /// </summary>
    /// <param name="source">The gameobject source of the hitbox, this is where the hitbox will center on</param>
    /// <param name="_dam">Integer damage value</param>
    /// <param name="dist">Distance in front of the source, 0 will overlap with source</param>
    public void init(GameObject _source, int _dam, float _dist, float _hitBoxTime = -1f)
    {
        source = _source;
        damage = _dam;
        dist = _dist;
        hitboxTime = _hitBoxTime;

        hitBox = gameObject.GetComponent<Collider>();


        // Set up the basic properties of the hitbox, displace it according to distance paramater
        hitBox.transform.position = source.transform.position;
        Vector3 displacement = source.transform.forward * dist;
        hitBox.transform.position += displacement;
        hitBox.transform.rotation = source.transform.rotation;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (elapsedTime < hitboxTime || hitboxTime == -1f)
        {
            // Updates the properties of the box to follow the source
            hitBox.transform.position = source.transform.position;
            Vector3 displacement = source.transform.forward * dist;
            hitBox.transform.position += displacement;
            hitBox.transform.rotation = source.transform.rotation;
        }
        else
        {
            Destroy(gameObject);
        }
        elapsedTime += Time.deltaTime;
    }

    // Activates when another collider enters this hitbox, 
    protected virtual void OnTriggerEnter(Collider other)
    {
        // If hit object is player, do damage
        if (other.GetComponentInParent<Player>() != null)
        {
            // Reduce health here other.GetComponentInParent<Player>().
        } else if (other.gameObject.CompareTag("enemy"))
        {
            other.gameObject.GetComponent<EnemyBase>().health -= 1;
        }
    }
}
