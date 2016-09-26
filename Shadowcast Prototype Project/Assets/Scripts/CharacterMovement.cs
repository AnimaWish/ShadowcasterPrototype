using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))] // Requires a CharacterController to be attached (attach in "Add Component")
public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float jumpHeight = 4f;
    public float gravity = -35f;

    private CharacterController controller;

    // Use this for initialization
    void Start ()
    {
        controller = GetComponent<CharacterController>(); // Allows use of Character Controller functionality
    }

    // Fixed update is called in sync with physics
    void FixedUpdate ()
    {
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        Vector3 velocity = controller.velocity;

        // So the movement stops
        velocity.x = 0;
        velocity.z = 0;

        // Movement Controlls
        if (horiz > 0)
        {
            velocity.z = moveSpeed;
        }
        else if (horiz < 0)
        {
            velocity.z = -moveSpeed;
        }
        if (vert > 0)
        {
            velocity.x = -moveSpeed;
        }
        else if (vert < 0)
        {
            velocity.x = moveSpeed;
        }

        // Make sure the character is grounded first
        if (Input.GetAxis("Jump") > 0 && controller.isGrounded) // Jump only returns positive number. Activated by spacebar
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -gravity);
        }

        velocity.y += gravity * Time.deltaTime; // Add gravity to y-direction velocity

        controller.Move(velocity * Time.deltaTime); // Make the move
    }
}
