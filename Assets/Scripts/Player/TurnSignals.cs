using Unity.XR.CoreUtils;
using UnityEngine;

public class TurnSignals : MonoBehaviour
{
    [SerializeField] private Transform cameraOffset;

    public Transform LeftController;
    public Transform RightController;

    [SerializeField] GameObject LeftArrow;
    [SerializeField] GameObject RightArrow;

    public bool signallingLeft = false;
    public bool signallingRight = false;

    public float signalThreshhold = 0.5f;

    public float handDistanceStop = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log($"Camera Offset Rot: {cameraOffset.transform.eulerAngles}");

        // Turn Signals, only one can be activated at a time
        if (
            RightArrow.activeSelf == false &&
            LeftController.position.x - cameraOffset.position.x < -signalThreshhold
            )
        {
            LeftArrow.SetActive(true);
            signallingLeft = true;
        }
        else {
            LeftArrow.SetActive(false);
            signallingLeft = false;
        }

        if (
            LeftArrow.activeSelf == false &&
            RightController.position.x - cameraOffset.position.x > signalThreshhold
            )
        {
            RightArrow.SetActive(true);
            signallingRight = true;
        }
        else
        {
            RightArrow.SetActive(false);
            signallingRight = false;
        }
    }

    
}
