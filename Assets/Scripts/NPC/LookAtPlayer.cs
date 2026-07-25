using UnityEngine;

[RequireComponent(typeof(Animator))]
public class LookAtPlayer : MonoBehaviour
{
    public Transform target;
    public Transform neckBone;
    [Range(0, 1)] public float lookWeight = 1f;
    [Range(0, 1)] public float bodyWeight = 0.2f;
    [Range(0, 1)] public float headWeight = 1f;
    [Range(0, 1)] public float eyesWeight = 1f;

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        animator.enabled = false;
    }

    private void OnDisable()
    {
        animator.enabled = true;
    }

    void LateUpdate()
    {
        Vector3 direction = target.position - neckBone.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        neckBone.rotation = Quaternion.Slerp(neckBone.rotation, lookRotation, Time.deltaTime * 5f);
    }
}