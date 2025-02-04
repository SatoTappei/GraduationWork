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
        [SerializeField] DroppedArtifact _droppedArtifact;

        Adventurer _adventurer;
        ItemInventory _item;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _item = GetComponent<ItemInventory>();
        }

        public async UniTask<bool> PlayAsync(CancellationToken token)
        {
            const float PlayTime = 2.5f;

            // 生存している場合。
            if (_adventurer.Status.IsAlive) return false;

            // アーティファクトを保持している場合は落とす。
            if (_item.IsHave(nameof(Artifact)))
            {
                DroppedArtifact a = Instantiate(_droppedArtifact);
                a.Place(_adventurer.Coords, Vector2Int.up);
            }

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
            GameLog.Add(
                "システム", 
                $"力尽きた…", 
                LogColor.Red,
                _adventurer.Sheet.DisplayID
            );

            // イベントを送信。
            EventData eventData1 = new EventData(EventDefine.DeathScream);
            VantanConnect.SendEvent(eventData1);
            EventData eventData2 = new EventData(EventDefine.KnockWindow);
            VantanConnect.SendEvent(eventData2);

            // 演出の終了を待つ。
            await UniTask.WaitForSeconds(PlayTime, cancellationToken: token);

            return true;
        }
    }
}
