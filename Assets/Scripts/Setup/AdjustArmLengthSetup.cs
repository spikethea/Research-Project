using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AdjustArmLengthSetup : MonoBehaviour
{
    public float avatarScaleCorrection = 1.0f;

    [SerializeField] GameManager gameManager;
    public GameObject leftController;
    public GameObject rightController;


    [SerializeField] private SkinnedMeshRenderer avatarRenderer;
    [SerializeField] private IKTargetFollowVRRig avatarIK;
    [SerializeField] private Transform head;

    private Vector3 originalHeadBodyPositionOffset;
    private float playerArmLength = 0f;
    private float avatarArmLength = 0f;
    private float scaleFactor = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        avatarRenderer.enabled = false;
    }

    //float DetectAvatarArmLength() {
    //    float leftArmLength = Vector3.Distance(avatarIK.leftHand.ikTarget.position, avatarIK.head.ikTarget.position);
    //    float rightArmLength = Vector3.Distance(avatarIK.rightHand.ikTarget.position, avatarIK.head.ikTarget.position);
    //    avatarArmLength = Mathf.Max(leftArmLength, rightArmLength);
    //    Debug.Log("Avatar Arm Length: " + avatarArmLength);
    //    return avatarArmLength;
    //}

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
            playerArmLength =  DetectPlayerArmLength();
            Debug.Log("Scale Factor: " + scaleFactor);

            gameManager.SetPlayerArmLength(playerArmLength);

        }

        
    }

    // Update is called once per frame
    void Update()
    {
    }
}
