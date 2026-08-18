using UnityEngine;

[System.Serializable]
public class VRMap
{
    public Transform vrTarget;
    public Transform ikTarget;

    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;
    public bool trackRotation = true;

    public void Map()
    {
        ikTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        if(trackRotation) ikTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
    }
    public void AmplifyMovement(VRMap head, float assistance)
    {
        Vector3 handPos = vrTarget.position;

        // Offset from the head
        Vector3 headPos = handPos - head.vrTarget.position;

        // Exaggerate
        handPos = head.vrTarget.position + headPos * (1 + assistance);

        // Raise the hand on top of the exaggeration
        handPos.y += 0.6f * assistance;

        // Apply tracking offset
        ikTarget.position = handPos + vrTarget.rotation * trackingPositionOffset;
    }
}



public class IKTargetFollowVRRig : MonoBehaviour
{
    [Range(0, 1)]
    public float turnSmoothness = 0.1f;
    public VRMap head;
    public VRMap leftHand;
    public VRMap rightHand;

    public Vector3 headBodyPositionOffset;
    public float headBodyYawOffset;
    public bool isCalibrated = false;

    void LateUpdate()
    {
        transform.position = head.ikTarget.position + headBodyPositionOffset;
        float targetYaw = head.vrTarget.eulerAngles.y; // + headBodyYawOffset;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, targetYaw, transform.eulerAngles.z), turnSmoothness);

        head.trackRotation = false;
        head.Map();
        leftHand.Map();
        rightHand.Map();

        if (isCalibrated)
        {
            leftHand.AmplifyMovement(head, GameManager.Instance.leftAssistance);
            rightHand.AmplifyMovement(head, GameManager.Instance.rightAssistance);
        }
    }
}
