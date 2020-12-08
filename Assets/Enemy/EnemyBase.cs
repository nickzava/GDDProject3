using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    // Some basic vars that all enemies have
    Vector2 movement;
    bool hunting;

    //Animation
    public GameObject enemyModel;
    private Animator enemyAnimator;

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
    protected bool seekingNode;
    // Tracks if the enemy is on attack cooldown
    protected bool attacking;

    // Stats which can be set in prefabs
    [SerializeField]
    float speed;
    [SerializeField]
    public int health;
    private int healthTracker;
    [SerializeField]
    protected float visionRange;
    [SerializeField]
    protected float maxVisionRange;
    [SerializeField]
    protected float approachRadius;

    // Stats for this unit's attacks
    [SerializeField]
    protected GameObject attackPrefab;
    [SerializeField]
    protected float attackWarmup;
    [SerializeField]
    protected float attackCooldown;

    //Model Feilds
    private Renderer mRenderer;
    public Texture damagedTex;
    private Texture mainTex;

    //stun
    bool isStunned = false;

	//sounds
	protected PlayAudioClips audioClips;

    public void Stun(float seconds)
    {
        IEnumerator StunRoutine()
        {
            isStunned = true;
            yield return new WaitForSeconds(seconds);
            isStunned = false;
        }
        StartCoroutine(StunRoutine());
    }

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

        mRenderer = GetComponentInChildren<Renderer>();
        mainTex = mRenderer.material.mainTexture;
        enemyAnimator = GetComponentInChildren<Animator>();

        //make speed independant of mass
        speed *= rb.mass;

		audioClips = GetComponent<PlayAudioClips>();
    }

    // Update is called once per frame
    protected void Update()
    {
        if (!isStunned)
        {
            LocateTarget();
            if(target)
                SeekTarget();
            if (hunting)
            {
                enemyAnimator.SetBool("isMoving", true);
            }
            else
            {
                enemyAnimator.SetBool("isMoving", false);
            }
        } else
        {
            // Keeps upright while stunned
            DoRotation(transform.forward);
        }
        CheckPulse();
    }

    // Fixedupdate is better for physics operations
    private void FixedUpdate()
    {
        if(!isStunned)
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
            DoRotation(target.transform.position - transform.position);
            return;
        }
        // Else, set our movement vector to a seeking vector only if we are not within seeking radius
        // If we are looking at a pathfinding node, will always be true (So we can ignore approach radius)
        if (GetDist(gameObject, target) > approachRadius || seekingNode)
        {
            movement = Vector2.ClampMagnitude(target.transform.position - gameObject.transform.position, speed);
            DoRotation(target.transform.position - transform.position);
        } else if (!seekingNode)
        {
            DoAttack();
            DoRotation(target.transform.position - transform.position);
            enemyAnimator.SetTrigger("Attack");
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

        if(target)
            DoRotation(tempMove);

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
        RaycastHit hitInfo = new RaycastHit();
        int rayLength = 10;
        Physics.Raycast(gameObject.transform.position, (goal.transform.position - gameObject.transform.position), out hitInfo, rayLength, -5, QueryTriggerInteraction.Ignore);
        // Checks to see if the target is within vision range, drops target if not
        if (GetDist(gameObject, goal) <= visionRange && hitInfo.collider.gameObject.tag == "Player")
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
			audioClips.PlayAudio(0);	//plays grunt sound
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

    // Rotates to face movement direction, constrains to an upright position
    void DoRotation(Vector2 _tempMove)
    {
        float angle = Mathf.Atan2(_tempMove.x, _tempMove.y) * Mathf.Rad2Deg;
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(angle - 90, 90, 0));
    }

    // Placeholder method for attacks, will be added in the child classes
    protected abstract void DoAttack();
}
