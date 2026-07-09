using UnityEngine;

public class BrakeLight : MonoBehaviour
{
    public bool isDisabled = false;
    [SerializeField] MeshRenderer meshRenderer;
    Material material;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float maxIntensity = 5f;
    public Color emissionColor = Color.red;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        material = meshRenderer.material;
        material.EnableKeyword("_EMISSION");
    }

    // Update is called once per frame
    void Update()
    {
        if (isDisabled)
        {
            material.color = Color.black;
            material.SetColor("_EmissionColor", emissionColor * 0);
            isDisabled = false;
        } else {
            material.color = Color.red;
            float intensity = maxIntensity;
            material.SetColor("_EmissionColor", emissionColor * intensity);
        }


           

       
    }

    
}
