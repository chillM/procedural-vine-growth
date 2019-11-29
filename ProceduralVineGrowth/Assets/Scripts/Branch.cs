using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossSection
{
    public Vector3 position;
    public Vector3 normal;
    public float radius;
    public Vector3[] vertices { get; private set; }
    public bool changeOccurs;

    /// <summary>
    /// Uses the position, plane normal, and radius to calculate the vertices for this cross section
    /// </summary>
    /// <param name="numSides">Decides how many points should be created</param>
    public void calculateVertices(int numSides) {
        // TODO implement vertex calculation
    }
}

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
    void updateBranch(float deltaTime) {
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
                crossSections[i].calculateVertices(vine.sides);
            }

            //the triangles array should only be modified when new points are added, and should only be appended to

            // TODO add the cross section vertices to the vertex array
        }
    }
}
