using UnityEngine;

public class AbilityScale : MonoBehaviour
{
    [SerializeField] Transform TopPoint;
    [SerializeField] Transform BottomPoint;

    [SerializeField] Transform measuringBall;


    private float returnValue = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float clampedY = Mathf.Clamp(measuringBall.position.y, BottomPoint.position.y, TopPoint.position.y);

        Debug.Log(returnValue);

        if (measuringBall.position.y > TopPoint.position.y || measuringBall.position.y < BottomPoint.position.y)
        {
            measuringBall.transform.position = new Vector3(measuringBall.position.x, clampedY, measuringBall.position.z);
        }
        else {
            if (transform.tag == "LeftController") {
                GameManager.Instance.leftAssistance = 1 - GetMobilityValue();
            }

            if (transform.tag == "RightController")
            {
                GameManager.Instance.rightAssistance = 1 - GetMobilityValue();
            }
        }

        
    }

    public float GetMobilityValue()
    {
        return (measuringBall.transform.position.y - BottomPoint.position.y) / (TopPoint.position.y - BottomPoint.position.y);
    }
}
