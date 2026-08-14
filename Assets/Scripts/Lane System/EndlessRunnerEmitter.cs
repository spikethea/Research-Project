using System.Collections;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;



public class EndlessRunnerEmitter : MonoBehaviour
{
    [SerializeField] private GameObject pavementPrefab;
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private GameObject busLanePrefab;
    [SerializeField] private GameObject buildingPrefab;
    [SerializeField] private TextMeshPro PhoneScreen;
    [SerializeField] private Player player;

    public Lane[] roadLanes;
    public Lane[] roadLanesBus;
    public CarLane[] CarLanes;
    public BusLane[] BusLanes;
    public BuildingLane buildingLaneLeft;
    public BuildingLane buildingLaneRight;

    public int tilesOnScreen = 8;
    public int buildingsOnScreen = 8;

    public float initialMoveSpeed;
    public float currentMoveSpeed = 10f;

    public bool experimentMode = true;
    public bool pedestrianMode = false;

    public float xOffset;
    public float yOffset;

    public bool gameStarted = false;

    void Start()
    {
        currentMoveSpeed = initialMoveSpeed;

        // Spawn road tiles
        for (int j = 0; j < roadLanes.Length; j++)
        {
            for (int i = 0; i < tilesOnScreen; i++)
            {
                roadLanes[j].SpawnTile();
            }
        }

        for (int j = 0; j < roadLanesBus.Length; j++)
        {
            for (int i = 0; i < tilesOnScreen; i++)
            {
                roadLanesBus[j].SpawnTile();
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
    }

    public void StartGame() {
        gameStarted = true;

        StartCoroutine(SpawnCars());
        StartCoroutine(SpawnBuses());
        StartCoroutine(GameStructure());
    }

    IEnumerator GameStructure() {
        while (true)
        {
            while (!gameStarted) yield return null;
            pedestrianMode = true;
            PhoneScreen.text = "Now for a 60 second break \n\n Press <b>X</b> (Left Hand) to Mirror bike movement \n\n Press <b>B</b> (Right Hand) to re-calibrate your head position";
            yield return new WaitForSeconds(45f);
            PhoneScreen.text = "Hold Handlebars to start\r\n\r\nTilt and Signal to change lanes\r\n\r\nRaise your Right Hand to STOP\r\n\r\n<color=red>Avoid Hazards, Cars and Buses</color>\r\n\r\n<color=green>Follow Road Signs, Collect and Deliver Food Bags</color>\r\n\r\n<color=red> STOP</color><color=blue> for Zebra Crossings and Deliver Food</color>\r\n";
            GameManager.Instance.SetRandomMode();
            pedestrianMode = false;
            yield return new WaitForSeconds(180f);
            

        }
    }

    IEnumerator SpawnCars()
    {
        while (true)
        {
            while (player.onPavement
                || !gameStarted
                || GameManager.Instance.reinforcementMode == Mode.Positive
                )
            {
                yield return null;
            }

            int emptyLane = Random.Range(0, CarLanes.Length);

            for (int i = 0; i < CarLanes.Length; i++)
            {
                if (i != emptyLane)
                {
                    // dont spawn extra cars unless bike is moving
                    if (currentMoveSpeed > 5)
                        CarLanes[i].SpawnCar(experimentMode);
                    
                }
            }


            yield return new WaitForSeconds(10f);
            

            //var randomCarLane = CarLanes[Random.Range(0, CarLanes.Length)];

            //if (!pedestrianMode)
            //{
            //    randomCarLane.SpawnCar(experimentMode);
            //}
            //yield return new WaitForSeconds(6f);

        }
    }

    IEnumerator SpawnBuses()
    {
        while (true)
        {
            while (player.onPavement || !gameStarted) yield return null;

            var randomBusLane = BusLanes[Random.Range(0, BusLanes.Length)];
            if (!pedestrianMode)
            {
                randomBusLane.SpawnBus();
            }

            yield return new WaitForSeconds(15f);
        }
    }

    void Update()
    {
        if (!gameStarted && currentMoveSpeed < 10f) return;

        if(pedestrianMode)
        {

            foreach (Lane lane in roadLanes)
            {
                if (lane.currentPrefab != pavementPrefab)
                {
                    lane.currentPrefab = pavementPrefab;

                }
            }

            foreach (Lane lane in roadLanesBus)
            {
                if (lane.currentPrefab != pavementPrefab)
                {
                    lane.currentPrefab = pavementPrefab;

                }
            }
        } else {
            // Allow Bike script to speed up player
            foreach (Lane lane in roadLanes)
            {
                lane.currentPrefab = roadPrefab;
            }

            foreach(Lane lane in roadLanesBus)
            {
                lane.currentPrefab = busLanePrefab;
            }
        }

            foreach (Lane lane in roadLanes)
            {
                //Debug.Log(
                //    lane.name + " x=" + lane.transform.position.x
                //);

                lane.MoveTiles(currentMoveSpeed);

                
                if (lane.activeTiles.Count > 0 && 
                lane.activeTiles.Peek().transform.position.z < -lane.tileLength)
                {

                    lane.RecycleTile();
                }
            }

            foreach (Lane lane in roadLanesBus)
            {
                //Debug.Log(

                //    lane.name + " x=" + lane.transform.position.x
                //);

                lane.MoveTiles(currentMoveSpeed);


                if (lane.activeTiles.Count > 0 && lane.activeTiles.Peek().transform.position.z < -lane.tileLength)
                {

                    lane.RecycleTile();
                }
            }

        foreach (CarLane carLane in CarLanes)
        {
           carLane.MoveObjects(currentMoveSpeed);
        }

        foreach (BusLane busLane in BusLanes)
        {
            busLane.MoveObjects(currentMoveSpeed);
        }

        // Building Lanes
        buildingLaneLeft.MoveTiles(currentMoveSpeed);
        buildingLaneRight.MoveTiles(currentMoveSpeed);


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