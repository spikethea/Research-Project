using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZebraCrossing : MonoBehaviour
{
    private Bike bike;
    [SerializeField] private Commuter commuter;

    [SerializeField] private Collider crossingCollider;

    [SerializeField] private Renderer zebraLightLeft;
    [SerializeField] private Renderer zebraLightRight;

    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;

    private Material sharedTargetMaterial;
    private Color creamBaseColor = new Color(1.0f, 0.99f, 0.82f);
    [SerializeField] private float emissionIntensity = 2.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {;
        bike = FindAnyObjectByType<Bike>();
        // 1. Get the material from the left renderer instance
        sharedTargetMaterial = zebraLightLeft.material;

        // 2. Assign the exact same instance to the right renderer so they share it
        zebraLightRight.material = sharedTargetMaterial;

        // 3. Pre-calculate and apply the HDR cream color once
        Color hdrCream = creamBaseColor * Mathf.Pow(2, emissionIntensity);
        sharedTargetMaterial.SetColor("_EmissionColor", hdrCream);

        StartCoroutine(FlashingLight());
    }


    IEnumerator FlashingLight()
    {
        while (true)
        {
            // Both lights turn ON together
            sharedTargetMaterial.EnableKeyword("_EMISSION");
            yield return new WaitForSeconds(3f);

            // Both lights turn OFF together
            sharedTargetMaterial.DisableKeyword("_EMISSION");
            yield return new WaitForSeconds(3f);
        }
    }
    
    public void MoveCommuterToLeftPoint()
    {
        commuter.MoveToPoint(leftPoint.position);
    }

    public void MoveCommuterToRightPoint()
    {
        commuter.MoveToPoint(rightPoint.position);
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(bike.transform.position, transform.position) < 5f)
        {
            MoveCommuterToLeftPoint();
            crossingCollider.enabled = true;

        }

        if (bike.isStopping && Vector3.Distance(commuter.transform.position, leftPoint.position) < 1f) {
            crossingCollider.enabled = false;
        }
        
    }
}
