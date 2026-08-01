using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CarLane : MonoBehaviour
{
    public float xOffset;
    public float yOffset;
    public float zSpawn = 100f;

    public GameObject CarPrefab;
    public Queue<CarBrain> activeCars =
        new Queue<CarBrain>();


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
        var hazardScript = hazard.GetComponent<CarBrain>();
        activeCars.Enqueue(hazardScript);
    }

    public void SpawnCar() {
        SpawnHazard(CarPrefab);
    }

    public void MoveObjects(float moveSpeed) {
        //Move to CarMotor
        foreach(CarBrain car in activeCars) {
            car.motor.Move(moveSpeed);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (CarBrain car in activeCars)
        {
            
            if (car.transform.position.z < -10f)
            {
                Destroy(car.gameObject);
                activeCars.Dequeue();
            }
        }
    }
}
