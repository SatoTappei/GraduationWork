using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    // GAS���Ńl�X�g���ꂽJSON�̃p�[�X����肭�����Ȃ��̂ŁACSV�`���ő��M����B
    public class AdventureResultReporter : MonoBehaviour
    {
        public void Send()
        {
            // ���O,�`������,�T�u�S�[��,�A�C�e���ő�3��,�o������`���ҍő�3�l�B
            string[] result = new string[9];

            // ���O�B
            TryGetComponent(out Adventurer adventurer);
            result[0] = adventurer.AdventurerSheet.FullName;

            // �`�����ʁA�E�o�����ꍇ�͈��m���ň��ށA���j���ꂽ�ꍇ�͈��m���Ŏ��S�B
            if (adventurer.Status.CurrentHp > 0)
            {
                if (Random.value <= 1.0f) result[1] = "Escape";
                else result[1] = "Retire";
            }
            else
            {
                if (Random.value <= 1.0f) result[1] = "Die";
                else result[1] = "Rescue";
            }

            // �T�u�S�[���B"�_���W�����̓����ɖ߂�B"�ȊO�Ń����_����1�I�ԁB
            if (TryGetComponent(out SubGoalPath path) && path.Path != null && path.Path.Count > 0)
            {
                string[] subGoals = path.Path
                    .Select(a => a.Description.Japanese)
                    .Where(s => s != "�_���W�����̓����ɖ߂�B")
                    .ToArray();

                result[2] = subGoals[Random.Range(0, subGoals.Length)];
            }

            // �A�C�e���B���M���鐔��3�܂ŁB
            if (TryGetComponent(out ItemInventory item))
            {
                string[] items = item.GetEntries().Select(e => e.Name).ToArray();
                for (int i = 0; i < Mathf.Min(items.Length, 3); i++)
                {
                    result[3 + i] = items[i];
                }
            }

            // ��b�����`���ҁB���M���鐔��3�l�܂ŁB
            string[] talk = adventurer.Status.TalkRecord.Record.ToArray();
            for (int i = 0; i < Mathf.Min(talk.Length, 3); i++)
            {
                result[6 + i] = talk[i];
            }

            // ���M�B
            string csv = string.Join(",", result);
            GameManager.ReportAdventureResult(adventurer, csv);
        }
    }
}
