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
    public bool isStopping = true;

    public float xSpeed = 5f;
    public float ySpeed = 5f;

    private Vector3 _initialHeadPosition;
    private float _bikeTilt;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _initialHeadPosition = HeadTransform.localPosition;

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
            Emitter.moveSpeed -= 0.1f;
        }
        else if (HeadTransform.rotation.x > -0.4)
        {
            Emitter.moveSpeed += 0.1f;
        }

        //Debug.Log("Head Rotation X: " + HeadTransform.rotation.x);

        Emitter.moveSpeed = Mathf.Clamp(Emitter.moveSpeed, 5f, 20f);
    }



    // Update is called once per frame
    void Update()
    {
        _bikeTilt = HeadTransform.localPosition.x - _initialHeadPosition.x;

        // Tilt the bike based on the head's horizontal movement
        BikeBody.transform.rotation = Quaternion.Euler(0, 0, -_bikeTilt * TiltSensitivity); // Adjust the multiplier for more or less tilt

        DetectStop(turnSignals.LeftController);
        DetectStop(turnSignals.RightController);

        if (isStopping)
        {
            StopSign.SetActive(true);
        }
        else {
            StopSign.SetActive(false);
        }
        TrackHeadOrientation();

        // disable is stopping if neither controller falls into the stopping threshold
        isStopping = false;

        


        
        if (Player.transform.position.x > -5 && Player.transform.position.x < 5) { //Prevent bike from going out of bounds
            Debug.Log("Player Position X: " + Player.transform.position.x);
            if (BikeBody.transform.rotation.x > -4f && BikeBody.transform.rotation.x < 4f)
            {
                if (BikeBody.transform.rotation.z > 0.01f)
                {
                    Player.transform.position += new Vector3(-xSpeed * Mathf.Abs(HeadTransform.position.x), 0, 0) * Time.deltaTime;
                    Debug.Log("Bike Moving Left: " + BikeBody.transform.rotation.z);
                }

                if (BikeBody.transform.rotation.z < -0.01f)
                {
                    Player.transform.position += new Vector3(xSpeed * Mathf.Abs(HeadTransform.position.x), 0, 0) * Time.deltaTime;
                    Debug.Log("Bike Moving Right: " + BikeBody.transform.rotation.z);
                }
            }
        } else {
            Player.transform.position = new Vector3(0, Player.transform.position.y, Player.transform.position.z); }
            

        


    }
}
