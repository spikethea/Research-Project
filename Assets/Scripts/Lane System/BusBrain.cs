using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class BusBrain : MonoBehaviour
{
    public bool isFlipped = false;
    public bool busStopping = false;
    public CarMotor motor;
    [SerializeField] float rayHeight = 1f;
    [SerializeField] float rayLength = 20f;

    [SerializeField] BrakeLight indicatorL;
    [SerializeField] BrakeLight indicatorR;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip carCrashClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        indicatorL.isDisabled = true;
        indicatorR.isDisabled = true;

        if (isFlipped)
        {
            BrakeLight temp = indicatorL;
            indicatorL = indicatorR;
            indicatorR = temp;
        }

        StartCoroutine(BusStopCoroutine());
    }

    float RaycastForward()
    {
        //Debug.Log("Raycasting...");
        Vector3 targetDirection = Vector3.forward;

        Ray ray = new Ray(transform.position + (Vector3.up * rayHeight), targetDirection);

        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(ray, out hitInfo, rayLength))
        {
            Debug.DrawRay(ray.origin, ray.direction * hitInfo.distance, Color.red);
            //Debug.Log("Hit: " + hitInfo.collider.name + ", Distance: " + hitInfo.distance);
            return hitInfo.distance;
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.white);
            return Mathf.Infinity;
        }


    }

    private void OnTriggerStay(Collider other)
    {
        var laneTile = other.gameObject.GetComponent<LaneTile>();
        if (laneTile != null && laneTile.tileType == LaneTileType.Pavement)
        {
            motor.decelerate(25f);
        }
    }

    private void OnTriggerEnter(UnityEngine.Collider other)
    {
        if (other.gameObject.CompareTag("Pavement"))
        {
            motor.decelerate(25f);

        }

        if (other.gameObject.CompareTag("Hazard") || other.gameObject.CompareTag("Car"))
        {
            
            motor.decelerate(25f); 
            

        }

        if (other.gameObject.CompareTag("Player"))
        {
            var bike = other.transform.GetComponentInChildren<Bike>();

            if (bike)
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

    void DetectRaycast()
    {
        if (RaycastForward() < rayLength)
        {
            // Implement collision logic here
            motor.decelerate(25f); // Example: decelerate when an obstacle is detected
        }
    }

    IEnumerator BusStopCoroutine()
    {
        while (true)
        {
            busStopping = true;
            indicatorL.isDisabled = true;
            indicatorR.isDisabled = false;
            yield return new WaitForSeconds(13f); // Example: stop for 3 seconds
            indicatorL.isDisabled = false;
            indicatorR.isDisabled = true;
            busStopping = false;
            yield return new WaitForSeconds(3f); // hand indicator for 3 seconds
            indicatorL.isDisabled = true;
            indicatorR.isDisabled = true;

            yield return new WaitForSeconds(10f); // Example: wait for 10 seconds before stopping again
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(busStopping)
        {
            motor.decelerate(5f);
            return;
        }
        DetectRaycast();
        if (RaycastForward() > rayLength)
        {
            motor.accelerate(25f);
        }

    }
}
