using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {


    // Weapon orbits around the player and enemies
    // weapons are the ball glove the player has and guns the enemies have
    // they point in a direction (mouse for the player, aim towards the player for the enemies)


    public float orbitDistance;
    public Transform parent;

    public Vector2 aimDirection;


	// Use this for initialization
	protected virtual void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        // get the world position of the mouse
        //Camera c = Camera.main;
        //aimDirection = (c.ScreenToWorldPoint( Input.mousePosition) - parent.position);
        //aimDirection.Normalize();

        // move the glove to point towards the aim point
        transform.position = parent.position + new Vector3(aimDirection.x, aimDirection.y, 0) * orbitDistance;
	}

    public void SetAimDirection(Vector2 dir)
    {
        aimDirection = dir;
        // from up because that is the default direction of the sprites. Sprites are made so up is the way they should point
        transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);
    }
}
