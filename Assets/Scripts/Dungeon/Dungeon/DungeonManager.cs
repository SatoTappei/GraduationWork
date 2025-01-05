using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DungeonManager : MonoBehaviour
    {
        static DungeonManager _instance;

        Dungeon _dungeon;
        Dictionary<string, List<Vector2Int>> _placed;
        HashSet<int> _placedID;

        void Awake()
        {
            if (_instance == null) _instance = this;
            else Destroy(this);

            _dungeon = GetComponent<Dungeon>();
            _placed = new Dictionary<string, List<Vector2Int>>();
            _placedID = new HashSet<int>();
        }

        void OnDestroy()
        {
            if (_instance = this) _instance = null;
        }

        public static void AddActor(Vector2Int coords, Actor actor)
        {
            GetCell(coords).AddActor(actor);
            AddPlaced(actor);
        }

        public static void RemoveActor(Vector2Int coords, Actor actor)
        {
            GetCell(coords).RemoveActor(actor);
            RemovePlaced(actor);
        }

        public static void SetTerrainEffect(Vector2Int coords, TerrainEffect effect)
        {
            GetCell(coords).TerrainEffect = effect;
        }

        public static void DeleteTerrainEffect(Vector2Int coords)
        {
            GetCell(coords).TerrainEffect = TerrainEffect.None;
        }

        public static void AddAvoidCell(Vector2Int coords)
        {
            GetCell(coords).IsAvoid = true;
        }

        public static void RemoveAvoidCell(Vector2Int coords)
        {
            GetCell(coords).IsAvoid = false;
        }

        public static IReadOnlyList<Actor> GetActors(Vector2Int coords)
        {
            return GetCell(coords).GetActors();
        }

        public static IEnumerable<Cell> GetPlacedCells(string id)
        {
            if (TryGetPlaced(id, out IReadOnlyList<Vector2Int> placed))
            {
                foreach (Vector2Int coords in placed)
                {
                    yield return GetCell(coords);
                }
            }
            else
            {
                Debug.LogWarning($"�z�u���ꂽ�Z���������B: {id}");
                yield break;
            }
        }

        public static Cell GetCell(Vector2Int coords)
        {
            return _instance._dungeon.Grid[coords.y, coords.x];
        }

        static void AddPlaced(Actor actor)
        {
            if (_instance._placedID.Contains(actor.GetInstanceID()))
            {
                Debug.LogWarning($"���ɃZ����ɔz�u�ς݁B:{actor.ID}");
                return;
            }
            else
            {
                _instance._placedID.Add(actor.GetInstanceID());
            }

            if (!_instance._placed.ContainsKey(actor.ID))
            {
                _instance._placed.Add(actor.ID, new List<Vector2Int>());
            }

            // �d����e���Ɠ������W��2�l�ȏ�̖`���҂�����ꍇ�Ƀo�O���N���邩���H
            _instance._placed[actor.ID].Add(actor.Coords);
        }

        static void RemovePlaced(Actor actor)
        {
            if (_instance._placedID.Contains(actor.GetInstanceID()))
            {
                _instance._placedID.Remove(actor.GetInstanceID());
            }
            else
            {
                Debug.LogWarning($"���ɃZ���ォ��폜�ς݁B: {actor.ID}");
                return;
            }

            if (_instance._placed.TryGetValue(actor.ID, out List<Vector2Int> list))
            {
                list.Remove(actor.Coords);
            }
        }

        static bool TryGetPlaced(string id,  out IReadOnlyList<Vector2Int> placed)
        {
            if (_instance._placed.ContainsKey(id))
            {
                placed = _instance._placed[id];
            }
            else
            {
                placed = null;
            }

            return placed != null;
        }
    }
}