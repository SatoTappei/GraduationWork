using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class PathfindingManager : MonoBehaviour
    {
        [SerializeField] int _width = 10;
        [SerializeField] int _height = 8;
        [SerializeField] float _scale = 1;

        Node[,] _grid;

        void Start()
        {
            _grid = new Node[_height, _width];
            for (int i = 0; i < _grid.GetLength(0); i++)
            {
                for (int k = 0; k < _grid.GetLength(1); k++)
                {
                    _grid[i, k] = new Node(transform.position + new Vector3(k, 0, i) * _scale, i, k, 1, Terrain.Floor);
                }
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Node n = _grid[0, 1];
                GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                g.transform.position = n.Position;
            }
        }

        void OnDrawGizmosSelected()
        {
            if (_grid != null)
            {
                foreach (Node n in _grid) n.Draw();
            }
        }
    }
}
