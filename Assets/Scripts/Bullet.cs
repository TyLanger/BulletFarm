using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {


    public float moveSpeed = 2;


    void FixedUpdate()
    {

        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.up, moveSpeed * Time.fixedDeltaTime);

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
                DestroyBullet();
            }
            if(col.transform.tag == "Catcher")
            {
                // hit something that can catch bullets
                DestroyBullet();
            }
        }
    }

    void DestroyBullet()
    {
        // play explosion animation if any?

        Destroy(gameObject);
    }

}
