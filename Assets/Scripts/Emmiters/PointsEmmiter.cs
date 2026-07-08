using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsEmmiter : MonoBehaviour
{
    [SerializeField] EndlessRunnerEmitter emitter;
    [SerializeField] private GameObject emitterObject;

    public float frequency = 1f;
    public float yOffset = 1;

    private int currentLanePosition = 0;
    private List<GameObject> emittedObjects = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Start");
        StartCoroutine(EmitFoodBag());
    }

    IEnumerator EmitFoodBag()
    {
        while (true)
        {

            currentLanePosition = Random.Range(0, emitter.roadLanes.Length);

            GameObject FoodBag = Instantiate(
                emitterObject,
                new Vector3(
                    emitter.roadLanes[currentLanePosition].transform.position.x,
                    emitter.roadLanes[currentLanePosition].transform.position.y + yOffset,
                    emitter.roadLanes[currentLanePosition].nextSpawnZ
                ),
                Quaternion.identity
            );

            emittedObjects.Add(FoodBag);
            Debug.Log("Emitted FoodBag at lane: " + currentLanePosition + " position: " + FoodBag.transform.position);
            yield return new WaitForSeconds(frequency);

            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = emittedObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = emittedObjects[i];

            obj.transform.position += Vector3.back * emitter.currentMoveSpeed * Time.deltaTime;

            if (obj.transform.position.z < -10f)
            {
                Destroy(obj);
                emittedObjects.RemoveAt(i);
            }
        }


    }

}
