using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyMesh : MonoBehaviour
{
    public float intentity = 1f;
    public float mass = 0.5f;
    public float stiffness = 0.08f;
    public float damping = 0.8f;


    private Mesh originalMesh, meshClone;
    private Renderer rend;
    private JellyVertex[] jv;

    Vector3[] vertexArray;

    // Start is called before the first frame update
    void Start()
    {
        originalMesh = GetComponent<MeshFilter>().sharedMesh;
        meshClone = Instantiate(originalMesh);
        GetComponent<MeshFilter>().sharedMesh = meshClone;
        rend = GetComponent<Renderer>();
        jv = new JellyVertex[meshClone.vertices.Length];
        for (int i = 0; i < meshClone.vertices.Length; i++)
        {
            jv[i] = new JellyVertex(i, transform.TransformPoint(meshClone.vertices[i]));
        }
    }

    void FixedUpdate()
    {
        vertexArray = originalMesh.vertices;
        for (int i = 0; i < jv.Length; i++)
        {
            Vector3 target = transform.TransformPoint(vertexArray[jv[i].ID]);
            float intensity = (1 - (rend.bounds.max.y - target.y) / rend.bounds.size.y) * intentity;
            jv[i].Shake(target, mass, stiffness, damping);
            target = transform.InverseTransformPoint(jv[i].position);
            vertexArray[jv[i].ID] = Vector3.Lerp(vertexArray[jv[i].ID], target, intensity);
        }
        meshClone.vertices = vertexArray;
    }

    public class JellyVertex
    {
        public int ID;
        public Vector3 position;
        public Vector3 velocity, force;
        public JellyVertex(int _id, Vector3 _pos)
        {
            ID = _id;
            position = _pos;
        }

        public void Shake(Vector3 target, float _mass, float _stiffness, float _damping)
        {
            force = (target - position) * _stiffness;
            velocity = (velocity + force / _mass) * _damping;
            position += velocity;
            if ((velocity + force + force / _mass).magnitude < 0.0001f)
            {
                position = target;
            }
        }
    }


}
