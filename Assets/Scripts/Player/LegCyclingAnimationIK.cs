using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class LegCyclingAnimationIK : MonoBehaviour
{
    [SerializeField] Transform pedalPoint;
    [SerializeField] Transform LeftLegTarget;
    [SerializeField] Transform RightLegTarget;
    [SerializeField] EndlessRunnerEmitter emitter;

    public float frequency = 2f;
    public float radius = 0.1f;

    public float yOffset = 0.16f;
    public float zOffset = -0.18f;

    private float pedalAngle = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void CircularMovement(Transform legTarget, float angle, float xOffset)
    {
        Vector3 offset = new Vector3(
            xOffset,
            Mathf.Cos(angle) * radius + yOffset,
            Mathf.Sin(angle) * radius + zOffset
        );

        legTarget.position = pedalPoint.position + offset;
    }

    // Update is called once per frame
    void Update()
    {
        pedalAngle += emitter.currentMoveSpeed * frequency * Time.deltaTime;

        CircularMovement(RightLegTarget, pedalAngle, 0f);
        CircularMovement(LeftLegTarget, pedalAngle + Mathf.PI, -0.2f);
    }
}
