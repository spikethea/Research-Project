using UnityEngine;

public class AdjustArmLength : MonoBehaviour
{
    [SerializeField] private GameObject leftArm;
    [SerializeField] private GameObject rightArm;
    [SerializeField] private float armLength = 1.0f;

    [SerializeField] private Transform handTarget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (handTarget != null)
        {
            Vector3 directionToHand = handTarget.position - transform.position;
            float currentDistance = directionToHand.magnitude;
            if (currentDistance > armLength)
            {
                currentDistance = armLength;

                leftArm.transform.localScale = new Vector3(leftArm.transform.localScale.x, leftArm.transform.localScale.y, currentDistance);
                rightArm.transform.localScale = new Vector3(rightArm.transform.localScale.x, rightArm.transform.localScale.y, currentDistance);
            }
        }
    }
}
