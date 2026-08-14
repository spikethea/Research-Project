using UnityEngine;
using UnityEngine.Audio;

public class Roadworks : MonoBehaviour
{

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip CrashClip;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var bike = other.transform.GetComponentInChildren<Bike>();



            if (bike)
            {
                audioSource.PlayOneShot(CrashClip);
                bike.CrashBike(2.5f);
                Destroy(gameObject, 2f);
            }
        }

        if (other.gameObject.CompareTag("Hazard") || other.gameObject.CompareTag("Consumable"))
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
