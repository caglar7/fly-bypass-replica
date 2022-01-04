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
    private float gravity = -40f;
    private float velocityY;
    private bool isOnGround;
    

    void Start()
    {
        charController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // check ground and apply gravity if not on ground
        isOnGround = Physics.CheckSphere(transform.position, .1f, groundLayer, QueryTriggerInteraction.Ignore);
        if (isOnGround && velocityY < 0f)
            velocityY = 0f;
        else
            velocityY += gravity * Time.deltaTime;

        // to the left or right, for now later with screen scroll
        float horizontal = Input.GetAxisRaw("Horizontal");
        rotation = rotationSpeed * horizontal;

        // rotation 
        float rotationY = transform.rotation.eulerAngles.y;
        float angle = rotationY + rotation * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        // jump
        if (isOnGround && Input.GetButtonDown("Jump"))
            velocityY += Mathf.Sqrt(jumpHeight * -2f * gravity);

        // get movingVector from direction and custom gravity
        Vector3 movingDir = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
        Vector3 movingVector = new Vector3(movingDir.x * speed, velocityY, movingDir.z * speed);
        charController.Move(movingVector * Time.deltaTime);

    }
}
