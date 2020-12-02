using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour
{

    [Header("Game Feel")]
    [SerializeField]
    //how quickly the player regains stamina
    float staminaPerSecond;
    [SerializeField]
    //how much of spent universal stamina is converted into special stamina
    float conversionPercentage;
    int maxStamina = 50;
    [Header("Experimental")]
    //TEMP values for testing
    [SerializeField]
    bool debugClicks;
    [SerializeField]
    bool regenOverSpecial;
    [SerializeField]
    bool allowNegitiveStamina;

    //all stamina values range from 0 - 100
    public float universalStamina { get; private set; }
    public float physicalStamina  { get; private set; }
    public float magicStamina { get; private set; }

    StaminaUI ui;

    //called to perform a physical attack, returns false if
    //the player is unable to perform the attack
    public bool Attack(float cost, bool isPhysical)
    {
        //check required stamina and spend any available special stamina
        if(isPhysical)
        {
            if (universalStamina + physicalStamina > cost ||
                (allowNegitiveStamina && universalStamina > 0))
            {
                //adjust universal cost to reflect physical stamina spent on attack
                physicalStamina -= cost;
                if (physicalStamina < 0)
                {
                    cost = -physicalStamina;
                    physicalStamina = 0;
                }
                else
                {
                    //universal cost is 0, can return early
                    return true;
                }
            }
            //not enough stamina, return
            else
            {
                return false;
            }
        }
        else
        {
            if (universalStamina + magicStamina > cost ||
                (allowNegitiveStamina && universalStamina > 0))
            {
                //adjust universal cost to reflect magic stamina spent on attack
                magicStamina -= cost;
                if (magicStamina < 0)
                {
                    cost = -magicStamina;
                    magicStamina = 0;
                }
                else
                {
                    //universal cost is 0, can return early
                    return true;
                }
            }
            //not enough stamina, return
            else
            {
                return false;
            }
        }

        //any remaining cost after spending special stamina is taken from universal stamina
        //and refunded to the type oppisite the type of the attack
        universalStamina -= cost;
        if (!allowNegitiveStamina || universalStamina > cost)
        {
            if (isPhysical)
            {
                magicStamina += conversionPercentage * cost;
            }
            else
            {
                physicalStamina += conversionPercentage * cost;
            }
        }
        //special calculations for negitive stamina
        else
        {
            if (isPhysical)
            {
                magicStamina += conversionPercentage * (cost + universalStamina);
            }
            else
            {
                physicalStamina += conversionPercentage * (cost + universalStamina);
            }
        }


        return true;
    }




    // Update is called once per frame
    void Update()
    {
        //stamina regen
        if(universalStamina < maxStamina)
        {
            universalStamina = Mathf.Min(maxStamina, Time.deltaTime * staminaPerSecond + universalStamina);
            if(regenOverSpecial && universalStamina > 0)
            {
                if (physicalStamina > 0)
                {
                    physicalStamina -= Time.deltaTime * staminaPerSecond;
                    if (physicalStamina < 0)
                        physicalStamina = 0;
                }
                else if (magicStamina > 0)
                {
                    magicStamina -= Time.deltaTime * staminaPerSecond;
                    if (magicStamina < 0)
                        magicStamina = 0;
                }
            }
        }
        //cap total stamina at 100
        physicalStamina = Mathf.Min(maxStamina - universalStamina, physicalStamina);
        magicStamina = Mathf.Min(maxStamina - magicStamina, magicStamina);

        
        ui.UpdateValues(universalStamina / maxStamina, physicalStamina / maxStamina, magicStamina / maxStamina);

        if (debugClicks)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Attack(20, true);
            }
            if (Input.GetMouseButtonDown(1))
            {
                Attack(20, false);
            }
        }
    }

    private void Awake()
    {
        universalStamina = maxStamina;
        magicStamina = 0;
        physicalStamina = 0;
        
        ui = FindObjectOfType<StaminaUI>();
    }
}
