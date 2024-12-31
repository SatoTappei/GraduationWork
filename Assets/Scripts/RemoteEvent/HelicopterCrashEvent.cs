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

        void Awake()
        {
            _effectPool = GetComponent<HelicopterCrashEffectPool>();
            AdventurerSpawner.TryFind(out _adventurerSpawner);
        }

        public void Execute()
        {
            Adventurer[] adventurers = _adventurerSpawner.Spawned.Where(a => a != null).ToArray();

            if (adventurers.Length == 0) return;

            // �����_���Ȗ`���҂ɑ΂��A�_�����Ƃ��o������W�̃Z���B
            Adventurer target = adventurers[Random.Range(0, adventurers.Length)];
            Cell placeCell = DungeonManager.GetCell(target.Coords);

            if (_effectPool.TryPop(out HelicopterCrashEffect effect))
            {
                // �ڕW�̓���ɐ�������B�����͓K���B
                effect.Play(placeCell.Position + Vector3.up * 3.0f, target);
            }

            // �C�x���g���s�����O�ɕ\���B
            GameLog.Add("�V�X�e��", "���҂����w���R�v�^�[��v�������B", GameLogColor.Green);
        }
    }
}
