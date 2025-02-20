using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class TalkReceiver : MonoBehaviour
    {
        HoldInformation _information;
        RevelationEffect _revelationEffect;
        TinfoilHatEffect _tinfoilHatEffect;

        void Awake()
        {
            _information = GetComponent<HoldInformation>();
            _revelationEffect = GetComponent<RevelationEffect>();
            _tinfoilHatEffect =GetComponent<TinfoilHatEffect>();
        }

        public void Talk(BilingualString text, string source, string effect = "")
        {
            _information.AddPending(text, source);
            PlayEffect(effect);
        }

        public void Talk(Information info, string effect = "")
        {
            _information.AddPending(info);
            PlayEffect(effect);
        }

        void PlayEffect(string effect)
        {
            // 各種イベントの場合、それぞれ専用の演出を再生する。
            if (effect == nameof(RevelationEvent))
            {
                _revelationEffect.Play();
            }
            else if (effect == nameof(MindReadingEvent))
            {
                _tinfoilHatEffect.Play();
            }
        }
    }
}
