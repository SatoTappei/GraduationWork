using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class MeshTransformer : MonoBehaviour
    {
        [SerializeField] MeshFilter _source;
        [SerializeField] float _height = 0.9f;
        [SerializeField] float _width = 0.3f;

        void Start()
        {
            Mesh mesh = new Mesh();
            mesh.name = "CopiedMesh";

            // ’¸“_‚ğ‚¸‚ç‚µ‚Ä’¼•û‘Ì‚Ì”ÍˆÍ‚Éû‚Ü‚é‚æ‚¤‚É‚·‚éB
            Vector3[] vertices = _source.sharedMesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                if (vertices[i].y <= _height) vertices[i].y = _height;
                if (vertices[i].x >= _width) vertices[i].x = _width;
                if (vertices[i].x <= -_width) vertices[i].x = -_width;
            }

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = _source.sharedMesh.triangles;
            mesh.normals = _source.sharedMesh.normals;
            mesh.uv = _source.sharedMesh.uv;
            mesh.Optimize();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            MeshFilter meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;
        }
    }
}