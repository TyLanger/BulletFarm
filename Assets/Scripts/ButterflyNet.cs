using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterflyNet : Weapon {


    SpriteRenderer spriteRenderer;
    public GameObject netVisuals;

    bool swinging = false;

    int numBulletsCaught = 0;
    Player player;
    CircleCollider2D circleCollider;

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
            player.CatchBullet(numBulletsCaught);
        }
    }

    IEnumerator SwingArc(Vector3 startDir, Vector3 endDir)
    {

        // dictates how fast the net swings
        // lower is faster
        int numIterations = 15;

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
            yield return new WaitForFixedUpdate();
        }
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
            numBulletsCaught++;
        }
    }

}
