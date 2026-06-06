using System.Collections.Generic;
using UnityEngine;

public class EndlessRunnerEmitter : MonoBehaviour
{
    [SerializeField] private GameObject roadPrefab;

    public int tilesOnScreen = 8;
    public float tileLength = 10f;
    public float moveSpeed = 10f;

    private float spawnZ = 0f;

    private Queue<GameObject> activeTiles =
        new Queue<GameObject>();

    void Start()
    {
        for (int i = 0; i < tilesOnScreen; i++)
        {
            SpawnTile();
        }
    }

    void Update()
    {
        MoveTiles();

        GameObject firstTile = activeTiles.Peek();

        if (firstTile.transform.position.z < -tileLength)
        {
            RecycleTile();
        }
    }

    void MoveTiles()
    {
        foreach (GameObject tile in activeTiles)
        {
            tile.transform.position +=
                Vector3.back *
                moveSpeed *
                Time.deltaTime;
        }
    }

    void SpawnTile()
    {
        GameObject tile = Instantiate(
            roadPrefab,
            new Vector3(0, 0, spawnZ),
            Quaternion.identity
        );

        activeTiles.Enqueue(tile);

        spawnZ += tileLength;
    }

    void RecycleTile()
    {
        GameObject tile = activeTiles.Dequeue();

        tile.transform.position =
            new Vector3(0, 0, spawnZ);

        spawnZ += tileLength;

        activeTiles.Enqueue(tile);
    }
}