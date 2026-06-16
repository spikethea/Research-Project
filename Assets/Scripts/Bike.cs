using Unity.XR.CoreUtils;
using UnityEngine;

public class Bike : MonoBehaviour
{
    [SerializeField] private GameObject BikeBody;
    [SerializeField] private Transform HeadTransform;
    [SerializeField] private float TiltSensitivity;

    [SerializeField] private GameObject Player;
    [SerializeField] private EndlessRunnerEmitter Emitter;
    [SerializeField] private TurnSignals turnSignals;
    [SerializeField] private GameObject StopSign;
    [SerializeField] private Transform XROrigin;
    public bool isStopping = true;

    public float xSpeed = 5f;
    public float ySpeed = 5f;

    private float _neutralLean;
    private float _bikeTilt;
    private int currentLanePosition = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       //currentLanePosition = 1;
       Invoke(nameof(SetInitialHeadPosition), 0.2f); // Delay to ensure XR rig is properly initialized

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
    }

    void DetectStop(Transform controller)
    {
        if (Mathf.Abs(controller.position.z - HeadTransform.position.z) > 0.5f) {
            isStopping = true;
        }

        //Debug.Log("Head Stop Distance: " + Mathf.Abs(controller.position.z - HeadTransform.position.z));
    }

    void TrackHeadOrientation()
    {
        if (isStopping)
        {
            Emitter.moveSpeed = 0f;
            return;
        }
        if(HeadTransform.rotation.x < 0.4 && Emitter.moveSpeed > 5f)
        {
            //Speeding up and slowing down disabled for now, motion sickness
            Emitter.moveSpeed -= 0.1f;
        }
        else if (HeadTransform.rotation.x > -0.4)
        {
            //Speeding up and slowing down disabled for now, motion sickness
            Emitter.moveSpeed += 0.1f;
        }

        //Debug.Log("Head Rotation X: " + HeadTransform.rotation.x);

        Emitter.moveSpeed = Mathf.Clamp(Emitter.moveSpeed, 5f, 20f);
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
        if (Player.transform.position.x > currentLanePosition * 10 - playerBoundsLeft && Player.transform.position.x < currentLanePosition * 10 + playerBoundsRight)
        {
            Debug.Log("Player Position X: " + Player.transform.position.x);


            if (HeadTransform.localPosition.x - _neutralLean > 0.1f)
            {
                Player.transform.position += new Vector3(1, 0, 0) * Time.deltaTime;
                //Debug.Log("Bike Moving Left: ");
            }

            if (HeadTransform.localPosition.x - _neutralLean < -0.1f)
            {
                Player.transform.position -= new Vector3(1, 0, 0) * Time.deltaTime;
                //Debug.Log("Bike Moving Right: ");
            }

        }
        else
        {
            // reset player position to middle of current lane if out of bounds
            Debug.Log("Player position " + Player.transform.position.x + " out of bounds, resetting position to centre of current lane: " + currentLanePosition);
            Player.transform.position = new Vector3(currentLanePosition * 10, Player.transform.position.y, Player.transform.position.z);

            return;
        }

        // If player exits current lane boundaries, change current lane
        if (Player.transform.position.x > currentLanePosition * 10 + 5.1)
        {
            if (currentLanePosition > -2 && currentLanePosition < 2)
            {
                currentLanePosition += 1;
                Debug.Log("Changed lane to: " + currentLanePosition);
            }
        }

        if (Player.transform.position.x < currentLanePosition * 10 - 5.1)
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
        ChangeLane();
        //limit bike tilt
        Vector3 offset =
        HeadTransform.position -
        XROrigin.position;

        float lean =
            Vector3.Dot(
                offset,
                XROrigin.right
            );

        _bikeTilt = Mathf.Clamp(
            lean - _neutralLean,
            -1f,
            1f
        );



        // Tilt the bike based on the head's horizontal movement
        BikeBody.transform.rotation = Quaternion.Euler(0, 0, -_bikeTilt * TiltSensitivity); // Adjust the multiplier for more or less tilt

        DetectStop(turnSignals.LeftController);
        DetectStop(turnSignals.RightController);

        if (isStopping)
        {
            StopSign.SetActive(true);
        }
        else
        {
            StopSign.SetActive(false);
        }
        TrackHeadOrientation();

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
