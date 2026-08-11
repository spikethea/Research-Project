using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour
{
    public Transform[] pressers;
    public UnityEvent onPressed;
    public float pressDuration = 1f;
    //[SerializeField] Train train;

    private bool pressed;
    private float pressedTimer;
    

    private void Update()
    {
        //if (!train.atPlatform) return;

        foreach (var presser in pressers) {
            var distance = Vector3.Distance(presser.transform.position, this.transform.position);

            if (distance < 0.2 && !pressed)
            {
                pressed = true;
                pressedTimer += Time.deltaTime;
                
            }

            else
            {
                pressed = false;
                pressedTimer = 0;
            }

            if (pressedTimer > pressDuration) {
                onPressed.Invoke();
            }
        }
    }

}
