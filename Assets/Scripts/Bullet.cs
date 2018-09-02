using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {


    public float moveSpeed = 2;
    bool isMoving = true;
    bool turningIn = false;
    public float topOfScreen = -2;
    public Vector3 topOfScreenEntrance;
    public float endHeight = -3;
    bool turningInToBoss = false;

    void FixedUpdate()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.up, moveSpeed * Time.fixedDeltaTime);
        }
        if(turningIn)
        {
            if(transform.position.y > topOfScreen)
            {
                TurnInBulletsToBoss();
            }
        }
        if(turningInToBoss)
        {
            if (transform.position.y < endHeight)
            {
                CaughtBullet();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.tag == "Bullet" || col.transform.tag == "Enemy")
        {
            // ignore other bullets
            return;
        }
        else
        {
            if (col.transform.tag == "Player")
            {
                //Debug.Log("Hit player");
                col.gameObject.GetComponent<Player>().HitByBullet(gameObject);
                CaughtBullet();
            }
            if(col.transform.tag == "Catcher")
            {
                // hit something that can catch bullets
                CaughtBullet();
            }
            if(col.transform.tag == "Wall")
            {
                DestroyBullet();
            }
        }
    }

    void TurnInBulletsToBoss()
    {
        // fire bullets down from the top of the screen
        transform.position = topOfScreenEntrance;

        turningIn = false;
        transform.rotation = Quaternion.Euler(0, 0, 180);
        turningInToBoss = true;
    }

    public void TurnInBullets()
    {
        // fire bullets up off screen

        // face bullet up
        transform.rotation = Quaternion.Euler(0, 0, 0);

        isMoving = true;
        turningIn = true;
    }

    void CaughtBullet()
    {
        // bullet is stopped and rides around with whatever caught it until it is destroyed
        isMoving = false;
        //transform.localScale = Vector3.one * 0.5f;
        GetComponent<BoxCollider2D>().enabled = false;
    }

    void DestroyBullet()
    {
        // play explosion animation if any?
        
        Destroy(gameObject);
    }

}
