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

            // �C�x���g�̏ꍇ�A��p�̉��o���Đ�����B
            if (source == nameof(SendInformationEvent))
            {
                if (TryGetComponent(out ReceiveInformationEffect effect)) effect.Play();
            }
        }
    }
}
