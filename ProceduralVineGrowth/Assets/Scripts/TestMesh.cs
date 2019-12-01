using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMesh : MonoBehaviour
{
    public int numSides = 4;

    private MeshFilter filter;

    void Start()
    {
        filter = gameObject.AddComponent<MeshFilter>();

        // test the cross section connection using a cube shape
        List<CrossSection> sections = new List<CrossSection>();

        //insert elements
        InsertElements(sections);

        //build a mesh
        BuildMesh(sections);
    }

    private void BuildMesh(List<CrossSection> sections) {
        Mesh mesh = filter.mesh;
        mesh.Clear();


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
            int modifier = (vert - (4*numSides)); // gets the triangles to align with the larger vertex array
            for(int j = 0; j < tempList.Count; j++) {
                tempList[j] += modifier; // adding where the previous cross section starts in the vertex array
            }
            triangles.AddRange(tempList); // add the new triangles to the list
        }
        // TODO add the cap on the end

        //add normals
        vert = 0;
        for (int i = 0; i < sections.Count; i++)
        {
            //add the normals for that section
            for(int n = 0; n < sections[i].normals.Length; n++) {
                normals[vert++] = sections[i].normals[n];
            }
        }

        // we should have vertices, triangles, and normals by now.
        // faces are sharing some vertices right now

        mesh.vertices = vertices;
        mesh.normals = normals;
        //mesh.uv = uvs;
        mesh.triangles = triangles.ToArray();
        
        mesh.RecalculateBounds();
        mesh.Optimize();
    }

    private void InsertElements(List<CrossSection> list) {
        list.Add(new CrossSection(new Vector3(0,0,0), Vector3.up, 2f));
        list.Add(new CrossSection(new Vector3(0,1,0), Vector3.up, 2f));
        list.Add(new CrossSection(new Vector3(0,2,0), Vector3.up, 2f));
    }
}
