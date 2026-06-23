using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessRunnerEmitter : MonoBehaviour
{
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private GameObject buildingPrefab;

    public Lane[] lanes;
    public CarLane[] CarLanes;
    public Lane buildingLaneLeft;
    public Lane buildingLaneRight;

    public int tilesOnScreen = 8;
    public int buildingsOnScreen = 8;
    public float moveSpeed = 10f;

    public float xOffset;
    public float yOffset;

    void Start()
    {

        // Spawn road tiles
        for (int j = 0; j < lanes.Length; j++)
        {
            for (int i = 0; i < tilesOnScreen; i++)
            {
                lanes[j].SpawnTile();
            }
        }

        // building lanes
        for (int i = 0; i < buildingsOnScreen; i++)
        {
            buildingLaneLeft.SpawnTile();
        }

        for (int i = 0; i < buildingsOnScreen; i++)
        {
            buildingLaneRight.SpawnTile();
        }

        StartCoroutine(SpawnCars());
    }

    IEnumerator SpawnCars()
    {
        while (true)
        {
            var randomCarLane = CarLanes[Random.Range(0, CarLanes.Length)];
            randomCarLane.SpawnCar();
            
            yield return new WaitForSeconds(3f);
        }
    }

    void Update()
    {


        foreach (Lane lane in lanes)
        {
            //Debug.Log(
            //    lane.name + " x=" + lane.transform.position.x
            //);

            lane.MoveTiles(moveSpeed);

            if (lane.activeTiles.Peek().transform.position.z < -lane.tileLength)
            {
                lane.RecycleTile();
            }
        }

        foreach(CarLane carLane in CarLanes)
        {
           carLane.MoveObjects(moveSpeed);
        }

        // Building Lanes
        buildingLaneLeft.MoveTiles(moveSpeed);
        buildingLaneRight.MoveTiles(moveSpeed);


        // Check for space on z-axis then recycle if true
        if (buildingLaneLeft.activeTiles.Peek().transform.position.z < -buildingLaneLeft.tileLength)
        {
            buildingLaneLeft.RecycleTile();
        }

        if (buildingLaneRight.activeTiles.Peek().transform.position.z < -buildingLaneRight.tileLength)
        {
            buildingLaneRight.RecycleTile();
        }
    }
}