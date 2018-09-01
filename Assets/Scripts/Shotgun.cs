using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Gun {

    public int bulletsPerShot = 3;
    public float bulletAngle = 3;


    public override void Fire()
    {


        if (Time.time > timeOfNextShot)
        {
            timeOfNextShot = Time.time + timeBetweenShots;

            if (currentNumBullets > 0)
            {
                currentNumBullets -= bulletsPerShot;

                for (int i = 0; i < bulletsPerShot; i++)
                {
                    var bulletCopy = Instantiate(bullet, transform.position, transform.rotation);
                    bulletCopy.transform.Rotate(Vector3.forward, bulletAngle *((float)i - ((float)bulletsPerShot / 2f)));
                }

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
