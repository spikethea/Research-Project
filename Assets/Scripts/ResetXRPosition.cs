using Unity.XR.CoreUtils;
using UnityEngine;

public class ResetXRPosition : MonoBehaviour
{
    [SerializeField] private XROrigin xrOrigin;

    void Start()
    {
        Invoke(nameof(Recenter), 0.2f);
    }

    void Recenter()
    {
        xrOrigin.MoveCameraToWorldLocation(Vector3.zero);

        xrOrigin.MatchOriginUpCameraForward(
            Vector3.up,
            Vector3.forward);
    }
}