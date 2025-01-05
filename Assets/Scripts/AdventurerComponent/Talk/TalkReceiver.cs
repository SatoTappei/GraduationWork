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

        public void Talk(BilingualString text, string source, Vector2Int coords, string effect = "")
        {
            _information.AddPending(text, source);

            // �e��C�x���g�̏ꍇ�A���ꂼ���p�̉��o���Đ�����B
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
