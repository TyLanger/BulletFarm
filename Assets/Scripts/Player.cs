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
    public UnityEngine.UI.Text bulletsCollectedText;
    int ammoCrates = 0;

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

    // health
    int maxNumBulletsHit = 3;
    int currentNumBulletsHit = 0;
    GameObject[] bulletsHit;
    bool canMove = true;

    public Vector3 inBetweenPoint;
    public Transform bulletPoint;
    public GameObject fadeScreen;

    GameObject[] bulletReference;

    // Use this for initialization
    void Start () {
        bulletsHit = new GameObject[maxNumBulletsHit];
		currentMoveSpeed = baseMoveSpeed;
        backpackRenderer = backpackVisuals.GetComponent<SpriteRenderer>();
        backpackCloseSprite = backpackRenderer.sprite;

        bulletReference = new GameObject[25];
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

        if (canMove)
        {
            // Use ball glove with left click
            if (Input.GetButtonDown("Fire1"))
            {
                gloveOpen = true;
            }
            if (Input.GetButtonUp("Fire1"))
            {
                gloveOpen = false;
            }
            ballGlove.SetGloveOpen(gloveOpen);

            // Use butterfly net with right click
            if (Input.GetButtonDown("Fire2"))
            {
                if (!usingButterflyNet)
                {
                    SwingBNet();
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + moveInput, currentMoveSpeed * Time.fixedDeltaTime);
        }
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

    public void GetAmmoCrate()
    {
        ammoCrates++;
        CatchBullet(0);
    }

    void CatchBullet()
    {
        CatchBullet(1);
    }

    void CatchBullet(int numBullets)
    {
        Debug.Log("Caught " + numBullets + " bullets");
        numBulletsCaught += numBullets;
        backpackRenderer.sprite = backPackOpenSprite;
        bulletsCollectedText.text = (numBulletsCaught + (ammoCrates * 25)).ToString();
        Invoke("BackpackClose", 0.5f);
    }

    void BackpackClose()
    {
        backpackRenderer.sprite = backpackCloseSprite;
    }

    public void HitByBullet(GameObject col)
    {
        if (currentNumBulletsHit < maxNumBulletsHit)
        {
            // only add to the array until it's full
            bulletsHit[currentNumBulletsHit] = col.gameObject;
        }

        col.transform.parent = transform;
        col.transform.position = Vector3.MoveTowards(col.transform.position, transform.position, 3 * Time.fixedDeltaTime);

        // remove the rigidbody so when you push against objects, the bullets don't fly off on their own
        Destroy(col.gameObject.GetComponent<Rigidbody2D>());
        currentNumBulletsHit++;
        if (currentNumBulletsHit > maxNumBulletsHit)
        {
            // you died
            StartCoroutine(Death());
        }
    }

    // player thinks it's hit by a bullet when one of its children is hit by a bullet
    // i.e. the glove and net
    /*
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Bullet")
        {
            //Debug.Log("Hit by bullet");
            

        }
    }
    */

    void Respawn()
    {
        canMove = true;
        // tp player
        Camera.main.GetComponent<CameraFollow>().UnhingeCamera(false);
        fadeScreen.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 1);
    }

    public void StoreBullet(GameObject bullet)
    {
        bulletReference[numBulletsCaught] = bullet;
        CatchBullet();
    }

    IEnumerator GiveBullets()
    {
        // at the end, give the bullets to doug, your employer
        // spawn bullets at player's backpack
        // bullets travel off the top of the screen
        // then come down from the top near Doug

        for (int i = 0; i < numBulletsCaught; i++)
        {
            bulletReference[i].GetComponent<Bullet>().TurnInBullets();
            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }

    IEnumerator Death()
    {

        // play death animation

        // fade to black
        SpriteRenderer sr = fadeScreen.GetComponent<SpriteRenderer>();
        while(sr.color.a < 0.95f)
        {
            sr.color += new Color(0, 0, 0, 0.01f);
            yield return new WaitForFixedUpdate();
        }

        transform.position = inBetweenPoint;
        Camera.main.GetComponent<CameraFollow>().UnhingeCamera(true);

        yield return new WaitForSeconds(5);

        StartCoroutine(GiveBullets());
    }
}
