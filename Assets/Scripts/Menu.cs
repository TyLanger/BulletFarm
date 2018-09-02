using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {


    public GameObject[] menuShots;
    AudioSource aSource;

	// Use this for initialization
	void Start () {
        aSource = GetComponent<AudioSource>();
        StartCoroutine(ShootTitle());
	}
	
	void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            // menu is sceen 0
            // game is scene 1
            SceneManager.LoadScene(1);
        }
    }

    IEnumerator ShootTitle()
    {
        for (int i = 0; i < menuShots.Length; i++)
        {
            menuShots[i].transform.Rotate(Vector3.forward, Random.Range(0, 3) * 90);
            menuShots[i].SetActive(true);
            /*
            if (!aSource.isPlaying)
            {
                aSource.Play();
            }
            */
            aSource.Play();

            yield return new WaitForSeconds(0.09f);

        }


    }
}
