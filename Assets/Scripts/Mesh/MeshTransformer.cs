using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class MeshTransformer : MonoBehaviour
    {
        [SerializeField] MeshFilter _source;

        void Start()
        {
            const float Height = 0.9f;
            const float Width = 0.3f;

            Mesh mesh = new Mesh();
            mesh.name = "CopiedMesh";

            // ˜r‚Æ‹r‚Æ“·‘Ì‚ÌƒƒbƒVƒ…‚ğˆ³k‚·‚éB
            Vector3[] vertices = _source.sharedMesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                if (vertices[i].y <= Height) vertices[i].y = Height;
                if (vertices[i].x >= Width) vertices[i].x = Width;
                if (vertices[i].x <= -Width) vertices[i].x = -Width;
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