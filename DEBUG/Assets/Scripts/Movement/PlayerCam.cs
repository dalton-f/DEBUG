using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;

    float xRotation;
    float yRotation;

    // Lock and hide cursor in the middle of the screen
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Get the mouse inputs and make them framerate independent 
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;

        // Clamp the x rotation so you can't look directly up or down
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate the player and camera
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        orientation.rotation = Quaternion.Euler(0f, yRotation, 0);
    }

    public void ChangeFOV(float targetFOV)
    {
        StartCoroutine(SmoothFOVChange(targetFOV));
    }

    private IEnumerator SmoothFOVChange(float targetFOV)
    {
        Camera camera = GetComponent<Camera>();

        float currentFOV = camera.fieldOfView;
        float duration = 0.5f; 
        float elapsed = 0f;

        while (elapsed < duration)
        {
            camera.fieldOfView = Mathf.Lerp(currentFOV, targetFOV, elapsed / duration);
            elapsed += Time.deltaTime * 3f;
            yield return null;
        }

        camera.fieldOfView = targetFOV;
    }
}
