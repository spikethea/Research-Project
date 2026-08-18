using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FoodPickup : MonoBehaviour
{

    [SerializeField] Transform staff;
    [SerializeField] Animator animator;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip chaChingClip;
    [SerializeField] MeshRenderer MoneyText;
    [SerializeField] Canvas stopSign;

    [SerializeField] Collider _collider;

    [SerializeField] GameObject foodBagMesh;
    [SerializeField] Transform pickupPoint;


    public float stoppingDuration;


    private float stoppingTimer;
    private Transform playerTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MoneyText.enabled = false;
        stopSign.enabled = true;

        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(stoppingTimer > stoppingDuration)
        {
            MoneyText.enabled = true;
            stopSign.enabled = false;
            if (!audioSource.isPlaying)
                audioSource.PlayOneShot(chaChingClip);
            animator.SetBool("Waving", false);
            animator.SetBool("Bowing", true);
            _collider.enabled = false;
            stoppingTimer = 0;


            
        }
        if (MoneyText.enabled)
            lookAtPlayer();
    }

    private void lookAtPlayer() {
        if (playerTransform == null) return;

        Vector3 direction = playerTransform.position - staff.position;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            staff.rotation = lookRotation * Quaternion.Euler(-90f, 0f, 0f);
        }


    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) {
            Bike bike = other.gameObject.GetComponentInChildren<Bike>();

            if ( stoppingTimer < stoppingDuration) {
                stoppingTimer += Time.deltaTime;
                

                // Food Bag to attendant
                if (!foodBagMesh.activeSelf)
                {
                    foodBagMesh.transform.position = bike.transform.position;
                    foodBagMesh.SetActive(true);
                }
                    

                var step = 5 * Time.deltaTime;
                foodBagMesh.transform.position = Vector3.MoveTowards(foodBagMesh.transform.position, pickupPoint.transform.position, step);
            }
        }

        if(other.gameObject.CompareTag("Hazard") || other.gameObject.CompareTag("Consumable") || other.gameObject.CompareTag("Car"))
        {
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("Pavement")) {
            Destroy(gameObject);
        }
    }
}
