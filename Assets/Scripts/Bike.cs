using Unity.XR.CoreUtils;
using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine;

public class Bike : MonoBehaviour
{
    [SerializeField] private GameObject BikeBody;
    [SerializeField] private Transform leftHandlePoint;
    [SerializeField] private Transform rightHandlePoint;

    [SerializeField] private Transform HeadTransform;
    [SerializeField] private float TiltSensitivity;

    [SerializeField] private Player player;
    [SerializeField] private EndlessRunnerEmitter Emitter;
    [SerializeField] private TurnSignals turnSignals;
    [SerializeField] private GameObject StopSign;
    [SerializeField] private Transform XROrigin;
    public bool isStopping = true;

    public float xSpeed = 1.5f;
    public float ySpeed = 10f;
    public float handlePromixity = 0.1f;

    private float _neutralLean;
    private float _currentLean;

    private float _bikeTilt;
    private int currentLanePosition = 0;

    private Vector3 _calibratedRight;
    private Vector3 _calibratedForward;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       //currentLanePosition = 1;
       Invoke(nameof(SetInitialHeadPosition), 1f); // Delay to ensure XR rig is properly initialized

    }



    void SetInitialHeadPosition()
    {
        Vector3 offset =
            HeadTransform.position -
            XROrigin.position;

        _neutralLean =
            Vector3.Dot(
                offset,
                XROrigin.right
            );

        _calibratedRight = XROrigin.right;
        _calibratedForward = XROrigin.forward;

        _currentLean = _neutralLean;
    }

    void DetectStop(Transform controller)
    {
        if (Mathf.Abs(controller.position.z - HeadTransform.position.z) > player.armLength - 0.1) {
            isStopping = true;
            StopSign.SetActive(true);
        } else {
            isStopping = false;
            StopSign.SetActive(false);
        }

        //Debug.Log("Head Stop Distance: " + Mathf.Abs(controller.position.z - HeadTransform.position.z));
    }

    void TrackHeadOrientation()
    {
        if (isStopping)
        {
            Emitter.currentMoveSpeed = 0f;
            return;
        }
        if(HeadTransform.rotation.x < 0.4 && Emitter.currentMoveSpeed > 5f)
        {
            //Speeding up and slowing down disabled for now, motion sickness
            Emitter.currentMoveSpeed -= 0.1f;
        }
        else if (HeadTransform.rotation.x > -0.4)
        {
            //Speeding up and slowing down disabled for now, motion sickness
            Emitter.currentMoveSpeed += 0.1f;
        }

        //Debug.Log("Head Rotation X: " + HeadTransform.rotation.x);

        Emitter.currentMoveSpeed = Mathf.Clamp(Emitter.currentMoveSpeed, 5f, 20f);
    }

    void ChangeLane()
    {


        float playerBoundsLeft = 5f; // Left boundary
        float playerBoundsRight = 5f; // Right boundary

        if (turnSignals.signallingLeft || currentLanePosition < -2)
        {
            playerBoundsLeft = 10;
            Debug.Log("Signalling Left");
        }

        if (turnSignals.signallingRight || currentLanePosition > 2)
        {
            Debug.Log("Signalling Right");
            playerBoundsRight = 10;
        }

        //Debug.Log("Player Bounds " + playerBoundsLeft + " " + playerBoundsRight);
        //Debug.Log("Current Lane Position: " + currentLanePosition);
        //Debug.Log("Player Range: " + (currentLanePosition * 10 - playerBoundsLeft) + " to " + (currentLanePosition * 10 + playerBoundsRight));

        //Prevent bike from going out of bounds in the lane, only allow movement if player is within lane boundaries
        // Except if the player is signalling a turn, then allow them to move out of bounds to change lanes
        if (player.transform.position.x > currentLanePosition * 10 - playerBoundsLeft && player.transform.position.x < currentLanePosition * 10 + playerBoundsRight)
        {
            //Debug.Log("Player Position X: " + player.transform.position.x);

            if (!isStopping)
            {
                if (_bikeTilt > 0.1f) {
                    player.transform.position += new Vector3(xSpeed, 0, 0) * Time.deltaTime;
                    //Debug.Log("Bike Moving Left: ");
                }

                if (_bikeTilt < -0.1f) {
                    player.transform.position -= new Vector3(xSpeed, 0, 0) * Time.deltaTime;
                    //Debug.Log("Bike Moving Right: ");
                }
            }
            

        }
        else
        {
            // reset player position to middle of current lane if out of bounds
            Debug.Log("Player position " + player.transform.position.x + " out of bounds, resetting position to centre of current lane: " + currentLanePosition);
            player.transform.position = new Vector3(currentLanePosition * 10, player.transform.position.y, player.transform.position.z);

            return;
        }

        // If player exits current lane boundaries, change current lane
        if (player.transform.position.x > currentLanePosition * 10 + 5.1)
        {
            if (currentLanePosition > -2 && currentLanePosition < 2)
            {
                currentLanePosition += 1;
                Debug.Log("Changed lane to: " + currentLanePosition);
            }
        }

        if (player.transform.position.x < currentLanePosition * 10 - 5.1)
        {
            if (currentLanePosition > -2 && currentLanePosition < 2)
            {
                currentLanePosition -= 1;
                Debug.Log("Changed lane to: " + currentLanePosition);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"XR Right   = {XROrigin.right}");
            Debug.Log($"XR Forward = {XROrigin.forward}");

        }

        ChangeLane();
        //limit bike tilt
        Vector3 offset =
        HeadTransform.position -
        XROrigin.position;

        Debug.Log(offset);

        _currentLean =
            Vector3.Dot(offset, _calibratedRight);

        _bikeTilt = Mathf.Clamp(
            _currentLean - _neutralLean,
            -1f,
            1f
        );



        // Tilt the bike based on the head's horizontal movement
        BikeBody.transform.rotation = Quaternion.Euler(0, 0, -_bikeTilt * TiltSensitivity); // Adjust the multiplier for more or less tilt

        DetectStop(turnSignals.LeftController);
        DetectStop(turnSignals.RightController);

        // Halt to a stop
        if (isStopping)
        {

            Emitter.currentMoveSpeed = 0;
        }


        if (
            Vector3.Distance(turnSignals.LeftController.position, leftHandlePoint.position) < handlePromixity &&
            Vector3.Distance(turnSignals.RightController.position, rightHandlePoint.position) < handlePromixity &&
            !isStopping)
        {
            
            if (Emitter.currentMoveSpeed < ySpeed)
                Emitter.currentMoveSpeed += 1.5f * Time.deltaTime;
        }
        else {
            if (Emitter.currentMoveSpeed > 0f)
                Emitter.currentMoveSpeed -= 1.5f * Time.deltaTime;
        }
            //TrackHeadOrientation();

            // disable is stopping if neither controller falls into the stopping threshold
            isStopping = false;


        //Debug.Log(
        //$"Parent Rotation: {HeadTransform.parent.rotation.eulerAngles}"
        //    );
        //Debug.Log(
        //    $"Head Rotation: {HeadTransform.rotation.eulerAngles}"
        //);

        //Debug.Log(
        //    $"Local: {HeadTransform.localPosition} " +
        //    $"World: {HeadTransform.position}"
        //);
    }
}
