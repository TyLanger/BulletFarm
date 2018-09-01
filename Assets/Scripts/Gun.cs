using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon {


    public GameObject bullet;

    public int clipSize = 6;
    int currentNumBullets;
    public float timeBetweenShots = 1;
    float timeOfNextShot = 0;
    public float timeToReload = 3;

    void Reload()
    {
        timeOfNextShot = Time.time + timeToReload;
        currentNumBullets = clipSize;
    }

    public void Fire()
    {
        if(Time.time > timeOfNextShot)
        {
            timeOfNextShot = Time.time + timeBetweenShots;

            if (currentNumBullets > 0)
            {
                currentNumBullets--;

                Instantiate(bullet, transform.position, transform.rotation);
            }
            else
            {
                // maybe have a different timer for reload?
                // like a dumb enemy hears *click* a few times before they know to reload
                // a smarter enemy reloads right after the last shot
                // takes longer to notice you're out for an assault rifle than a hand gun with only 6 shots
                Reload();
            }
        }
    }
}
