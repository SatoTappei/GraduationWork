using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class BuffStatusEffect : MonoBehaviour
    {
        Blackboard _blackboard;
        StatusBuffEffect _effect;
        int[] _remainingTurns;

        void Awake()
        {
            _blackboard = GetComponent<Blackboard>();
            _effect = GetComponent<StatusBuffEffect>();

            // ����u�X�s�[�h�v�Ɓu�U���́v��2�����Ȃ��̂ŁA���ꂼ��̎c�莞�Ԃ�ێ����Ă��������ŏ\���B
            _remainingTurns = new int[2];
        }

        public void Apply(string type, float value)
        {
            if (type == "Speed")
            {
                _blackboard.SpeedMagnification = value;
                _remainingTurns[0] = 10; // �K���ȃ^�[����
                _effect.Play();
            }
            else if (type == "Attack")
            {
                _blackboard.AttackMagnification = value;
                _remainingTurns[1] = 10; // �K���ȃ^�[����
                _effect.Play();
            }
            else
            {
                Debug.LogWarning($"�Ή�����X�e�[�^�X�o�t�������B�X�y���~�X�H:{type}");
            }
        }

        public void Remove(string type)
        {
            if (type == "Speed")
            {
                _blackboard.SpeedMagnification = 1.0f;
                _remainingTurns[0] = 0;
            }
            else if (type == "Attack")
            {
                _blackboard.AttackMagnification = 1.0f;
                _remainingTurns[1] = 0;
            }
            else
            {
                Debug.LogWarning($"�Ή�����X�e�[�^�X�o�t�������B�X�y���~�X�H:{type}");
            }
        }

        public void Tick()
        {
            // �S�Ẵo�t�̎c��^�[���������炷�B
            for (int i = 0; i < _remainingTurns.Length; i++)
            {
                _remainingTurns[i]--;
                _remainingTurns[i] = Mathf.Max(0, _remainingTurns[i]);
            }

            // �^�[������0�ɂȂ����ꍇ�̓o�t�������B
            if (_remainingTurns[0] == 0) Remove("Speed");
            if (_remainingTurns[1] == 0) Remove("Attack");

            // �S�Ẵo�t�̎c��^�[������0�ɂȂ����ꍇ�A���o����������B
            if (_remainingTurns.Sum() == 0)
            {
                _effect.Stop();
            }
        }
    }
}
