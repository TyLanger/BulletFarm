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

    // backpack open/close
    public GameObject backpackVisuals;
    SpriteRenderer backpackRenderer;
    public Sprite backPackOpenSprite;
    Sprite backpackCloseSprite;

    // butterfly net
    public float butterflyNetCooldown = 1;
    float timeOfNextButterflyNetUse = 0;
    public GameObject butterflyNet;
    // while swinging the net, can't use the glove
    // can't change your aim
    bool usingButterflyNet = false;
    float timeToSwingButterflyNet = 0.5f;

    // alternate implementation
    public ButterflyNet bNet;


    int maxNumBulletsCaught = 3;
    int currentNumBulletsCaught = 0;
    GameObject[] bulletsCaught;


    // Use this for initialization
    void Start () {
        bulletsCaught = new GameObject[maxNumBulletsCaught];
		currentMoveSpeed = baseMoveSpeed;
        backpackRenderer = backpackVisuals.GetComponent<SpriteRenderer>();
        backpackCloseSprite = backpackRenderer.sprite;
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
            if (!usingButterflyNet)
            {
                // can't swap while using the butterfly net
                SwapFacing();
            }
        }

        // Use ball glove with left click
        if(Input.GetButtonDown("Fire1"))
        {
            gloveOpen = true;
        }
        if(Input.GetButtonUp("Fire1"))
        {
            gloveOpen = false;
        }
        ballGlove.SetGloveOpen(gloveOpen);

        // Use butterfly net with right click
        if(Input.GetButtonDown("Fire2"))
        {
            if (!usingButterflyNet)
            {
                SwingBNet();
            }
        }
    }

    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + moveInput, currentMoveSpeed * Time.fixedDeltaTime);
    }

    void SwingBNet()
    {
        // butterfly net as a weapon
        // swings with setting the aim direction

        // start swinging in the current direction
        bNet.SetAimDirection(aimDirection);
        // butterflyNet now calls swing net from its SetAimDirection()
        //bNet.SwingNet();
    }
    

    void SwapFacing()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        facingRight = !facingRight;
    }

    public void CatchBullet()
    {
        CatchBullet(1);
    }

    public void CatchBullet(int numBullets)
    {
        numBulletsCaught += numBullets;
        backpackRenderer.sprite = backPackOpenSprite;
        Invoke("BackpackClose", 0.5f);
    }

    void BackpackClose()
    {
        backpackRenderer.sprite = backpackCloseSprite;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Bullet")
        {
            if (currentNumBulletsCaught < maxNumBulletsCaught)
            {
                // only add to the array until it's full
                bulletsCaught[currentNumBulletsCaught] = col.gameObject;
            }

            col.transform.parent = transform;
            col.transform.position = Vector3.MoveTowards(col.transform.position, transform.position, 3 * Time.fixedDeltaTime);

            // remove the rigidbody so when you push against objects, the bullets don't fly off on their own
            Destroy(col.gameObject.GetComponent<Rigidbody2D>());
            currentNumBulletsCaught++;
            if (currentNumBulletsCaught > maxNumBulletsCaught)
            {
                // you died
            }

        }
    }
}
