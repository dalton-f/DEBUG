using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Keybinds")]

    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Checks")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Freeze rotation otherwise player falls over
        rb.freezeRotation = true;   

        readyToJump = true;
    }

    void Update() {
        // Check if the player is grounded by shooting a raycast downwards where the raycast is size is the players height + some extra
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();

        // Apply drag if grounded
        if(grounded) 
            rb.drag = groundDrag;
        else 
            rb.drag = 0;
    }

    void FixedUpdate() {
        MovePlayer();
    }

    private void MyInput() {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // If the spacebar is pressed, and the player is grounded and hasn't jumped in the past jumpCooldown seconds,
        if(Input.GetKey(jumpKey) && readyToJump && grounded) {
            readyToJump = false;

            // Make the player jump
            Jump();

            // Invoke the cooldown to reset the readyToJump bool
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer(){
        // Calculate the movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Add force on the ground
        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        // Add additional force in the air
        else 
            rb.AddForce(moveDirection.normalized * moveSpeed * airMultiplier * 10f, ForceMode.Force);

    }   

    // Limit player speed manually
    private void SpeedControl(){
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // If the flat velocity goes above the maximum movement speed
        if(flatVelocity.magnitude > moveSpeed) {
            // Calculate what the max velocity would be
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            // Apply that to the character
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }

    private void Jump() {
        // Reset y velocity before adding forces so you always jump the same height
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

    }

    private void ResetJump() {
        readyToJump = true;
    }

}
