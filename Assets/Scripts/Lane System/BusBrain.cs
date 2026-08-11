using System.Collections;
using UnityEngine;

public class BusBrain : MonoBehaviour
{
    public bool busStopping = false;
    public CarMotor motor;
    [SerializeField] float rayHeight = 1f;
    [SerializeField] float rayLength = 50f;

    [SerializeField] BrakeLight indicatorL;
    [SerializeField] BrakeLight indicatorR;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        indicatorL.isDisabled = true;
        indicatorR.isDisabled = true;

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
        if (other.gameObject.GetComponent<LaneTile>().tileType == LaneTileType.Pavement)
        {
            motor.decelerate(25f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Car"))
        {
            // Implement collision logic here
            motor.decelerate(5f); // Example: decelerate when a car is hit
        }


    }

    void DetectRaycast()
    {
        if (RaycastForward() < rayLength)
        {
            // Implement collision logic here
            motor.decelerate(5f); // Example: decelerate when an obstacle is detected
        }
    }

    IEnumerator BusStopCoroutine()
    {
        while (true)
        {
            busStopping = true;
            indicatorL.isDisabled = true;
            indicatorR.isDisabled = false;
            yield return new WaitForSeconds(3f); // Example: stop for 3 seconds
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
        if (RaycastForward() > 5f)
        {
            motor.accelerate(25f);
        }

    }
}
