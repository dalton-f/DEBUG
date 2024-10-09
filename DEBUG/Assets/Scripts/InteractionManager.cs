using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IInteractable {
    public void Interact();
}

public class InteractionManager : MonoBehaviour
{
    public KeyCode interactionKey = KeyCode.F;
    public float interactionRange = 20f;

    void Update()
    {
        if(Input.GetKeyDown(interactionKey)) {
            if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, interactionRange)) {
                if(hit.collider.gameObject.TryGetComponent(out IInteractable interactedObj)) {
                    interactedObj.Interact();
                }
            }

        }
        
    }
}
