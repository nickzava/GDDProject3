using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    const float SPEED = 4.0f;
    public GameObject playerGameObject;
    public GameObject fireballPrefab;
    private Stamina playerStamina;
    
    // Start is called before the first frame update
    void Start()
    {
        playerStamina = GetComponent<Stamina>();
    }

    // Update is called once per frame
    void Update()
    {
        MovementCheck();
        CheckRoation();
        CheckAction();
        
        
    }

    //This will check if W A S or D are pressed and move in the corresponding direction
    void MovementCheck()
    {
        if(Input.GetKey(KeyCode.W) == true)
        {
            playerGameObject.transform.Translate(new Vector3(0, Time.deltaTime * SPEED, 0), Space.World);
        }
        if(Input.GetKey(KeyCode.A) == true)
        {
            playerGameObject.transform.Translate(new Vector3( Time.deltaTime * -SPEED, 0, 0), Space.World);
        }
        if (Input.GetKey(KeyCode.S) == true)
        {
            playerGameObject.transform.Translate(new Vector3(0, Time.deltaTime * -SPEED, 0), Space.World);
        }
        if (Input.GetKey(KeyCode.D) == true)
        {
            playerGameObject.transform.Translate(new Vector3(Time.deltaTime * SPEED, 0, 0), Space.World);
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
        if (Input.GetKeyDown(KeyCode.Q) == true)
        {
            if (playerStamina.Attack(20, false))
            {
                //Paramater for rotation
                GameObject newObject = Instantiate(fireballPrefab, playerGameObject.transform);
                Fireball newFireball = newObject.GetComponent<Fireball>();
                
                Quaternion rotationQuaternion = playerGameObject.transform.rotation;
                newFireball.ChangeRotation(rotationQuaternion);
                
                newFireball.FireballRigid = newFireball.GetComponent<Rigidbody>();

            }
        }
    }
}
