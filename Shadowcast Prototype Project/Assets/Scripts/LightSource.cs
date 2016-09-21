using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class LightSource : MonoBehaviour
{

    LightCollider[] lightcolliders;
    public int shadowDistance = 20;
    public GameObject cube;
    // Use this for initialization
    void Start()
    {
        lightcolliders = FindObjectsOfType(typeof(LightCollider)) as LightCollider[];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (var lightcollider in lightcolliders)
        {
            if (lightcollider.castsShadow)
            {
                lightcollider.UpdateMesh(GetMeshFromLightCollider(lightcollider));
                
            }
        }
    }

    Mesh GetMeshFromLightCollider(LightCollider lc)
    {
        List<Vector3> lcVerts = lc.GetWorldVertices();

        List<Vector3> newVerts = new List<Vector3>();

        foreach (var v in lcVerts)
        {
            Ray ray = new Ray(transform.position, (v - transform.position));
            List<RaycastHit> hits = SortAndUnique(Physics.RaycastAll(ray));

           // RaycastHit bounce;
            //Physics.Raycast(hits[hits.Count - 1].point, Vector3.Scale(ray.direction, new Vector3(-1, -1, -1)), out bounce);

            if (hits.Count > 1 && hits[0].point == v) // && hits[0].point == bounce.point)
            {
                newVerts.Add(hits[0].point);
                newVerts.Add(hits[1].point);
            }
        }

        Mesh mesh = new Mesh();
        if (newVerts.Count > 0)
        {
            mesh.vertices = SortEdgesRadially(lc.transform.position, (lc.transform.position - transform.position).normalized, newVerts).ToArray();
            mesh.triangles = GenerateTriangles(newVerts.Count);
        }

        return mesh;
    }

    List<RaycastHit> SortAndUnique(RaycastHit[] array)
    {
        List<RaycastHit> list = new List<RaycastHit>();
        HashSet<Vector3> vectors = new HashSet<Vector3>();
        foreach (var hit in array)
        {
            if (!vectors.Contains(hit.point))
            {
                list.Add(hit);
                vectors.Add(hit.point);
            }
        }

        list.Sort((x, y) => x.distance.CompareTo(y.distance));

        return list;
    }

    //Takes a list of verts, where each sequential pair of vertices forms an edge. Projecting to the plane, sorts the pairs
    //in order of their angle. Uses the first point
    List<Vector3> SortEdgesRadially(Vector3 origin, Vector3 normal, List<Vector3> verts)
    {
        double[] angles = new double[verts.Count / 2];

        List<Color> debugColors = new List<Color>();
        while (debugColors.Count < verts.Count)
        {
            debugColors.Add(Color.red);
            debugColors.Add(Color.yellow);
            debugColors.Add(Color.green);
            debugColors.Add(Color.cyan);
            debugColors.Add(Color.blue);
            debugColors.Add(Color.magenta);
            debugColors.Add(Color.white);
            debugColors.Add(Color.black);
        }

        // Project the corners of the object onto the plane created by normal, and populate the angles array with their 2d angles on the plane
        Vector3 firstProj = verts[0] - origin - Vector3.Project(verts[0] - origin, normal);
        Vector3 leftHand = Vector3.Cross(firstProj, normal);
        Debug.DrawRay(origin, normal, Color.white);
        Debug.DrawRay(origin, firstProj * 3, Color.black);
        Debug.DrawRay(origin, leftHand * 3, Color.black);
        for (var i = 0; i < verts.Count; i += 2)
        {
            Vector3 projVector = verts[i] - origin - Vector3.Project(verts[i] - origin, normal);
            double angle = Vector3.Angle(firstProj, projVector);
            //double angle = Math.Asin(Vector3.Dot(firstProj, projVector) / (firstProj.magnitude * projVector.magnitude));
            if (Vector3.Dot(leftHand, projVector) < 0)
            {
                angle = 360 - angle;
            }
            //Debug.Log(debugColors[i / 2].ToString() + angle.ToString());

            angles[i / 2] = angle;
            //angles[i / 2] = Math.Atan((projVector.y + origin.y) / (projVector.x + origin.x)) * 180 / Math.PI + 90;
            Debug.DrawRay(origin, projVector, debugColors[i / 2]);
        }
        //We now have a list of angles corresponding to verts

        //Sort the angles, and get the indices into the old angles array in order (to index into verts)
        //http://stackoverflow.com/questions/1760185/c-sharp-sort-list-while-also-returning-the-original-index-positions
        var sortedAngles = angles
            .Select((x, i) => new KeyValuePair<double, int>(x, i))
            .OrderBy(x => x.Key)
            .ToList();
        List<int> sortedIndices = sortedAngles.Select(x => x.Value).ToList();

        List<Vector3> newList = new List<Vector3>();
        foreach (var i in sortedIndices)
        {
            newList.Add(verts[2 * i]);
            newList.Add(verts[2 * i + 1]);
            Debug.DrawRay(verts[2 * i], verts[2 * i + 1] - verts[2 * i], debugColors[i]);
        }


        for (var i = 0; i < newList.Count; i += 2)
        {
            //Debug.DrawRay(newList[i], newList[i + 1] - newList[i], debugColors[i/2]);
        }

        return newList;
    }

    void PrintHitPoints(IEnumerable<RaycastHit> list)
    {
        string sconstructor = "";
        foreach (var i in list)
        {
            sconstructor = sconstructor + i.point;
        }

        Debug.Log(sconstructor);
    }

    void PrintVectorList(IEnumerable<Vector3> list)
    {
        string sconstructor = "";
        foreach (var i in list)
        {
            sconstructor = sconstructor + i;
        }

        Debug.Log(sconstructor);
    }

    //Generates a list of integers following the following pattern:
    // 0, 1, 2,   2, 1, 3,   2, 3, 4,   4, 3, 5   ...
    int[] GenerateTriangles(int listLength)
    {
        int[] anticlockwise = new int[] { 1, 0, 2 };
        int[] tris = new int[listLength * 3];
        for (var i = 0; i < listLength; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                var adder = 0;
                if (i % 2 == 0)
                {
                    adder = j;
                }
                else
                {
                    adder = anticlockwise[j];
                }
                tris[3 * i + j] = (i + adder) % listLength;
            }
        }
        return tris;
    }

}