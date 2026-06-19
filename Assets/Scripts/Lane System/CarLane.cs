using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CarLane : MonoBehaviour
{
    public float xOffset;
    public float yOffset;
    public float zSpawn = 100f;

    public GameObject CarPrefab;
    public Queue<GameObject> activeHazards =
        new Queue<GameObject>();

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

    public void SpawnHazard(GameObject hazardPrefab) {
        GameObject hazard = Instantiate(
            hazardPrefab,
            new Vector3(xPosition, yPosition, zSpawn),
            Quaternion.identity
        );
        activeHazards.Enqueue(hazard);
    }

    public void SpawnCar() {
        SpawnHazard(CarPrefab);
    }

    public void MoveObjects(float moveSpeed) {
        foreach(GameObject hazard in activeHazards) {
            hazard.transform.position +=
                Vector3.back *
                moveSpeed *
                Time.deltaTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
