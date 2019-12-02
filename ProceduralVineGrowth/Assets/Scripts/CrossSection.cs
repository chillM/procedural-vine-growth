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

    public CrossSection(Vector3 position, Vector3 normal, float radius) {
        this.position = position;
        this.normal = normal.normalized;
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
        //float _2pi = Mathf.PI * 2f; not using because angle axis uses degrees

        vertices = new Vector3[numSides*2];
        int vert = 1; // goes through the vertices

        //first point
        vertices[0] = position + temp;
        //all other points (entered twice)
        for(int v = 1; v < numSides; v++) {
            //rotate the temp vector and get the point there
            temp = Quaternion.AngleAxis(360/numSides, normal) * temp; //rotate temp 1/numSides around the normal vector
            //add the point twice
            vertices[vert++] = position + temp;
            vertices[vert] = vertices[vert-1];
            vert++;

        }
        //last point
        vertices[vertices.Length-1] = vertices[0];


        // normal calculation
        normals = new Vector3[vertices.Length];
        vert = 0;

        for(;vert < normals.Length-1; vert+=2) {
            //calculate the normal for one face at a time
            Vector3 faceCenter = 0.5f * vertices[vert] + 0.5f * vertices[vert]; // get the center of the face
            normals[vert] = faceCenter - position; // get the vector to the face center
            normals[vert+1] = normals[vert];
        }
    }

    /// <summary>
    /// Connects two cross sections together assuming they are adjacent in a vertex array
    /// </summary>
    /// <param name="prev"></param>
    /// <param name="current"></param>
    /// <returns>The triangle list that would result assuming prev was index 0 of the vertex array</returns>
    public static List<int> ConnectSections(CrossSection prev, CrossSection current) {
        List<int> triangles = new List<int>();
        int stride = prev.vertices.Length;

        // result is based only on number of sides right now
        
        // first triangle
        triangles.AddRange(new int[] {1, stride, 0});

        // add two triangles for all intermediary vertices
        for(int i = 1; i < prev.vertices.Length-1; i++) {
            triangles.AddRange(new int[] {stride+i, stride+i-1, i});
            triangles.AddRange(new int[] {i+1, stride+i, i});
        }

        //last triangle
        triangles.AddRange(new int[] {stride*2-1, stride*2-2, stride-1});

        return triangles;
    }

    public static void ConvertToWorldSpace(CrossSection section, Transform localTransform) {
        section.position = localTransform.TransformPoint(section.position);
        section.normal = localTransform.TransformDirection(section.normal);

        if(section.vertices == null) return;

        for(int i = 0; i < section.vertices.Length; i++) {
            section.vertices[i] = localTransform.TransformPoint(section.vertices[i]);
            section.normals[i] = localTransform.TransformDirection(section.normals[i]);
        }
    }

    public static void ConvertToLocalSpace(CrossSection section, Transform localTransform) {
        section.position = localTransform.InverseTransformPoint(section.position);
        section.normal = localTransform.InverseTransformDirection(section.normal);

        if(section.vertices == null) return;

        for(int i = 0; i < section.vertices.Length; i++) {
            section.vertices[i] = localTransform.InverseTransformPoint(section.vertices[i]);
            section.normals[i] = localTransform.InverseTransformDirection(section.normals[i]);
        }
    }
}
