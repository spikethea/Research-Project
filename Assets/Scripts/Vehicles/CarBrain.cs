using UnityEngine;

public class CarBrain : MonoBehaviour
{
    public CarMotor motor;
    [SerializeField] float rayHeight = 1f;
    [SerializeField] float MaxRayLength = 20f;
    [SerializeField] float MinRayLength = 5f;
    public LayerMask IgnoreMe;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    float RaycastForward() {
        //Debug.Log("Raycasting...");
        Vector3 targetDirection = Vector3.forward;
                
            Ray ray = new Ray(transform.position + (Vector3.up * rayHeight), targetDirection);
            
            RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(ray, out hitInfo, MaxRayLength, ~IgnoreMe))
        {
            Debug.DrawRay(ray.origin, ray.direction * hitInfo.distance, Color.red);
            //Debug.Log("Hit: " + hitInfo.collider.name + ", Distance: " + hitInfo.distance);
            return hitInfo.distance;
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * MaxRayLength, Color.white);
            return Mathf.Infinity;
        }


    }

    void DetectCollision()
    {
        if(RaycastForward() > MaxRayLength)
        {
            // Implement collision logic here
            motor.accelerate(5f); // Example: decelerate when an obstacle is detected
        } else if (RaycastForward() < MinRayLength)
        {
            motor.decelerate(5f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        DetectCollision();
        
        
    }
}
