using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoCrate : MonoBehaviour {

    public int ammo = 25;
    Vector3 spawnPoint;


    public Transform storageSpot;
    public float topOfScreen = -2;
    public Vector3 topEntrance;
    public float endHeight = -4;

    void Start()
    {
        spawnPoint = transform.position;
    }

    public void HandInCrates()
    {
        StartCoroutine(HandIn());
    }

    public IEnumerator HandIn()
    {
        while(transform.position.y < topOfScreen)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.up, 1 * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        transform.position = topEntrance;

        while(transform.position.y > endHeight)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.down, 1 * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

    }

    public void ResetCrate()
    {
        transform.position = spawnPoint;
        GetComponent<BoxCollider2D>().enabled = true;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            // give player 100 ammo
            col.GetComponent<Player>().GetAmmoCrate(gameObject);
            GetComponent<BoxCollider2D>().enabled = false;
            transform.position = storageSpot.position;
            //Destroy(gameObject);
        }
    }
}
