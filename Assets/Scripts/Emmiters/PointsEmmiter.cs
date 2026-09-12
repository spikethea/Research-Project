using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsEmmiter : MonoBehaviour
{
    [SerializeField] EndlessRunnerEmitter emitter;
    [SerializeField] private GameObject emitterObject;

    public float emitterOffset;
    public float WaitingTime = 1f;
    public float ExperimentWaitingTime = 1f;
    public float xOffset = 0f;
    public float yOffset = 0f;
    public float ZThreshold = -50f;

    private int currentLanePosition = 0;
    private List<GameObject> emittedObjects = new List<GameObject>();
    public List<Mode> reinforcementModes = new List<Mode>();

    [SerializeField]
    public List<bool> ActiveLanes = new List<bool> 
        {
        true,
        true,
        true,
        };

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {

        emitterOffset = Random.Range(0, 10);

        Debug.Log("Start");
        StartCoroutine(EmitFoodBag());
    }

    int GetRandomEnabledLane()
    {
        List<int> availableLanes = new List<int>();

        for (int i = 0; i < ActiveLanes.Count; i++)
        {
            if (ActiveLanes[i])
                availableLanes.Add(i);
        }

        if (availableLanes.Count == 0)
            return -1;

        return availableLanes[Random.Range(0, availableLanes.Count)];
    }

    public void SpawnFoodBagInLane(int lanePosition, float zSpawn)
    {
        
        if (emitter.gameStarted && emitter.currentMoveSpeed > 10)
        {

                GameObject FoodBag = Instantiate(
                    emitterObject,
                    new Vector3(
                        emitter.roadLanes[lanePosition].transform.position.x + xOffset,
                        emitter.roadLanes[lanePosition].transform.position.y + yOffset,
                        zSpawn
                    ),
                    Quaternion.identity
                );

                emittedObjects.Add(FoodBag);
                Debug.Log("Emitted FoodBag at lane: " + currentLanePosition + " position: " + FoodBag.transform.position);
            }
        
    }

    IEnumerator EmitFoodBag()
    {
        yield return new WaitUntil(() => emitter.gameStarted && emitter.currentMoveSpeed > 10);
        yield return new WaitForSeconds(emitterOffset);
        while (true)
        {
            if (emitter.currentMoveSpeed > 10) {

                if (reinforcementModes.Contains(GameManager.Instance.reinforcementMode)
                ) {
                    currentLanePosition = GetRandomEnabledLane();

                    GameObject FoodBag = Instantiate(
                        emitterObject,
                        new Vector3(
                            emitter.roadLanes[currentLanePosition].transform.position.x + xOffset,
                            emitter.roadLanes[currentLanePosition].transform.position.y + yOffset,
                            emitter.roadLanes[currentLanePosition].nextSpawnZ
                        ),
                        Quaternion.identity
                    );

                    emittedObjects.Add(FoodBag);
                    Debug.Log("Emitted FoodBag at lane: " + currentLanePosition + " position: " + FoodBag.transform.position);
                    yield return new WaitForSeconds(emitter.experimentMode ? ExperimentWaitingTime : WaitingTime);
                }



            }

            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = emittedObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = emittedObjects[i];

            if (emitter.gameStarted && obj != null) // only move if the game has started 
                obj.transform.position += Vector3.back * emitter.currentMoveSpeed * Time.deltaTime;

            if (obj != null && obj.transform.position.z < ZThreshold)
            {
                Destroy(obj);
                emittedObjects.RemoveAt(i);
            }
        }


    }

}
