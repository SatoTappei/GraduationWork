using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class HelicopterCrashEvent : MonoBehaviour
    {
        HelicopterCrashEffectPool _effectPool;
        AdventurerSpawner _spawner;

        void Awake()
        {
            _effectPool = GetComponent<HelicopterCrashEffectPool>();
            _spawner = AdventurerSpawner.Find();
        }

        public void Execute()
        {
            if (_spawner.Spawned.Count == 0) return;

            // �����_���Ȗ`���҂ɑ΂��A�_�����Ƃ��o������W�̃Z���B
            int random = Random.Range(0, _spawner.Spawned.Count);
            Adventurer target = _spawner.Spawned[random];
            Cell placeCell = DungeonManager.GetCell(target.Coords);

            if (_effectPool.TryPop(out HelicopterCrashEffect effect))
            {
                // �ڕW�̓���ɐ�������B�����͓K���B
                effect.Play(placeCell.Position + Vector3.up * 3.0f, target);
            }
        }
    }
}
