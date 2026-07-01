using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Lane : MonoBehaviour
{

    public float xOffset;
    public float yOffset;

    private float nextSpawnZ = 0f;
    private float xPosition;
    private float yPosition;

    public float tileLength = 10f;

    public GameObject currentPrefab;

    public Queue<GameObject> activeTiles =
        new Queue<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        xPosition = transform.position.x + xOffset;
        yPosition = transform.position.y + yOffset;
    }

    // Update is called once per frame

    void Update()
    {
        //Debug.Log(gameObject.name + "nextSpawnZ " + nextSpawnZ);
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

    public void SpawnTile()
    {

        Debug.Log(gameObject.name + " spawning tile position: " + xPosition + ", " + yPosition + ", " + nextSpawnZ);
        GameObject tile = Instantiate(
            currentPrefab,
            new Vector3(xPosition, yPosition, nextSpawnZ),
            Quaternion.identity
        );

        if (activeTiles.Count == 0)
        {
            Renderer renderer = tile.GetComponent<Renderer>();

            if (renderer != null)
            {
                Debug.Log("Tile length: " + renderer.bounds.size.z);
                Debug.Log("Tile width: " + renderer.bounds.size.x);
                tileLength = renderer.bounds.size.z;
            }
        }

        activeTiles.Enqueue(tile);

        nextSpawnZ += tileLength;
    }

    public void RecycleTile()
    {
        GameObject oldTile = activeTiles.Dequeue();

        // Find the tile currently at the end of the lane
        GameObject lastTile = null;
        foreach (GameObject tile in activeTiles)
        {
            lastTile = tile;
        }

        LaneTile lastTileInfo = lastTile.GetComponent<LaneTile>();

        if (oldTile.GetComponent<LaneTile>().tileType == currentPrefab.GetComponent<LaneTile>().tileType)
        {
            LaneTile oldTileInfo = oldTile.GetComponent<LaneTile>();

            float spawnDistance =
                lastTileInfo.Length * 0.5f +
                oldTileInfo.Length * 0.5f;

            oldTile.transform.position =
                lastTile.transform.position +
                Vector3.forward * spawnDistance;

            activeTiles.Enqueue(oldTile);
        }
        else
        {
            Destroy(oldTile);

            GameObject newTile = Instantiate(
                currentPrefab,
                Vector3.zero,
                Quaternion.identity);

            LaneTile newTileInfo = newTile.GetComponent<LaneTile>();

            float spawnDistance =
                lastTileInfo.Length * 0.5f +
                newTileInfo.Length * 0.5f;

            newTile.transform.position =
                lastTile.transform.position +
                Vector3.forward * spawnDistance;

            activeTiles.Enqueue(newTile);
        }
    }
}
