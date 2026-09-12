using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoveInterest : Gaze
{
    [SerializeField] Transform player;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Animator animator;

    //Effects

    // NeckMovement to look at player
    [SerializeField] LookAtPlayer lookAtPlayer;

    [SerializeField] Image SignalArrow;
    [SerializeField] Image StopSign;


    public float volume = 0.5f;
    public float timeToLeave = 8;

    // Distance check variables
    public bool playerHasLooked = false;
    public bool distanceReached = false;
    public bool exitedTrain = false;
    public float WalkOffDistance;

    //// Track train movement
    //private Vector3 lastTrainPos;
    //private Transform currentTrain;

    float LeavingTimer = 0f;
    private int idleStateHash;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource.loop = true;
        SignalArrow.enabled = false;
        StopSign.enabled = false;

        idleStateHash = Animator.StringToHash("Idle");
        Invoke("EnableLookAtPlayer", 5f); // Delay to ensure the animator has initialized
    }

    // Disable the animator at first to prevent it from overriding the neck rotation
    private void EnableLookAtPlayer()
    {
        lookAtPlayer.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (!audioSource.isPlaying)
            audioSource.Play();
        LeavingTimer += Time.deltaTime;

        Vector3 headPos = transform.position;
        Vector3 targetDirection = headPos - player.position;

        float distanceBetween = Vector3.Distance(player.transform.position, transform.position);

        //checking if the player is within the NPC's field of view
        float angleToPlayer = Vector3.Angle(targetDirection, player.forward);

        angleToPlayer = Mathf.Clamp(angleToPlayer, 0, 90);

        audioSource.volume = 1 - (angleToPlayer / 90f); // Normalize to 0-1 range

        if (LeavingTimer > 4)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.shortNameHash == idleStateHash)
            {
                lookAtPlayer.enabled = true;
            }
            else {
                lookAtPlayer.enabled = false;
            }
        }


        // Check if the NPC is idle
        
    }

    public void SignalAnim() {
        animator.enabled = true;
        animator.ResetTrigger("doStopping");
        animator.SetTrigger("doSignalling");
    }
    public void StopAnim() {
        animator.enabled = true;
        animator.ResetTrigger("doSignalling");
        animator.SetTrigger("doStopping");

    }

    public void enableStopSign() {
        StopSign.enabled = true;
    }

    public void disableStopSign()
    {
        StopSign.enabled = false;

    }

    public void enableSignalArrow() {
        SignalArrow.enabled = true;
    }

    public void disableSignalArrow() {
        SignalArrow.enabled = false;
    }


    ////Inherit Parent trains movement
    //private void InheritTrainMovement() {
    //    if (currentTrain != null)
    //    {
    //        // Calculate how much train moved this frame
    //        Vector3 trainDelta = currentTrain.position - lastTrainPos;
    //        velocityFromTrain = trainDelta / Time.deltaTime;

    //        lastTrainPos = currentTrain.position;
    //    }

    //    // Apply the train's movement to the player
    //    // Move() handles collisions and slopes properly
    //    controller.Move(velocityFromTrain * Time.deltaTime);
    //}

    protected override void Interact() {
        playerHasLooked = true;
    }
}
