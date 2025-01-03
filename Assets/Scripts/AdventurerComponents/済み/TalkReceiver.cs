using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class TalkReceiver : MonoBehaviour
    {
        InformationStock _information;
        ReceiveInformationEffect _receiveInformationEffect;
        TinfoilHatEffect _tinfoilHatEffect;

        void Awake()
        {
            _information = GetComponent<InformationStock>();
            _receiveInformationEffect = GetComponent<ReceiveInformationEffect>();
            _tinfoilHatEffect =GetComponent<TinfoilHatEffect>();
        }

        public void Talk(BilingualString text, string source, Vector2Int coords, string effect = "")
        {
            _information.AddPending(text, source);

            // �e��C�x���g�̏ꍇ�A���ꂼ���p�̉��o���Đ�����B
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
