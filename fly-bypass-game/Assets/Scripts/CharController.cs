using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    private CharacterController charController;
    public float speed = 5f;

    public float rotationSpeed = 50f;
    private float rotation = 0f;

    void Start()
    {
        charController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // to the left or right, for now later with screen scroll
        float horizontal = Input.GetAxisRaw("Horizontal");
        rotation = rotationSpeed * horizontal;

        // rotation
        float rotationY = transform.rotation.eulerAngles.y;
        float angle = rotationY + rotation * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        // move on direction obtained from rotation
        Vector3 movingDir = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
        charController.Move(movingDir * speed * Time.deltaTime);

    }
}
