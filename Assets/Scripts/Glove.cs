using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glove : Weapon {


    Player player;
    CircleCollider2D gloveCollider;
    SpriteRenderer spriteRenderer;
    Sprite openGlove;
    public Sprite closedGlove;

    // glove can only catch on bullet at a time
    // once it catches a bullet, the bullet must be put away
    public float timeToPutAwayBullet = 0.5f;
    bool puttingAwayBullet = false;
    GameObject caughtBullet;

    public Transform bulletStoragePoint;

    protected override void Start()
    {
        base.Start();
        gloveCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        openGlove = spriteRenderer.sprite;
        player = GetComponentInParent<Player>();
    }

    public void SetGloveOpen(bool isOpen)
    {
        // can freely open and close bullet
        // except when a bullet is being put away
        if (!puttingAwayBullet)
        {
            // when the glove is open, the collider is enabled so it can catch bullets
            // when the glove is closed, the collider is off so the glove cannot catch bullets
            gloveCollider.enabled = isOpen;

            // change the sprite to a closed glove if need be
            if(!isOpen)
            {
                spriteRenderer.sprite = closedGlove;
                spriteRenderer.color = Color.grey;
            }
            else
            {
                spriteRenderer.sprite = openGlove;
                spriteRenderer.color = Color.white;

            }
        }
    }

    void FreeUpGlove()
    {
        // store the bullet before you increment the score
        StoreBullet(caughtBullet);

        //SetGloveOpen(true);
        // catchbullet is now rolled into storebullet
        //player.CatchBullet();
        puttingAwayBullet = false;
        //Destroy(caughtBullet);
        caughtBullet.transform.position = bulletStoragePoint.position + new Vector3(Random.Range(-0.05f, 0.05f), 0, 0);
    }

    public void CatchBullet()
    {
        // when you catch a bullet, you put the bullet in your backpack
        // during this time, you can't manually open the glove
        SetGloveOpen(false);
        puttingAwayBullet = true;
        // move the bullet into the glove a bit
        caughtBullet.transform.position = Vector3.MoveTowards(caughtBullet.transform.position, transform.position, 3 * Time.fixedDeltaTime);
        Invoke("FreeUpGlove", timeToPutAwayBullet);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Bullet")
        {
            caughtBullet = col.gameObject;
            col.transform.parent = transform;
            CatchBullet();
        }
    }
}
