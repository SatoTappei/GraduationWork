using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    // �L�����N�^�[�V�[�g����o�ꂩ��E�o�܂ł̒����I�ȃv�������쐬�B
    //  �󔠂�n�擾����A�����^�[��������A�G��n�̓|���A������T���A�Ȃǂ�g�ݍ��킹��B
    // �Z���I�ȍs����B�����悤�Ƃ���B
    public class AdventureAI : MonoBehaviour
    {
        // ������AI�̔��f�ɒu�������B
        public async UniTask<string> SelectNextActionAsync()
        {
            return await RandomNextActionAsync();
        }

        // �s���̌��ʂ�񍐁B
        public void ReportActionResult(string result)
        {

        }

        // �L�[���͂Ŏ蓮���䂷��ꍇ�B
        async UniTask<string> InputNextActionAsync()
        {
            await UniTask.WaitUntil(() => Input.anyKey);

            if (Input.GetKeyDown(KeyCode.Alpha1)) return NumberToActionName(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) return NumberToActionName(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) return NumberToActionName(2);
            if (Input.GetKeyDown(KeyCode.Alpha4)) return NumberToActionName(3);
            if (Input.GetKeyDown(KeyCode.Alpha5)) return NumberToActionName(4);
            if (Input.GetKeyDown(KeyCode.Alpha6)) return NumberToActionName(5);
            else return string.Empty;
        }

        // �����_���ȍs����I������ꍇ�B
        async UniTask<string> RandomNextActionAsync()
        {
            await UniTask.Yield(); // �҂K�v�Ȃ����x�����o��̂ňꉞ�B
            return NumberToActionName(Random.Range(0, 5));
        }

        string NumberToActionName(int number)
        {
            if (number == 0) return "Move Treasure";
            if (number == 1) return "Move Enemy";
            if (number == 2) return "Move Entrance";
            if (number == 3) return "Interact Attack";
            if (number == 4) return "Interact Scav";
            if (number == 5) return "Interact Talk";
            else return string.Empty;
        }
    }
}