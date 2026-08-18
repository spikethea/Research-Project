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
        reinforcementMode = (Mode)Random.Range(
            0,
            System.Enum.GetValues(typeof(Mode)).Length
        );
    }

}