using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    // char controller and speed adjusting
    private CharacterController charController;

    // character controls, ground layer, jumping
    [Header("Character Controls")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 50f;
    private float rotation = 0f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpHeight = 10f;   // if any changes, calculate the time it takes to reach peak and adjust applytime

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
    [SerializeField] private GameObject collectWingsOnBack;
    [SerializeField] private float loseWingsPeriod = 2f;
    private float wingsTimer;
    public static int wingCount;
    
    // animation parameters
    [Header("Animation Parameters")]
    [SerializeField] private Animator animator;
    private bool isRunning = false;
    private bool isFlying = false;

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
        // check ground and apply gravity if not on ground
        isOnGround = Physics.CheckSphere(transform.position, .1f, groundLayer, QueryTriggerInteraction.Ignore);

        // every period of time not on ground, lose wings, then adjust gravity
        if(isOnGround == false)
        {
            wingsTimer -= Time.deltaTime;
            if(wingsTimer <= 0f)
            {
                // lose wings here and animate
                wingsTimer = loseWingsPeriod;
                wingCount -= 2;
                wingCount = (wingCount <= 0) ? 0 : wingCount;
                gravity = GravityAtWingCount(wingCount);
            }
        }

        if (isOnGround && velocityY < 0f)
        {
            velocityY = 0f;
            gravity = gravityRun;
            isRunning = true;
            isFlying = false;
            isJumped = false;
        }
        else
        {
            velocityY += gravity * Time.deltaTime;
        }

        // to the left or right, for now later with screen scroll
        float horizontal = Input.GetAxisRaw("Horizontal");
        rotation = rotationSpeed * horizontal;

        // rotation 
        float rotationY = transform.rotation.eulerAngles.y;
        float angle = rotationY + rotation * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

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

    public void ShowCollectWings()
    {
        collectWingsOnBack.SetActive(true);
    }

    // wait and apply proper gravity
    // considering wing count
    IEnumerator ApplyGravityAfterDelay()
    {
        yield return new WaitForSeconds(gravityApplyTime);
        velocityY = 0f;
        gravity = GravityAtWingCount(wingCount);
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
