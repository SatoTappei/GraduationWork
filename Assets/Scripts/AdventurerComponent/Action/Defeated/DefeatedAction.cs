using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VTNConnect;

namespace Game
{
    public class DefeatedAction : BaseAction
    {
        [SerializeField] AudioClip _defeatedSE;

        Adventurer _adventurer;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
        }

        public async UniTask<bool> PlayAsync(CancellationToken token)
        {
            const float PlayTime = 2.5f;

            // 生存している場合。
            if (_adventurer.Status.IsAlive) return false;

            // 死亡時のアニメーション再生。
            Animator animator = GetComponentInChildren<Animator>();
            if (animator != null)
            {
                animator.Play("Death");
            }

            // 死亡時のSE再生。
            AudioSource audioSource = GetComponent<AudioSource>();
            if (_defeatedSE != null)
            {
                audioSource.clip = _defeatedSE;
                audioSource.Play();
            }
            else 
            {
                Debug.LogWarning("力尽きた際のSEがアサインされていない。");
            }

            // 死亡時の台詞。
            if (TryGetComponent(out LineDisplayer line))
            {
                line.ShowLine(RequestLineType.Defeated);
            }

            // ログに表示。
            Adventurer adventurer = GetComponent<Adventurer>();
            GameLog.Add(
                "システム", 
                $"{adventurer.AdventurerSheet.DisplayName}は力尽きた。", 
                GameLogColor.Red
            );

            // イベントを送信。
            EventData eventData = new EventData(EventDefine.DeathScream);
            VantanConnect.SendEvent(eventData);

            // 演出の終了を待つ。
            await UniTask.WaitForSeconds(PlayTime, cancellationToken: token);

            return true;
        }
    }
}
