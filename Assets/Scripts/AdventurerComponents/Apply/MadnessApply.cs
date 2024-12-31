using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class MadnessApply : MonoBehaviour, IStatusEffectDisplayable
    {
        static BilingualString Text = new BilingualString(
            "自分以外の冒険者を皆殺しにしよう。", 
            "All adventurers other than yourself are enemies, so it is advisable to attack them aggressively."
        );

        InformationStock _informationStock;
        AvailableActions _availableActions;

        WaitForSeconds _waitInterval;
        bool _isEnabled;

        void Awake()
        {
            _informationStock = GetComponent<InformationStock>();
            _availableActions = GetComponent<AvailableActions>();
        }

        public void Apply()
        {
            if (_isEnabled) return;

            StartCoroutine(ApplyAsync());
        }

        IEnumerator ApplyAsync()
        {
            _isEnabled = true;

            // 一定間隔でAIに冒険者を攻撃するよう促す情報を追加。回数と間隔の値は適当に設定。
            for (int i = 0; i < 10; i++)
            {
                _informationStock.AddPending(Text, "Myself", isShared: false);
                //_availableActions.Remove("Scavenge Surrounding");
                //_availableActions.Remove("Talk Surrounding");
                // _availableActions.Add("Attack Surrounding Adventurer");

                yield return _waitInterval ??= new WaitForSeconds(3.0f);
            }

            //_availableActions.Add("Scavenge Surrounding");
            //_availableActions.Add("Talk Surrounding");
            //_availableActions.Remove("Attack Surrounding Adventurer");

            _isEnabled = false;
        }

        bool IStatusEffectDisplayable.IsEnabled()
        {
            return _isEnabled;
        }

        string IStatusEffectDisplayable.GetEntry()
        {
            return "頭おかしい。";
        }
    }
}
