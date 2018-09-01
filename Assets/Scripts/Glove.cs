using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glove : Weapon {


    Player player;
    CircleCollider2D gloveCollider;
    SpriteRenderer spriteRenderer;

    // glove can only catch on bullet at a time
    // once it catches a bullet, the bullet must be put away
    public float timeToPutAwayBullet = 0.5f;
    bool puttingAwayBullet = false;
	
    protected override void Start()
    {
        base.Start();
        gloveCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
                spriteRenderer.color = Color.grey;
            }
            else
            {
                spriteRenderer.color = Color.white;

            }
        }
    }

    void FreeUpGlove()
    {
        //SetGloveOpen(true);
        puttingAwayBullet = false;
    }

    public void CatchBullet()
    {
        // when you catch a bullet, you put the bullet in your backpack
        // during this time, you can't manually open the glove
        SetGloveOpen(false);
        puttingAwayBullet = true;
        player.CatchBullet();
        Invoke("FreeUpGlove", timeToPutAwayBullet);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Bullet")
        {
            CatchBullet();
        }
    }
}
