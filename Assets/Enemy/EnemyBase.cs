using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    // Some basic vars that all enemies have
    Vector2 movement;
    bool hunting;
    [SerializeField]
    GameObject target;

    // Stats which can be set in prefabs
    [SerializeField]
    float speed;
    [SerializeField]
    int health;
    [SerializeField]
    float visionRange;
    [SerializeField]
    float maxVisionRange;
    [SerializeField]
    float approachRadius;

    // Start is called before the first frame update
    protected void Start()
    {
        // Hunting set to false by default, will switch on if player gets too close
        hunting = false;
    }

    // Update is called once per frame
    protected void Update()
    {
        LocateTarget();
        SeekTarget();
        DoMovement();
    }

    // Gets the enemy's target, for now it is a gameobject dropped into a prefab but will probably just grab the player later on
    void LocateTarget()
    {
        // Checks to see if the target is within vision range, drops target if not
        if (GetDist(gameObject, target) <= visionRange)
        {
            hunting = true;
        } else if (hunting == true && GetDist(gameObject, target) > maxVisionRange)
        {
            hunting = false;
        }
    }

    // Sets our movement vector so we can move towards the target
    void SeekTarget()
    {
        // If not hunting, stops the method
        if (!hunting)
        {
            movement = new Vector2(0, 0);
            return;
        }
        // Else, set our movement vector to a seeking vector only if we are not within seeking radius
        if (GetDist(gameObject, target) > approachRadius)
        {
            movement = Vector2.ClampMagnitude(target.transform.position - gameObject.transform.position, speed);
        } else
        {
            movement = new Vector2(0, 0);
        }
    }

    // Moves the enemy
    void DoMovement()
    {
        // Cast to vec3
        Vector3 tempMove = movement;
        gameObject.transform.position += tempMove;
    }

    // Helper method to find distance between two gameobjects
    float GetDist(GameObject seeker, GameObject target)
    {
        float dist = Vector2.Distance(seeker.transform.position, target.transform.position);

        return dist;
    }
}
