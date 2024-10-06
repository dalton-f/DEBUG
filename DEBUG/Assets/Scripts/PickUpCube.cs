using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpCube : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Maximum range for picking up objects.")]
    public float pickUpRange = 10f;
    
    [Header("References")]
    [Tooltip("The player character or object that will be holding the picked-up object.")]
    public GameObject player;
    [Tooltip("The position where the held object will appear.")]
    public Transform holdPos;

    [Header("Keybinds")]

    public KeyCode pickupKey = KeyCode.Q;

    private GameObject heldObj; 
    private Rigidbody heldObjRb;

    void Update()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            if (heldObj == null)
            {
                RaycastHit hit;

                // Check that the cube is within pickup range

                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                {
                    // Check that the cube can be picked up

                    if (hit.transform.CompareTag("canPickup"))
                    {
                        PickUpObject(hit.transform.gameObject);
                    }
                }
            } else {
                DropObject();
            }
        }

        if (heldObj != null)
        {
            MoveObject();
        }
    }

    void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>())
        {
            heldObj = pickUpObj;
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();

            heldObjRb.useGravity = false;
            heldObjRb.freezeRotation = true;
            heldObj.transform.parent = holdPos.transform;
        }
    }

    void MoveObject()
    {
        heldObj.transform.position = holdPos.transform.position;
    }

    void DropObject()
    {
        heldObj.transform.parent = null;
        heldObj = null;
        heldObjRb.useGravity = true;
        heldObjRb.freezeRotation = false;

    }
}
