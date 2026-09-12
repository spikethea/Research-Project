using System.Collections.Generic;
using UnityEngine;

public class BusLane : MonoBehaviour
{
    public bool flippedIndicators = false;

    public float ZboundsThreshold = -50f;
    public float xOffset;
    public float yOffset;
    public float zSpawn = 100f;

    public GameObject BusPrefab;
    public Queue<BusBrain> activeBuses =
        new Queue<BusBrain>();


    private float xPosition;
    private float yPosition;

    private void Awake()
    {
        xPosition = transform.position.x + xOffset;
        yPosition = transform.position.y + yOffset;
    }

    void Start()
    {

    }

    public void SpawnHazard(GameObject hazardPrefab)
    {
        GameObject hazard = Instantiate(
            hazardPrefab,
            new Vector3(xPosition, yPosition, zSpawn),
            Quaternion.identity
        );
        var hazardScript = hazard.GetComponent<BusBrain>();
        if(hazardScript != null && flippedIndicators)
            hazardScript.isFlipped = true;
        activeBuses.Enqueue(hazardScript);
    }

    public void SpawnBus()
    {
    //    if (GameManager.Instance.reinforcementMode == Mode.Negative
    //|| GameManager.Instance.reinforcementMode == Mode.Mixed)
    //    {
            SpawnHazard(BusPrefab);
        //}
    }

    public void MoveObjects(float moveSpeed)
    {
        //Move to BusMotor
        foreach (BusBrain bus in activeBuses)
        {
            if (!bus)
                continue;

            if (!bus.motor)
                continue;
            bus.motor.Move(moveSpeed);
        }
    }

    // Update is called once per frame
    void Update()
    {
        while (activeBuses.Count > 0)
        {
            BusBrain bus = activeBuses.Peek();
            if (!bus)
            {
                activeBuses.Dequeue();
                continue;
            }

            if (activeBuses.Peek().transform.position.z < ZboundsThreshold) {
                activeBuses.Dequeue();
                Destroy(bus.gameObject);
            }
            else
            {
                break;
            }
        }
    }
}
