using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    // char controller and speed adjusting
    private CharacterController charController;

    // bot parameters
    [Header("Bot Controls")]
    [SerializeField] private bool isBot = false;

    // character controls, ground layer, jumping
    [Header("Character Controls")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpHeight = 10f;   // if any changes, calculate the time it takes to reach peak and adjust applytime
    private float horizontal = 0f;
    private float rotation = 0f;

    [Header("Gravity Parameters")]
    [SerializeField] private float gravityRun = -40f;
    [SerializeField] private float gravityFly = -1f;
    [SerializeField] private float gravityDecend = -20f;
    [SerializeField] private float gravityFall = -30f;
    [SerializeField] private float gravityApplyTime = 1f;
    private float gravity;
    private float velocityY;
    private bool isOnGround;
    private bool isJumped = false; // toggle to apply jump and fly only once

    // wing parameters, -1 for all wing lengths but -20 for one pair then -30 for falling
    [Header("Wing Parameters")]
    [SerializeField] private GameObject characterWings;
    [SerializeField] private float loseWingsPeriod = 2f;
    private float wingsTimer;
    private int wingCount;
    private bool isWingsOpen = false;
    
    // animation parameters
    [Header("Animation Parameters")]
    [SerializeField] private Animator animator;
    private bool isRunning = false;
    private bool isFlying = false;

    // player score parameters
    [Header("Player Scored Parameters")]
    [SerializeField] private Transform finishMarkerT;
    [SerializeField] private Transform scoreEndMarkerT;
    private bool levelFinished = false;
    private float scoreSections = 40f;

    void Start()
    {
        charController = GetComponent<CharacterController>();
        gravity = gravityRun;
        isRunning = true;
        wingCount = 0;
        wingsTimer = loseWingsPeriod;
    }

    void Update()
    {
        if (levelFinished)
            return;


        if(isBot)
        {
            // assigning horizontal for bots

            // test
            horizontal = 0f;
        }
        else
        {
            // player control, later with touch scroll
            horizontal = Input.GetAxisRaw("Horizontal");
        }


        // assign rotation on proper direction
        rotation = rotationSpeed * horizontal;

        // rotation 
        float rotationY = transform.rotation.eulerAngles.y;
        float angle = rotationY + rotation * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        // check ground and apply gravity if not on ground, check landing triggers
        isOnGround = Physics.CheckSphere(transform.position, .1f, groundLayer, QueryTriggerInteraction.Ignore);

        // every period of time not on ground, lose wings, then adjust gravity
        if(!isOnGround && isWingsOpen)
        {
            wingsTimer -= Time.deltaTime;
            if(wingsTimer <= 0f)
            {
                wingsTimer = loseWingsPeriod;
                // gfx
                if (wingCount >= 4)
                    characterWings.GetComponent<WingController>().LoseWings();
                else if (wingCount >= 2)
                    characterWings.GetComponent<WingController>().LoseMainWings();
                // logic
                wingCount = (wingCount >= 2) ? (wingCount - 2) : wingCount;
                gravity = GravityAtWingCount(wingCount);
            }
        }

        if (isOnGround && velocityY < 0f)
        {
            // setup for running
            velocityY = 0f;
            gravity = gravityRun;
            isRunning = true;
            isFlying = false;
            isJumped = false;
            // close wings
            characterWings.GetComponent<WingController>().CloseWings();
            isWingsOpen = false;
        }
        else
        {
            gravity = (GameController.instance.GetLandingValue(gameObject.name)) ? gravityRun : gravity;
            velocityY += gravity * Time.deltaTime;
        }

        // jump and fly when off the ground
        if(isOnGround == false && isJumped == false)
        {
            isRunning = false;
            isFlying = true;
            isJumped = true;
            velocityY += Mathf.Sqrt(jumpHeight * -2f * gravity);
            StartCoroutine(ApplyGravityAfterDelay());
        }

        // get movingVector from direction and custom gravity
        Vector3 movingDir = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
        Vector3 movingVector = new Vector3(movingDir.x * speed, velocityY, movingDir.z * speed);
        charController.Move(movingVector * Time.deltaTime);

        // play proper animations
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsFlying", isFlying);
    }

    // wait and apply proper gravity
    // considering wing count
    IEnumerator ApplyGravityAfterDelay()
    {
        yield return new WaitForSeconds(gravityApplyTime);
        velocityY = 0f;
        gravity = GravityAtWingCount(wingCount);

        // open wings
        characterWings.GetComponent<WingController>().OpenWings();
        isWingsOpen = true;
    }

    public void ShowCollectWings()
    {
        characterWings.GetComponent<WingController>().ShowMainWings();
    }

    public void IncreaseWings(int value)
    {
        wingCount += value;
        Debug.Log(gameObject.name + " wing count : " + wingCount);
    }

    public int GetWings()
    {
        return wingCount;
    }

    // look for score road, when it hits finish the level
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.tag == "ScoreRoad" && levelFinished == false)
        {
            // set bool to finish level, set anim parameters
            levelFinished = true;
            isRunning = false;
            isFlying = false;

            // play player score animation, idle for now
            animator.SetBool("IsRunning", isRunning);
            animator.SetBool("IsFlying", isFlying);

            // calculate score from the marker transforms
            float sectionLength = (scoreEndMarkerT.position.z - finishMarkerT.position.z) / scoreSections;
            float distanceFlied = transform.position.z - finishMarkerT.position.z;
            int score = (int)(distanceFlied / sectionLength) + 1;

            Debug.Log(gameObject.name + " score: " + score);
            // these value will be put on a simple leaderboard after the level is over
        }
    }

    private float GravityAtWingCount(int count)
    {
        if (count >= 4)
            return gravityFly;
        else if (count >= 2)
            return gravityDecend;
        else
            return gravityFall;
    }
}
