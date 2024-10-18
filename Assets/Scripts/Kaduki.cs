using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class Kaduki : Adventure, IStatusBarDisplayStatus
    {
        [SerializeField] Vector2Int _spawnCoords;
        //[SerializeField] Transform _seeker;

        Vector2Int _currentCoords;
        Vector2Int _currentDirection;
        List<Cell> _path;
        int _statusID;

        public override Vector2Int Coords => _currentCoords;
        public override Vector2Int Direction => _currentDirection;

        Sprite IStatusBarDisplayStatus.Icon => null;
        string IStatusBarDisplayStatus.DisplayName => "Kaduki";
        int IStatusBarDisplayStatus.MaxHp => 100;
        int IStatusBarDisplayStatus.CurrentHp => 100;
        int IStatusBarDisplayStatus.MaxEmotion => 100;
        int IStatusBarDisplayStatus.CurrentEmotion => 100;

        void Start()
        {
            _currentCoords = _spawnCoords;
            _path = new List<Cell>();

            DungeonManager dm = DungeonManager.Find();
            Cell cell = dm.GetCell(_currentCoords);
            transform.position = cell.Position;

            UiManager ui = UiManager.Find();
            _statusID = ui.RegisterToStatusBar(this);
            ui.ShowLine(_statusID, "こんにちわ");
            ui.AddLog("Kadukiがダンジョンにやてきた。");

            dm.AddActorOnCell(_currentCoords, this);
            // 本来ならここで、初期Directionのセットが必要。

            StartCoroutine(ActionAsync());
        }

        void OnDestroy()
        {
            UiManager ui = UiManager.Find();
            if (ui != null) ui.DeleteStatusBarStatus(_statusID);
        }

        IEnumerator ActionAsync()
        {
            // ランダムが怪しいので自分で指定する。
            //const int Target = 2;

            DungeonManager dm = DungeonManager.Find();
            IEnumerable<Cell> treasures = dm.GetCells("Treasure");

            int Target = Random.Range(0, treasures.Count());

            Cell targetTreasureCell = null;
            int i = 0;
            foreach (Cell c in treasures)
            {
                if (i == Target)
                {
                    foreach(Actor actor in c.GetActors())
                    {
                        // 宝箱のマスへは経路探索が出来ないので、正面の位置までの経路探索。
                        if (actor.ID == "Treasure")
                        {
                            targetTreasureCell = c;

                            Vector2Int cds = c.Coords;
                            if (actor.Direction == Vector2Int.up) cds += Vector2Int.up;
                            else if (actor.Direction == Vector2Int.down) cds += Vector2Int.down;
                            else if (actor.Direction == Vector2Int.left) cds += Vector2Int.left;
                            else if (actor.Direction == Vector2Int.right) cds += Vector2Int.right;
                            dm.Pathfinding(_currentCoords, cds, _path);
                            break;
                        }
                    }
                }

                i++;
            }

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

                // 目の前のセルの文字を確認してドアかチェックして開ける。
                if ("2468".Contains(Blueprint.Doors[front.y][front.x]))
                {
                    Actor actor = dm.GetActorsOnCell(front).Where(c => c.ID == "Door").FirstOrDefault();
                    if (actor != null && actor is DungeonEntity door)
                    {
                        door.Interact(actor);
                    }
                }
                //Debug.Log("目の前のセルは" + Blueprint.Doors[front.y][front.x]);

                // 移動先に他のKadukiがいるかチェック
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
                    // 自身の向いている方向マーカー
                    //_seeker.position = transform.position + new Vector3(_currentDirection.x, 0, _currentDirection.y);

                    Quaternion look = Quaternion.Lerp(fwd, rot, t * 4);
                    forwardAxis.rotation = look;
                    transform.position = Vector3.Lerp(start, goal, t);

                    yield return null;
                }
                
                start = goal;
            }

            {
                // 目標が2マス以上先にいる状況は想定していない。(以下の式でDirectionの長さが2以上になってしまう。)
                Vector2Int newDirection = targetTreasureCell.Coords - _currentCoords;
                _currentDirection = newDirection;

                // 目標地点に到着後、目標がいる方向に向く処理が必要。
                Vector3 targetDirection = targetTreasureCell.Position - transform.position;
                Quaternion fwd = forwardAxis.rotation;
                Quaternion rot = Quaternion.LookRotation(targetDirection);
                for (float t = 0; t <= 1; t += Time.deltaTime * 1.0f)
                {
                    // 自身の向いている方向マーカー
                    //_seeker.position = transform.position + new Vector3(_currentDirection.x, 0, _currentDirection.y);

                    Quaternion look = Quaternion.Lerp(fwd, rot, t * 4);
                    forwardAxis.rotation = look;

                    yield return null;
                }
            }

            // 宝箱に到達したが、既に先駆者に宝箱を取られているかもしれない可能性がある。
            {
                Actor actor = 
                    dm.GetActorsOnCell(_currentCoords + _currentDirection)
                    .Where(c => c.ID == "Treasure")
                    .FirstOrDefault();

                if (actor != null && actor is DungeonEntity e)
                {
                    e.Interact(this);

                    animator.Play("Jump");
                }
            }

            // 喜びの時間
            yield return new WaitForSeconds(1.5f);

            // 帰宅
            {
                Cell entranceCell = dm.GetCells("Entrance").FirstOrDefault();
                dm.Pathfinding(_currentCoords, entranceCell.Coords, _path);

                animator.Play("Walk");

                Vector3 startPos = transform.position;
                for (int k = 0; k < _path.Count; k++)
                {
                    // 自身の向いている方向更新
                    _currentDirection = _path[k].Coords - Coords;
                    // 正面のセルの座標
                    Vector2Int front = _currentCoords + _currentDirection;

                    // 目の前のセルの文字を確認してドアかチェックして開ける。
                    if ("2468".Contains(Blueprint.Doors[front.y][front.x]))
                    {
                        Actor actor = dm.GetActorsOnCell(front).Where(c => c.ID == "Door").FirstOrDefault();
                        if (actor != null && actor is DungeonEntity door)
                        {
                            door.Interact(actor);
                        }
                    }

                    Vector3 goal = _path[k].Position;
                    Quaternion fwd = forwardAxis.rotation;
                    Vector3 dir = (goal - start).normalized;
                    Quaternion rot = Quaternion.LookRotation(dir);
                    for (float t = 0; t <= 1; t += Time.deltaTime * 1.0f)
                    {
                        // 自身の向いている方向マーカー
                        //_seeker.position = transform.position + new Vector3(_currentDirection.x, 0, _currentDirection.y);

                        Quaternion look = Quaternion.Lerp(fwd, rot, t * 4);
                        forwardAxis.rotation = look;
                        transform.position = Vector3.Lerp(start, goal, t);

                        yield return null;
                    }

                    start = goal;

                    dm.RemoveActorOnCell(_currentCoords, this);
                    Vector2Int newCoords = _path[k].Coords;
                    _currentCoords = newCoords;
                    dm.AddActorOnCell(_currentCoords, this);
                }

                dm.RemoveActorOnCell(_currentCoords, this);
                Destroy(gameObject);
            }
        }

        void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            //DungeonManager dm = DungeonManager.Find();
            //for (int i = 0; i < Blueprint.Height; i++)
            //{
            //    for (int k = 0; k < Blueprint.Width; k++)
            //    {
            //        var actors = dm.GetActorsOnCell(new Vector2Int(k, i));
            //        if (actors.Count > 0)
            //        {
            //            if (actors.Where(a => a.gameObject.name == "DungeonEntity_Door(Clone)").ToList().Count > 0)
            //            {
            //                Gizmos.color = Color.cyan;
            //                Cell c = dm.GetCell(new Vector2Int(k, i));
            //                Gizmos.DrawCube(c.Position, new Vector3(0.5f, 10, 0.5f));
            //            }
            //        }
            //    }
            //}
        }
    }
}