using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Branch : MonoBehaviour
{
    public Vine vine; // a reference to the parent vine that defines specific branch parameters

    private MeshFilter filter;
    private List<CrossSection> crossSections;

    void Start()
    {
        filter = gameObject.GetComponent<MeshFilter>();

        //TODO initialize the mesh with one cross section (or two)
    }

    /// <summary>
    /// Update the branch mesh
    /// </summary>
    /// <param name="deltaTime">Controls how much change occurs</param>
    void UpdateBranch(float deltaTime) {
        for (int i = 1; i < crossSections.Count; i++)
        {
            crossSections[i].changeOccurs = false;
            //check distance
            if(Vector3.Distance(crossSections[i].position, crossSections[i-1].position) < vine.sectionLength) {
                // TODO increase the distance of the cross section
                crossSections[i].changeOccurs = true;
            }
            //check radius
            if(crossSections[i].radius < vine.maxRadius) {
                // TODO increase the radius of the cross section
                crossSections[i].changeOccurs = true;
            }

            if(crossSections[i].changeOccurs) {
                crossSections[i].CalculateVertices(vine.sides);
            }

            //the triangles array should only be modified when new points are added, and should only be appended to

            // TODO add the cross section vertices to the vertex array
            
            if(i == crossSections.Count - 1) {
                // TODO look at the last cross section; if it has reached a full section length and there is space, then create a new cross section

                //add new faces to the triangle array
            }
        }

    }
}
