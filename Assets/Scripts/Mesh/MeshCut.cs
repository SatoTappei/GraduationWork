using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    // 作りかけ。断面となる新たな頂点を作るところまで出来た。
    // 首から下のメッシュをカットするだけならば、それより下の頂点を上にずらすだけで十分なので没。
    public class MeshCut : MonoBehaviour
    {
        class GizmosDrawer
        {
            Vector3 _a , _b, _c;

            public GizmosDrawer(Vector3 a, Vector3 b, Vector3 c)
            {
                _a = a;
                _b = b;
                _c = c;
            }

            public void Draw()
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(_a, 0.033f);
                Gizmos.DrawWireSphere(_b, 0.033f);
                Gizmos.DrawWireSphere(_c, 0.033f);
                Gizmos.DrawLine(_a, _b);
                Gizmos.DrawLine(_b, _c);
                Gizmos.DrawLine(_c, _a);
            }
        }

        [SerializeField] float _height;
        [SerializeField] Transform _fbx;
        [SerializeField] SkinnedMeshRenderer _renderer;
        [SerializeField] bool _drawCrossTriangles;
        [SerializeField] bool _drawTriangles;
        [SerializeField] bool _drawRectangles;
        [SerializeField] bool _drawUpperMeshes;

        GizmosDrawer _gizmos;
        Plane _plane;

        List<int> _rightSideTriangles;
        List<int> _leftSideTriangles;
        List<int> _crossTriangles;
        List<Vector3> _crossPoints;
        List<Vector3> _triangles;
        List<Vector3> _rectangles;
        List<Vector3[]> _upperMeshes;

        void Start()
        {
            Vector3 a = transform.position + Vector3.forward;
            Vector3 b = transform.position + Vector3.right;
            Vector3 c = transform.position + Vector3.left;
            a.y = _height;
            b.y = _height;
            c.y = _height;
            _plane = new Plane(a, b, c);
            _gizmos = new GizmosDrawer(a, b, c);

            _rightSideTriangles = new List<int>();
            _leftSideTriangles = new List<int>();
            _crossTriangles = new List<int>();

            for (int i = 0; i < _renderer.sharedMesh.triangles.Length; i += 3)
            {
                int ia = _renderer.sharedMesh.triangles[i];
                int ib = _renderer.sharedMesh.triangles[i + 1];
                int ic = _renderer.sharedMesh.triangles[i + 2];
                Vector3 va = _renderer.sharedMesh.vertices[ia];
                Vector3 vb = _renderer.sharedMesh.vertices[ib];
                Vector3 vc = _renderer.sharedMesh.vertices[ic];

                bool sa = _plane.GetSide(va);
                bool sb = _plane.GetSide(vb);
                bool sc = _plane.GetSide(vc);

                if (sa && sb && sc)
                {
                    _rightSideTriangles.Add(ia);
                    _rightSideTriangles.Add(ib);
                    _rightSideTriangles.Add(ic);
                }
                else if (!sa && !sb && !sc)
                {
                    _leftSideTriangles.Add(ia);
                    _leftSideTriangles.Add(ib);
                    _leftSideTriangles.Add(ic);
                }
                else
                {
                    _crossTriangles.Add(ia);
                    _crossTriangles.Add(ib);
                    _crossTriangles.Add(ic);
                }
            }

            _crossPoints = new List<Vector3>();
            _triangles = new List<Vector3>();
            _rectangles = new List<Vector3>();
            _upperMeshes = new List<Vector3[]>();

            for (int i = 0; i < _crossTriangles.Count; i += 3)
            {
                Vector3 va = _renderer.sharedMesh.vertices[_crossTriangles[i]];
                Vector3 vb = _renderer.sharedMesh.vertices[_crossTriangles[i + 1]];
                Vector3 vc = _renderer.sharedMesh.vertices[_crossTriangles[i + 2]];
                float distAB = Vector3.Distance(va, vb);
                float distBC = Vector3.Distance(vb, vc);
                float distCA = Vector3.Distance(vc, va);

                List<Vector3> triangle = new List<Vector3>();
                List<Vector3> rectangle = new List<Vector3>();

                Ray rayAB = new Ray(va, (vb - va).normalized);
                Ray rayAC = new Ray(va, (vc - va).normalized);
                bool checkAB = _plane.Raycast(rayAB, out float hitAB) && hitAB <= distAB;
                bool checkAC = _plane.Raycast(rayAC, out float hitAC) && hitAC <= distCA;

                if (checkAB && checkAC)
                {
                    triangle.Add(va);
                    triangle.Add(rayAB.GetPoint(hitAB));
                    triangle.Add(rayAC.GetPoint(hitAC));

                    rectangle.Add(rayAB.GetPoint(hitAB));
                    rectangle.Add(vb);
                    rectangle.Add(vc);
                    rectangle.Add(rayAC.GetPoint(hitAC));

                    _triangles.AddRange(triangle);
                    _rectangles.AddRange(rectangle);

                    AddHigherShape(triangle, rectangle);

                    continue;
                }

                Ray rayBA = new Ray(vb, (va - vb).normalized);
                Ray rayBC = new Ray(vb, (vc - vb).normalized);
                bool checkBA = _plane.Raycast(rayBA, out float hitBA) && hitBA <= distAB;
                bool checkBC = _plane.Raycast(rayBC, out float hitBC) && hitBC <= distBC;

                if (checkBA && checkBC)
                {
                    triangle.Add(vb);
                    triangle.Add(rayBA.GetPoint(hitBA));
                    triangle.Add(rayBC.GetPoint(hitBC));

                    rectangle.Add(rayBA.GetPoint(hitBA));
                    rectangle.Add(rayBC.GetPoint(hitBC));
                    rectangle.Add(vc);
                    rectangle.Add(va);

                    _triangles.AddRange(triangle);
                    _rectangles.AddRange(rectangle);

                    AddHigherShape(triangle, rectangle);

                    continue;
                }

                Ray rayCA = new Ray(vc, (va - vc).normalized);
                Ray rayCB = new Ray(vc, (vb - vc).normalized);
                bool checkCA = _plane.Raycast(rayCA, out float hitCA) && hitCA <= distCA;
                bool checkCB = _plane.Raycast(rayCB, out float hitCB) && hitCB <= distBC;

                if (checkCA && checkCB)
                {
                    triangle.Add(vc);
                    triangle.Add(rayCA.GetPoint(hitCA));
                    triangle.Add(rayCB.GetPoint(hitCB));

                    rectangle.Add(rayCA.GetPoint(hitCA));
                    rectangle.Add(va);
                    rectangle.Add(vb);
                    rectangle.Add(rayCB.GetPoint(hitCB));

                    _triangles.AddRange(triangle);
                    _rectangles.AddRange(rectangle);

                    AddHigherShape(triangle, rectangle);

                    continue;
                }
            }
        }

        void AddHigherShape(IReadOnlyList<Vector3> triangle, IReadOnlyList<Vector3> rectangle)
        {
            float highest = 0;
            string shape = string.Empty;
            foreach (Vector3 vertex in triangle)
            {
                if (vertex.y >= highest)
                {
                    highest = vertex.y;
                    shape = "Triangle";
                }
            }
            foreach (Vector3 vertex in rectangle)
            {
                if (vertex.y >= highest)
                {
                    highest = vertex.y;
                    shape = "Rectangle";
                }
            }

            if (shape == "Triangle")
            {
                Vector3[] array = new Vector3[3];
                array[0] = triangle[0];
                array[1] = triangle[1];
                array[2] = triangle[2];
                _upperMeshes.Add(array);
            }
            else
            {
                Vector3[] array = new Vector3[4];
                array[0] = rectangle[0];
                array[1] = rectangle[1];
                array[2] = rectangle[2];
                array[3] = rectangle[3];
                _upperMeshes.Add(array);
            }
        }

        void OnDrawGizmosSelected()
        {
            if (_gizmos != null) _gizmos.Draw();

            if (_drawCrossTriangles) DrawCrossTriangles();
            if (_drawTriangles) DrawTriangles();
            if (_drawRectangles) DrawRectangles();
            if (_drawUpperMeshes) DrawUpperMeshes();
        }

        void DrawRightSideTriangles()
        {
            if (_rightSideTriangles == null) return;

            Gizmos.color = Color.cyan;
            for (int i = 0; i < _rightSideTriangles.Count; i += 3)
            {
                Vector3 va = _renderer.sharedMesh.vertices[_rightSideTriangles[i]];
                Vector3 vb = _renderer.sharedMesh.vertices[_rightSideTriangles[i + 1]];
                Vector3 vc = _renderer.sharedMesh.vertices[_rightSideTriangles[i + 2]];
                Gizmos.DrawLine(va, vb);
                Gizmos.DrawLine(vb, vc);
                Gizmos.DrawLine(vc, va);
            }
        }

        void DrawLeftSideTriangles()
        {
            if (_leftSideTriangles == null) return;

            Gizmos.color = Color.magenta;
            for (int i = 0; i < _leftSideTriangles.Count; i += 3)
            {
                Vector3 va = _renderer.sharedMesh.vertices[_leftSideTriangles[i]];
                Vector3 vb = _renderer.sharedMesh.vertices[_leftSideTriangles[i + 1]];
                Vector3 vc = _renderer.sharedMesh.vertices[_leftSideTriangles[i + 2]];
                Gizmos.DrawLine(va, vb);
                Gizmos.DrawLine(vb, vc);
                Gizmos.DrawLine(vc, va);
            }
        }

        void DrawCrossTriangles()
        {
            if (_crossTriangles == null) return;

            Gizmos.color = Color.yellow;
            for (int i = 0; i < _crossTriangles.Count; i += 3)
            {
                Vector3 va = _renderer.sharedMesh.vertices[_crossTriangles[i]];
                Vector3 vb = _renderer.sharedMesh.vertices[_crossTriangles[i + 1]];
                Vector3 vc = _renderer.sharedMesh.vertices[_crossTriangles[i + 2]];
                Gizmos.DrawLine(va, vb);
                Gizmos.DrawLine(vb, vc);
                Gizmos.DrawLine(vc, va);
            }
        }

        void DrawCrossPoints()
        {
            if (_crossPoints == null) return;

            Gizmos.color = Color.red;
            foreach (Vector3 p in _crossPoints)
            {
                Gizmos.DrawWireSphere(p, 0.001f);
            }
        }

        void DrawTriangles()
        {
            if (_triangles == null) return;

            Gizmos.color = Color.magenta;
            for (int i = 0; i < _triangles.Count; i += 3)
            {
                Gizmos.DrawLine(_triangles[i], _triangles[i + 1]);
                Gizmos.DrawLine(_triangles[i + 1], _triangles[i + 2]);
                Gizmos.DrawLine(_triangles[i + 2], _triangles[i]);
            }
        }

        void DrawRectangles()
        {
            if (_rectangles == null) return;

            Gizmos.color = Color.cyan;
            for (int i = 0; i < _rectangles.Count; i += 4)
            {
                Gizmos.DrawLine(_rectangles[i], _rectangles[i + 1]);
                Gizmos.DrawLine(_rectangles[i + 1], _rectangles[i + 2]);
                Gizmos.DrawLine(_rectangles[i + 2], _rectangles[i + 3]);
                Gizmos.DrawLine(_rectangles[i + 3], _rectangles[i]);
            }
        }

        void DrawUpperMeshes()
        {
            if (_upperMeshes == null) return;

            Gizmos.color = Color.green;
            for (int i = 0; i < _upperMeshes.Count; i++)
            {
                for (int k = 0; k < _upperMeshes[i].Length; k++)
                {
                    Gizmos.DrawLine(_upperMeshes[i][k], _upperMeshes[i][(k + 1) % _upperMeshes[i].Length]);
                }
            }
        }
    }
}