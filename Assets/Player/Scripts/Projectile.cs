using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 3.0f;
    public float damage = 0;
    protected Rigidbody rb;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = transform.right * speed;
    }

    //called when the rb collides with another rb
    protected virtual void OnCollisionEnter(Collision collision)
    {

    }

}
