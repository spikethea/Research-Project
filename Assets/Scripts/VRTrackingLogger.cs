using System;
using System.IO;
using System.Text;
using System.Globalization;
using UnityEngine;

public class VRTrackingLogger : MonoBehaviour
{
    [Header("Tracking")]
    [SerializeField] private Player player;
    [SerializeField] private Transform head;
    [SerializeField] private Transform leftController;
    [SerializeField] private Transform rightController;

    [Header("Current Trial")]
    [SerializeField] private int trialNumber = 1;

    private string condition = "None";

    private string filePath;
    private StringBuilder csvBuffer = new StringBuilder();

    public float trialTime;
    private float lastSaveTime;

    private bool logging = false;

    private const float saveInterval = 1f;


    // =========================================================
    // AUTOMATIC PARTICIPANT NUMBER
    // =========================================================

    private string GetParticipantID()
    {
        return $"Participant_{GameManager.Instance.participantNumber:000}";
    }


    // =========================================================
    // START TRIAL
    // =========================================================

    public void StartTrial()
    {
        if (logging)
            EndTrial();


        // -----------------------------------------------------
        // CHECK GAMEMANAGER
        // -----------------------------------------------------

        if (GameManager.Instance == null)
        {
            Debug.LogError(
                "VRTrackingLogger: GameManager.Instance is null!"
            );

            return;
        }


        // -----------------------------------------------------
        // CHECK TRACKING REFERENCES
        // -----------------------------------------------------

        if (player.transform == null ||
            head == null ||
            leftController == null ||
            rightController == null)
        {
            Debug.LogError(
                "VRTrackingLogger: One or more tracking transforms are missing!"
            );

            return;
        }


        // -----------------------------------------------------
        // CHECK PARTICIPANT NUMBER
        // -----------------------------------------------------

        if (GameManager.Instance.participantNumber <= 0)
        {
            Debug.LogError(
                "VRTrackingLogger: No valid participant number has been set!"
            );

            return;
        }


        // -----------------------------------------------------
        // AUTOMATICALLY FIND NEXT TRIAL
        // -----------------------------------------------------

        trialNumber = GetNextTrialNumber();


        // -----------------------------------------------------
        // RESET TRIAL DATA
        // -----------------------------------------------------

        trialTime = 0f;
        lastSaveTime = 0f;

        condition = "None";

        csvBuffer.Clear();


        // -----------------------------------------------------
        // CREATE CSV
        // -----------------------------------------------------

        CreateTrialCSV();

        logging = true;


        Debug.Log(
            $"Started Trial {trialNumber} " +
            $"for {GetParticipantID()}"
        );
    }


    // =========================================================
    // FIND NEXT TRIAL NUMBER
    // =========================================================

    private int GetNextTrialNumber()
    {
        string participantFolder =
            Path.Combine(
                Application.persistentDataPath,
                "VRExperiment",
                GetParticipantID()
            );

        Directory.CreateDirectory(
            participantFolder
        );


        string[] trialFiles =
            Directory.GetFiles(
                participantFolder,
                "Trial_*.csv"
            );


        int highestTrial = 0;


        foreach (string file in trialFiles)
        {
            string fileName =
                Path.GetFileNameWithoutExtension(file);

            string numberPart =
                fileName.Replace("Trial_", "");


            if (int.TryParse(
                numberPart,
                out int trial))
            {
                if (trial > highestTrial)
                {
                    highestTrial = trial;
                }
            }
        }


        return highestTrial + 1;
    }


    // =========================================================
    // END TRIAL
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
    // CONDITION
    // =========================================================

    public void SetCondition(string newCondition)
    {
        condition = newCondition;

        Debug.Log(
            $"Condition changed to: {condition}"
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
        // PLAYER POSITION
        // -----------------------------------------------------

        Vector3 playerPosition =
            player.transform.position;


        // -----------------------------------------------------
        // LOCAL POSITIONS
        // -----------------------------------------------------

        Vector3 headLocalPosition =
            player.transform.InverseTransformPoint(
                head.position
            );

        Vector3 leftLocalPosition =
            player.transform.InverseTransformPoint(
                leftController.position
            );

        Vector3 rightLocalPosition =
            player.transform.InverseTransformPoint(
                rightController.position
            );


        // -----------------------------------------------------
        // PLAYER ROTATION
        // -----------------------------------------------------

        Quaternion playerRotation =
            player.transform.rotation;


        // -----------------------------------------------------
        // LOCAL ROTATIONS
        // -----------------------------------------------------

        Quaternion headLocalRotation =
            Quaternion.Inverse(player.transform.rotation)
            * head.rotation;

        Quaternion leftLocalRotation =
            Quaternion.Inverse(player.transform.rotation)
            * leftController.rotation;

        Quaternion rightLocalRotation =
            Quaternion.Inverse(player.transform.rotation)
            * rightController.rotation;


        // -----------------------------------------------------
        // TIMESTAMP
        // -----------------------------------------------------

        string timestamp =
            DateTime.Now.ToString(
                "yyyy-MM-dd HH:mm:ss.fff"
            );


        // -----------------------------------------------------
        // CSV
        // -----------------------------------------------------

        csvBuffer.AppendLine(
            $"{GetParticipantID()}," +
            $"{trialNumber}," +
            $"{player.armLength}," +
            $"{(player.onPavement ? "None" : condition )}," + // If the player is still on the pavement dont count as a condition

            $"{Time.frameCount}," +
            $"{timestamp}," +
            $"{trialTime.ToString("F4", CultureInfo.InvariantCulture)}," +

            // Player position
            $"{playerPosition.x.ToString("F4", CultureInfo.InvariantCulture)}," +
            $"{playerPosition.y.ToString("F4", CultureInfo.InvariantCulture)}," +
            $"{playerPosition.z.ToString("F4", CultureInfo.InvariantCulture)}," +

            // Player rotation
            $"{playerRotation.x.ToString("F4", CultureInfo.InvariantCulture)}," +
            $"{playerRotation.y.ToString("F4", CultureInfo.InvariantCulture)}," +
            $"{playerRotation.z.ToString("F4", CultureInfo.InvariantCulture)}," +
            $"{playerRotation.w.ToString("F4", CultureInfo.InvariantCulture)}," +

            // Head local position
            $"{headLocalPosition.x.ToString("F4", CultureInfo.InvariantCulture)}," +
            $"{headLocalPosition.y.ToString("F4", CultureInfo.InvariantCulture)}," +
            $"{headLocalPosition.z.ToString("F4", CultureInfo.InvariantCulture)}," +

            // Head local rotation
            $"{headLocalRotation.x.ToString("F4", CultureInfo.InvariantCulture)}," +
            $"{headLocalRotation.y.ToString("F4", CultureInfo.InvariantCulture)}," +
            $"{headLocalRotation.z.ToString("F4", CultureInfo.InvariantCulture)}," +
            $"{headLocalRotation.w.ToString("F4", CultureInfo.InvariantCulture)}," +

            // Left controller local position
            $"{leftLocalPosition.x.ToString("F4", CultureInfo.InvariantCulture)}," +
            $"{leftLocalPosition.y.ToString("F4", CultureInfo.InvariantCulture)}," +
            $"{leftLocalPosition.z.ToString("F4", CultureInfo.InvariantCulture)}," +

            // Left controller local rotation
            $"{leftLocalRotation.x.ToString("F4", CultureInfo.InvariantCulture)}," +
            $"{leftLocalRotation.y.ToString("F4", CultureInfo.InvariantCulture)}," +
            $"{leftLocalRotation.z.ToString("F4", CultureInfo.InvariantCulture)}," +
            $"{leftLocalRotation.w.ToString("F4", CultureInfo.InvariantCulture)}," +

            // Right controller local position
            $"{rightLocalPosition.x.ToString("F4", CultureInfo.InvariantCulture)}," +
            $"{rightLocalPosition.y.ToString("F4", CultureInfo.InvariantCulture)}," +
            $"{rightLocalPosition.z.ToString("F4", CultureInfo.InvariantCulture)}," +

            // Right controller local rotation
            $"{rightLocalRotation.x.ToString("F4", CultureInfo.InvariantCulture)}," +
            $"{rightLocalRotation.y.ToString("F4", CultureInfo.InvariantCulture)}," +
            $"{rightLocalRotation.z.ToString("F4", CultureInfo.InvariantCulture)}," +
            $"{rightLocalRotation.w.ToString("F4", CultureInfo.InvariantCulture)}"
        );


        // -----------------------------------------------------
        // SAVE
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
                GetParticipantID()
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


        csvBuffer.AppendLine(
            "ParticipantID," +
            "Trial," +
            "ReachLength," +
            "Condition," +

            "Frame," +
            "Timestamp," +
            "TrialTime," +

            "PlayerX," +
            "PlayerY," +
            "PlayerZ," +

            "PlayerRotX," +
            "PlayerRotY," +
            "PlayerRotZ," +
            "PlayerRotW," +

            "HeadLocalX," +
            "HeadLocalY," +
            "HeadLocalZ," +

            "HeadRotX," +
            "HeadRotY," +
            "HeadRotZ," +
            "HeadRotW," +

            "LeftLocalX," +
            "LeftLocalY," +
            "LeftLocalZ," +

            "LeftRotX," +
            "LeftRotY," +
            "LeftRotZ," +
            "LeftRotW," +

            "RightLocalX," +
            "RightLocalY," +
            "RightLocalZ," +

            "RightRotX," +
            "RightRotY," +
            "RightRotZ," +
            "RightRotW"
        );


        File.WriteAllText(
            filePath,
            csvBuffer.ToString()
        );

        csvBuffer.Clear();


        Debug.Log(
            $"CSV created at:\n{filePath}"
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
    // APPLICATION EXIT
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