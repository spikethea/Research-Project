using System.Collections.Generic;
using UnityEngine;

public class BuildingLane : MonoBehaviour
{

    public float xOffset;
    public float yOffset;
    public float nextSpawnZ = 0f;

    private float xPosition;
    private float yPosition;

    public float tileLength = 10f;
    public float floorHeight = 3f;

    public GameObject currentPrefab;
    public List<Texture> Facades;

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
        
        float buildingHeight = UnityEngine.Random.Range(1, 4); // Random height between 1 and 3

        GameObject tile = Instantiate(
            currentPrefab,
            new Vector3(xPosition, buildingHeight*floorHeight + yOffset, nextSpawnZ),
            Quaternion.identity
        );

        tile.GetComponent<Renderer>().material.mainTexture = Facades[UnityEngine.Random.Range(0, Facades.Count)];



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

            float buildingHeight = UnityEngine.Random.Range(1, 4); // Random height between 1 and 3


            oldTile.transform.position =
                new Vector3(xPosition, buildingHeight * floorHeight + yOffset, lastTile.transform.position.z) +
                Vector3.forward * spawnDistance;

            activeTiles.Enqueue(oldTile);
        }
        else
        {
            Destroy(oldTile);

            float buildingHeight = UnityEngine.Random.Range(1, 4); // Random height between 1 and 3

            GameObject newTile = Instantiate(
                currentPrefab,
                new Vector3(xPosition, buildingHeight * floorHeight + yOffset, nextSpawnZ),
                Quaternion.identity
            );

            newTile.GetComponent<Renderer>().material.mainTexture = Facades[UnityEngine.Random.Range(0, Facades.Count)];



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
