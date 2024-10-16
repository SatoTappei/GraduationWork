using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Wall : Environment
    {
        [SerializeField] GameObject _upWall;
        [SerializeField] GameObject _downWall;
        [SerializeField] GameObject _leftWall;
        [SerializeField] GameObject _rightWall;

        protected override void OnInitialized(string[] blueprint, int x, int y)
        {
            DisableHiddenMesh(blueprint, x, y);
        }

        // 上下左右それぞれ隣が壁の場合は、面がカメラから見えないのでメッシュを非表示にしておく。
        void DisableHiddenMesh(string[] blueprint, int x, int y)
        {
            char tile = blueprint[y][x];
            int h = Blueprint.Height;
            int w = Blueprint.Width;
            int d = y - 1;
            int u = y + 1;
            int r = x + 1;
            int l = x - 1;

            if (_upWall != null && u < h && blueprint[u][x] == tile) _upWall.SetActive(false);
            if (_downWall != null && 0 <= d && blueprint[d][x] == tile) _downWall.SetActive(false);
            if (_leftWall != null && r < w && blueprint[y][r] == tile) _rightWall.SetActive(false);
            if (_rightWall != null && 0 <= l && blueprint[y][l] == tile) _leftWall.SetActive(false);
        }
    }
}