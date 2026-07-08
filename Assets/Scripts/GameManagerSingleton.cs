using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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


}