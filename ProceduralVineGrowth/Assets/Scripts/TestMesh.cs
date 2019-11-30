using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMesh : MonoBehaviour
{
    public int numSides = 4;

    void Start()
    {
        // test the cross section connection using a cube shape
        List<CrossSection> sections = new List<CrossSection>();

        //insert elements
        InsertElements(sections);

        //build a mesh
        BuildMesh(sections);
    }

    private void BuildMesh(List<CrossSection> sections) {
        Vector3[] vertices = new Vector3[sections.Count * numSides * 2];
        Vector3[] normals = new Vector3[vertices.Length];
        int vert = 0;
        List<int> triangles = new List<int>();

        sections[0].CalculateVertices(numSides);
        foreach (Vector3 point in sections[0].vertices)
        {
            vertices[vert++] = point;
        }
        // TODO add the cap on the end
        for(int i = 1; i < sections.Count; i++) {
            sections[i].CalculateVertices(numSides);
            // add the vertices to the vertex array
            foreach (Vector3 point in sections[i].vertices)
            {
                vertices[vert++] = point;
            }

            // calculate the faces
            List<int> tempList = CrossSection.ConnectSections(sections[i-1], sections[i]);
            // adjust the triangle values returned
            for(int j = 0; j < tempList.Count; j++) {
                tempList[j] += vert - 2*numSides; // adding where the previous cross section starts in the vertex array
            }
            triangles.AddRange(tempList); // add the new triangles to the list

            // add the normals

        }
        // TODO add the cap on the end
    }

    private void InsertElements(List<CrossSection> list) {

    }
}
