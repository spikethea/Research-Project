using UnityEngine;

public class CarBrain : MonoBehaviour
{
    public CarMotor motor;
    [SerializeField] float rayHeight = 1f;
    [SerializeField] float MaxRayLength = 20f;
    [SerializeField] float MinRayLength = 5f;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip carCrashClip;

    public LayerMask IgnoreMe;
    public bool experimentMode = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (experimentMode) {
        motor.setCarSpeed(5f); // Set a constant speed for the car in experiment mode
        }
    }

    float RaycastForward(float rayLength)
    {
        Vector3 targetDirection = Vector3.forward;

        Ray ray = new Ray(
            transform.position + (Vector3.up * rayHeight),
            targetDirection
        );

        if (Physics.Raycast(ray, out RaycastHit hitInfo, rayLength, ~IgnoreMe))
        {
            Debug.Log("Hit: " + hitInfo.collider.gameObject.name);
            return hitInfo.distance;
        }
        else
        {
            return Mathf.Infinity;
        }
    }

    void DetectRaycast()
    {
        float distance = RaycastForward(MaxRayLength);

        if (experimentMode) {
            motor.constantSpeed();

            return;
        }

        if (distance > MaxRayLength)
        {
            // Implement collision logic here
            motor.accelerate(5f); // Example: decelerate when an obstacle is detected
            Debug.DrawRay(transform.position + (Vector3.up * rayHeight), Vector3.forward * distance, Color.green);
        }
        else if (distance < MinRayLength)
        {
            motor.decelerate(15f);
            Debug.DrawRay(transform.position + (Vector3.up * rayHeight), Vector3.forward * distance, Color.red);
        }
        else {
            motor.constantSpeed();
            Debug.DrawRay(transform.position + (Vector3.up * rayHeight), Vector3.forward * distance, Color.white);
        }
    }

    private void OnTriggerEnter(UnityEngine.Collider other)
    {
        if (other.gameObject.CompareTag("Pavement"))
        {
            Destroy(gameObject);

        }

        if (other.gameObject.CompareTag("Hazard") || other.gameObject.CompareTag("Car"))
        {
            // Implement collision logic here
            if (experimentMode)
            {
                Destroy(gameObject);
            }
            else {
                motor.decelerate(25f); // Example: decelerate when a car is hit
            }
                
        }

        if (other.gameObject.CompareTag("Player")) {
            var bike = other.transform.GetComponentInChildren<Bike>();

            if(bike)
            {
                audioSource.PlayOneShot(carCrashClip);
                bike.CrashBike(2.5f);
                Destroy(gameObject, 2f);
            }
            
        }

        LaneTile laneTile = other.gameObject.GetComponent<LaneTile>();

        if (laneTile != null && laneTile.tileType == LaneTileType.Pavement)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        DetectRaycast();
        
        
    }
}
