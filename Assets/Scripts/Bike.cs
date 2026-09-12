using System.Collections;
using UnityEngine;

public class Bike : MonoBehaviour
{
    [SerializeField] private GameObject BikeBody;
    [SerializeField] private Transform leftHandlePoint;
    [SerializeField] private Transform rightHandlePoint;
    [SerializeField] private MeshRenderer leftHandlePointVisual;
    [SerializeField] private MeshRenderer rightHandlePointVisual;

    [SerializeField] private Transform HeadTransform;
    [SerializeField] private float TiltSensitivity;

    [SerializeField] private Player player;
    [SerializeField] private EndlessRunnerEmitter Emitter;
    [SerializeField] private TurnSignals turnSignals;
    [SerializeField] private GameObject StopSign;
    [SerializeField] private Transform XROrigin;

    [SerializeField ] private AudioSource audioSource;
    [SerializeField] private AudioClip carHornClip;
    [SerializeField] private AudioClip positiveLaneChangeClip;

    public bool isCrashing = false;
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

    private bool handlebarLeft = false;
    private bool handlebarRight = false;
    private bool handlebarHasBeenTouched = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       //currentLanePosition = 1;
       Invoke(nameof(SetInitialHeadPosition), 1f); // Delay to ensure XR rig is properly initialized
       StartCoroutine(handlebarVibration());

    }

    IEnumerator handlebarVibration()
    {
        while (true) {
            yield return new WaitForSeconds(0.1f);

            float vibrationVal = isStopping ? 0.5f : 0.2f;
            float vibrationDuration = 0.1f;

            if (isCrashing)
            {
                vibrationVal = 1f;
                vibrationDuration = 0.5f;
            }

            if (handlebarLeft || isCrashing) player.haptics.VibrateLeft((Emitter.currentMoveSpeed/ ySpeed) * vibrationVal, vibrationDuration);
            if (handlebarRight || isCrashing) player.haptics.VibrateRight((Emitter.currentMoveSpeed/ySpeed) * vibrationVal, vibrationDuration);
            
        }
        
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

    bool DetectStop(Transform controller)
    {
        
        
        if (
            controller.position.y > HeadTransform.position.y + 0.05f &&
            controller.position.z < HeadTransform.position.z + player.armLength - 0.13f
            ) {
            return true;
        }

        if (handlebarLeft && player.BrakeL)
            return true;
        

        if (handlebarRight && player.BrakeR)
            return true;

        return false;
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
        if ((player.transform.position.x > currentLanePosition * 10 - playerBoundsLeft && player.transform.position.x < currentLanePosition * 10 + playerBoundsRight) || player.onPavement || GameManager.Instance.reinforcementMode == Mode.Positive)
        {
            //Debug.Log("Player Position X: " + player.transform.position.x);

            if (!isStopping && Emitter.currentMoveSpeed > 2f)
            {
                if (_bikeTilt > 0.1f && player.transform.position.x < 24)
                {
                    player.transform.position += new Vector3(xSpeed * _bikeTilt, 0, 0) * Time.deltaTime;
                    //Debug.Log("Bike Moving Left: ");
                }

                if (_bikeTilt < -0.1f && player.transform.position.x > -24)
                {
                    player.transform.position += new Vector3(xSpeed * _bikeTilt, 0, 0) * Time.deltaTime;
                    //Debug.Log("Bike Moving Right: ");
                }
            }


        }
        else
        
            if (GameManager.Instance.reinforcementMode == Mode.Negative
                || GameManager.Instance.reinforcementMode == Mode.Mixed)
            {
                // reset player position to middle of current lane if out of bounds
                Debug.Log("Player position " + player.transform.position.x + " out of bounds, resetting position to centre of current lane: " + currentLanePosition);
                player.transform.position = new Vector3(currentLanePosition * 10, player.transform.position.y, player.transform.position.z);
                audioSource.PlayOneShot(carHornClip);
                player.haptics.VibrateLeft(1f, 1f);
                player.haptics.VibrateRight(1f, 1f);
                return;
            }


        // If player exits current lane boundaries, change current lane
        if (player.transform.position.x > currentLanePosition * 10 + 5.1)
        {
            if (currentLanePosition >= -2 && currentLanePosition < 2)
            {
                currentLanePosition += 1;
                Debug.Log("Changed lane to: " + currentLanePosition);

                if (GameManager.Instance.reinforcementMode == Mode.Positive && !player.onPavement && turnSignals.signallingRight ||
                        GameManager.Instance.reinforcementMode == Mode.Mixed && !player.onPavement && turnSignals.signallingRight)
                {
                    audioSource.PlayOneShot(positiveLaneChangeClip);
                }
            }
        }

        if (player.transform.position.x < currentLanePosition * 10 - 5.1)
        {
            if (currentLanePosition > -2 && currentLanePosition <= 2 )
            {
                currentLanePosition -= 1;
                Debug.Log("Changed lane to: " + currentLanePosition);

                if (GameManager.Instance.reinforcementMode == Mode.Positive && !player.onPavement && turnSignals.signallingLeft||
                        GameManager.Instance.reinforcementMode == Mode.Mixed && !player.onPavement && turnSignals.signallingLeft)
                {
                    audioSource.PlayOneShot(positiveLaneChangeClip);
                }
            }
        
        }

    }

    bool HandlebarTouch(Transform controller, Transform handlebarPoint) {
        if (
                Vector3.Distance(controller.position, handlebarPoint.position) < handlePromixity)
        {
            if (controller.tag == "LeftController")
            {
                handlebarLeft = true;
                leftHandlePointVisual.enabled = false;
            }

            if (controller.tag == "RightController")
            {
                handlebarRight = true;
                rightHandlePointVisual.enabled = false;
            }

            return true;
        }
        else {

            if (controller.tag == "LeftController")
            {
                handlebarLeft = false;
                leftHandlePointVisual.enabled = true;
            }

            if (controller.tag == "RightController")
            {
                handlebarRight = false;
                rightHandlePointVisual.enabled = true;
            }

            return false;
        }
        
    }

    public void CrashBike(float duration) {
        StartCoroutine(Crash(duration));
    }

    IEnumerator Crash(float duration) {
        float timer = 0f;

        while (timer < duration)
        {
           

            isCrashing = true;

            timer += Time.deltaTime;

            yield return null;
        }

        Debug.Log("Crash reached END");

        isCrashing = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Time.frameCount % 60 == 0)
        //{
        //    Debug.Log($"XR Right   = {XROrigin.right}");
        //    Debug.Log($"XR Forward = {XROrigin.forward}");

        //}

        ChangeLane();
        //limit bike tilt
        Vector3 offset =
        HeadTransform.position -
        XROrigin.position;

        //Debug.Log(offset);

        _currentLean =
            Vector3.Dot(offset, _calibratedRight);

        _bikeTilt = Mathf.Clamp(
            _currentLean - _neutralLean,
            -1f,
            1f
        );

        // Mirrored mode
        if(player.Mirrored)
        {
            _bikeTilt = -_bikeTilt;
        }



        // Tilt the bike based on the head's horizontal movement
        BikeBody.transform.rotation = Quaternion.Euler(0, 0, -_bikeTilt * TiltSensitivity); // Adjust the multiplier for more or less tilt

        bool leftStop = DetectStop(turnSignals.LeftController);
        bool rightStop = DetectStop(turnSignals.RightController);

        if (leftStop || rightStop)
        {
            isStopping = true;
        }
        else
        {
            isStopping = false;
        }

        // Halt to a stop
        if (isStopping || isCrashing)
        {

            if (Emitter.currentMoveSpeed > 0)
                Emitter.currentMoveSpeed -= 12f * Time.deltaTime;
        }

        // Show stop sign if purposefully stopping, hide if not
        if (isStopping) {
            StopSign.SetActive(true);
        }
        else {
            StopSign.SetActive(false);
        }


        bool leftTouch = HandlebarTouch(
            turnSignals.LeftController,
            leftHandlePoint
        );

        bool rightTouch = HandlebarTouch(
            turnSignals.RightController,
            rightHandlePoint
        );

        if (!Emitter.gameStarted)
        {
            leftHandlePointVisual.enabled = false;
            rightHandlePointVisual.enabled = false;
        }

        if (
            leftTouch || rightTouch ||
            (Emitter.experimentMode && handlebarHasBeenTouched) // In experiment mode, the player will keep moving unless stopped for timing purposes 
            )
        {
            
            handlebarHasBeenTouched = true;

            if(!isStopping)
            if (Emitter.currentMoveSpeed < ySpeed && Emitter.gameStarted)
                Emitter.currentMoveSpeed += 3f * Time.deltaTime;
        }
        else
        {
            if (!isStopping)
                if (Emitter.currentMoveSpeed > 0f)
                Emitter.currentMoveSpeed -= 0.5f * Time.deltaTime;
        }
        //TrackHeadOrientation();


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

        if (player.Reset) {
            player.Reset = false;
            SetInitialHeadPosition();
        }
    }
}
