using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    // GAS���Ńl�X�g���ꂽJSON�̃p�[�X����肭�����Ȃ��̂ŁACSV�`���ő��M����B
    public class AdventureResultApply : MonoBehaviour
    {
        public void Send()
        {
            // ���O,�`������,�T�u�S�[���ő�3��,�A�C�e���ő�3�B
            string[] result = new string[8];

            // ���O�B
            TryGetComponent(out Adventurer adventurer);
            result[0] = adventurer.AdventurerSheet.FullName;

            // �`�����ʁA�E�o�����ꍇ�͈��m���ň��ށA���j���ꂽ�ꍇ�͈��m���Ŏ��S�B
            TryGetComponent(out Blackboard blackboard);
            if (blackboard.CurrentHp > 0)
            {
                if (Random.value <= 1.0f) result[1] = "Escape";
                else result[1] = "Retire";
            }
            else
            {
                if (Random.value <= 1.0f) result[1] = "Defeated";
                else result[1] = "Rescue";
            }

            // �T�u�S�[���B
            if (TryGetComponent(out SubGoalPath path) && path.Path != null && path.Path.Count > 0)
            {
                string[] subGoals = path.Path.Select(a => a.Text.Japanese).ToArray();
                for (int i = 0; i < subGoals.Length; i++)
                {
                    result[2 + i] = subGoals[i];
                }
            }

            // �A�C�e���B
            if (TryGetComponent(out ItemInventory item))
            {
                string[] items = item.GetEntries().Select(e => e.Name).ToArray();
                for (int i = 0; i < items.Length; i++)
                {
                    result[5 + i] = items[i];
                }
            }

            // ���M�B
            string csv = string.Join(",", result);
            GameManager.ReportAdventureResult(adventurer, csv);
        }
    }
}
