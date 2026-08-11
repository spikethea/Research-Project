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
            bus.motor.Move(moveSpeed);
        }
    }

    // Update is called once per frame
    void Update()
    {
        while (activeBuses.Count > 0 &&
           activeBuses.Peek().transform.position.z < -10f)
        {
            BusBrain bus = activeBuses.Dequeue();
            Destroy(bus.gameObject);
        }
    }
}
