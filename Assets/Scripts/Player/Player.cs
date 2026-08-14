using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] InputSystem_Actions input;
    [SerializeField] EndlessRunnerEmitter emitter;

    public float armLength;
    public HapticsManager haptics;

    public bool onPavement = false; 
    
    public bool Mirrored = false;
    public bool BrakeL = false;
    public bool BrakeR = false;
    public bool Reset = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        input = new InputSystem_Actions();
    }
    void OnEnable()
    {
        input.Player.Enable();

        input.Player.Mirror.performed += OnMirrorPressed;
        input.Player.Reset.performed += OnResetPressed;

    }

    private void OnDisable()
    {
        input.Player.Mirror.performed -= OnMirrorPressed;
    }

    private void OnMirrorPressed(InputAction.CallbackContext context)
    {
        Mirrored = !Mirrored;
    }

    private void OnResetPressed(InputAction.CallbackContext context)
    {
        Reset = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Pavement"))
        {
            onPavement = true;
            emitter.currentMoveSpeed = 8f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pavement"))
        {
            onPavement = false;
        }
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
