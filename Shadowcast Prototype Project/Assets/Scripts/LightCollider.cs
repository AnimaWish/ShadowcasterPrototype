using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightCollider : MonoBehaviour
{

    public GameObject shadow;
    public bool castsShadow;

    // Use this for initialization
    void Start()
    {
        shadow = new GameObject(name + "Shadow");
        shadow.AddComponent<MeshFilter>();
        shadow.AddComponent<MeshRenderer>();
        shadow.AddComponent<MeshCollider>();
        shadow.GetComponent<MeshRenderer>().material.color = Color.black;
        shadow.GetComponent<MeshRenderer>().material.shader = Shader.Find("Unlit/Color");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateMesh(Mesh mesh)
    {
        shadow.GetComponent<MeshFilter>().mesh.Clear();
        shadow.GetComponent<MeshFilter>().mesh = mesh;
        shadow.GetComponent<MeshCollider>().sharedMesh = mesh;

    }

    // Returns a list of vertices representing the object in world coordinates
    public List<Vector3> GetWorldVertices()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        List<Vector3> vertices = new List<Vector3>();
        foreach (var v in mesh.vertices)
        {
            vertices.Add(transform.TransformPoint(v));
        }
        return vertices;
    }
}