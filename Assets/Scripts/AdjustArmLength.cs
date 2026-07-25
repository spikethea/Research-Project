using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AdjustArmLength : MonoBehaviour
{
    public float avatarScaleCorrection = 1.0f;
    public float bikeScaleCorrection = 0.8f;

    public Vector3 transformedPelvicVector;
    public Vector3 transformedHeadVector;

    public GameObject leftController;
    public GameObject rightController;
    [SerializeField] private GameObject leftControllerVisual;
    [SerializeField] private GameObject rightControllerVisual;
    [SerializeField] private XROrigin xROrigin;

    [SerializeField] private Player player;
    [SerializeField] private GameObject bike;
    [SerializeField] private SkinnedMeshRenderer avatarRenderer;
    [SerializeField] private IKTargetFollowVRRig avatarIK;
    [SerializeField] private Transform pelvicBone;
    [SerializeField] private Transform headBone;
    [SerializeField] private Transform bikeSeat;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform head;

    private bool headAdjusted = false;
    private Vector3 originalHeadBodyPositionOffset;
    private float playerArmLength = 0f;
    public float avatarArmLength = 0f;
    private float scaleFactor = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //avatarRenderer.enabled = false;
        //bike.SetActive(false);
    }

    float DetectAvatarArmLength() {
        float leftArmLength = Vector3.Distance(leftHand.position, avatarIK.head.ikTarget.position);
        float rightArmLength = Vector3.Distance(rightHand.position, avatarIK.head.ikTarget.position);

        float armLengthMax = Mathf.Max(leftArmLength, rightArmLength);
        Debug.Log("Avatar Arm Length: " + armLengthMax);
        
        return armLengthMax;
    }

    float DetectPlayerArmLength() {
        float leftArmLength = Vector3.Distance(leftController.transform.position, head.position);
        float rightArmLength = Vector3.Distance(rightController.transform.position, head.position);

        float armLengthMax = Mathf.Max(leftArmLength, rightArmLength);
        Debug.Log("Player Arm Length: " + armLengthMax);

        return armLengthMax;
    }

    public void Calibrate(float delay = 0f) {
        StartCoroutine(CalibrateCoroutine(delay));
    }

    IEnumerator CalibrateCoroutine(float delayTime) {
        if (delayTime > 0f) {
            yield return new WaitForSeconds(delayTime);
        }

        if (leftController != null && rightController != null)
        {
            player.armLength = DetectPlayerArmLength();
            avatarArmLength = DetectAvatarArmLength();
            scaleFactor = (player.armLength / avatarArmLength) ;
            Debug.Log("Scale Factor: " + scaleFactor);

            Vector3 originalHeadPosition = pelvicBone.position;

            // transform scale of character & bike
            avatarIK.transform.localScale = Vector3.one * scaleFactor * avatarScaleCorrection;
            bike.transform.localScale = Vector3.one * scaleFactor * bikeScaleCorrection;
            

            //render avatar & bike visuals
            avatarRenderer.enabled = true;
            bike.SetActive(true);

            //hide controller visuals
            leftControllerVisual.SetActive(false);
            rightControllerVisual.SetActive(false);

            yield return new WaitForSeconds(0.5f); // Wait for a short moment to ensure the avatar is scaled properly
            avatarIK.isCalibrated = true;
        }


    }

    void adjustHeadVector() {
        // adjust head position for local scale

        if (headAdjusted) return;
        Debug.Log("headBodyPositionOffset");
        Debug.Log(avatarIK.headBodyPositionOffset);
        transformedPelvicVector = bikeSeat.position - pelvicBone.position;
        
        xROrigin.transform.position = xROrigin.transform.position + (transformedPelvicVector);
        

        transformedHeadVector = headBone.position - head.position;

        
        avatarIK.headBodyPositionOffset = avatarIK.headBodyPositionOffset - (transformedHeadVector);
        xROrigin.transform.position = xROrigin.transform.position + transformedHeadVector;

        headAdjusted = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(avatarIK.isCalibrated)
            adjustHeadVector();
    }
}
