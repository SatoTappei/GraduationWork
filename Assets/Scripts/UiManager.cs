using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Game
{
    public class UiManager : MonoBehaviour
    {
        BadgeGroup _badgeGroup;

        

        Queue<string> _log;
        StringBuilder _stringBuilder;

        void Awake()
        {
            
        }

        public static UiManager Find()
        {
            return FindAnyObjectByType<UiManager>();
        }



        public void AddLog(string message)
        {
            _log ??= new Queue<string>();
            _stringBuilder ??= new StringBuilder();
            _stringBuilder.Clear();

            _log.Enqueue(message);
            if (_log.Count > 4) _log.Dequeue();

            foreach (string s in _log) _stringBuilder.Append(s);

            //_log
        }

        public int SetStatusToNewBadge(IBadgeDisplayStatus status)
        {
            // BadgeGroup�N���X�ɂ� �؂�� �� �ԋp���� ��Get���\�b�h������B
            // �������A���ꂾ�ƔC�ӂ̒l���X�V�����肷��̂ɂ͎؂��id���擾�B
            // Get��Badge���擾���Ēl���X�V����K�v������B
            // ���܂ł�Badge�̔z���Private�������̂�public�ɂȂ�K�v�����蕡�G�����Ă��܂��B
            // BadgeGroup���Œl�̑���͊������A�O����Badge�N���X�͓n���Ȃ�����Simple�Ȃ̂ł́H
            // ���������X�e�[�^�X�o�[�Ƃ������O��UI�p�[�c�炵���B
            int id = _badgeGroup.Provide(status);
        }

        public void UpdateBadgeStatus()
        {

        }

        public void DeleteBadgeStatus()
        {

        }
    }
}
