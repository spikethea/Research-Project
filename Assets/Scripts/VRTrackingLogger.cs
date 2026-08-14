using System;
using System.IO;
using System.Text;
using UnityEngine;

public class VRTrackingLogger : MonoBehaviour
{
    [Header("Participant")]
    [SerializeField] private string participantID = "Participant_001";

    [Header("Tracking")]
    [SerializeField] private Transform head;
    [SerializeField] private Transform leftController;
    [SerializeField] private Transform rightController;

    [Header("Current Experiment")]
    [SerializeField] private int trialNumber = 1;

    // These can change while the experiment is running
    private string condition = "None";
    private string phase = "Experiment";

    private string filePath;
    private StringBuilder csvBuffer = new StringBuilder();

    private float trialTime;
    private float lastSaveTime;

    private Vector3 previousHeadPosition;
    private Vector3 previousLeftPosition;
    private Vector3 previousRightPosition;

    private Vector3 previousHeadVelocity;
    private Vector3 previousLeftVelocity;
    private Vector3 previousRightVelocity;

    private bool hasPreviousFrame = false;
    private bool logging = false;

    // Save to disk every second rather than writing every frame
    private const float saveInterval = 1f;


    // =========================================================
    // START EXPERIMENT / TRIAL
    // =========================================================

    public void StartTrial(int trial)
    {
        // Stop previous trial if one is still running
        if (logging)
            EndTrial();

        trialNumber = trial;

        trialTime = 0f;
        lastSaveTime = 0f;

        hasPreviousFrame = false;

        previousHeadPosition = Vector3.zero;
        previousLeftPosition = Vector3.zero;
        previousRightPosition = Vector3.zero;

        previousHeadVelocity = Vector3.zero;
        previousLeftVelocity = Vector3.zero;
        previousRightVelocity = Vector3.zero;

        condition = "None";
        phase = "Experiment";

        csvBuffer.Clear();

        CreateTrialCSV();

        logging = true;

        Debug.Log(
            $"Started Trial {trialNumber}"
        );
    }


    // =========================================================
    // END EXPERIMENT / TRIAL
    // =========================================================

    public void EndTrial()
    {
        if (!logging)
            return;

        SaveBuffer();

        logging = false;

        Debug.Log(
            $"Ended Trial {trialNumber}"
        );
    }


    // =========================================================
    // CHANGE CONDITION
    // =========================================================

    public void SetCondition(string newCondition)
    {
        condition = newCondition;

        Debug.Log(
            $"Condition changed to: {condition}"
        );
    }


    // =========================================================
    // CHANGE PHASE
    // =========================================================

    public void SetPhase(string newPhase)
    {
        phase = newPhase;

        Debug.Log(
            $"Phase changed to: {phase}"
        );
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (!logging)
            return;

        float deltaTime = Time.deltaTime;

        if (deltaTime <= 0f)
            return;

        trialTime += deltaTime;


        // -----------------------------------------------------
        // CURRENT POSITIONS
        // -----------------------------------------------------

        Vector3 headPosition = head.position;
        Vector3 leftPosition = leftController.position;
        Vector3 rightPosition = rightController.position;


        // -----------------------------------------------------
        // CURRENT ROTATIONS
        // -----------------------------------------------------

        Vector3 headRotation = head.eulerAngles;
        Vector3 leftRotation = leftController.eulerAngles;
        Vector3 rightRotation = rightController.eulerAngles;


        // -----------------------------------------------------
        // VELOCITY
        // -----------------------------------------------------

        Vector3 headVelocity = Vector3.zero;
        Vector3 leftVelocity = Vector3.zero;
        Vector3 rightVelocity = Vector3.zero;

        if (hasPreviousFrame)
        {
            headVelocity =
                (headPosition - previousHeadPosition)
                / deltaTime;

            leftVelocity =
                (leftPosition - previousLeftPosition)
                / deltaTime;

            rightVelocity =
                (rightPosition - previousRightPosition)
                / deltaTime;
        }


        // -----------------------------------------------------
        // ACCELERATION
        // -----------------------------------------------------

        Vector3 headAcceleration = Vector3.zero;
        Vector3 leftAcceleration = Vector3.zero;
        Vector3 rightAcceleration = Vector3.zero;

        if (hasPreviousFrame)
        {
            headAcceleration =
                (headVelocity - previousHeadVelocity)
                / deltaTime;

            leftAcceleration =
                (leftVelocity - previousLeftVelocity)
                / deltaTime;

            rightAcceleration =
                (rightVelocity - previousRightVelocity)
                / deltaTime;
        }


        // -----------------------------------------------------
        // SPEED
        // -----------------------------------------------------

        float headSpeed =
            headVelocity.magnitude;

        float leftSpeed =
            leftVelocity.magnitude;

        float rightSpeed =
            rightVelocity.magnitude;


        // -----------------------------------------------------
        // ACCELERATION MAGNITUDE
        // -----------------------------------------------------

        float headAccelerationMagnitude =
            headAcceleration.magnitude;

        float leftAccelerationMagnitude =
            leftAcceleration.magnitude;

        float rightAccelerationMagnitude =
            rightAcceleration.magnitude;


        // -----------------------------------------------------
        // TIMESTAMP
        // -----------------------------------------------------

        string timestamp =
            DateTime.Now.ToString(
                "yyyy-MM-dd HH:mm:ss.fff"
            );


        // -----------------------------------------------------
        // CSV ROW
        // -----------------------------------------------------

        csvBuffer.AppendLine(
            $"{participantID}," +
            $"{trialNumber}," +
            $"{condition}," +
            $"{phase}," +

            $"{Time.frameCount}," +
            $"{timestamp}," +
            $"{trialTime:F4}," +

            // -------------------------
            // HEAD POSITION
            // -------------------------

            $"{headPosition.x:F4}," +
            $"{headPosition.y:F4}," +
            $"{headPosition.z:F4}," +

            // -------------------------
            // HEAD ROTATION
            // -------------------------

            $"{headRotation.x:F4}," +
            $"{headRotation.y:F4}," +
            $"{headRotation.z:F4}," +

            // -------------------------
            // LEFT POSITION
            // -------------------------

            $"{leftPosition.x:F4}," +
            $"{leftPosition.y:F4}," +
            $"{leftPosition.z:F4}," +

            // -------------------------
            // LEFT ROTATION
            // -------------------------

            $"{leftRotation.x:F4}," +
            $"{leftRotation.y:F4}," +
            $"{leftRotation.z:F4}," +

            // -------------------------
            // RIGHT POSITION
            // -------------------------

            $"{rightPosition.x:F4}," +
            $"{rightPosition.y:F4}," +
            $"{rightPosition.z:F4}," +

            // -------------------------
            // RIGHT ROTATION
            // -------------------------

            $"{rightRotation.x:F4}," +
            $"{rightRotation.y:F4}," +
            $"{rightRotation.z:F4}," +

            // -------------------------
            // HEAD MOVEMENT
            // -------------------------

            $"{headSpeed:F4}," +
            $"{headAccelerationMagnitude:F4}," +

            // -------------------------
            // LEFT MOVEMENT
            // -------------------------

            $"{leftSpeed:F4}," +
            $"{leftAccelerationMagnitude:F4}," +

            // -------------------------
            // RIGHT MOVEMENT
            // -------------------------

            $"{rightSpeed:F4}," +
            $"{rightAccelerationMagnitude:F4}"
        );


        // -----------------------------------------------------
        // STORE CURRENT VALUES
        // -----------------------------------------------------

        previousHeadPosition =
            headPosition;

        previousLeftPosition =
            leftPosition;

        previousRightPosition =
            rightPosition;

        previousHeadVelocity =
            headVelocity;

        previousLeftVelocity =
            leftVelocity;

        previousRightVelocity =
            rightVelocity;

        hasPreviousFrame = true;


        // -----------------------------------------------------
        // PERIODIC SAVE
        // -----------------------------------------------------

        if (trialTime - lastSaveTime >= saveInterval)
        {
            SaveBuffer();

            lastSaveTime = trialTime;
        }
    }


    // =========================================================
    // CREATE CSV
    // =========================================================

    private void CreateTrialCSV()
    {
        string participantFolder =
            Path.Combine(
                Application.persistentDataPath,
                "VRExperiment",
                participantID
            );

        Directory.CreateDirectory(
            participantFolder
        );


        string fileName =
            $"Trial_{trialNumber:000}.csv";


        filePath =
            Path.Combine(
                participantFolder,
                fileName
            );


        // -----------------------------------------------------
        // CSV HEADER
        // -----------------------------------------------------

        csvBuffer.AppendLine(
            "ParticipantID," +
            "Trial," +
            "Condition," +
            "Phase," +

            "Frame," +
            "Timestamp," +
            "TrialTime," +

            // Head
            "HeadX," +
            "HeadY," +
            "HeadZ," +
            "HeadRotX," +
            "HeadRotY," +
            "HeadRotZ," +

            // Left controller
            "LeftX," +
            "LeftY," +
            "LeftZ," +
            "LeftRotX," +
            "LeftRotY," +
            "LeftRotZ," +

            // Right controller
            "RightX," +
            "RightY," +
            "RightZ," +
            "RightRotX," +
            "RightRotY," +
            "RightRotZ," +

            // Movement
            "HeadSpeed," +
            "HeadAcceleration," +
            "LeftSpeed," +
            "LeftAcceleration," +
            "RightSpeed," +
            "RightAcceleration"
        );


        File.WriteAllText(
            filePath,
            csvBuffer.ToString()
        );

        csvBuffer.Clear();


        Debug.Log(
            $"CSV created: {filePath}"
        );
    }


    // =========================================================
    // SAVE BUFFER
    // =========================================================

    private void SaveBuffer()
    {
        if (csvBuffer.Length == 0)
            return;

        File.AppendAllText(
            filePath,
            csvBuffer.ToString()
        );

        csvBuffer.Clear();
    }


    // =========================================================
    // SAFETY
    // =========================================================

    private void OnApplicationQuit()
    {
        if (logging)
            EndTrial();
    }

    private void OnDestroy()
    {
        if (logging)
            EndTrial();
    }
}