using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    // ���g�̎��͂̃Z���ɑ��݂���Ώۂ��擾�ł���B
    // �ΏۂɃC���^���N�g����L�����N�^�[�̊e�s���N���X�͂��̃N���X���p������B
    public class SurroundingAction : BaseAction
    {
        public bool TryGetTarget<T>(out Actor target)
        {
            Actor blackboard = GetComponent<Actor>();
            DungeonManager dungeonManager = DungeonManager.Find();

            for (int i = -1; i <= 1; i++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    // �㉺���E��4�����̂݁B
                    if ((i == 0 && k == 0) || Mathf.Abs(i * k) > 0) continue;

                    // �w�肵���^�ɃL���X�g�ł���ꍇ�͖ڕW�Ɣ��肷��B
                    Vector2Int coords = blackboard.Coords + new Vector2Int(k, i);
                    IReadOnlyList<Actor> actors = dungeonManager.GetCell(coords).GetActors();
                    foreach (Actor actor in actors)
                    {
                        if (actor is T) { target = actor; return true; }
                    }
                }
            }

            target = null;
            return false;
        }
    }
}
