using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorBuilder : MonoBehaviour {


    public GameObject[] floorTiles;

    public int xSize;
    public int ySize;

    public float xSpacing;
    public float ySpacing;

    Transform parent;

	// Use this for initialization
	void Start () {
        InitializeFloor();
        BuildFloor();
	}

    
    private void OnValidate()
    {
        InitializeFloor();
        BuildFloor();
    }

    void InitializeFloor()
    {
        if(parent != null)
        {
            Destroy(parent.gameObject);
        }
        parent = new GameObject("FloorTilesParent").transform;
        parent.parent = transform;
        
    }

    void BuildFloor()
    {
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                
                Instantiate(floorTiles[Random.Range(0, floorTiles.Length-1)], transform.position + new Vector3(i * xSpacing, j * ySpacing, 0), Quaternion.identity, parent);
            }
        }
    }
}
