using System.Collections.Generic;
using UnityEngine;

public class EndlessRunnerEmitter : MonoBehaviour
{
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private GameObject buildingPrefab;

    public Lane[] lanes;

    public int tilesOnScreen = 8;
    public float tileLength = 10f;
    public float moveSpeed = 10f;

    public float xOffset;
    public float yOffset;

    private float spawnZ = 0f;

    private Queue<GameObject> activeTiles =
        new Queue<GameObject>();

    void Start()
    {

            // Spawn road tiles
            for (int j = 0; j < lanes.Length; j++)
            {
                lanes[j].SpawnTile(roadPrefab, spawnZ);
            }
    }

    void Update()
    {
        

        

        for (int j = 0; j < lanes.Length; j++)
        {
            GameObject firstTile = lanes[j].activeTiles.Peek();

            lanes[j].MoveTiles(moveSpeed);
            if (firstTile.transform.position.z < -tileLength)
            {
                lanes[j].RecycleTile(spawnZ);
            }
        }
    }
}