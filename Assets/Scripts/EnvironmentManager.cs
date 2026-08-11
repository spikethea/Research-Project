using System.Collections;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public float minFogDensity;
    public float maxFogDensity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Default environment settings
        //RenderSettings.skybox.SetColor("_Tint", DefaultSkyColor);

        //RenderSettings.ambientIntensity = defaultAmbientIntensity;
        RenderSettings.fogDensity = 0.1f;

        //sunLight.intensity = 2f;
    }

    public void ClearFog()  {
       StartCoroutine(decreaseFogDensity());
    }

    IEnumerator decreaseFogDensity() {
        float duration = 5f; // Duration of the transition in seconds
        float elapsedTime = 0f;
        float startFogDensity = RenderSettings.fogDensity;
        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            RenderSettings.fogDensity = Mathf.Lerp(startFogDensity, minFogDensity, t);
            yield return null;
        }
        RenderSettings.fogDensity = minFogDensity; // Ensure it reaches the target value
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
