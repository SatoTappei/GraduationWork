using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class テスト : MonoBehaviour
{
    TerrainFeature _terrain;
    Vector2Int _coords;

    void Start()
    {
        _coords = new Vector2Int(11, 8);
        _terrain = TerrainFeature.Find();

        _terrain.TryGet(_coords, out _);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _coords += Vector2Int.up;
            _terrain.TryGet(_coords, out _);
        }
    }
}
