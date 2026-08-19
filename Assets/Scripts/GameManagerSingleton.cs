using System.Collections.Generic;
using UnityEngine;

public enum Mode
{
    Positive,
    Negative,
    Mixed,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int participantNumber;

    public float playerArmLength = 1.0f;
    public float leftAssistance = 0f;
    public float rightAssistance = 0f;

    public bool isMirrorMode = false;
    public bool isParticipant = false;

    public Mode reinforcementMode = 0;
    public List<Mode> modePool = new List<Mode>();

    // Game Objectives
    public string currentTarget = null;


    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void SwitchScene (string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void SwitchToNextScene()
    {
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex + 1) % UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneIndex);
    }

    public void SetPlayerArmLength(float armLength)
    {
        playerArmLength = armLength;
    }

    public void SetRandomMode()
    {
        // Refill and shuffle when empty
        if (modePool.Count == 0)
        {
            modePool = new List<Mode>
        {
            Mode.Positive,
            Mode.Negative,
            Mode.Mixed,
            Mode.Positive,
            Mode.Negative,
            Mode.Mixed
        };

            // Fisher-Yates shuffle algorithm
            for (int i = modePool.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);

                Mode temp = modePool[i];
                modePool[i] = modePool[j];
                modePool[j] = temp;
            }
        }

        // Take the first mode
        reinforcementMode = modePool[0];

        // Remove it so it can't be selected again
        modePool.RemoveAt(0);
    }

}