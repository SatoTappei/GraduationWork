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

            // イベントの場合、専用の演出を再生する。
            if (source == nameof(SendInformationEvent))
            {
                if (TryGetComponent(out ReceiveInformationEffect effect)) effect.Play();
            }
        }
    }
}
