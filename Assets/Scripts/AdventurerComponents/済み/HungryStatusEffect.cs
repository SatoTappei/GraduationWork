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
                // まだない
            }
            else
            {
                _isApplied = true;

                _blackboard.AddStatusEffect("空腹で体力が減り続ける。");
            }
        }

        public void Remove()
        {
            if (_isApplied)
            {
                _isApplied = false;

                _blackboard.RemoveStatusEffect("空腹で体力が減り続ける。");
            }
        }

        public void Tick()
        {
            if (_isApplied)
            {
                // ダメージ量は適当。
                _blackboard.CurrentHp -= 5;
            }
        }
    }
}
