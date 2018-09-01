using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {


    Transform player;

    Vector2 aimDirection;
    bool facingRight = true;

    public float missAmount = 0.5f;
    Vector3 missPoint;
    float timeBetweenNewMissPoint = 2;
    float timeOfNextReAim = 0;

    Gun weapon;

    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<Player>().transform;
        weapon = GetComponentInChildren<Gun>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > timeOfNextReAim)
        {
            timeOfNextReAim = Time.time + timeBetweenNewMissPoint;
            SetMissPoint();
        }
        if (CanSeePlayer())
        {
            // aim a little bit around the player
            // + new Vector3(Random.Range(-missAmount, missAmount), Random.Range(-missAmount, missAmount), 0)
            // makes the enemy shake around a bunch
            // because they update their aim in real time
            // miss point created at start
            aimDirection = (player.position + missPoint) - transform.position;
            aimDirection.Normalize();


            if ((facingRight && (aimDirection.x < 0)) || (!facingRight && (aimDirection.x > 0)))
            {
                // if facing right, but mouse is to the left, turn
                // if facing left, but mouse to the right, turn

                SwapFacing();
            }

            weapon.SetAimDirection(aimDirection);

            weapon.Fire();

        }
    }

    void SetMissPoint()
    {
        missPoint = new Vector3(Random.Range(-missAmount, missAmount), Random.Range(-missAmount, missAmount), 0);

    }

    bool CanSeePlayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (player.position - transform.position));
        if (hit.transform != null)
        {
            if (hit.transform.tag == "Player")
            {
                //Debug.Log("Hit Player");
                return true;
            }
        }


        return false;
    }

    void SwapFacing()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        facingRight = !facingRight;
    }
}
