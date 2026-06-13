using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Lane : MonoBehaviour
{

    public float xOffset;
    public float yOffset;
    private float nextSpawnZ;

    private float xPosition;
    private float yPosition;

    public float tileLength = 10f;

    public Queue<GameObject> activeTiles =
        new Queue<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        xPosition = transform.position.x + xOffset;
        yPosition = transform.position.y + yOffset;

        nextSpawnZ = 0f;
    }

    // Update is called once per frame

    void Update()
    {

    }

    public void MoveTiles(float moveSpeed)
    {
        foreach (GameObject tile in activeTiles)
        {
            tile.transform.position +=
                Vector3.back *
                moveSpeed *
                Time.deltaTime;
        }
    }

    public void SpawnTile(GameObject prefab)
    {
        Debug.Log(gameObject.name + " spawning tile position: " + xPosition + ", " + yPosition + ", " + nextSpawnZ);
        GameObject tile = Instantiate(
            prefab,
            new Vector3(xPosition, yPosition, nextSpawnZ),
            Quaternion.identity
        );

        activeTiles.Enqueue(tile);

        nextSpawnZ += tileLength;
    }

    public void RecycleTile()
    {
        GameObject tile = activeTiles.Dequeue();

        tile.transform.position =
            new Vector3(xPosition, yPosition, nextSpawnZ);

        nextSpawnZ += tileLength;

        activeTiles.Enqueue(tile);
    }
}
