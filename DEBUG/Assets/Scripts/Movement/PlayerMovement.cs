using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Movement Settings")]
    private float moveSpeed;
    public float walkSpeed;
    public float maxYSpeed;
    public float dashSpeed;
    public float groundDrag = 5f;

    [Header("Jumping Settings")]

    public float jumpForce = 10f;
    public float jumpCooldown = 0.25f;
    public float airMultiplier = 0.5f;
    bool readyToJump = true;

    [Header("Dashing Settings")]
    public float dashForce;
    public float dashDuration;
    public float dashCooldown;
    public float dashSpeedChangeFactor;
    public ParticleSystem dashEffect;
    private float dashCooldownTimer;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.E;

    [Header("Ground Checks")]
    public float playerHeight = 2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("References")]
    public Transform orientation;
    public Transform cameraPosition;
    public PlayerCam playerCamera;

    private ParticleSystem speedVFX;

    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private Rigidbody rb;
    private float speedChangeFactor;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    private MovementState lastState;
    private bool keepMomentum;

    private Coroutine moveSpeedCoroutine;

    [Header("States")]

    public bool isDashing;
    public MovementState state;

    public enum MovementState 
    {
        walking,
        dashing,
        air
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Freeze rotation otherwise player falls over
        rb.freezeRotation = true;   

        // Allow jumping immediately
        readyToJump = true;
    }

    void Update() {
        // Check if the player is grounded by shooting a raycast downwards where the raycast is size is the players height + some extra
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);

        HandleInput();
        SpeedControl();
        StateHandler();

        // Apply drag if grounded/walking
        rb.drag = state == MovementState.walking ? groundDrag : 0f;
    }

    void FixedUpdate() {
        ApplyMovement();
    }

    private void HandleInput() {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // If the spacebar is pressed, and the player is grounded and hasn't jumped in the past jumpCooldown seconds,
        if(Input.GetKey(jumpKey) && readyToJump && isGrounded) {
            readyToJump = false;

            // Make the player jump
            Jump();

            // Invoke the cooldown to reset the readyToJump bool
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Handle dashing
        if(Input.GetKey(dashKey)) {
            Dash();
        }
        
        // If the cooldown timer is greater than 0, count it down
        if(dashCooldownTimer > 0) {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    private void StateHandler() {
        // Dashing
        if(isDashing) {
            state = MovementState.dashing; 
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;
        }
        // Walking
        else if(isGrounded) {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        } 
        // Air
        else {
            state = MovementState.air;
            desiredMoveSpeed = walkSpeed;
        }

        // Tracking most recent moveSpeed and state to ensure we can maintain monentum when dashing

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;

        if(lastState == MovementState.dashing) keepMomentum = true;

        if(desiredMoveSpeedHasChanged) {
            // If we want to maintain our momentum, use a coroutine to lerp smoothly between the desiredMoveSpeed and the current mvoeSpeed
            if(keepMomentum) {
                StopAllCoroutines();
  
                StartCoroutine(SmoothlyLerpMoveSpeed());
            } 
            // The speed will instantly change to the desired move speed if the momentum doesn't need to be kept
            else {
                StopAllCoroutines();
  
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;
    }

    private void ApplyMovement(){
        // Calculate the movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Add force on the ground
        if(isGrounded)
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

        if(maxYSpeed != 0 && rb.velocity.y > maxYSpeed) {
            rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);
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

    private void Dash() {
        // If the cooldown timer is above zero, we cannot dash
        if(dashCooldownTimer > 0) return;
        // Otherwise, perform the dash and reset the cooldown
        else dashCooldownTimer = dashCooldown;
        
        isDashing = true;

        // Disable gravity and reset velocity
        rb.useGravity = false;
        rb.velocity = Vector3.zero;

        // Add the dash force to the rigid body
        rb.AddForce(orientation.forward * dashForce, ForceMode.Impulse);

        // Zoom out camera
        playerCamera.ChangeFOV(75);
        // Instantiate particle effect
        speedVFX = Instantiate(dashEffect, cameraPosition.transform.position, cameraPosition.transform.rotation);
        // Set it as a child of the camera
        speedVFX.transform.SetParent(cameraPosition.transform);
        // Apply offset so it is visible
        speedVFX.transform.localPosition = new Vector3(0, 0, 8f);

        Invoke(nameof(ResetDash), dashDuration);
    }

    private void ResetDash() {
        isDashing = false;
        rb.useGravity = true;

        playerCamera.ChangeFOV(60);
        Destroy(speedVFX);
    }

    private IEnumerator SmoothlyLerpMoveSpeed() {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        float boostFactor = speedChangeFactor;

        while(time < difference) {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            time += Time.deltaTime * boostFactor;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
        speedChangeFactor = 1f;
        keepMomentum = false;
    }
}
