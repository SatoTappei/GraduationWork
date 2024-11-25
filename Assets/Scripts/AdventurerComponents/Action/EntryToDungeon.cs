using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class EntryToDungeon : BaseAction
    {
        // �䎌��\������̂ŁA�X�e�[�^�X�o�[�ɓo�^������ɌĂԑz��B
        public void Entry()
        {
            // �o�ꎞ�̉��o�B
            if (TryGetComponent(out EntryEffect entryEffect)) entryEffect.Play();

            // �o�ꎞ�̑䎌�B
            if (TryGetComponent(out LineApply line)) line.ShowLine(RequestLineType.Entry);

            // �Q�[���i�s���O�ɕ\���B
            UiManager uiManager = UiManager.Find();
            Blackboard blackboard = GetComponent<Blackboard>();
            uiManager.AddLog($"{blackboard.DisplayName}���_���W�����ɂ���Ă����B");
        }
    }
}