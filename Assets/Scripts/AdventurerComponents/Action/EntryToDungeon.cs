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
            UiManager.TryFind(out UiManager uiManager);
            Blackboard blackboard = GetComponent<Blackboard>();
            uiManager.AddLog($"�V�X�e��", $"{blackboard.DisplayName}���_���W�����ɂ���Ă����B", GameLogColor.White);
        }
    }
}
