using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhysicalButton : MonoBehaviour
{
    [SerializeField] HapticsManager haptics;
    [SerializeField] GameObject buttonVisual;
    public Transform[] pressers;
    public UnityEvent onPressed;
    public float pressDuration = 0.5f;

    private List<bool> isPressed = new List<bool>();
    private float pressedTimer;

    private void Start()
    {
        foreach (Transform presser in pressers)
        {
            isPressed.Add(false);
        }
    }

    private void Update()
    {
        bool isPressing = false;

        // 1. Check all hands to see if at least one is pressing
        for (int i = 0; i < pressers.Length; i++)
        {
            var presser = pressers[i];
            if (presser == null) continue;

            var distance = Vector3.Distance(presser.transform.position, this.transform.position);

            if (distance < 0.1f)
            {
                if (!isPressed[i])
                {
                    if(presser.CompareTag("LeftController")) haptics.VibrateLeft(0.5f, 0.1f);
                    if (presser.CompareTag("RightController")) haptics.VibrateRight(0.5f, 0.1f);
                }
                isPressed[i] = true;
                isPressing = true;
            }
            else
            {
                isPressed[i] = false;
            }
        }


        if (isPressing)
        {
            pressedTimer += Time.deltaTime;

            if (pressedTimer > pressDuration)
            {
                onPressed.Invoke();
                pressedTimer = 0f;
            }

            buttonVisual.transform.localPosition = new Vector3(0, 0.0f, 0);
        }
        else
        {
            pressedTimer = 0f;
            buttonVisual.transform.localPosition = new Vector3(0, 0.02f, 0);
        }
    }
}
