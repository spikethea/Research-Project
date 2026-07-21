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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource.loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (playerHasLooked) {
            if (distanceReached) return;

            
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

            if(LeavingTimer > 4) {
                // WalkTowardsPlayer();
            }

            if (distanceBetween < WalkOffDistance) {
                distanceReached = true;
                
            }

        }
    }

    void WalkTowardsPlayer()
    {
        // Gradually move the NPC towards the player as timeToLeave decreases
        float moveSpeed = 0.5f; // Adjust this value to control how fast the NPC moves away
        Vector3 playerPos = new Vector3(player.position.x, 1.6f, player.position.z); // Adjust this value based on the NPC's height
        Vector3 direction = (transform.position - (playerPos)).normalized; // Move away from the player
        transform.position -= direction * moveSpeed * Time.deltaTime; // Move the NPC

        // Rotate the NPC's neck to look at the player
        Quaternion lookRotation = Quaternion.LookRotation(-direction);
        transform.rotation = Quaternion.Slerp(lookRotation, transform.rotation, Time.deltaTime * 5f);
        animator.enabled = true;
        animator.SetBool("isWalking", true);
        animator.speed = 0.5f;
        lookAtPlayer.enabled = false;
    }

    IEnumerator DisplayArrow (float delay, float duration)
    {
        lookAtPlayer.enabled = false;
        animator.SetBool("isSignalling", true);
        yield return new WaitForSeconds(delay); // Wait for the animation to finish
        SignalArrow.enabled = true;
        yield return new WaitForSeconds(duration); // Wait for the duration
        SignalArrow.enabled = false;
        lookAtPlayer.enabled = false;
        //animator.SetBool("isSignalling", false); // not neccescary
    }

    public void SignalAnimation()
    {
        
    StartCoroutine(DisplayArrow(1f, 2f)); // Example values for delay and duration
       
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
