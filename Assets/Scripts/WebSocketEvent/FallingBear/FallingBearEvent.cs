using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class FallingBearEvent : MonoBehaviour
    {
        FallingBearEffectPool _effectPool;
        AdventurerSpawner _spawner;

        void Awake()
        {
            _effectPool = GetComponent<FallingBearEffectPool>();
            _spawner = AdventurerSpawner.Find();
        }

        public void Execute()
        {
            if (_spawner.Spawned.Count == 0) return;

            // �܂��_���W�������ɂ���`���҂݂̂�_���B
            Adventurer[] alive = _spawner.Spawned.Where(a => !a.IsCompleted).ToArray();
            if (alive.Length == 0) return;

            // �����_���Ȗ`���҂ɑ΂��A�_�����Ƃ��o����Z���B
            int random = Random.Range(0, alive.Length);
            Adventurer target = alive[random];
            Cell placeCell = DungeonManager.GetCell(target.Coords);

            if (_effectPool.TryPop(out FallingBearEffect effect))
            {
                // �ڕW�̓���ɐ�������B�����͓K���B
                effect.Play(placeCell.Position + Vector3.up * 3.0f, target);
            }
        }
    }
}
