using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PostActionEvaluator : MonoBehaviour
    {
        AvailableActions _actions;

        void Awake()
        {
            _actions = GetComponent<AvailableActions>();
        }

        public void Evaluate(ActionResult result)
        {
            if (result == null)
            {
                Debug.LogWarning("�X�R�A�t���ɕK�v�ȍs�����ʂ̒l��null�ɂȂ��Ă���B");
                return;
            }

            if (result.Action == "Move")
            {
                if (result.Result == "Success")
                {
                    // ���΂炭�ړ������甠��M������s����I������悤�����B
                    float scavengeWeight = _actions.GetWeight("Scavenge");
                    scavengeWeight += 0.1f; // �l�͒����K�v�����B
                    scavengeWeight = Mathf.Clamp01(scavengeWeight);
                    _actions.SetWeight("Scavenge", scavengeWeight);
                }
                else if (result.Result == "Failure")
                {
                    //
                }
                else
                {
                    Debug.LogWarning($"�ړ��̌��ʂɑ΂���X�R�A�t�����o���Ȃ��B{result.Result}");
                }
            }
            else if (result.Action == "Attack")
            {
                if (result.Result == "Defeat")
                {
                    //
                }
                else if (result.Result == "Hit")
                {
                    //
                }
                else if (result.Result == "Corpse")
                {
                    //
                }
                else if (result.Result == "Miss")
                {
                    //
                }
                else
                {
                    Debug.LogWarning($"�U���̌��ʂɑ΂���X�R�A�t�����o���Ȃ��B{result.Result}");
                }
            }
            else if (result.Action == "Talk")
            {
                if (result.Result == "Success")
                {
                    //
                }
                else if (result.Result == "Failure")
                {
                    //
                }
                else
                {
                    Debug.LogWarning($"��b�̌��ʂɑ΂���X�R�A�t�����o���Ȃ��B{result.Result}");
                }
            }
            else if (result.Action == "Scavenge")
            {
                if (result.Result == "Success")
                {
                    //
                }
                else if (result.Result == "Failure")
                {
                    // ���s����x�ɒቺ���邪�A0������邱�Ƃ͖����B
                    // �ڂ̑O�ɔ���M������̂�AI������Ƃ����I��������邱�Ƃ��o���Ȃ��Ȃ�̂�h�����߁B
                    float weight = _actions.GetWeight("Scavenge");
                    weight -= 1.0f; // �l�͒����K�v�����B
                    weight = Mathf.Clamp01(weight);
                    _actions.SetWeight("Scavenge", weight);
                }
                else
                {
                    Debug.LogWarning($"����̌��ʂɑ΂���X�R�A�t�����o���Ȃ��B{result.Result}");
                }
            }
            else if (result.Action == "Help")
            {
                //
            }
            else if (result.Action == "Throw")
            {
                //
            }
            else
            {
                Debug.LogWarning($"�s����̃X�R�A�t�����o���Ȃ��B{result.Action}");
            }
        }
    }
}