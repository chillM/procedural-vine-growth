using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Branch : MonoBehaviour
{
    public Vine vine; // a reference to the parent vine that defines specific branch parameters

    private MeshFilter filter;
    private List<CrossSection> crossSections = new List<CrossSection>();


    void Start()
    {
        filter = gameObject.GetComponent<MeshFilter>();

        //TODO initialize the mesh with one cross section (or two)
    }

    void Update() {
        UpdateBranch(Time.deltaTime);
    }

    /// <summary>
    /// This method will create a new cross section based on the RaycastHit and append it to the list of cross sections
    /// </summary>
    /// <param name="hit"></param>
    public void addCrossSection(CrossSection section) {
        crossSections.Add(section);
    }

    /// <summary>
    /// Update the branch mesh
    /// </summary>
    /// <param name="deltaTime">Controls how much change occurs</param>
    void UpdateBranch(float deltaTime) {
        // check the first cross section
        crossSections[0].changeOccurs = false;
        if(crossSections[0].radius < vine.maxRadius) {
            // increase the radius of the cross section
            crossSections[0].radius += vine.maxRadius / 4 * deltaTime; // get a fourth of the radius in a second
            crossSections[0].changeOccurs = true;
        }

        for (int i = 1; i < crossSections.Count; i++)
        {
            crossSections[i].changeOccurs = false;
            //check distance
            if(Vector3.Distance(crossSections[i].position, crossSections[i-1].position) < vine.sectionLength) {
                // increase the distance of this cross section and all that follow it
                for(int j = i; j < crossSections.Count; j++) {
                    //shift them all in the i-1th cross section's normal direction
                    crossSections[j].position += crossSections[i-1].normal * vine.sectionLength / 6 * deltaTime; //get a third of the way in a second
                    crossSections[j].position += crossSections[i].surfaceDirection * vine.sectionLength / 2 * crossSections[i].concavity * deltaTime; // move some towards the surface
                    // TODO change here to make sure you don't go over the max length
                }
                crossSections[i].changeOccurs = true;
            }
            //check radius
            if(crossSections[i].radius < vine.maxRadius) {
                // increase the radius of the cross section
                crossSections[i].radius += vine.maxRadius / 4 * deltaTime; // get a fourth of the radius in a second
                crossSections[i].changeOccurs = true;
            }

            if(crossSections[i].changeOccurs) {
                crossSections[i].CalculateVertices(vine.sides);
            }

            //the triangles array should only be modified when new points are added, and should only be appended to

            // TODO add the cross section vertices to the vertex array
        }

        BuildMesh(crossSections);

        // check the last cross section to see if a new one needs to be created
        if(crossSections.Count > 1) {
            float distance = Vector3.Distance(crossSections[crossSections.Count - 1].position, crossSections[crossSections.Count - 2].position);
            if(distance >= vine.sectionLength) {
                GrowBranch();
            }
        }
    }

    /// <summary>
    /// Takes a list of cross sections and builds a mesh. Makes the vertex, normal, and triangle arrays from scratch each time
    /// </summary>
    /// <param name="sections"></param>
    private void BuildMesh(List<CrossSection> sections) {
        Mesh mesh = filter.mesh;
        mesh.Clear();


        Vector3[] vertices = new Vector3[sections.Count * vine.sides * 2];
        Vector3[] normals = new Vector3[vertices.Length];
        int vert = 0;
        List<int> triangles = new List<int>();

        //sections[0].CalculateVertices(vine.sides);
        foreach (Vector3 point in sections[0].vertices)
        {
            vertices[vert++] = point;
        }
        // TODO add the cap on the end
        for(int i = 1; i < sections.Count; i++) {
            //sections[i].CalculateVertices(vine.sides);
            // add the vertices to the vertex array
            foreach (Vector3 point in sections[i].vertices)
            {
                vertices[vert++] = point;
            }

            // calculate the faces
            List<int> tempList = CrossSection.ConnectSections(sections[i-1], sections[i]);
            // adjust the triangle values returned
            int modifier = (vert - (4*vine.sides)); // gets the triangles to align with the larger vertex array
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

    /// <summary>
    /// This method will determine where the next cross section should go and place it
    /// </summary>
    void GrowBranch() {
        CrossSection end = crossSections[crossSections.Count-1];
        CrossSection newSection = new CrossSection(end.position + end.normal*vine.sectionLength/2, Vector3.up, vine.startRadius);

        // get everything in world space
        CrossSection.ConvertToWorldSpace(end, transform);
        CrossSection.ConvertToWorldSpace(newSection, transform);

        // decide which direction the closest surface is by using the end and shooting out rays in all directions
        RaycastHit hit; // this will have the result of the closest raycast hit
        Vector3 direction; // this will have the direction in which that ray went
        
        if(FindClosestSurface(out hit, out direction, end)) {

            newSection.surfaceDirection = direction;
            // get the normal for the new section
            float offset = vine.sectionLength / 5;
            Ray aheadRay = new Ray(newSection.position + end.normal*offset*4f, direction);
            Ray behindRay = new Ray(newSection.position - end.normal*offset, direction);
            RaycastHit aheadHit, behindHit;

            if(Physics.Raycast(aheadRay, out aheadHit, vine.maxRadius*2) && Physics.Raycast(behindRay, out behindHit, vine.maxRadius*2)) {
                //both rays hit something, average the normals for the new cross section's normal
                Vector3 avgNormal = aheadHit.normal * 0.6f + behindHit.normal * 0.4f;
                avgNormal.Normalize();

                newSection.normal = end.normal - Vector3.Project(end.normal, avgNormal);
                newSection.normal.Normalize();

                // check concavity
                // move some amount in the surface normal direction for each ray
                Vector3 posAhead = newSection.position + aheadHit.normal;
                Vector3 posBehind = newSection.position + behindHit.normal;

                float distAhead  = Vector3.Distance(posAhead, end.position);
                float distBehind = Vector3.Distance(posBehind, end.position);

                newSection.concavity = distBehind - distAhead;

                newSection.CalculateVertices(vine.sides);
                crossSections.Add(newSection); // we only add the new section if it has a surface to follow/attach to
            }
            // if both rays don't find a surface, then don't grow any more
        }
        // if we don't find a close surface then don't grow any more

        // put everything back to local space
        CrossSection.ConvertToLocalSpace(end, transform);
        CrossSection.ConvertToLocalSpace(newSection, transform);
    }

    /// <summary>
    /// This method takes a cross section with points calculated and finds the closest surface by sending raycasts in the
    /// direction of each point in the cross section
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="section"></param>
    /// <returns></returns>
    bool FindClosestSurface(out RaycastHit hit, out Vector3 direction, CrossSection section) {
        bool surfaceFound = false;
        RaycastHit minHit;
        float minDistance = Mathf.Infinity;
        direction = Vector3.zero;

        Ray ray = new Ray(section.vertices[0], section.vertices[0] - section.position);
        if(Physics.Raycast(ray, out hit, vine.maxRadius)) {
            minDistance =  hit.distance;
            surfaceFound = true;
            direction = section.vertices[0] - section.position;
        }
        minHit = hit;
        

        for(int i = 1; i < section.vertices.Length; i++) {
            ray = new Ray(section.vertices[i], section.vertices[i] - section.position);
            if(Physics.Raycast(ray, out hit, vine.maxRadius)) {
                surfaceFound = true;
                if(minDistance > hit.distance) {
                    minDistance =  hit.distance;
                    minHit = hit;
                    direction = section.vertices[i] - section.position;
                }
            }
            
        }

        hit = minHit;
        return surfaceFound;
    }
}
