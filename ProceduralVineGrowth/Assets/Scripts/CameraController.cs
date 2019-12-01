using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public event Action<RaycastHit> OnRaycastHit;
    //[SerializeField] private Reticle m_Reticle;                     // The reticle, if applicable.
    [SerializeField] private bool m_ShowDebugRay;                   // Optionally show the debug ray.
    [SerializeField] private float m_DebugRayLength = 10f;           // Debug ray length.
    [SerializeField] private float m_DebugRayDuration = 1f;         // How long the Debug ray will remain visible.
    [SerializeField] private float m_RayLength = 50f;              // How far into the scene the ray is cast.

    [SerializeField] bool mouseControl = true;  //Should the mouse control the camera? Use this to easily disable this script
    [SerializeField] float camSpeed = 30f;      //The speed the camera rotates
    [SerializeField] float movementSpeed = 3f;  //The speed the player moves

    Transform m_Camera;                         //Reference to the HMD's position


    void Awake()
    {
        //If this is not the editor (it's a build), if we don't want mouse control, or if VR is enabled then
        //destroy this script and exit
        if (!Application.isEditor || !mouseControl || UnityEngine.XR.XRSettings.enabled)
        {
            Destroy(this);
            return;
        }

        //Otherwise, we want editor control and should lock the cursor
        LockCursor();

        //Get the HMD's transform and find all tracked hand objects in the hierarchy
        m_Camera = GetComponentInChildren<Camera>().transform;


        //Start manaing the player's movement
        StartCoroutine(ManageMovement());
    }

    //Detect mouse movements and move camera accordingly
    IEnumerator ManageMovement()
    {
        while (mouseControl)
        {
            //check for raycast trigger to place a vine
            if(Input.GetKeyUp(KeyCode.Space)) {
                Ray ray = new Ray(m_Camera.position, m_Camera.forward);
                RaycastHit hit;

                if (m_ShowDebugRay)
                {
                    Debug.DrawRay(m_Camera.position, m_Camera.forward * m_DebugRayLength, Color.blue, m_DebugRayDuration);
                }

                if (Physics.Raycast(ray, out hit, m_RayLength)) {
                   if(OnRaycastHit != null) {
                       OnRaycastHit(hit);
                   }
                }
            }

            //Get the movement of the mouse
            float horizontal = Input.GetAxis("Mouse X") * camSpeed * Time.deltaTime;
            float vertical = Input.GetAxis("Mouse Y") * camSpeed * Time.deltaTime;

            //Rotate the camera accordingly
            transform.Rotate(0f, horizontal, 0f, Space.World);
            m_Camera.Rotate(-vertical, 0f, 0f, Space.Self);

            //Get the player's body movement from the keyboard
            float moveLR = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;
            float moveFB = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
            //Apply the movement
            transform.Translate(moveLR, 0f, moveFB);

            //If the user presses "escape", unlock the cursor
            if (Input.GetButtonDown("Cancel"))
                UnlockCursor();
            //Exit until the next frame
            yield return null;
        }
    }

    void LockCursor()
    {
        //Lock the cursor to the middle of the screen and then hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        //Release the cursor and show it
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
