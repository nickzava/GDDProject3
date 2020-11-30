using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    // Some basic vars that all enemies have
    Vector2 movement;
    bool hunting;
    // Target is the current object we are pathfinding to, goal is the overall target (The player).
    // Target will change when we pathfind, goal should always be the same thing
    [SerializeField]
    GameObject target;
    [SerializeField]
    GameObject goal;
    Rigidbody rb;
    [SerializeField]
    GameObject[] nodes;
    // Bool to indicate if we are actively seeking a pathfinding node
    bool seekingNode;
    // Tracks if the enemy is on attack cooldown
    bool attacking;

    // Stats which can be set in prefabs
    [SerializeField]
    float speed;
    [SerializeField]
    public int health;
    private int healthTracker;
    [SerializeField]
    float visionRange;
    [SerializeField]
    float maxVisionRange;
    [SerializeField]
    float approachRadius;

    // Stats for this unit's attacks
    [SerializeField]
    GameObject attackPrefab;
    [SerializeField]
    float attackWarmup;
    [SerializeField]
    float attackCooldown;

    //Model Feilds
    private Renderer mRenderer;
    public Texture damagedTex;
    private Texture mainTex;

    // Start is called before the first frame update
    protected void Start()
    {
        // Hunting set to false by default, will switch on if player gets too close
        hunting = false;
        attacking = false;

        // Gets our rigidbody
        rb = gameObject.GetComponent<Rigidbody>();

        // If player exists, set it to the target
        try
        {
            goal = GameObject.FindGameObjectWithTag("Player");
        }
        catch (System.Exception)
        {

            Debug.LogWarning("Player not found in scene!");
        }

        // Populates the array with every pathfinding node in scene
        nodes = GameObject.FindGameObjectsWithTag("node");

        healthTracker = health;

        mRenderer = GetComponent<Renderer>();
        mainTex = mRenderer.material.mainTexture;
        //create a texture, should be replaced later 
        Texture2D temp = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        temp.SetPixel(0, 0, new Color(1, 0, 0, 1));
        damagedTex = temp;

        //make speed independant of mass
        speed *= rb.mass;
    }

    // Update is called once per frame
    protected void Update()
    {
        LocateTarget();
        SeekTarget();
        CheckPulse();
    }

    // Fixedupdate is better for physics operations
    private void FixedUpdate()
    {
        DoMovement();
    }

    // Sets our movement vector so we can move towards the target
    void SeekTarget()
    {
        // If not hunting, stops the method
        // Also exits if we are currently on attack cooldown
        if (!hunting || attacking)
        {
            movement = new Vector2(0, 0);
            return;
        }
        // Else, set our movement vector to a seeking vector only if we are not within seeking radius
        // If we are looking at a pathfinding node, will always be true (So we can ignore approach radius)
        if (GetDist(gameObject, target) > approachRadius || seekingNode)
        {
            movement = Vector2.ClampMagnitude(target.transform.position - gameObject.transform.position, speed);
        } else if (!seekingNode)
        {
            StartCoroutine("Attack");
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
        // Ignores if movement vector is too small
        if (movement.magnitude <= .01f)
        {
            return;
        }
        // Rotate to face movement 
        gameObject.transform.LookAt(transform.position + tempMove);
        rb.AddForce(transform.forward * speed);
    }

    // Helper method to find distance between two gameobjects
    float GetDist(GameObject seeker, GameObject target)
    {
        float dist = Vector2.Distance(seeker.transform.position, target.transform.position);

        return dist;
    }

    // Checks if player is within sight range, Then decides to seek them or a pathfinding node
    void LocateTarget()
    {
        // Checks to see if the target is within vision range, drops target if not
        if (GetDist(gameObject, goal) <= visionRange)
        {
            hunting = true;
        }
        else if (hunting == true && GetDist(gameObject, goal) > maxVisionRange)
        {
            hunting = false;
        }

        // If we are hunting, figure out if player is within target range
        if (hunting)
        {
            RaycastHit hitInfo;
            int rayLength = 10;

            if (Physics.Raycast(gameObject.transform.position, (goal.transform.position - gameObject.transform.position), out hitInfo, rayLength))
            {
                if (hitInfo.collider.gameObject.tag == "Player")
                {
                    // Player is within range, and is not obstructed
                    target = goal;
                    seekingNode = false;
                }
                else
                {
                    // Player is within range, but IS obstructed, set target to closest node
                    target = GetPathfindNode();
                    seekingNode = true;
                }
            }
        }


    }

    // Checks if dead
    void CheckPulse()
    {
        //bad code, health should be made private with a setter method where damagedRoutine
        //would be called to avoid the health tracker
        if(health != healthTracker)
        {
            IEnumerator DamagedRoutine()
            {
                const float flashTime = .25f;
                mRenderer.material.mainTexture = damagedTex;
                yield return new WaitForSeconds(flashTime);
                mRenderer.material.mainTexture = mainTex;
                yield return null;
            }
            StartCoroutine(DamagedRoutine());
            healthTracker = health;
        }
        if (health <= 0)
        {
            // Would be cool to just leave dead enemy corpses
            //Destroy(this);
            Destroy(gameObject);
        }
    }

    // Gets the closest pathfinding node
    GameObject GetPathfindNode()
    {
        // Holds the current lowest distance
        float dist = float.MaxValue;
        GameObject currNode = null;

        // Iterates through each node, once done the closest one will be stored in currNode
        foreach (GameObject i in nodes)
        {
            if (GetDist(goal, i) < dist)
            {
                dist = GetDist(goal, i);
                currNode = i;
            }
        }

        return currNode;
    }

    // Attack coroutine
    IEnumerator Attack()
    {
        attacking = true;
        float timePassed = 0;

        // Wait for the attack warmup
        while (timePassed <= attackWarmup)
        {
            timePassed += Time.deltaTime;
            yield return null;
        }
        timePassed = 0;

        // Attacking Code
        BasicAttack attackBox = Instantiate(attackPrefab, gameObject.transform).GetComponent<BasicAttack>();
        attackBox.init(gameObject, 1, .5f, .3f);

        // Wait for attack cooldown
        while (timePassed <= attackCooldown)
        {
            timePassed += Time.deltaTime;
            yield return null;
        }

        attacking = false;
        yield break;
    }
}
