using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {


    // base and current move speed to make it easy to slow down and go back to the original speed
    public float baseMoveSpeed = 0.1f;
    float currentMoveSpeed;
    Vector3 moveInput;

    bool facingRight = true;
    //Vector2 mouseWorldPoint;
    Vector2 aimDirection;
    public Glove ballGlove;
    bool gloveOpen = false;


    // bullets caught
    int numBulletsCaught = 0;


    // Use this for initialization
    void Start () {
		currentMoveSpeed = baseMoveSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);

        // get the world position of the mouse
        // then calculate the direction the mouse is aiming relative to the player
        aimDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
        aimDirection.Normalize();

        ballGlove.SetAimDirection(aimDirection);
        if ((facingRight && (aimDirection.x < 0)) || (!facingRight && (aimDirection.x > 0)))
        {
            // if facing right, but mouse is to the left, turn
            // if facing left, but mouse to the right, turn
            
            SwapFacing();
        }

        if(Input.GetButtonDown("Fire1"))
        {
            gloveOpen = true;
        }
        if(Input.GetButtonUp("Fire1"))
        {
            gloveOpen = false;
        }
        ballGlove.SetGloveOpen(gloveOpen);
    }

    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + moveInput, currentMoveSpeed * Time.fixedDeltaTime);
    }

    void SwapFacing()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        facingRight = !facingRight;
    }

    public void CatchBullet()
    {
        numBulletsCaught++;
    }
}
