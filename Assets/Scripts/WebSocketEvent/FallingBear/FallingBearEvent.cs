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

            // �����_���Ȗ`���҂ɑ΂��A�_�����Ƃ��o����Z���B
            int random = Random.Range(0, _spawner.Spawned.Count);
            Adventurer target = _spawner.Spawned[random];
            Cell placeCell = DungeonManager.GetCell(target.Coords);

            if (_effectPool.TryPop(out FallingBearEffect effect))
            {
                // �ڕW�̓���ɐ�������B�����͓K���B
                effect.Play(placeCell.Position + Vector3.up * 3.0f, target);
            }

            // �C�x���g���s�����O�ɕ\���B
            GameLog.Add("�V�X�e��", "���҂����`���҂ɂ������炵���B", GameLogColor.Green);
        }
    }
}
