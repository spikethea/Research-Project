using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Lane : MonoBehaviour
{
    public float xPosition;
    public float yPosition;

    public float tileLength = 10f;

    public Queue<GameObject> activeTiles =
        new Queue<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        xPosition = transform.position.x;
        yPosition = transform.position.y;
    }

    // Update is called once per frame

    void Update()
    {
        GameObject firstTile = activeTiles.Peek();
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

    public void SpawnTile(GameObject prefab, float spawnZ)
    {
        GameObject tile = Instantiate(
            prefab,
            new Vector3(xPosition, yPosition, spawnZ),
            Quaternion.identity
        );

        activeTiles.Enqueue(tile);

        spawnZ += tileLength;
    }

    public void RecycleTile(float spawnZ)
    {
        GameObject tile = activeTiles.Dequeue();

        tile.transform.position =
            new Vector3(xPosition, yPosition, spawnZ);

        spawnZ += tileLength;

        activeTiles.Enqueue(tile);
    }
}
