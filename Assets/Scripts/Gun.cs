﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon {


    public GameObject bullet;

    public int clipSize = 6;
    protected int currentNumBullets;
    public float timeBetweenShots = 1;
    protected float timeOfNextShot = 0;
    public float timeToReload = 3;

    AudioSource aSource;

    protected override void Start()
    {
        base.Start();
        currentNumBullets = clipSize;
        aSource = GetComponent<AudioSource>();
    }

    protected void Reload()
    {
        GetComponent<SpriteRenderer>().color = Color.grey;
        timeOfNextShot = Time.time + timeToReload;
        currentNumBullets = clipSize;
        Invoke("ResetColour", timeToReload);
    }

    void ResetColour()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    protected void ShootSound()
    {
        aSource.Play();
    }

    public virtual void Fire()
    {
        if(Time.time > timeOfNextShot)
        {
            timeOfNextShot = Time.time + timeBetweenShots;

            if (currentNumBullets > 0)
            {
                currentNumBullets--;

                Instantiate(bullet, transform.position, transform.rotation);
                ShootSound();
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
