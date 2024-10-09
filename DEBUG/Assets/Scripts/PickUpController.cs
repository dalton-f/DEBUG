using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
  public GameObject player;
    public Transform holdPos;
    public float pickUpRange = 20f;
    private GameObject heldObj;
    private Rigidbody heldObjRb;
    private bool canDrop = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (heldObj == null)
            {
                RaycastHit hit;

                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                {
                    if (hit.transform.gameObject.tag == "canPickup")
                    {
                        PickUpObject(hit.transform.gameObject);
                    }
                }
            }
            else
            {
                if(canDrop == true)
                {
                    StopClipping();
                    DropObject();
                }
            }
        }


        if(heldObj != null) {
            MoveObject();
        }
    }

    void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>())
        {
            heldObj = pickUpObj;
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();
            heldObjRb.isKinematic = true;
            // Set the held object as a child of the camera
            heldObjRb.transform.parent = holdPos.transform;
        }
    }

    void DropObject()
    {
        heldObjRb.isKinematic = false;
        // Remove the parent
        heldObj.transform.parent = null;
        heldObj = null;
    }

    void MoveObject()
    {
        heldObj.transform.position = holdPos.transform.position;
    }
    
    void StopClipping()
    {
        // While the cube is still clipping, move it upwards into the nearest free spaec
        while(IsClipping()) {
            heldObj.transform.position += new Vector3(0, 1f, 0);
        }
    }

    bool IsClipping()
    {
        // Get the collider of the held object
        Collider objCollider = heldObj.GetComponent<Collider>();

        // Check for overlaps in a sphere cast upwards while using half of the object's size
        RaycastHit[] hits = Physics.SphereCastAll(heldObj.transform.position, objCollider.bounds.extents.magnitude, Vector3.up, 0.1f);

        // Exclude self-collision
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider != objCollider) 
            {
                return true;
            }
        }

        return false;
    }
}
