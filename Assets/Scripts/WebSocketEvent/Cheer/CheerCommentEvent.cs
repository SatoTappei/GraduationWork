using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    // コメントが来るタイミングはゲーム側と同期していない。
    // 任意のタイミングでコメントを流すために、イベント呼び出しで保留、冒険者のタイミングで表示させる。
    public class CheerCommentEvent : MonoBehaviour
    {
        [System.Serializable]
        class Range
        {
            public Vector2 BottomLeft;
            public Vector2 TopRight;
        }

        class CommentData
        {
            public int UserId;
            public string Comment;
            public int Emotion;
        }

        [SerializeField] Range[] _ranges;
        [SerializeField] CheerCommentUI[] _cheerCommentUI;

        Dictionary<int, Queue<CommentData>> _pending;

        void Awake()
        {
            _pending = new Dictionary<int, Queue<CommentData>>();
        }

        void OnEnable()
        {
            // 1回の冒険が終了したタイミングで、画面に表示していない保留中のものは消す。
            GameManager.OnGameEnd += ClearPending;
        }

        void OnDisable()
        {
            GameManager.OnGameEnd -= ClearPending;
        }

        public static CheerCommentEvent Find()
        {
            return GameObject.FindGameObjectWithTag("WebSocketEvent").GetComponent<CheerCommentEvent>();
        }

        public void AddPending(int targetUserId, string comment, int emotion = 0)
        {
            if (!_pending.ContainsKey(targetUserId))
            {
                _pending.Add(targetUserId, new Queue<CommentData>());
            }

            _pending[targetUserId].Enqueue(new CommentData 
            { 
                UserId = targetUserId, 
                Comment = comment, 
                Emotion = emotion
            });
        }

        public int Display(int userID, int displayID)
        {
            // 自身へのコメントが無い場合。
            if (!_pending.TryGetValue(userID, out Queue<CommentData> comments))
            {
                return 0;
            }

            // 画面を4分割しており、その画面に映る冒険者に0から3の番号が割り当てられる。
            if (displayID < 0 || 3 < displayID)
            {
                Debug.LogWarning($"冒険者の番号が範囲外。{displayID}");

                return 0;
            }

            int emotion = 0;
            foreach (CommentData comment in comments)
            {
                if (!TryGetUI(out CheerCommentUI ui)) break;

                // 画面からはみ出さないような長さにカット。
                string text = Cut(comment.Comment);

                float x = Random.Range(
                    _ranges[displayID].BottomLeft.x,
                    _ranges[displayID].TopRight.x - GetOffset(text)
                );
                float y = Random.Range(
                    _ranges[displayID].BottomLeft.y,
                    _ranges[displayID].TopRight.y
                );
                ui.transform.position = new Vector2(x, y);
                ui.Play(text, comment.Emotion);

                emotion += comment.Emotion;
            }

            comments.Clear();

            return emotion;
        }

        void ClearPending()
        {
            _pending.Clear();
        }

        bool TryGetUI(out CheerCommentUI ui)
        {
            foreach (CheerCommentUI c in _cheerCommentUI)
            {
                // 非アクティブなものを返す。
                if (!c.gameObject.activeSelf)
                {
                    c.gameObject.SetActive(true);

                    ui = c;
                    return true;
                }
            }

            ui = null;
            return false;
        }

        static string Cut(string comment)
        {
            const int MaxLength = 27;

            if (comment.Length > MaxLength)
            {
                comment = comment.Substring(0, MaxLength);
                comment += "…";
            }

            return comment;
        }

        static float GetOffset(string comment)
        {
            const int LetterSize = 31;

            float offset = 0;
            for (int i = 0; i < comment.Length; i++)
            {
                // 半角英数字の場合は幅が半分。
                if (33 <= comment[i] && comment[i] <= 122)
                {
                    offset += LetterSize / 2;
                }
                else
                {
                    offset += LetterSize;
                }
            }

            return offset;
        }
    }
}
