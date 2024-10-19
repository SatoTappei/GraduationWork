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
            ui.AddLog("BlackKaduki���_���W�����ɏo���B");

            dm.AddActorOnCell(_currentCoords, this);

            // ���������͌��󃂃f���̌����Ƃ��l�����K���Ɏw��B
            _currentDirection = Vector2Int.up;

            StartCoroutine(ActionAsync());
        }

        IEnumerator ActionAsync()
        {
            while (true)
            {
                yield return WalkAsync();
                yield return null; // �������[�v�h�~�B
            }
        }

        IEnumerator WalkAsync()
        {
            // �����n�_�𒆐S�Ƃ���3*3�͈̔͂̂ǂ����̈ʒu�B
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
                // ���g�̌����Ă�������X�V
                _currentDirection = _path[k].Coords - Coords;
                // ���ʂ̃Z���̍��W
                Vector2Int front = _currentCoords + _currentDirection;

                //Debug.Log("�ڂ̑O�̃Z����" + Blueprint.Doors[front.y][front.x]);

                // �ړ����Kaduki�����邩�`�F�b�N
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

        // �`���҂ƃZ�����d�Ȃ�Ȃ��悤�ɂ��Ȃ��Ƃ����Ȃ��B
        //  �L�����N�^�[�͈ړ��O�Ɉړ��悪�\��ς݂��ǂ����`�F�b�N����悤�ȃ��W�b�N�ɂ��邱�Ƃŏd�Ȃ��h���H
        //   ��1�}�X�̒ʘH���ƁA���݂����������ꂸ�ς�ł��܂��B���[�O���C�N�Ȃ炱�̏󋵂��v���C���[���ǂ��炩���E�����ƂőŔj�ł��邪�c
        // �����ʒu���瓮���Ȃ���Ζ��͉������邪�c
        // �����ʒu�𒆐S��3*3�͈͓̔������낤�낷��悤�ɂ��A�`���҂��x���͈͂ɓ������ꍇ�A�����ʒu�ɖ߂��Čx������Ƃ��H
        // �^�[�����ɂ���H
    }
}
