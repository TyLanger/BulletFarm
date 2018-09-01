using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {


    Transform player;

    Vector2 aimDirection;
    bool facingRight = true;

    Weapon weapon;

    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<Player>().transform;
        weapon = GetComponentInChildren<Weapon>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CanSeePlayer())
        {
            aimDirection = player.position - transform.position;
            aimDirection.Normalize();


            if ((facingRight && (aimDirection.x < 0)) || (!facingRight && (aimDirection.x > 0)))
            {
                // if facing right, but mouse is to the left, turn
                // if facing left, but mouse to the right, turn

                SwapFacing();
            }

            weapon.SetAimDirection(aimDirection);

        }
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
