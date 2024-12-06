using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class FatigueDamageApply : MonoBehaviour, IStatusEffectDisplayable
    {
        Blackboard _blackboard;

        void Awake()
        {
            _blackboard = GetComponent<Blackboard>();
        }

        public void Damage()
        {
            // 徐々にダメージを受けていく。ダメージ量は適当。
            _blackboard.CurrentHp -= 5;
        }

        bool IStatusEffectDisplayable.IsEnabled()
        {
            return _blackboard.CurrentFatigue >= 100;
        }

        string IStatusEffectDisplayable.GetEntry()
        {
            return "疲労で体力が減り続ける。";
        }
    }
}
