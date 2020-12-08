using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : EnemyBase
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    protected override void DoAttack()
    {
        StartCoroutine("Attack");
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

        // We store the enemy transform information here so we can instantiate NOT as a child of the enemy
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        Instantiate(attackPrefab, pos, rot).GetComponent<BasicAttack>();
		audioClips.PlayAudio(1);

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
