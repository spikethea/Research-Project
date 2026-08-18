using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ZebraCrossing : MonoBehaviour
{
    private Bike bike;
    [SerializeField] private Commuter commuter;

    [SerializeField] private BoxCollider crossingCollider;

    [SerializeField] private Renderer zebraLightLeft;
    [SerializeField] private Renderer zebraLightRight;
    [SerializeField] private Renderer zebraLightLeft2;
    [SerializeField] private Renderer zebraLightRight2;

    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip screamClip;

    private bool eventStarted;
    private bool destinationReached;
    private bool bikeCrashed = false;
    private Material sharedTargetMaterial;
    private Color creamBaseColor = new Color(1.0f, 0.99f, 0.82f);
    [SerializeField] private float emissionIntensity = 2.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        commuter.randomWalkingDirection = false;
        Invoke(nameof(DisableCollider), 1f);
        bike = FindAnyObjectByType<Bike>();
        // 1. Get the material from the left renderer instance
        sharedTargetMaterial = zebraLightLeft.material;

        // 2. Assign the exact same instance to the right renderer so they share it
        zebraLightLeft2.material = sharedTargetMaterial;
        zebraLightRight.material = sharedTargetMaterial;
        zebraLightRight2.material = sharedTargetMaterial;
        

        // 3. Pre-calculate and apply the HDR cream color once
        Color hdrCream = creamBaseColor * Mathf.Pow(2, emissionIntensity);
        sharedTargetMaterial.SetColor("_EmissionColor", hdrCream);

        StartCoroutine(FlashingLight());

        //MoveCommuterToRightPoint();
    }

    void DisableCollider()
    {
        crossingCollider.enabled = false;
    }


    IEnumerator FlashingLight()
    {
        while (true)
        {
            // Both lights turn ON together
            sharedTargetMaterial.EnableKeyword("_EMISSION");
            yield return new WaitForSeconds(1f);

            // Both lights turn OFF together
            sharedTargetMaterial.DisableKeyword("_EMISSION");
            yield return new WaitForSeconds(1f);
        }
    }
    
    public void MoveCommuterToLeftPoint()
    {
        commuter.MoveToPoint(leftPoint.position, 10);
    }

    public void MoveCommuterToRightPoint()
    {
        commuter.MoveToPoint(rightPoint.position, 10);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (crossingCollider.enabled && other.gameObject.CompareTag("Player"))
        {
            var bike = other.transform.GetComponentInChildren<Bike>();

            if (bike && !destinationReached)
            {
                if (!audioSource.isPlaying && !bikeCrashed)
                {
                    bikeCrashed = true;
                    audioSource.PlayOneShot(screamClip);
                    bike.CrashBike(7f);
                }
            }

        }

        if(other.gameObject.CompareTag("Hazard") || other.gameObject.CompareTag("Consumable"))
        {
            Destroy(other.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Move the collider right as the commuter walks
        if (crossingCollider && eventStarted && !destinationReached) {
            crossingCollider.center += new Vector3(
                0.95f,
                0,
                0
             ) * Time.deltaTime;
        }
        //Debug.Log($"Bike: {bike.transform.position} | This: {transform.position}");
        if (Vector3.Distance(bike.transform.position, transform.position) < 80f && !eventStarted)
        {
            MoveCommuterToRightPoint();
            crossingCollider.enabled = true;
            eventStarted = true;
        }

        if (crossingCollider && Mathf.Abs(commuter.transform.position.x - rightPoint.position.x) < 2f) {
            crossingCollider.enabled = false;
            destinationReached = true;
        }
        
    }
}
