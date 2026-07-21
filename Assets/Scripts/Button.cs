using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour
{
    public Transform[] pressers;
    public UnityEvent onPressed;
    //[SerializeField] Train train;

    private bool pressed;

    private void Update()
    {
        //if (!train.atPlatform) return;

        foreach (var presser in pressers) {
            var distance = Vector3.Distance(presser.transform.position, this.transform.position);

            if (distance < 0.02 && !pressed)
            {
                onPressed.Invoke();
                pressed = true;
            }

            if (distance > 0.02) {
                pressed = false;
            }
        }
    }

}
