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
            heldObjRb.transform.parent = holdPos.transform;
        }
    }

    void DropObject()
    {
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObj = null;
    }

    void MoveObject()
    {
        heldObj.transform.position = holdPos.transform.position;
    }
    
    void StopClipping()
    {
        var clipRange = Vector3.Distance(heldObj.transform.position, transform.position);

        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
     
        if (hits.Length > 1)
        {
            heldObj.transform.position = new Vector3(heldObj.transform.position.x, heldObj.transform.position.y + 1f, heldObj.transform.position.z);
        }
    }
}
