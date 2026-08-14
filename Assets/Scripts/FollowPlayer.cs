using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    public Transform playerTransform;
    public float followSpeed = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, playerTransform.position, Time.deltaTime * followSpeed);
        transform.rotation = Quaternion.Euler(transform.rotation.x, playerTransform.rotation.eulerAngles.y, transform.rotation.z); ;
    }
}
