using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FoodPickup : MonoBehaviour
{

    [SerializeField] Transform staff;
    [SerializeField] Animator animator;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip chaChingClip;
    [SerializeField] MeshRenderer MoneyText;
    [SerializeField] Collider _collider;

    

    public float stoppingDuration;


    private float stoppingTimer;
    private Transform playerTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MoneyText.enabled = false;

        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(stoppingTimer > stoppingDuration)
        {
            MoneyText.enabled = true;
            if(!audioSource.isPlaying)
                audioSource.PlayOneShot(chaChingClip);
            animator.SetBool("Waving", false);
            animator.SetBool("Bowing", true);
            _collider.enabled = false;
            stoppingTimer = 0;
        }

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

            //if (bike.isStopping) {
            stoppingTimer += Time.deltaTime;
            //}
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
