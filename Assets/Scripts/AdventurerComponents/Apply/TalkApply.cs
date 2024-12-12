using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class TalkApply : MonoBehaviour
    {
        public void Talk(BilingualString text, string source)
        {
            if (TryGetComponent(out InformationStock information))
            {
                information.AddPending(text, source);
            }

            // �e��C�x���g�̏ꍇ�A���ꂼ���p�̉��o���Đ�����B
            if (source == nameof(SendInformationEvent))
            {
                if (TryGetComponent(out ReceiveInformationEffect effect)) effect.Play();
            }
            else if (source == nameof(MindReadingEvent))
            {
                if (TryGetComponent(out TinfoilHatEffect effect)) effect.Play();
            }
        }
    }
}
