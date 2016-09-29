using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class LightSource : MonoBehaviour {

	LightCollider[] lightcolliders;
	public int shadowDistance = 200000;
	public int lightRadius    = 10;

	// Use this for initialization
	void Start () {
		lightcolliders = FindObjectsOfType(typeof(LightCollider)) as LightCollider[];
	}
	
	// Update is called once per frame
	void Update () {
        foreach (var lightcollider in lightcolliders) {
            if (lightcollider.castsShadow && Vector3.Distance(lightcollider.transform.position, transform.position) < lightRadius) {
            	Debug.Log(Vector3.Distance(lightcollider.transform.position, transform.position));
                lightcollider.UpdateMesh(GetMeshFromLightCollider(lightcollider));
            }
        }
    }

    // Returns a mesh for the shadow cast by LightCollider from this light source
    Mesh GetMeshFromLightCollider(LightCollider lc) {
        List<Vector3> lcVerts = lc.GetWorldVertices();

        List<Vector3> newVerts = new List<Vector3>();

        var color_i = 0;

        foreach (var v in lcVerts) {
            Ray ray = new Ray(transform.position, (v - transform.position));
            List<RaycastHit> hits = SortAndUniqueHits(Physics.RaycastAll(ray, shadowDistance, ~(1 << 10)));

            foreach (var hit in hits) {
            	Debug.DrawRay(transform.position, hit.point - transform.position, DebugColor(color_i));
            }

            if (hits.Count > 1) {

            	//Find the first raycast hit that is actually on the lightcollider
	            RaycastHit firstHitOnObject = hits[0];
	            RaycastHit endHit = hits[1];
	            for (var hit_i = 0; hit_i < hits.Count; hit_i++) {
					if (hits[hit_i].collider == lc.GetComponent<Collider>()) {
						firstHitOnObject = hits[hit_i];
						endHit = hits[hit_i+1];
					}
	            }

            	//"Bounce" the ray back to check if it finds the same point. This ensures that the ray hit an exterior
            	//vertex of the lightcollider, removing troublesome interior points for the shadow mesh
            	// Ray bounceRay = new Ray(endHit.point, Vector3.Scale(ray.direction, new Vector3(-1, -1, -1)));
            	// //Physics.Raycast(hits[hits.Count - 1].point, Vector3.Scale(ray.direction, new Vector3(-1, -1, -1)), out bounce);
            	// List<RaycastHit> bounceHits = SortAndUniqueHits(Physics.RaycastAll(bounceRay));

	            // bool bounceHitIsFirstHit = false;
	            // foreach (var bhit in bounceHits) {
	            // 	if (bhit.point == firstHitOnObject.point) {
	            // 		bounceHitIsFirstHit = true;
	            // 		break;
	            // 	}
	            // }

	            //if (hits.Count > 1 && hits[0].point == v && hits[0].point == bounce.point) {
	            // if (hits.Count > 1 && hits[0].point == v) {
	            //     newVerts.Add(hits[0].point);
	            //     newVerts.Add(hits[1].point);
	            // }
	            if (firstHitOnObject.point == v) {
	                newVerts.Add(firstHitOnObject.point);
	                newVerts.Add(endHit.point);
	            }
	        }
            color_i++;
        }

        Mesh mesh = new Mesh();
        if (newVerts.Count > 0) {
            mesh.vertices = SortEdgesRadially(lc.transform.position, (lc.transform.position - transform.position).normalized, newVerts).ToArray();
            mesh.triangles = GenerateTriangles(newVerts.Count);
        }

        return mesh;
    }

    //Sort the hits in order of distance (ascending) and remove duplicates
    List<RaycastHit> SortAndUniqueHits(RaycastHit[] array) {
		List<RaycastHit> list = new List<RaycastHit>();
        HashSet<Vector3> vectors = new HashSet<Vector3>();
		foreach (var hit in array) {
            if (!vectors.Contains(hit.point)) {
                list.Add(hit);
                vectors.Add(hit.point);
            }
		}

        list.Sort((x, y) => x.distance.CompareTo(y.distance));

        return list;
	}

    //Takes a list of verts, where each sequential pair of vertices forms an edge. Projecting to the plane, sorts the pairs
    //in order of their angle. Uses the first point of each pair.
    List<Vector3> SortEdgesRadially(Vector3 origin, Vector3 normal, List<Vector3> verts) {
        double[] angles = new double[verts.Count / 2];

        // Project the corners of the object onto the plane created by normal, and 
        // populate the angles array with their 2d angles on the plane
        Vector3 firstProj = verts[0] - origin - Vector3.Project(verts[0] - origin, normal);

        //Since Vector3.angle always returns the acute angle, we take the dot product of the projected vector
        // with leftHand to tell if the angle measurement should've taken the long way araound
        Vector3 leftHand = Vector3.Cross(firstProj, normal); 

        Debug.DrawRay(origin, normal, Color.white);
        Debug.DrawRay(origin, firstProj*3, Color.black);
        Debug.DrawRay(origin, leftHand * 3, Color.black);

        for (var i = 0; i < verts.Count; i += 2) {
            Vector3 projVector = verts[i] - origin - Vector3.Project(verts[i] - origin, normal);
            double angle = Vector3.Angle(firstProj, projVector);
            if (Vector3.Dot(leftHand, projVector) < 0) {
                angle = 360 - angle;
            }

            angles[i / 2] = angle;
            Debug.DrawRay(origin, projVector, DebugColor(i / 2));
        }
        //We now have a list of angles corresponding to verts

        //Sort the angles, and get the indices into the old angles array in order (to index into verts)
        //http://stackoverflow.com/questions/1760185/c-sharp-sort-list-while-also-returning-the-original-index-positions
        var sortedAngles = angles
            .Select((x, i) => new KeyValuePair<double, int>(x, i))
            .OrderBy(x => x.Key)
            .ToList();
        List<int> sortedIndices = sortedAngles.Select(x => x.Value).ToList();

        // Populate the sorted list of vertices
        List<Vector3> newList = new List<Vector3>();
        foreach (var i in sortedIndices) {
            newList.Add(verts[2*i]);
            newList.Add(verts[2*i+1]);
            Debug.DrawRay(verts[2*i], verts[2*i + 1] - verts[2*i], DebugColor(i));
        }

        return newList;
    }

    //Generates a list of integers following the following pattern:
    // 0, 1, 2,   2, 1, 3,   2, 3, 4,   4, 3, 5   ...
    // This is necessary to get the tri normals all facing the right direction
    int[] GenerateTriangles(int listLength) {
        int[] anticlockwise = new int[] { 1, 0, 2 };
        int[] tris = new int[listLength * 3];
        for (var i = 0; i < listLength; i++) {
            for (var j = 0; j < 3; j++) {
                var adder = 0;
                if (i % 2 == 0) {
                    adder = j;
                } else {
                    adder = anticlockwise[j];
                }
                tris[3 * i + j] = (i + adder) % listLength;
            }
        }
        return tris;
    }



    void PrintHitPoints(IEnumerable<RaycastHit> list) {
        string sconstructor = "";
        foreach (var i in list) {
            sconstructor = sconstructor + i.point;
        }

        Debug.Log(sconstructor);
    }

    void PrintVectorList(IEnumerable<Vector3> list) {
        string sconstructor = "";
        foreach (var i in list) {
            sconstructor = sconstructor + i;
        }

        Debug.Log(sconstructor);
    }

    Color DebugColor(int i) {
    	List<Color> debugColors = new List<Color>();
        debugColors.Add(Color.red);
        debugColors.Add(Color.yellow);
        debugColors.Add(Color.green);
        debugColors.Add(Color.cyan);
        debugColors.Add(Color.blue);
        debugColors.Add(Color.magenta);
        debugColors.Add(Color.white);
        debugColors.Add(Color.black);

	    return debugColors[i%debugColors.Count];
    }

    

}
