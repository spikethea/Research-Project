using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class LegCyclingAnimationIK : MonoBehaviour
{
    [SerializeField] Transform LeftLegTarget;
    [SerializeField] Transform RightLegTarget;

    public float frequency = 2f;
    public float radius = 0.1f;

    public float yOffset = 0.16f;
    public float zOffset = -0.18f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void CircularMovement(Transform legTarget, float delay)
    {
        float y = Mathf.Cos(Time.time * frequency + delay) * radius;
        float z = Mathf.Sin(Time.time * frequency + delay) * radius;
        float x = transform.position.x;// preserving x-axis 

        legTarget.transform.position = new Vector3(x, y + yOffset, z + zOffset);
    }

    // Update is called once per frame
    void Update()
    {
        
        CircularMovement(RightLegTarget, 0f);
        CircularMovement(LeftLegTarget, Mathf.PI);
    }
}
