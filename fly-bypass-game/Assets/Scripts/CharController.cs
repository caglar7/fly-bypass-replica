using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    // char controller and speed adjusting
    private CharacterController charController;
    [SerializeField] private float speed = 5f;

    // rotation parameters
    [SerializeField] private float rotationSpeed = 50f;
    private float rotation = 0f;

    // gravity, ground check, jump height
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpHeight = 10f;
    [SerializeField] private float gravityRun = -40f;
    [SerializeField] private float gravityFly = -5f;
    [SerializeField] private float gravityApplyTime = 1f;
    private float gravity;
    private float velocityY;
    private bool isOnGround;
    private bool isJumped = false; // toggle to apply jump and fly only once

    // animation parameters
    [SerializeField] private Animator animator;
    private bool isRunning = false;
    private bool isFlying = false;

    // wing parameters
    [SerializeField] private GameObject collectWingsOnBack;
    public static int wingCount;


    void Start()
    {
        charController = GetComponent<CharacterController>();
        gravity = gravityRun;
        isRunning = true;
        wingCount = 0;
    }

    void Update()
    {
        // check ground and apply gravity if not on ground
        isOnGround = Physics.CheckSphere(transform.position, .1f, groundLayer, QueryTriggerInteraction.Ignore);
        if (isOnGround && velocityY < 0f)
        {
            velocityY = 0f;
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

    IEnumerator ApplyGravityAfterDelay()
    {
        yield return new WaitForSeconds(gravityApplyTime);
        gravity = gravityFly;
    }
}
