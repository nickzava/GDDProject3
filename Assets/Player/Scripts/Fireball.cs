using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public Vector3 rotationVector;
    private const float SPEED = 3.0f;
    private Rigidbody fireballRigid;
    // Start is called before the first frame update

    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
    }
    /// <summary>
    /// Rigidbody Property
    /// </summary>
    public Rigidbody FireballRigid
    {
        get
        {
            return fireballRigid;
        }
        set
        {
            fireballRigid = value;
        }
    }
    /// <summary>
    /// Changes rotation and velocity at the start
    /// </summary>
    /// <param name="fireballRotation"></param>
    public void ChangeRotation(Quaternion fireballRotation)
    {
        fireballRigid.transform.rotation = fireballRotation;

        //Apply force instead of setting velocity
        fireballRigid.velocity = transform.forward * SPEED;

    }
    //This will be used for collision detection
    public void HitDetection()
    {

    }
    //This is where the fireball does damage on collision
    public void Explode()
    {

    }
}
