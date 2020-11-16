using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : MonoBehaviour
{
    // Stats of the attack
    int damage;
    float dist;
    Vector3 size;
    GameObject source;
    float hitboxTime;
    float elapsedTime;
    // Reference to the hitbox trigger
    PolygonCollider2D hitBox;
    

    /// <summary>
    /// Initializer method, called by player/enemy who caused the attack to fill out paramaters
    /// </summary>
    /// <param name="source">The gameobject source of the hitbox, this is where the hitbox will center on</param>
    /// <param name="_dam">Integer damage value</param>
    /// <param name="dist">Distance in front of the source, 0 will overlap with source</param>
    /// <param name="_size">Size of the hitbox</param>
    public void init(GameObject _source, int _dam, float _dist, Vector3 _size, float _hitBoxTime = -1f)
    {
        source = _source;
        damage = _dam;
        size = _size;
        dist = _dist;
        hitboxTime = _hitBoxTime;

        hitBox = gameObject.GetComponent<PolygonCollider2D>();


        // Set up the basic properties of the hitbox, displace it according to distance paramater
        hitBox.transform.position = source.transform.position;
        Vector3 displacement = source.transform.forward * dist;
        hitBox.transform.position += displacement;
        hitBox.transform.rotation = source.transform.rotation;
    }

    // Update is called once per frame
    void Update()
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
            Debug.Log("Death");
        }
        elapsedTime += Time.deltaTime;
    }
}
