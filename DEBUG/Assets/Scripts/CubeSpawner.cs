using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class CubeSpawner : MonoBehaviour, IInteractable
{
    public Transform spawnPosition;

    private GameObject spawnedCube;

    public void Interact(){
        spawnedCube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        spawnedCube.transform.position = spawnPosition.transform.position;

        spawnedCube.tag = "canPickup";
        // Ground layer is layer 6, could also use LayerMask.NameToLayer("Ground");
        spawnedCube.layer = 6;

        spawnedCube.AddComponent<Rigidbody>();
    }
}