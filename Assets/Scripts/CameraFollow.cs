using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {


    public GameObject target;
    public Vector3 offset;
    public float moveSpeed = 1;

    public float leftBound;
    public float rightBound;
    public float topBound;
    public float bottomBound;

    // camera can go out of the bounds
    bool unHinged = true;
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position + offset, moveSpeed * Time.fixedDeltaTime);

        // stay within the bounds
        if (!unHinged)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, leftBound, rightBound), Mathf.Clamp(transform.position.y, bottomBound, topBound), transform.position.z);
        }
	}

    public void UnhingeCamera(bool hinged)
    {
        unHinged = hinged;
    }
}
