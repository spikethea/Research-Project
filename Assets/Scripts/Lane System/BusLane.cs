using System.Collections.Generic;
using UnityEngine;

public class BusLane : MonoBehaviour
{
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
        activeBuses.Enqueue(hazardScript);
    }

    public void SpawnBus()
    {
        SpawnHazard(BusPrefab);
    }

    public void MoveObjects(float moveSpeed)
    {
        //Move to BusMotor
        foreach (BusBrain bus in activeBuses)
        {
            bus.motor.Move(moveSpeed);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (BusBrain bus in activeBuses)
        {

            if (bus.transform.position.z < -10f)
            {
                Destroy(bus.gameObject);
                activeBuses.Dequeue();
            }
        }
    }
}
