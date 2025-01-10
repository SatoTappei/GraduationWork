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
            // ���O,�`������,�T�u�S�[��,�T�u�S�[���̌���(�B��/���߂�)�B
            string[] result = new string[4];

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

            // �T�u�S�[���B�u�_���W�����̓����ɖ߂�B�v�ȊO�Ń����_����1�I�ԁB
            TryGetComponent(out SubGoalPath path);
            if (path.Path == null)
            {
                Debug.LogWarning("�T�u�S�[����null�Ȃ̂ŋ󗓂̂܂ܑ��M�B");
            }
            else if (path.Path.Count == 0)
            {
                Debug.LogWarning("�T�u�S�[����1���Ȃ��̂ŋ󗓂̂܂ܑ��M�B");
            }
            else
            {
                string[] subGoals = path.Path
                    .Select(a => a.Description.Japanese)
                    .Where(s => s != "�_���W�����̓����ɖ߂�B")
                    .ToArray();

                result[2] = subGoals[Random.Range(0, subGoals.Length)];
            }

            if (path.Path[0].Check() == SubGoal.State.Completed)
            {
                result[3] = "Completed";
            }
            else if (path.Path[0].Check() == SubGoal.State.Failed)
            {
                result[3] = "Failed";
            }
            else
            {
                Debug.LogWarning($"�T�u�S�[�����I���Ă��Ȃ��B:{path.Path[0].Check()}");
                result[3] = "Failed";
            }

            // ���M�B
            string csv = string.Join(",", result);
            Debug.Log(csv);
            GameManager.ReportAdventureResult(adventurer, csv);
        }
    }
}
