using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject playerGameObject; //Player Game Object
    public GameObject fireballPrefab; //Fireball Prefab
    public GameObject bulletPrefab;  //Bullet Prefab
    public GameObject canePrefab; //Cane Attack Prefab
    public GameObject windPrefab; //Windblast Attack Prefab
    private Stamina playerStamina; //Stamina Object
    public bool invincible = false; //Used for shield
    private const float gunshotSpeed = 15f; //Usedd for gunshot projectile speed
    private Transform gunshotLocation;
    private Light gunLight;
	public bool paused = false;		//disables player input when paused

    private bool performingAction = false; //Used to lock player during attacks

    public GameObject playerModel; //Reference to Model Object for Animator
    private Animator playerAnimator; //Animator Object

    public Texture playerDamagedTex;
    private Texture playerDefaultTex;
    Renderer mRenderer;

	public PlayAudioClips audioClips;
	private float walkAudioCooldown;

   [Header("GameFeel")]
    public float windInTime;
    public float windOutTime;
    public float dashInTime;
    public float dashOutTime;
    public float gunInTime;
    public float gunOutTime;
    public float fireballInTime;
    public float fireballOutTime;
    public float caneInTime;
    public float caneOutTime;

    private int health = 3;
    private ParticleSystem dashParticles;
    private List<ParticleSystem> gunParticles;

    // We create a vector which is modified by key presses so that holding A and W for example does not give you an increase in velocity.
    // This method only applies one force to the player while the original method I tried would apply one for each key press, leading to that behavior.
    Vector3 frameMovement = new Vector3(0, 0, 0);

    // Adjust the amount of force we apply
    [SerializeField]
    float acceletation;
    [SerializeField]
    float maxSpeed;
    // Holds the player's rigidbody so we can apply forces to it
    private Rigidbody rb;
    
    // Start is called before the first frame update
    void Start()
    {
		playerStamina = GetComponent<Stamina>();
		rb = gameObject.GetComponent<Rigidbody>();
        dashParticles = transform.Find("DashTrail").GetComponent<ParticleSystem>();
        gunshotLocation = transform.Find("GunShot");
        gunParticles = new List<ParticleSystem>();
        gunParticles.Add(gunshotLocation.Find("cone").GetComponent<ParticleSystem>());
        gunParticles.Add(gunshotLocation.Find("line").GetComponent<ParticleSystem>());
        playerAnimator = GetComponentInChildren<Animator>();
        mRenderer = transform.Find("DresdenBody").Find("Dresden").Find("Body").GetComponent<Renderer>();
        playerDefaultTex = mRenderer.material.mainTexture;
        gunLight = GetComponentInChildren<Light>();
        gunLight.enabled = false;
		audioClips = GetComponent<PlayAudioClips>();
    }

    // Update is called once per frame
    void Update()
	{
		if (!paused)
		{
			CheckRoation();
			CheckAction();
		}
    }

    // We use fixedupdate because it is more reliable specifically for physics interactions
    private void FixedUpdate()
    {
            MovementCheck();        
    }

    public int Health
    {
        get { return health; }
        set {
            if (value < health)
                Damaged();
            health = value; 
        }
    }

    //This will check if W A S or D are pressed and move in the corresponding direction
    void MovementCheck()
    {
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        if (acceletation == 0)
            return;

        // Reset framemovement
        frameMovement = Vector3.zero;

        if(Input.GetKey(KeyCode.W) == true)
        {
            frameMovement += new Vector3(0, 1, 0);
        }
        if(Input.GetKey(KeyCode.A) == true)
        {
            frameMovement += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.S) == true)
        {
            frameMovement += new Vector3(0, -1, 0);
        }
        if (Input.GetKey(KeyCode.D) == true)
        {
            frameMovement += new Vector3(1, 0, 0);
        }

        // Now we normalize the temporary vector, to get our direction which will always be the same magnitude
        frameMovement.Normalize();

        // Actually apply the force here
        rb.AddForce(frameMovement * acceletation);

		

        //Setting animation for idle or moving
        if (frameMovement == Vector3.zero)
        {
            playerAnimator.SetBool("isMoving", false);
        } else if(frameMovement != Vector3.zero)
        {
            playerAnimator.SetBool("isMoving", true);

			//play sound
			if (walkAudioCooldown > 1.1f)
			{
				audioClips.PlayAudio(6);
				walkAudioCooldown = 0;
			}
			else
			{
				walkAudioCooldown += Time.deltaTime;
			}
		}
    }
    /// <summary>
    /// This will check the rotation the sprite should be facing
    /// </summary>
    void CheckRoation()
    {
        Vector3 mouse_pos = Input.mousePosition;
        Vector3 object_pos = Camera.main.WorldToScreenPoint(playerGameObject.transform.position);
        mouse_pos.x = mouse_pos.x - object_pos.x;
        mouse_pos.y = mouse_pos.y - object_pos.y;
        float angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;
        playerGameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));        
    }

    //This will check if the user has pressed an "Action Key" which includes firing a weapon, ability or some other use
    void CheckAction()
    {
        if (performingAction)
            return;
        /*
        //SHIELD
        //Inital Press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (playerStamina.Attack(15, false))
            {
                invincible = true;
                playerGameObject.GetComponent<SpriteRenderer>().color = Color.blue; //Test for invincible color
                playerAnimator.SetTrigger("MagicAtk");

                // Just a note from when Will is reading thru here, if we get a chance we should take GetComponent<> out of update cause it may cause performance issues
                // Although this is labeled as a test so I'm guessing you already knew that lol
            }
        }
        //Shield Hold
        else if (Input.GetKey(KeyCode.Space))
        {
            //Invincible == true makes it so that this attack can not be the first press of shield.
            if (invincible == true && playerStamina.Attack(60f * Time.deltaTime, false))
            {
            }
            //This will stop the hold from mattering until shield is pressed again
            else
            {
                invincible = false;
                playerGameObject.GetComponent<SpriteRenderer>().color = Color.white; //Test for invincible color
            }
        }
        //Shield End
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            invincible = false;
            playerGameObject.GetComponent<SpriteRenderer>().color = Color.white;

        }
        */
        //FIREBALL
        if (Input.GetKeyDown(KeyCode.Q) == true)
        {
            void SpawnFireBall()
            {
                //Paramater for rotation
                GameObject newObject = Instantiate(fireballPrefab,
                    playerGameObject.transform.position + transform.right * 1f,
                    playerGameObject.transform.rotation);
                Fireball newFireball = newObject.GetComponent<Fireball>();
				//play sound
				audioClips.PlayAudio(3);
			}
            if (playerStamina.Attack(25, false))
            {
                //Magic Attack animation
                playerAnimator.SetTrigger("MagicAtk");
                DelayAttack(fireballInTime, fireballOutTime, SpawnFireBall, .1f);
            }
        }
        //REVOLVER
        else if (Input.GetKeyDown(KeyCode.Mouse1) == true)
        {
            void Gunshot()
            {
                //Paramater for rotation
                GameObject newObject = Instantiate(bulletPrefab,
                    playerGameObject.transform.position + transform.right * 1f,
                    playerGameObject.transform.rotation);
                Gunshot newGunshot = newObject.GetComponent<Gunshot>();
                newGunshot.speed = gunshotSpeed; //Changes the speed of the gunshot to the proper amount.
                foreach (ParticleSystem ps in gunParticles)
                {
                    ps.Play();
                }
				//play sound
				audioClips.PlayAudio(2);
			}
            if (playerStamina.Attack(20, true))
            {
                //Revolver Animation
                DelayAttack(gunInTime, gunOutTime, Gunshot);
                playerAnimator.SetTrigger("Revolver");
            }
        }
        //DASH 
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            IEnumerator Dash()
            {
                performingAction = true;
                float baseAccel = acceletation;
                float baseMax = maxSpeed;
                maxSpeed *= 2;
                acceletation = 0;

                float secondsElapsed = 0;

                // Add dashing force
                dashParticles.Play();
				//play sound
				audioClips.PlayAudio(5);

                //Dash loop for duration
                while (secondsElapsed < .3f)
                {
                    rb.AddForce(frameMovement * baseAccel * 3);
                    secondsElapsed += Time.deltaTime;
                    yield return null;
                }

                performingAction = false;
                acceletation = baseAccel;
                maxSpeed = baseMax;
                yield break;

            }
            if (playerStamina.Attack(20, true))
            {
                StartCoroutine(Dash());
                playerAnimator.SetTrigger("DodgeRight");
            }
        }
        //CANE
        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            void CaneAttack()
            {
                GameObject newCane = Instantiate(canePrefab,
                    playerGameObject.transform.position + transform.right * 1f,
                    playerGameObject.transform.rotation);
                //Instantiate New Hitbox
                BasicAttack caneAttack = newCane.GetComponent<BasicAttack>();
                caneAttack.init(playerGameObject, 1, .15f, .1f);
				//play sound
				audioClips.PlayAudio(1);
            }
            if (playerStamina.Attack(15, true))
            {
                DelayAttack(caneInTime, caneOutTime, CaneAttack);
                playerAnimator.SetTrigger("CaneSweep");
            }
        }
        //Wind Attack
        else if (Input.GetKeyDown(KeyCode.E))
        {
            void WindAttack()
            {
                //Paramater for rotation
                GameObject newWind = Instantiate(windPrefab,
                    playerGameObject.transform.position + transform.right * 1f,
                    playerGameObject.transform.rotation);
                //Instantiate new Hitbox
                BasicAttack caneAttack = newWind.GetComponent<BasicAttack>();
                caneAttack.init(playerGameObject, 1, 0f, .2f);
				//play sound
				audioClips.PlayAudio(4);
			}
            if (playerStamina.Attack(15, false))
            {
                DelayAttack(windInTime, windOutTime, WindAttack, 1);
                playerAnimator.SetTrigger("MagicAtk");
            }
        }
    }

    // Method to apply force to the player
    private void MovePlayer(Vector3 movement)
    {
        rb.AddForce(movement);
    }

    delegate void AttackMethod();

    //slow factor determines the percentage the player will be slowed by BEFORE the 
    //attack, with 1 being a full stop
    private void DelayAttack(float inSeconds, float outSeconds, AttackMethod method, float slowfactor=0)
    {
        performingAction = true;
        IEnumerator Delay()
        {
            yield return new WaitForSeconds(inSeconds);
            method();
            yield return new WaitForSeconds(outSeconds);
            performingAction = false;
            yield return null;
        }
        IEnumerator DelayWithSlow()
        {
            float elapsedTime = 0;
            float baseSpeed = maxSpeed;
            float targetSpeed = maxSpeed * (1 - slowfactor);
            //lerp to target maxSpeed over the delay
            while (elapsedTime < inSeconds)
            {
                elapsedTime += Time.deltaTime;
                maxSpeed = Mathf.Lerp(baseSpeed, targetSpeed, elapsedTime / inSeconds);
                yield return null;
            }
            method();
            elapsedTime = 0;
            while (elapsedTime < outSeconds)
            {
                elapsedTime += Time.deltaTime;
                maxSpeed = Mathf.Lerp(targetSpeed, baseSpeed, elapsedTime / outSeconds);
                yield return null;
            }
            maxSpeed = baseSpeed;
            performingAction = false;
        }
        if (slowfactor > 0)
            StartCoroutine(DelayWithSlow());
        else
            StartCoroutine(Delay());
    }

    private void Damaged()
    {
        IEnumerator DamagedRoutine()
        {
            const float flashTime = .25f;
			audioClips.PlayAudio(0);
            mRenderer.material.mainTexture = playerDamagedTex;
            yield return new WaitForSeconds(flashTime);
			if (health <= 0)	//brings up death UI and pauses game if player is out of health
			{
				Camera.main.GetComponent<Buttons>().PlayerDeathUI();
			}
            mRenderer.material.mainTexture = playerDefaultTex;
            yield return null;
        }
        StartCoroutine(DamagedRoutine());
    }
}
