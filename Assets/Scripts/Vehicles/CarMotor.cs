using UnityEngine;

public class CarMotor : MonoBehaviour
{
    float carSpeed;
    [SerializeField] BrakeLight brakeLightL;
    [SerializeField] BrakeLight brakeLightR;
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
        if (carSpeed < 10)
            carSpeed += Mathf.Abs(acceleration * Time.deltaTime);
        //Debug.Log("Car Accelerating: " + acceleration + ", Current Speed: " + carSpeed);

        brakeLightL.isDisabled = true;
        brakeLightR.isDisabled = true;
    }

    public void decelerate(float deceleration)
    {
        // Implement deceleration logic here
        if(carSpeed > 0)
        {
            carSpeed -= Mathf.Abs(deceleration * Time.deltaTime);
            if(carSpeed < 0)
            {
                carSpeed = 0; // prevent speed from going negative
            }
        }
        //Debug.Log("Car Decelerating: " + deceleration + ", Current Speed: " + carSpeed);

        brakeLightL.isDisabled = false;
        brakeLightR.isDisabled = false;
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
