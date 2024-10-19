using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class BlackKaduki : Enemy
    {
        Vector2Int _spawnCoords;
        Vector2Int _currentCoords;
        Vector2Int _currentDirection;
        List<Cell> _path;

        public override Vector2Int Coords => _currentCoords;
        public override Vector2Int Direction => _currentDirection;

        public override void Place(Vector2Int coords)
        {
            _spawnCoords = coords;
            _currentCoords = coords;
            _path = new List<Cell>();

            DungeonManager dm = DungeonManager.Find();
            Cell cell = dm.GetCell(_currentCoords);
            transform.position = cell.Position;

            UiManager ui = UiManager.Find();
            ui.AddLog("BlackKadukiがダンジョンに出現。");

            dm.AddActorOnCell(_currentCoords, this);

            // 初期方向は現状モデルの向きとか考えず適当に指定。
            _currentDirection = Vector2Int.up;

            StartCoroutine(ActionAsync());
        }

        IEnumerator ActionAsync()
        {
            while (true)
            {
                yield return WalkAsync();
                yield return null; // 無限ループ防止。
            }
        }

        IEnumerator WalkAsync()
        {
            // 生成地点を中心とした3*3の範囲のどこかの位置。
            List<Vector2Int> choices = GetSpawnCoordsNeighbours().ToList();
            Vector2Int goalCoords = choices[Random.Range(0, choices.Count)];
            DungeonManager dm = DungeonManager.Find();
            dm.Pathfinding(_currentCoords, goalCoords, _path);

            Animator animator = GetComponentInChildren<Animator>();
            animator.Play("Walk");

            Transform forwardAxis = transform.Find("ForwardAxis");

            Vector3 start = transform.position;
            for (int k = 0; k < _path.Count; k++)
            {
                // 自身の向いている方向更新
                _currentDirection = _path[k].Coords - Coords;
                // 正面のセルの座標
                Vector2Int front = _currentCoords + _currentDirection;

                //Debug.Log("目の前のセルは" + Blueprint.Doors[front.y][front.x]);

                // 移動先にKadukiがいるかチェック
                while (true)
                {
                    if (dm.GetActorsOnCell(_path[k].Coords).Where(ca => ca.ID == "Kaduki").Count() == 0) break;

                    yield return null;
                }

                // 移動予定のセルに自身を登録。
                dm.RemoveActorOnCell(_currentCoords, this);
                Vector2Int newCoords = _path[k].Coords;
                _currentCoords = newCoords;
                dm.AddActorOnCell(_currentCoords, this);

                Vector3 goal = _path[k].Position;
                Quaternion fwd = forwardAxis.rotation;
                Vector3 dir = (goal - start).normalized;
                Quaternion rot = Quaternion.LookRotation(dir);
                for (float t = 0; t <= 1; t += Time.deltaTime * 1.0f)
                {
                    Quaternion look = Quaternion.Lerp(fwd, rot, t * 4);
                    forwardAxis.rotation = look;
                    transform.position = Vector3.Lerp(start, goal, t);

                    yield return null;
                }

                start = goal;
            }
        }

        IEnumerable<Vector2Int> GetSpawnCoordsNeighbours()
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    if (i == 0 && k == 0) continue;

                    int nx = _spawnCoords.x + k;
                    int ny = _spawnCoords.y + i;

                    if (Blueprint.Base[ny][nx] == '_')
                    {
                        yield return new Vector2Int(nx, ny);
                    }
                }
            }
        }

        // 冒険者とセルが重ならないようにしないといけない。
        //  キャラクターは移動前に移動先が予約済みかどうかチェックするようなロジックにすることで重なりを防ぐ？
        //   幅1マスの通路だと、お互いがを避けられず積んでしまう。ローグライクならこの状況をプレイヤーがどちらかを殺すことで打破できるが…
        // 生成位置から動かなければ問題は解決するが…
        // 生成位置を中心に3*3の範囲内をうろうろするようにし、冒険者が警戒範囲に入った場合、生成位置に戻って警戒するとか？
        // ターン制にする？
    }
}
