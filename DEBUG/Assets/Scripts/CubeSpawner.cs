using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour, IInteractable
{
    public void Interact(){
        Debug.Log("interacted");
    }
}