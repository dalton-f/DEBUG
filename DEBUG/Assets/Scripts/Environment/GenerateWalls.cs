using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateWalls : MonoBehaviour
{
    public float wallHeight;
    public float wallWidth;

    void Start()
    {
        // Create an empty game object to contain the wall
        GameObject wallContainer = new GameObject("Wall Container");

        // Move the container
        wallContainer.transform.position = new Vector3(20f, 1f, -20f);

        for(int i = 0; i < wallWidth; i++) {
            for(int j = 0; j < wallHeight; j++) {
                // Create a cube
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                // Add a rigidbody
                cube.AddComponent<Rigidbody>();
                // Set the size of the cube
                cube.transform.localScale = Vector3.one * 1f;
                // Set it as a child of the container
                cube.transform.SetParent(wallContainer.transform);
                // Offset the position
                cube.transform.position = new Vector3(
                    Mathf.Round(wallContainer.transform.position.x + i),   
                    Mathf.Round(wallContainer.transform.position.y + j),    
                    wallContainer.transform.position.z 
                );
            }
        }
    }
}
