using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class HungryStatusEffect : MonoBehaviour
    {
        Blackboard _blackboard;
        bool _isApplied;

        void Awake()
        {
            _blackboard = GetComponent<Blackboard>();
        }

        public void Apply()
        {
            if (_isApplied)
            {
                // �܂��Ȃ�
            }
            else
            {
                _isApplied = true;

                _blackboard.AddStatusEffect("�󕠂ő̗͂����葱����B");
            }
        }

        public void Remove()
        {
            if (_isApplied)
            {
                _isApplied = false;

                _blackboard.RemoveStatusEffect("�󕠂ő̗͂����葱����B");
            }
        }

        public void Tick()
        {
            if (_isApplied)
            {
                // �_���[�W�ʂ͓K���B
                _blackboard.CurrentHp -= 5;
            }
        }
    }
}
