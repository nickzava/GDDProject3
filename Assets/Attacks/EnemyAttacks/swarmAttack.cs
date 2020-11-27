using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swarmAttack : BasicAttack
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    // Triggers when we hit another hitbox. Check if it's a player, then damage them if so
    protected override void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().Health -= 1;
        }
    }
}
