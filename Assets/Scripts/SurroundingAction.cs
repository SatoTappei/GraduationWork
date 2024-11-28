using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    // 自身の周囲のセルに存在する対象を取得できる。
    // 対象にインタラクトするキャラクターの各行動クラスはこのクラスを継承する。
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
                    // 上下左右の4方向のみ。
                    if ((i == 0 && k == 0) || Mathf.Abs(i * k) > 0) continue;

                    // 指定した型にキャストできる場合は目標と判定する。
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
