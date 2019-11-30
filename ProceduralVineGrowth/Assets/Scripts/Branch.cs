using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossSection
{
    public Vector3 position;
    public Vector3 normal;
    public float radius;
    public Vector3[] vertices { get; private set; }
    public Vector3[] normals { get; private set; }
    public bool changeOccurs;

    CrossSection(Vector3 position, Vector3 normal, float radius) {
        this.position = position;
        this.normal = normal;
        this.radius = radius;
    }

    /// <summary>
    /// Uses the position, plane normal, and radius to calculate the vertices for this cross section
    /// </summary>
    /// <param name="numSides">Decides how many points should be created</param>
    public void CalculateVertices(int numSides) {
        Vector3 temp = Vector3.Angle(normal, Vector3.right) > 0.1f ? Vector3.Cross(normal, Vector3.right) : Vector3.Cross(normal, Vector3.forward);
        temp.Normalize();
        temp *= radius;
        float _2pi = Mathf.PI * 2f;

        vertices = new Vector3[numSides*2];
        int vert = 0;

        //first point
        vertices[0] = position + temp;
        //all other points (entered twice)
        for(int v = 1; v < numSides; v++) {
            //rotate the temp vector and get the point there
            temp = Quaternion.AngleAxis(1/numSides * _2pi, normal) * temp; //rotate temp 1/numSides around the normal vector
            vertices[vert++] = position + temp;

        }
        //last point
        vertices[vertices.Length-1] = vertices[0];


        // TODO also include normal calculation
    }

    /// <summary>
    /// Connects two cross sections together assuming they are adjacent in a vertex array
    /// </summary>
    /// <param name="prev"></param>
    /// <param name="current"></param>
    /// <returns>The triangle list that would result assuming prev was index 0 of the vertex array</returns>
    public static List<int> ConnectSections(CrossSection prev, CrossSection current) {
        List<int> triangles = new List<int>();

        return triangles;
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
