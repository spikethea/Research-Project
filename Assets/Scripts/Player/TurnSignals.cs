using UnityEngine;

public class TurnSignals : MonoBehaviour
{
    [SerializeField] private Transform cameraOffset;

    public Transform LeftController;
    public Transform RightController;

    [SerializeField] GameObject LeftArrow;
    [SerializeField] GameObject RightArrow;


    public float signalThreshhold = 0.5f;

    public float handDistanceStop = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        // Turn Signals, only one can be activated at a time
        if (
            RightArrow.activeSelf == false &&
            LeftController.position.x - cameraOffset.position.x < -signalThreshhold
            )
        {
            LeftArrow.SetActive(true);
        }
        else {
            LeftArrow.SetActive(false);
        }

        if (
            LeftArrow.activeSelf == false &&
            RightController.position.x - cameraOffset.position.x > signalThreshhold
            )
        {
            RightArrow.SetActive(true);
        }
        else
        {
            RightArrow.SetActive(false);
        }
    }

    
}
