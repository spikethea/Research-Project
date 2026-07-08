using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CalibrateArmUI : MonoBehaviour
{
    [SerializeField] private EndlessRunnerEmitter emitter;
    [SerializeField] private AdjustArmLength adjustArmLength;
    [SerializeField] private Canvas HandCanvas;
    [SerializeField] private Canvas SignalsCanvas;
    [SerializeField] private Transform cameraOffset;



    private Vector3 initialPosition;

    public float minimumArmMovement = 0.3f;
    public bool initialPositionSet = false;
    public bool canvasMoved = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke(nameof(SetInitialPosition), 1f); // Delay to ensure XR rig is properly initialized
        SignalsCanvas.enabled = false;
    }

    void SetInitialPosition ()
    {
        initialPositionSet = true;
        transform.position = cameraOffset.position + new Vector3(0f, -0.4f, 0.2f); // Position the UI in front of the players hands
        initialPosition = transform.position;

        HandCanvas.enabled = true;
    }

    IEnumerator HideHandImageAndStart(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        HandCanvas.enabled = false;
        SignalsCanvas.enabled = true;
        emitter.StartGame();
    }


    // Update is called once per frame
    void Update()
    {
        if (!initialPositionSet) return;

        GameObject leftController = adjustArmLength.leftController;
        GameObject rightController = adjustArmLength.rightController;

        if (leftController != null && rightController != null)
        {

            
            

            // Position the UI in front of the players armspan limit
            float handPositionZ = (leftController.transform.position.z + rightController.transform.position.z) / 2;
            if(handPositionZ > transform.position.z + 0.2f) {
                transform.position += new Vector3(0, 0, 0.01f);
            }

            if (!canvasMoved) {
                float distanceMoved = Mathf.Abs(initialPosition.z - transform.position.z);
                Debug.Log("Distance moved: " + distanceMoved + " - Calibrating arm length.");
                if (distanceMoved > minimumArmMovement) {
                    adjustArmLength.Calibrate(2f);
                    StartCoroutine(HideHandImageAndStart(5f));
                    
                    canvasMoved = true;
                }
            }
            

        }
        else
        {
            HandCanvas.enabled = false;
        }
    }
}
