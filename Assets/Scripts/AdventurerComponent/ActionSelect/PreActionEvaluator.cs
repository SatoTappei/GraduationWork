using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Game.ItemData;
using System;

namespace Game
{
    public class PreActionEvaluator : MonoBehaviour
    {
        struct State
        {
            public bool Adventurer;
            public bool Enemy;
            public bool Scavengeable;
            public bool Flaming;
            public bool Impassable;
        }

        Adventurer _adventurer;
        SubGoalPath _subGoalPath;
        AvailableActions _actions;
        MadnessStatusEffect _madness;
        ItemInventory _item;

        Artifact _artifact;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _subGoalPath = GetComponent<SubGoalPath>();
            _actions = GetComponent<AvailableActions>();
            _madness = GetComponent<MadnessStatusEffect>();
            _item = GetComponent<ItemInventory>();
        }

        void Start()
        {
            // アーティファクトの生成座標を手動で指定。
            _artifact = DungeonManager
                .GetActors(new Vector2Int(17, 21))
                .Select(a => a as Artifact)
                .FirstOrDefault();

            if (_artifact == null)
            {
                Debug.LogWarning("アーティファクトが見つからない。");
            }
        }

        public void Evaluate()
        {
            State north = CheckCell(_adventurer.Coords + Vector2Int.up);
            State south = CheckCell(_adventurer.Coords + Vector2Int.down);
            State east = CheckCell(_adventurer.Coords + Vector2Int.right);
            State west = CheckCell(_adventurer.Coords + Vector2Int.left);

            // 以下の理由により、0より大きい値を設定しておく。
            // 床が燃え上がっている場合など、移動は出来るがしない方が良い場合があるため。
            // 箱や樽が空っぽの状態で漁らずに移動するよう促すため。
            _actions.SetScore("MoveNorth", 0.1f);
            _actions.SetScore("MoveSouth", 0.1f);
            _actions.SetScore("MoveEast", 0.1f);
            _actions.SetScore("MoveWest", 0.1f);

            // 燃え上がっているタイルの場合はスコアを0にし、その方向に移動しないように促す。
            if (north.Flaming) _actions.SetScore("MoveNorth", 0);
            if (south.Flaming) _actions.SetScore("MoveSouth", 0);
            if (east.Flaming) _actions.SetScore("MoveEast", 0);
            if (west.Flaming) _actions.SetScore("MoveWest", 0);

            // それぞれの方向に冒険者がいる場合、その方向に移動できない。
            if (north.Adventurer) _actions.SetScore("MoveNorth", -1.0f);
            if (south.Adventurer) _actions.SetScore("MoveSouth", -1.0f);
            if (east.Adventurer) _actions.SetScore("MoveEast", -1.0f);
            if (west.Adventurer) _actions.SetScore("MoveWest", -1.0f);

            // 壁がある歩行には移動できない。
            if (north.Impassable) _actions.SetScore("MoveNorth", -1.0f);
            if (south.Impassable) _actions.SetScore("MoveSouth", -1.0f);
            if (east.Impassable) _actions.SetScore("MoveEast", -1.0f);
            if (west.Impassable) _actions.SetScore("MoveWest", -1.0f);

            // スコアが0より大きい方向へは何の障害も無く移動できる。
            // その中から探索回数が少ない方向に移動するよう促す。
            // 対象がいるのにスルーしないよう、攻撃や漁るなどの行動よりかは低い値に設定しておく。
            List<(string, int)> explore = new List<(string, int)>(4);
            if (_actions.GetScore("MoveNorth") > 0)
            {
                int count = _adventurer.ExploreRecord.Get(_adventurer.Coords + Vector2Int.up);
                explore.Add(("North", count));
            }
            if(_actions.GetScore("MoveSouth") > 0)
            {
                int count = _adventurer.ExploreRecord.Get(_adventurer.Coords + Vector2Int.down);
                explore.Add(("South", count));
            }
            if (_actions.GetScore("MoveEast") > 0)
            {
                int count = _adventurer.ExploreRecord.Get(_adventurer.Coords + Vector2Int.down);
                explore.Add(("East", count));
            }
            if (_actions.GetScore("MoveWest") > 0)
            {
                int count = _adventurer.ExploreRecord.Get(_adventurer.Coords + Vector2Int.down);
                explore.Add(("West", count));
            }
            if (explore.Count > 0)
            {
                string minExplore =
                    explore
                    .OrderBy(x => Guid.NewGuid())
                    .OrderBy(x => x.Item2)
                    .First()
                    .Item1;
                _actions.SetScore($"Move{minExplore}", 0.33f);
            }

            // 「入口に移動する」の選択肢。
            // 現在のサブゴールが「ダンジョンの入口に戻る」の場合、移動よりスコアが高い。
            // それ以外の時は選択肢から外す。
            if (_subGoalPath.GetCurrent() == null)
            {
                _actions.SetScore("MoveToEntrance", -1.0f);
            }
            else if (_subGoalPath.GetCurrent().Description.Japanese == "ダンジョンの入口に戻る")
            {
                _actions.SetScore("MoveToEntrance", 0.5f);
            }
            else
            {
                _actions.SetScore("MoveToEntrance", -1.0f);
            }

            // 「アーティファクトの位置に移動する」の選択肢。
            // アーティファクトが出現している場合は最優先で向かうよう促す。
            if (_artifact.IsEmpty)
            {
                _actions.SetScore("MoveToArtifact", -1.0f);
            }
            else
            {
                // アーティファクトがある座標の1歩手前の座標を目指す。
                if (_adventurer.Coords != new Vector2Int(17, 20))
                {
                    _actions.SetScore("MoveToArtifact", 1.0f);
                }
                else
                {
                    _actions.SetScore("MoveToArtifact", -1.0f);
                }
            }

            // 何れかの方向に敵がいる場合は、周囲の敵に攻撃できる。
            if (north.Enemy || south.Enemy || east.Enemy || west.Enemy)
            {
                _actions.SetScore("AttackToEnemy", 1.0f);
            }
            else
            {
                _actions.SetScore("AttackToEnemy", -1.0f);
            }

            // 何れかの方向に冒険者がいる場合は、周囲の冒険者を攻撃もしくは会話できる。
            if (north.Adventurer || south.Adventurer || east.Adventurer || west.Adventurer)
            {
                // 狂気状態のときは攻撃しかできない。
                if (_madness.IsValid)
                {
                    _actions.SetScore("AttackToAdventurer", 1.0f);
                    _actions.SetScore("TalkWithAdventurer", -1.0f);
                }
                else
                {
                    _actions.SetScore("AttackToAdventurer", 0);
                    _actions.SetScore("TalkWithAdventurer", 0.5f);
                }
            }
            else
            {
                _actions.SetScore("AttackToAdventurer", -1.0f);
                _actions.SetScore("TalkWithAdventurer", -1.0f);
            }

            // 何れかの方向に宝箱やレバーなどインタラクトできる対象がある場合は、周囲を漁ることが出来る。
            if(north.Scavengeable || south.Scavengeable || east.Scavengeable || west.Scavengeable)
            {
                // 狂気状態のときは漁ることが出来ない。
                if (_madness.IsValid)
                {
                    _actions.SetScore("Scavenge", -1.0f);
                }
                else
                {
                    _actions.SetScore("Scavenge", 0.5f);
                }
            }
            else
            {
                _actions.SetScore("Scavenge", -1.0f);
            }

            // 同じセルを何回も通った場合、周囲をうろうろしている可能性がある。
            // 回数が多いほど助け(AIの初期化)を求めるよう強く促す。
            int history = _adventurer.ExploreRecord.Get(_adventurer.Coords);
            if (history >= 5)
            {
                _actions.SetScore("RequestHelp", history / 10.0f);
            }
            else
            {
                _actions.SetScore("RequestHelp", -1.0f);
            }

            _actions.SetScore("ThrowItem", -1.0f);

            // 投げることが出来るアイテムを持っている場合のみ「アイテムを投げる」選択肢を選ぶことが出来る。
            foreach (IReadOnlyList<Item> items in _item.Get().Values)
            {
                if (items[0].Usage == Usage.Throw)
                {
                    // 優先度は最低で良い？
                    _actions.SetScore("ThrowItem", 0);
                    break;
                }
            }
        }

        static State CheckCell(Vector2Int coords)
        {
            State state = new State();
            foreach (Actor a in DungeonManager.GetActors(coords))
            {
                if (a is Adventurer) state.Adventurer = true;
                if (a is Enemy) state.Enemy = true;
                if (a is IScavengeable) state.Scavengeable = true;
            }

            state.Flaming = DungeonManager.GetCell(coords).TerrainEffect == TerrainEffect.Flaming;
            state.Impassable = DungeonManager.GetCell(coords).IsImpassable();

            return state;
        }
    }
}
