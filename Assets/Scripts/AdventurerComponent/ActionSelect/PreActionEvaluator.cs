using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Game.ItemData;

namespace Game
{
    public class PreActionEvaluator : MonoBehaviour
    {
        struct State
        {
            public bool Adventurer;
            public bool Enemy;
            public bool Scavengeable;
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

            // それぞれの方向に冒険者がいる場合、その方向に移動できない。
            // いない場合のスコアは、箱や樽が空っぽの状態で漁らずに移動するよう促すために0より少し高い。
            _actions.SetScore("MoveNorth", north.Adventurer ? -1.0f : 0.1f);
            _actions.SetScore("MoveSouth", south.Adventurer ? -1.0f : 0.1f);
            _actions.SetScore("MoveEast", east.Adventurer ? -1.0f : 0.1f);
            _actions.SetScore("MoveWest", west.Adventurer ? -1.0f : 0.1f);

            // 燃え上がっているタイルの場合はスコアを0にし、その方向に移動しないように促す。
            if (DungeonManager.GetCell(_adventurer.Coords + Vector2Int.up).TerrainEffect == TerrainEffect.Flaming)
            {
                _actions.SetScore("MoveNorth", 0);
            }
            if (DungeonManager.GetCell(_adventurer.Coords + Vector2Int.down).TerrainEffect == TerrainEffect.Flaming)
            {
                _actions.SetScore("MoveSouth", 0);
            }
            if (DungeonManager.GetCell(_adventurer.Coords + Vector2Int.right).TerrainEffect == TerrainEffect.Flaming)
            {
                _actions.SetScore("MoveEast", 0);
            }
            if (DungeonManager.GetCell(_adventurer.Coords + Vector2Int.left).TerrainEffect == TerrainEffect.Flaming)
            {
                _actions.SetScore("MoveWest", 0);
            }

            // 壁がある歩行には移動できない。
            if (DungeonManager.GetCell(_adventurer.Coords + Vector2Int.up).IsImpassable())
            {
                _actions.SetScore("MoveNorth", -1.0f);
            }
            if (DungeonManager.GetCell(_adventurer.Coords + Vector2Int.down).IsImpassable())
            {
                _actions.SetScore("MoveSouth", -1.0f);
            }
            if (DungeonManager.GetCell(_adventurer.Coords + Vector2Int.right).IsImpassable())
            {
                _actions.SetScore("MoveEast", -1.0f);
            }
            if (DungeonManager.GetCell(_adventurer.Coords + Vector2Int.left).IsImpassable())
            {
                _actions.SetScore("MoveWest", -1.0f);
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
                // 大部屋にいる場合、アーティファクトがある座標の1歩手前の座標を目指す。
                bool isBossRoom = Blueprint.Location[_adventurer.Coords.y][_adventurer.Coords.x] == '5';
                bool isAway = _adventurer.Coords != new Vector2Int(17, 20);
                if (isBossRoom && isAway)
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
            int explore = _adventurer.ExploreRecord.Get(_adventurer.Coords);
            if (explore >= 5)
            {
                _actions.SetScore("RequestHelp", explore / 10.0f);
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
            State direction = new State();
            foreach (Actor a in DungeonManager.GetActors(coords))
            {
                if (a is Adventurer) direction.Adventurer = true;
                if (a is Enemy) direction.Enemy = true;
                if (a is IScavengeable) direction.Scavengeable = true;
            }

            return direction;
        }
    }
}
