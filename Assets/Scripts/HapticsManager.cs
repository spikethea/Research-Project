using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

public class HapticsManager : MonoBehaviour
{

    [Header("Controller Haptics")]
    public HapticImpulsePlayer leftHaptics;
    public HapticImpulsePlayer rightHaptics;

    public void VibrateRight(float amplitude, float duration)
    {
        rightHaptics?.SendHapticImpulse(amplitude, duration);
    }

    public void VibrateLeft(float amplitude, float duration)
    {
        leftHaptics?.SendHapticImpulse(amplitude, duration);
    }
}