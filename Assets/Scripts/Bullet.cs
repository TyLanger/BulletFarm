using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {


    public float moveSpeed = 2;
    bool isMoving = true;

    void FixedUpdate()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.up, moveSpeed * Time.fixedDeltaTime);
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
