using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoCrate : MonoBehaviour {

    public int ammo = 25;


    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            // give player 100 ammo
            col.GetComponent<Player>().GetAmmoCrate();
            Destroy(gameObject);
        }
    }
}
