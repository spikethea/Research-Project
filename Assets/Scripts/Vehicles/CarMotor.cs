using UnityEngine;

public class CarMotor : MonoBehaviour
{
    float carSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void accelerate(float acceleration)
    {
        // Implement acceleration logic here
        if(carSpeed < 0)
        {
            carSpeed = 0; // prevent acceleration from becoming negative
        }
        carSpeed += acceleration * Time.deltaTime;
        Debug.Log("Car Accelerating: " + acceleration + ", Current Speed: " + carSpeed);
    }

    public void Move(float moveSpeed)
    {
        transform.position +=
            Vector3.back *
            (moveSpeed - carSpeed) *
            Time.deltaTime;
        
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
