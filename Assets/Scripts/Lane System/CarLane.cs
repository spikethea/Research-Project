using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CarLane : MonoBehaviour
{
    public float ZboundsThreshold = -50f;
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
    public void SpawnCar(bool isExperimentMode) {



            GameObject hazard = Instantiate(
            CarPrefab,
            new Vector3(xPosition, yPosition, zSpawn),
            Quaternion.identity
        );
            var hazardScript = hazard.GetComponent<CarBrain>();
            hazardScript.experimentMode = isExperimentMode;
            activeCars.Enqueue(hazardScript);
        

    }

    public void MoveObjects(float moveSpeed)
    {
        foreach (CarBrain car in activeCars)
        {
            if (!car)
                continue;

            if (!car.motor)
                continue;

            car.motor.Move(moveSpeed);
        }
    }

    // Update is called once per frame
    void Update()
    {
        while (activeCars.Count > 0)
        {
            CarBrain car = activeCars.Peek();

            if (!car)
            {
                activeCars.Dequeue();
                continue;
            }

            if (car.transform.position.z < -10f)
            {
                activeCars.Dequeue();
                Destroy(car.gameObject);
            }
            else
            {
                break;
            }
        }
    }
}
