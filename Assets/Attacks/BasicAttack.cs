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

    // Reference to the hitbox trigger
    BoxCollider box;

    /// <summary>
    /// Initializer method, called by player/enemy who caused the attack to fill out paramaters
    /// </summary>
    /// <param name="source">The gameobject source of the hitbox, this is where the hitbox will center on</param>
    /// <param name="_dam">Integer damage value</param>
    /// <param name="dist">Distance in front of the source, 0 will overlap with source</param>
    /// <param name="_size">Size of the hitbox</param>
    public void init(GameObject _source, int _dam, float _dist, Vector3 _size)
    {
        source = _source;
        damage = _dam;
        size = _size;
        dist = _dist;

        box = gameObject.GetComponent<BoxCollider>();

        // Set up the basic properties of the hitbox, displace it according to distance paramater
        box.transform.position = source.transform.position;
        Vector3 displacement = source.transform.forward * dist;
        box.transform.position += displacement;
        box.transform.rotation = source.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        // Updates the properties of the box to follow the source
        box.transform.position = source.transform.position;
        Vector3 displacement = source.transform.forward * dist;
        box.transform.position += displacement;
        box.transform.rotation = source.transform.rotation;
    }
}
