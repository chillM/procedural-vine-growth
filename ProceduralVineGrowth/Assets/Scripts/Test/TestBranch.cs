using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBranch : MonoBehaviour
{
    public CameraController cameraController;

    void Start()
    {
        cameraController.OnRaycastHit += CreateBranch; //subscribe to the event
    }

    void Update()
    {
        
    }

    /// <summary>
    /// Create a new branch at the point the raycast hits
    /// </summary>
    /// <param name="hit"></param>
    void CreateBranch(RaycastHit hit) {
        Debug.Log("Creating Branch");
    }
}
