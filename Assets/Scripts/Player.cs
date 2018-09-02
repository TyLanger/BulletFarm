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
    int totalBulletsHandedIn = 0;


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
    public SpriteRenderer bNetVisuals;

    // health
    int maxNumBulletsHit = 3;
    int currentNumBulletsHit = 0;
    GameObject[] bulletsHit;
    bool canMove = false;

    public Vector3 inBetweenPoint;
    public Transform bulletPoint;
    public GameObject fadeScreen;

    GameObject[] bulletReference;
    GameObject[] ammoCrateReference;

    public Vector3 spawnPoint;
    public UnityEngine.UI.Text chatText;
    bool gloveEnabled = false;
    bool netEnabled = false;

    public GameObject[] netEnemies;
    public GameObject[] finalEnemies;
    bool endGame = false;
    public GameObject endJohhny;
    public SpriteRenderer Doug;
    public Sprite deadDoug;
    public GameObject dougHat;

    public Transform sunsetPoint;
    public Transform sunset;
    bool rideIntoSunset = false;
    public UnityEngine.UI.Text endText;
    public UnityEngine.UI.Image bulletImage;
    public GameObject endGameFadeScreen;


    // Use this for initialization
    void Start () {
        bulletsHit = new GameObject[maxNumBulletsHit];
		currentMoveSpeed = baseMoveSpeed;
        backpackRenderer = backpackVisuals.GetComponent<SpriteRenderer>();
        backpackCloseSprite = backpackRenderer.sprite;

        bulletReference = new GameObject[25];
        // only 7 crates in game
        ammoCrateReference = new GameObject[7];

        StartCoroutine(Intro());
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
            if (gloveEnabled)
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
            }
            if (netEnabled)
            {
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
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + moveInput, currentMoveSpeed * Time.fixedDeltaTime);
        }

        if(rideIntoSunset)
        {
            // walk slowly into the sunset
            transform.position = Vector3.MoveTowards(transform.position, sunset.position, currentMoveSpeed * 0.04f * Time.fixedDeltaTime);
            // shrink as you walk
            transform.localScale = Vector3.one * Mathf.Lerp(0.5f, 1, (Vector3.Distance(transform.position, sunset.position) / Vector3.Distance(sunsetPoint.position, sunset.position)));

            if (Vector3.Distance(transform.position, sunset.position) < 0.1f)
            {
                // end game
                rideIntoSunset = false;
                StartCoroutine(EndGame());
            }
        }
    }

    IEnumerator EndGame()
    {
        SpriteRenderer sr = endGameFadeScreen.GetComponent<SpriteRenderer>();
        while (sr.color.a < 0.95f)
        {
            sr.color += new Color(0, 0, 0, 0.01f);
            yield return new WaitForFixedUpdate();
        }

        while(endText.color.a < 0.95f)
        {
            endText.color += new Color(0, 0, 0, 0.01f);
            yield return new WaitForFixedUpdate();
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

    public void GetAmmoCrate(GameObject crate)
    {
        ammoCrateReference[ammoCrates] = crate;
        ammoCrates++;
        CatchBullet(0);
    }

    void CatchBullet()
    {
        CatchBullet(1);
    }

    void CatchBullet(int numBullets)
    {
        //Debug.Log("Caught " + numBullets + " bullets");
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

    IEnumerator Intro()
    {
        chatText.text = "Howdy kid. So you're gonna be working for me here. It's an easy job, dontcha worry.";

        // click to advance
        while(!Input.GetButtonDown("Fire1"))
        {
            yield return new WaitForFixedUpdate();
        }
        chatText.text = "";
        yield return new WaitForFixedUpdate();

        chatText.text = "All you gotta do is go into this here saloon and pick up some bullets for me." ;

        while (!Input.GetButtonDown("Fire1"))
        {
            yield return new WaitForFixedUpdate();
        }
        chatText.text = "";
        yield return new WaitForFixedUpdate();

        chatText.text = "Easy stuff. So easy a child such as yourself could do it.";

        while (!Input.GetButtonDown("Fire1"))
        {
            yield return new WaitForFixedUpdate();
        }
        chatText.text = "";
        yield return new WaitForFixedUpdate();

        chatText.text = "Those thugs in there may give you a hard time. Here, take this ball glove. Kids like ball, don't they?";
        gloveEnabled = true;

        EnableWeapons();

        while (!Input.GetButtonDown("Fire1"))
        {
            yield return new WaitForFixedUpdate();
        }
        chatText.text = "";
        yield return new WaitForFixedUpdate();

        chatText.text = "It's a children's mitt to fit your small hands so it can only fit one bullet at a time.";
        while (!Input.GetButtonDown("Fire1"))
        {
            yield return new WaitForFixedUpdate();
        }
        chatText.text = "";
        yield return new WaitForFixedUpdate();

        chatText.text = "Hold left click to open the ball glove to catch stuff.";
        while (!Input.GetButtonDown("Fire1"))
        {
            yield return new WaitForFixedUpdate();
        }
        chatText.text = "";
        yield return new WaitForFixedUpdate();

        Respawn();

        yield return null;
    }

    IEnumerator TalkToBoss(int bulletsCaughtThisRound)
    {
        if (!endGame)
        {
            chatText.text = "Kid, you brought me " + bulletsCaughtThisRound + " bullets which brings your total up to " + totalBulletsHandedIn.ToString();

            while (!Input.GetButtonDown("Fire1"))
            {
                yield return new WaitForFixedUpdate();
            }
            chatText.text = "";
            yield return new WaitForFixedUpdate();


            if (!netEnabled)
            {
                if (totalBulletsHandedIn > 75)
                {
                    chatText.text = "That's good and all, but dontcha think a child as big as you should be pulling in more bullets?";

                    while (!Input.GetButtonDown("Fire1"))
                    {
                        yield return new WaitForFixedUpdate();
                    }
                    chatText.text = "";
                    yield return new WaitForFixedUpdate();

                    chatText.text = "Here, try out this here butterfly net. You oughta be able to catch twice as many bullets with that.";
                    netEnabled = true;
                    EnableWeapons();

                    while (!Input.GetButtonDown("Fire1"))
                    {
                        yield return new WaitForFixedUpdate();
                    }
                    chatText.text = "";
                    yield return new WaitForFixedUpdate();

                    chatText.text = "Right click to swing the net";
                    while (!Input.GetButtonDown("Fire1"))
                    {
                        yield return new WaitForFixedUpdate();
                    }
                    chatText.text = "";
                    yield return new WaitForFixedUpdate();
                }
                else
                {
                    chatText.text = "Kid, these numbers just ain't good enough. Don't think I'd treat you any different just cuz you is a kid";
                    while (!Input.GetButtonDown("Fire1"))
                    {
                        yield return new WaitForFixedUpdate();
                    }
                    chatText.text = "";
                    yield return new WaitForFixedUpdate();

                    chatText.text = "Get outta here with your child labour laws. I treat all of my employees the same";
                }
            }
            else
            {
                // you have the net. Now what?
                if (totalBulletsHandedIn > 350)
                {
                    endGame = true;

                    chatText.text = "\"Who am I selling these bullets to if the only people in town are these thugs and a bunch of kids?\"";
                    while (!Input.GetButtonDown("Fire1"))
                    {
                        yield return new WaitForFixedUpdate();
                    }
                    chatText.text = "";
                    yield return new WaitForFixedUpdate();

                    chatText.text = "Good question, kid! Why, to the thugs of course! I sell them the bullets, then you go get them for me.";

                    while (!Input.GetButtonDown("Fire1"))
                    {
                        yield return new WaitForFixedUpdate();
                    }
                    chatText.text = "";
                    yield return new WaitForFixedUpdate();

                    chatText.text = "It's a great business we've got going here";

                    while (!Input.GetButtonDown("Fire1"))
                    {
                        yield return new WaitForFixedUpdate();
                    }
                    chatText.text = "";
                    yield return new WaitForFixedUpdate();
                }
            }

            while (!Input.GetButtonDown("Fire1"))
            {
                yield return new WaitForFixedUpdate();
            }
            chatText.text = "";
            yield return new WaitForFixedUpdate();
            chatText.text = "Now go out and get me more bullets";

            while (!Input.GetButtonDown("Fire1"))
            {
                yield return new WaitForFixedUpdate();
            }
            chatText.text = "";
            yield return new WaitForFixedUpdate();

            Respawn();
        }
        else
        {
            chatText.text = "Kid, I'm Johhny. We killed that crooked Doug guy you called your boss.";

            while (!Input.GetButtonDown("Fire1"))
            {
                yield return new WaitForFixedUpdate();
            }
            chatText.text = "";
            yield return new WaitForFixedUpdate();

            chatText.text = "He was playing us both for fools. Without him around, we're both free.";

            while (!Input.GetButtonDown("Fire1"))
            {
                yield return new WaitForFixedUpdate();
            }
            chatText.text = "";
            yield return new WaitForFixedUpdate();

            chatText.text = "Me and the boys are thinking about a change of pace.";

            while (!Input.GetButtonDown("Fire1"))
            {
                yield return new WaitForFixedUpdate();
            }
            chatText.text = "";
            yield return new WaitForFixedUpdate();

            chatText.text = "Maybe we'll leave this replica wild west town and rejoin the modern day. We sure do miss video games.";

            while (!Input.GetButtonDown("Fire1"))
            {
                yield return new WaitForFixedUpdate();
            }
            chatText.text = "";
            yield return new WaitForFixedUpdate();

            chatText.text = "You're welcome to join us, kid.";

            while (!Input.GetButtonDown("Fire1"))
            {
                yield return new WaitForFixedUpdate();
            }
            chatText.text = "";
            yield return new WaitForFixedUpdate();

            chatText.text = "\"No\"? Well, suit yourself. Go on then, ride off into that there sunset, cowboy.";

            while (!Input.GetButtonDown("Fire1"))
            {
                yield return new WaitForFixedUpdate();
            }
            chatText.text = "";
            yield return new WaitForFixedUpdate();

            transform.position = sunsetPoint.position;
            rideIntoSunset = true;
            Camera.main.GetComponent<CameraFollow>().target = sunsetPoint.gameObject;
            bulletsCollectedText.gameObject.SetActive(false);
            bulletImage.gameObject.SetActive(false);
        }
    }

    void EndGameSetup()
    {
        // kill Doug
        // spawn Johhny in the end screen

        endJohhny.SetActive(true);
        Doug.sprite = deadDoug;
        dougHat.SetActive(true);
    }

    void EnableWeapons()
    {
        if (gloveEnabled)
        {
            ballGlove.GetComponent<SpriteRenderer>().enabled = true;
        }
        if (netEnabled)
        {
            //bNet.GetComponent<SpriteRenderer>().enabled = true;
            bNetVisuals.enabled = true;
        }
    }

    void SetupEnemies()
    {

        if(netEnabled)
        {

            // enable these enemies
            for (int i = 0; i < netEnemies.Length; i++)
            {
                netEnemies[i].SetActive(true);
            }
        }
        if(endGame)
        {
            for (int i = 0; i < finalEnemies.Length; i++)
            {
                finalEnemies[i].SetActive(false);
            }
            EndGameSetup();
        }

    }

    void Respawn()
    {
        SetupEnemies();
        // Destroy the bullets given to the boss
        for (int i = 0; i < bulletReference.Length; i++)
        {
            if (bulletReference[i] != null)
            {
                Destroy(bulletReference[i].gameObject);
            }
            else
            {
                // got all the bullets in the array
                break;
            }
        }

        for (int i = 0; i < ammoCrates; i++)
        {
            ammoCrateReference[i].GetComponent<AmmoCrate>().ResetCrate();
        }

        //Debug.Break();
        canMove = true;
        // tp player
        transform.position = spawnPoint;
        numBulletsCaught = 0;
        ammoCrates = 0;
        currentNumBulletsHit = 0;

        Camera.main.GetComponent<CameraFollow>().UnhingeCamera(false);
        fadeScreen.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 1);
    }

    public void StoreBullet(GameObject bullet)
    {
        if (numBulletsCaught < bulletReference.Length)
        {
            bulletReference[numBulletsCaught] = bullet;
        }
        else
        {
            // array is full
            // destroy bullets instead
            Destroy(bullet);
        }
        CatchBullet();
    }

    IEnumerator GiveBullets()
    {
        // at the end, give the bullets to doug, your employer
        // spawn bullets at player's backpack
        // bullets travel off the top of the screen
        // then come down from the top near Doug

        
        for (int i = 0; i < Mathf.Clamp(numBulletsCaught, 0, bulletReference.Length); i++)
        {
            bulletReference[i].GetComponent<Bullet>().TurnInBullets();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

        }
        for (int i = 0; i < ammoCrates; i++)
        {
            ammoCrateReference[i].GetComponent<AmmoCrate>().HandInCrates();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

        }
        if (!endGame)
        {
            chatText.text = "Those bullets embedded in your small body are good too";
        }

        // take all bullets off your body
        var hitBullets = GetComponentsInChildren<Bullet>();
        for (int i = 0; i < hitBullets.Length; i++)
        {
            hitBullets[i].transform.parent = null;
            hitBullets[i].TurnInBullets();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            CatchBullet();
        }

        while (!Input.GetButtonDown("Fire1"))
        {
            yield return new WaitForFixedUpdate();
        }
        chatText.text = "";
        yield return new WaitForFixedUpdate();

        //Debug.Log(numBulletsCaught);
        totalBulletsHandedIn += (numBulletsCaught + (ammoCrates * 25));
        //Debug.Log(totalBulletsHandedIn);

        for (int i = 0; i < hitBullets.Length; i++)
        {
            Destroy(hitBullets[i].gameObject);
        }
        StartCoroutine(TalkToBoss((numBulletsCaught + (ammoCrates * 25))));
    }

    IEnumerator Death()
    {
        if (canMove)
        {
            canMove = false;
            // play death animation

            // fade to black
            SpriteRenderer sr = fadeScreen.GetComponent<SpriteRenderer>();
            while (sr.color.a < 0.95f)
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
}
