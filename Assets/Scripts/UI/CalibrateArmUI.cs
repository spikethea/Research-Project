using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CalibrateArmUI : MonoBehaviour
{
    [SerializeField] private AdjustArmLength adjustArmLength;
    [SerializeField] private Image HandCanvas;
    [SerializeField] private Transform cameraOffset;
    private Vector3 initialPosition;
    private bool canvasMoved = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke(nameof(SetInitialPosition), 1f); // Delay to ensure XR rig is properly initialized
    }

    void SetInitialPosition ()
    {
        
        transform.position = cameraOffset.position + new Vector3(0f, 0f, 0.5f); // Position the UI in front of the players hands
        initialPosition = transform.position;
    }

    IEnumerator HideHandImage(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        HandCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject leftController = adjustArmLength.leftController;
        GameObject rightController = adjustArmLength.rightController;

        if (leftController != null && rightController != null)
        {
            if(!canvasMoved) HandCanvas.enabled = true;

            // Position the UI in front of the players armspan limit
            float handPositionZ = (leftController.transform.localPosition.z + rightController.transform.localPosition.z) / 2;
            if(handPositionZ > transform.position.z + 0.2f) {
                transform.position += new Vector3(0, 0, 0.01f);
            }

            if (!canvasMoved) {
                float distanceMoved = Vector3.Distance(initialPosition, transform.position);
                Debug.Log("Distance moved: " + distanceMoved + " - Calibrating arm length.");
                if (distanceMoved > 0.5f) {
                    adjustArmLength.Calibrate(5f);
                    StartCoroutine(HideHandImage(5f));
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
