using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class EntryAction : BaseAction
    {
        [SerializeField] AudioClip _entrySE;

        public void Play()
        {
            // 登場時の演出。
            TryGetComponent(out AudioSource audioSource);
            if (_entrySE != null)
            {
                audioSource.clip = _entrySE;
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning("入場時のSEがアサインされていない。");
            }

            // 登場時の台詞。
            TryGetComponent(out LineDisplayer line);
            line.Show(RequestLineType.Entry);

            // ゲーム進行ログに表示。
            TryGetComponent(out Adventurer adventurer);
            GameLog.Add(
                $"システム", 
                $"ダンジョンにやってきた。", 
                LogColor.White,
                adventurer.Sheet.DisplayID
            );
        }
    }
}
