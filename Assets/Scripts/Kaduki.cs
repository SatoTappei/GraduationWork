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
            ui.ShowLine(_statusID, "����ɂ���");
            ui.AddLog("Kaduki���_���W�����ɂ�Ă����B");

            dm.AddActorOnCell(_currentCoords, this);
            // �{���Ȃ炱���ŁA����Direction�̃Z�b�g���K�v�B

            StartCoroutine(ActionAsync());
        }

        void OnDestroy()
        {
            UiManager ui = UiManager.Find();
            if (ui != null) ui.DeleteStatusBarStatus(_statusID);
        }

        IEnumerator ActionAsync()
        {
            // �����_�����������̂Ŏ����Ŏw�肷��B
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
                        // �󔠂̃}�X�ւ͌o�H�T�����o���Ȃ��̂ŁA���ʂ̈ʒu�܂ł̌o�H�T���B
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
                // ���g�̌����Ă�������X�V
                _currentDirection = _path[k].Coords - Coords;
                // ���ʂ̃Z���̍��W
                Vector2Int front = _currentCoords + _currentDirection;

                // �ڂ̑O�̃Z���̕������m�F���ăh�A���`�F�b�N���ĊJ����B
                if ("2468".Contains(Blueprint.Doors[front.y][front.x]))
                {
                    Actor actor = dm.GetActorsOnCell(front).Where(c => c.ID == "Door").FirstOrDefault();
                    if (actor != null && actor is DungeonEntity door)
                    {
                        door.Interact(actor);
                    }
                }
                //Debug.Log("�ڂ̑O�̃Z����" + Blueprint.Doors[front.y][front.x]);

                // �ړ���ɑ���Kaduki�����邩�`�F�b�N
                while (true)
                {
                    if (dm.GetActorsOnCell(_path[k].Coords).Where(ca => ca.ID == "Kaduki").Count() == 0) break;

                    yield return null;
                }

                // �ړ��\��̃Z���Ɏ��g��o�^�B
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
                    // ���g�̌����Ă�������}�[�J�[
                    //_seeker.position = transform.position + new Vector3(_currentDirection.x, 0, _currentDirection.y);

                    Quaternion look = Quaternion.Lerp(fwd, rot, t * 4);
                    forwardAxis.rotation = look;
                    transform.position = Vector3.Lerp(start, goal, t);

                    yield return null;
                }
                
                start = goal;
            }

            {
                // �ڕW��2�}�X�ȏ��ɂ���󋵂͑z�肵�Ă��Ȃ��B(�ȉ��̎���Direction�̒�����2�ȏ�ɂȂ��Ă��܂��B)
                Vector2Int newDirection = targetTreasureCell.Coords - _currentCoords;
                _currentDirection = newDirection;

                // �ڕW�n�_�ɓ�����A�ڕW����������Ɍ����������K�v�B
                Vector3 targetDirection = targetTreasureCell.Position - transform.position;
                Quaternion fwd = forwardAxis.rotation;
                Quaternion rot = Quaternion.LookRotation(targetDirection);
                for (float t = 0; t <= 1; t += Time.deltaTime * 1.0f)
                {
                    // ���g�̌����Ă�������}�[�J�[
                    //_seeker.position = transform.position + new Vector3(_currentDirection.x, 0, _currentDirection.y);

                    Quaternion look = Quaternion.Lerp(fwd, rot, t * 4);
                    forwardAxis.rotation = look;

                    yield return null;
                }
            }

            // �󔠂ɓ��B�������A���ɐ��҂ɕ󔠂�����Ă��邩������Ȃ��\��������B
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

            // ��т̎���
            yield return new WaitForSeconds(1.5f);

            // �A��
            {
                Cell entranceCell = dm.GetCells("Entrance").FirstOrDefault();
                dm.Pathfinding(_currentCoords, entranceCell.Coords, _path);

                animator.Play("Walk");

                Vector3 startPos = transform.position;
                for (int k = 0; k < _path.Count; k++)
                {
                    // ���g�̌����Ă�������X�V
                    _currentDirection = _path[k].Coords - Coords;
                    // ���ʂ̃Z���̍��W
                    Vector2Int front = _currentCoords + _currentDirection;

                    // �ڂ̑O�̃Z���̕������m�F���ăh�A���`�F�b�N���ĊJ����B
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
                        // ���g�̌����Ă�������}�[�J�[
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