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
    void Start()
    {

            // Spawn road tiles
            for (int j = 0; j < lanes.Length; j++)
            {
                for (int i = 0; i < tilesOnScreen; i++)
                {
                    lanes[j].SpawnTile(roadPrefab);
                }
            }
    }

    void Update()
    {




        foreach (Lane lane in lanes)
        {
            lane.MoveTiles(moveSpeed);

            if (lane.activeTiles.Peek().transform.position.z < -tileLength)
            {
                lane.RecycleTile();
            }
        }
    }
}