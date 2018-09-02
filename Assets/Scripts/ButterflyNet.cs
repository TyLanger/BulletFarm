using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterflyNet : Weapon {


    SpriteRenderer spriteRenderer;
    public GameObject netVisuals;
    public Sprite[] swooshes;
    public SpriteRenderer swoosh;

    bool swinging = false;

    int numBulletsCaught = 0;
    Player player;
    CircleCollider2D circleCollider;

    // only the first bullet gets drug along.
    // other bullets you catch get destroyed.
    // this is just visuals. You still get the points for all of the bullets
    GameObject caughtBullet;
    public Transform netPosition;

    public Transform bulletStoragePoint;

    protected override void Start()
    {
        base.Start();

        spriteRenderer = GetComponent<SpriteRenderer>();
        player = parent.GetComponent<Player>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    void SwingNet()
    {
        // enables and disables sprites and colliders
        if (!swinging)
        {
            swinging = true;
            netVisuals.SetActive(false);
            spriteRenderer.enabled = true;
            circleCollider.enabled = true;
        }
    }

    void StopSwing()
    {
        // enables and disables sprites and colliders

        swinging = false;
        netVisuals.SetActive(true);
        spriteRenderer.enabled = false;
        circleCollider.enabled = false;

        // updates bullets caught, if any
        if (numBulletsCaught > 0)
        {
            //Destroy(caughtBullet.gameObject);
            caughtBullet.transform.position = bulletStoragePoint.position + new Vector3(Random.Range(-0.05f, 0.05f), 0, 0);

            // store bullet before you increment the score
            StoreBullet(caughtBullet);

            // catch bullet is now rolled into storebullet
            //player.CatchBullet(numBulletsCaught);
            numBulletsCaught = 0;
        }
    }

    IEnumerator SwingArc(Vector3 startDir, Vector3 endDir)
    {

        // dictates how fast the net swings
        // lower is faster
        int numIterations = 15;
        swoosh.enabled = true;
        for (int i = 0; i < numIterations; i++)
        {
            base.SetAimDirection(Vector3.Lerp(startDir, endDir, (float)i / (float)numIterations));
            // don't change facing with the player
            // else the net will swing backwards
            if (parent.localScale.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                transform.localScale = Vector3.one;
            }

            //update the swoosh
            if(i < 5)
            {
                swoosh.sprite = swooshes[0];
            }
            else if (i < 9)
            {
                swoosh.sprite = swooshes[1];
            }
            else if(i < 13)
            {
                swoosh.sprite = swooshes[2];
            }
            else if(i < 15)
            {
                swoosh.sprite = swooshes[3];
            }
            else
            {
                swoosh.sprite = swooshes[0];
            }

            yield return new WaitForFixedUpdate();
        }
        swoosh.sprite = swooshes[0];
        swoosh.enabled = false;

        StopSwing();
    }

    public override void SetAimDirection(Vector2 dir)
    {
        if (!swinging)
        {
            SwingNet();
            float halfNetArc = 60 * Mathf.Deg2Rad;

            Vector3 netStartDirection = new Vector3(dir.x * Mathf.Cos(-halfNetArc) - dir.y * Mathf.Sin(-halfNetArc), dir.x * Mathf.Sin(-halfNetArc) + dir.y * Mathf.Cos(-halfNetArc), 0);
            Vector3 netEndDirection = new Vector3(dir.x * Mathf.Cos(halfNetArc) - dir.y * Mathf.Sin(halfNetArc), dir.x * Mathf.Sin(halfNetArc) + dir.y * Mathf.Cos(halfNetArc), 0);

            base.SetAimDirection(netStartDirection);

            StartCoroutine(SwingArc(netStartDirection, netEndDirection));
        }

    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Bullet")
        {
            if (numBulletsCaught == 0)
            {
                // only have the first bullet show in the net
                caughtBullet = col.gameObject;
                col.transform.parent = transform;
                // move the bullet into the net a bit
                col.transform.position = Vector3.MoveTowards(col.transform.position, netPosition.position, 5 * Time.fixedDeltaTime);
            }
            else
            {
                col.transform.position = bulletStoragePoint.position + new Vector3(Random.Range(-0.05f, 0.05f), 0, 0);
                StoreBullet(col.gameObject);
                //Destroy(col.gameObject);
            }
            numBulletsCaught++;
        }
    }

}
