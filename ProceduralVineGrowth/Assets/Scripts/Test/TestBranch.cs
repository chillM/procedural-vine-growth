using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBranch : MonoBehaviour
{
    public CameraController cameraController;
    public GameObject branchPrefab;
    public Vine vine;

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

        // create a branch object
        GameObject branchObject = Instantiate(branchPrefab, hit.point + hit.normal * vine.maxRadius, Quaternion.identity);
        Branch branch = branchObject.GetComponent<Branch>();
        branch.vine = vine;
        branchObject.transform.position = hit.point + hit.normal * vine.maxRadius; // position it according to the raycast hit information


        // create the first cross section at the hit location; translate the direction into local branch space
        Vector3 sectionNormal = Vector3.up - Vector3.Project(Vector3.up, hit.normal);
        CrossSection section = new CrossSection(Vector3.zero, sectionNormal, vine.startRadius);
        section.surfaceDirection = branchObject.transform.InverseTransformPoint(hit.point) - section.position; //calculating surface direction in local space
        section.CalculateVertices(vine.sides);
        section.concavity = 0f;

        // add the first cross section to the branch cross section list
        branch.addCrossSection(section);

        CrossSection section2 = new CrossSection(Vector3.zero, sectionNormal, vine.startRadius);
        section2.position += vine.sectionLength/2 * sectionNormal;
        section2.surfaceDirection = section.surfaceDirection; // just give them the same direction
        section2.CalculateVertices(vine.sides);
        section2.concavity = 0f;
        // add a second cross section
        branch.addCrossSection(section2);
    }
}
