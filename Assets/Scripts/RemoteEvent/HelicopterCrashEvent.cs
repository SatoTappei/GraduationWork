using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class HelicopterCrashEvent : MonoBehaviour
    {
        HelicopterCrashEffectPool _effectPool;
        AdventurerSpawner _adventurerSpawner;
        DungeonManager _dungeonManager;
        UiManager _uiManager;

        void Awake()
        {
            _effectPool = GetComponent<HelicopterCrashEffectPool>();
            AdventurerSpawner.TryFind(out _adventurerSpawner);
            DungeonManager.TryFind(out _dungeonManager);
            UiManager.TryFind(out _uiManager);
        }

        public void Execute()
        {
            Adventurer[] adventurers = _adventurerSpawner.Spawned.Where(a => a != null).ToArray();

            if (adventurers.Length == 0) return;

            // �����_���Ȗ`���҂ɑ΂��A�_�����Ƃ��o������W�̃Z���B
            Adventurer target = adventurers[Random.Range(0, adventurers.Length)];
            Cell placeCell = _dungeonManager.GetCell(target.Coords);

            if (_effectPool.TryPop(out HelicopterCrashEffect effect))
            {
                // �ڕW�̓���ɐ�������B�����͓K���B
                effect.Play(placeCell.Position + Vector3.up * 3.0f, target);
            }

            // �C�x���g���s�����O�ɕ\���B
            _uiManager.AddLog("<color=#00ff00>���҂����w���R�v�^�[��v�������B</color>");
        }
    }
}
