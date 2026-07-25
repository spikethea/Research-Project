using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] InputSystem_Actions input;

    public float armLength;
    public HapticsManager haptics;
    public bool Mirrored = false;
    public bool BrakeL = false;
    public bool BrakeR = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        input = new InputSystem_Actions();
    }
    void OnEnable()
    {
        input.Player.Enable();

        input.Player.Mirror.performed += OnMirrorPressed;

    }

    private void OnDisable()
    {
        input.Player.Mirror.performed -= OnMirrorPressed;
    }

    private void OnMirrorPressed(InputAction.CallbackContext context)
    {
        Mirrored = !Mirrored;
    }



    // Update is called once per frame
    void Update()
    {
        if (input.Player.BrakeL.ReadValue<float>() > 0.2)
        {
            BrakeL = true;
        }
        else
        {
            BrakeL = false;
        }

        if (input.Player.BrakeR.ReadValue<float>() > 0.2)
        {
            BrakeR = true;
        }
        else
        {
            BrakeR = false;
        }
    }
}
