using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class TalkReceiver : MonoBehaviour
    {
        HoldInformation _informationInventory;
        ReceiveInformationEffect _receiveInformationEffect;
        TinfoilHatEffect _tinfoilHatEffect;

        void Awake()
        {
            _informationInventory = GetComponent<HoldInformation>();
            _receiveInformationEffect = GetComponent<ReceiveInformationEffect>();
            _tinfoilHatEffect =GetComponent<TinfoilHatEffect>();
        }

        public void Talk(BilingualString text, string source, Vector2Int coords, string effect = "")
        {
            _informationInventory.AddPending(text, source);

            // 各種イベントの場合、それぞれ専用の演出を再生する。
            if (effect == nameof(SendInformationEvent))
            {
                _receiveInformationEffect.Play();
            }
            else if (effect == nameof(MindReadingEvent))
            {
                _tinfoilHatEffect.Play();
            }
        }
    }
}
