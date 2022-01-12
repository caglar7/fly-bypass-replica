using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    #region PARAMETERS
    // char controller and speed adjusting
    private CharacterController charController;

    // bot parameters
    [Header("Bot Controls")]
    [SerializeField] private bool isBot = false;
    [SerializeField] private Transform[] xPositions;
    [SerializeField] private float xChangeTime1 = 1f, xChangeTime2 = 3f; // no zero for time1
    [SerializeField] private float smoothTime = .3f; // rotation set back to zero in smoothtime
    private float xChangeTimer;
    private bool xPositionChange = false;
    private float nextXPosition;
    private bool isSmoothing = false;
    private float prevAngle;
    private bool botFinished = false;

    // character controls, ground layer, jumping
    [Header("Character Controls")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpHeight = 10f;   // if any changes, calculate the time it takes to reach peak and adjust applytime
    private float horizontal = 0f;
    private float rotation, rotationY, angle;

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
    [SerializeField] private Transform startMarkerT;
    [SerializeField] private Transform finishMarkerT;
    [SerializeField] private Transform scoreEndMarkerT;
    private bool levelFinished = false;
    private float scoreSections = 40f;

    [Header("Check Point Parameters")]
    [SerializeField] private float checkPointDelay = 1f;
    private bool characterFall = false;

    #endregion

    void Start()
    {
        charController = GetComponent<CharacterController>();
        gravity = gravityRun;
        isRunning = true;
        wingCount = 0;
        wingsTimer = loseWingsPeriod;
        xChangeTimer = Random.Range(xChangeTime1, xChangeTime2);
    }

    void Update()
    {
        // don't run the code when level is over
        if (levelFinished)
            return;

        // checkpoint
        if (characterFall)
            return;

        // keep angle in range
        angle = (angle < -90f) ? (angle + 360f) : angle;
        angle = (angle > 90f) ? (360 - angle) : angle;
        if (isBot && botFinished == false)
        {
            if(transform.position.z >= finishMarkerT.position.z)
            {
                Debug.Log(transform.gameObject.name + " no rotation point");
                
                botFinished = true;
                angle = 0f;
                horizontal = 0f;
                return;

            }

            // every random time in t1-t2, bot has a new next x position target
            xChangeTimer -= Time.deltaTime;
            if(xChangeTimer <= 0f && xPositionChange == false && isSmoothing == false)
            {
                xChangeTimer = Random.Range(xChangeTime1, xChangeTime2);
                xPositionChange = true;
                nextXPosition = GetNextXPoint();

            }

            // horizontal value left or right till reaches the xpos
            // position change and smoothing time should be less than xChangeTimes
            if(xPositionChange && isSmoothing == false)
            {
                horizontal = ((nextXPosition - transform.position.x) > 0f) ? 1f : horizontal;
                horizontal = ((nextXPosition - transform.position.x) < 0f) ? -1f : horizontal;
                if (Mathf.Abs(nextXPosition - transform.position.x) <= .1f)
                {
                    horizontal = 0f;
                    xPositionChange = false;
                    isSmoothing = true;
                    prevAngle = angle;
                }
            }

            if(isSmoothing)
            {
                // bot rotation angle assign
                angle = Mathf.Lerp(prevAngle, 0f, rotationSpeed * Time.deltaTime / smoothTime);
                if(angle == 0f)      
                {
                    isSmoothing = false;
                }  
            }
            else
            {
                // rotation
                CharacterRotation();
            }

        }
        else
        {
            // with touch input, before apk
            //horizontal = TouchController.touchMoveDirection;

            // with keyboard
            horizontal = Input.GetAxisRaw("Horizontal");

            // rotation
            CharacterRotation();
        }

        // rotation assign
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        // bot rotation fix
        if (transform.gameObject.tag == "Bot" && botFinished == true)
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);

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

        // assign arrow UI for distance, only if char in range
        if (transform.position.z <= finishMarkerT.position.z && transform.gameObject.tag == "Player")
            CanvasController.instance.AssignArrowUI(transform.position.z, startMarkerT.position.z, finishMarkerT.position.z);
    }


    #region WING METHODS
    public void ShowCollectWings()
    {
        characterWings.GetComponent<WingController>().ShowMainWings();
    }

    public void IncreaseWings(int value)
    {
        wingCount += value;
    }

    public int GetWings()
    {
        return wingCount;
    }
    #endregion


    #region CHECK POINT CALL AND DELAY
    public void CharacterCheckPoint(Vector3 pos)
    {
        // toggle, assign pos, idle anim, delay
        characterFall = true;
        transform.position = pos;
        transform.eulerAngles = Vector3.zero;
        isRunning = false;
        isFlying = false;
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsFlying", isFlying);
        StartCoroutine(CheckPointWait());
    }

    IEnumerator CheckPointWait()
    {
        yield return new WaitForSeconds(checkPointDelay);
        characterFall = false;
    }
    #endregion


    #region FINISH ROAD COLLIDER
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

            // assign score value to dictionary
            GameController.instance.SetScoreValue(transform.gameObject.name, score);
            StartCoroutine(WaitAndUpdateBoard());    // only tries to update when subcanvas is active

            // if player, switch canvas to leaderboards
            if(transform.gameObject.tag == "Player")
                CanvasController.instance.SwitchCanvas(CanvasType.LeaderBoard);

        }
    }

    IEnumerator WaitAndUpdateBoard()
    {
        yield return new WaitForSeconds(.1f);
        CanvasController.instance.UpdateLeaderboard();
    }

    #endregion


    #region METHOD CALLS AND DELAYS IN UPDATE
    // character rotation
    private void CharacterRotation()
    {
        rotation = rotationSpeed * horizontal;
        rotationY = transform.rotation.eulerAngles.y;
        angle = rotationY + rotation * Time.deltaTime;
    }

    // for AI movements
    private float GetNextXPoint()
    {
        // return a random x value in platform horizontal range
        return Random.Range(xPositions[0].position.x, xPositions[1].position.x);
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
    #endregion
}
