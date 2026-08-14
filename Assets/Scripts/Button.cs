using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour
{
    [SerializeField] HapticsManager haptics;
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
        }
        else
        {
            pressedTimer = 0f;
        }
    }
}
