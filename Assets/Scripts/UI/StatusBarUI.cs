using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class StatusBarUI : MonoBehaviour
    {
        [SerializeField] Image _icon;
        [SerializeField] Sprite _emptyIcon;
        [SerializeField] Transform _hpGauge;
        [SerializeField] Transform _emotionGauge;
        [SerializeField] Text _name;
        [SerializeField] GameObject _cover;
        [SerializeField] GameObject _line;
        [SerializeField] Text _lineText;

        bool _isLineShowing;
        WaitForSeconds _keepShowLine;

        void Awake()
        {
            DeleteStatus();
            DisableLine();
        }

        public void SetStatus(IStatusBarDisplayable status)
        {
            SetProfile(status.Icon, status.DisplayName);
            SetHpGaugeScale(status.CurrentHp, status.MaxHp);
            SetEmotionGaugeScale(status.CurrentEmotion, status.MaxEmotion);
            _cover.SetActive(false);

            StartCoroutine(PopAnimationAsync());
        }

        public void UpdateStatus(IStatusBarDisplayable status)
        {
            SetHpGaugeScale(status.CurrentHp, status.MaxHp);
            SetEmotionGaugeScale(status.CurrentEmotion, status.MaxEmotion);
        }

        public void DeleteStatus()
        {
            SetProfile(_emptyIcon, string.Empty);
            SetHpGaugeScale(0, 0);
            SetEmotionGaugeScale(0, 0);
            _cover.SetActive(true);
        }

        public void ShowLine(string line)
        {
            if (_isLineShowing) return;
            
            StartCoroutine(ShowLineAsync(line));
        }

        void SetProfile(Sprite icon, string displayName)
        {
            _icon.sprite = icon;
            // 表示名が長すぎる場合はカット。
            _name.text = displayName.Substring(0, Mathf.Min(displayName.Length, 8));
        }

        void SetHpGaugeScale(int current, int max) => SetGaugeScale(_hpGauge, current, max);
        void SetEmotionGaugeScale(int current, int max) => SetGaugeScale(_emotionGauge, current, max);

        static void SetGaugeScale(Transform gauge, int current, int max)
        {
            float x;
            if (max == 0) x = 0;
            else x = 1.0f * current / max;
            
            gauge.localScale = new Vector3(x, 1, 1);
        }

        IEnumerator PopAnimationAsync()
        {
            // 移動量。
            const float Movement = 20.0f;

            yield return VerticalAnimationAsync(Movement);
            yield return VerticalAnimationAsync(-Movement);
        }

        IEnumerator VerticalAnimationAsync(float movement)
        {
            // 上下する速さ。
            const float Speed = 6.0f;

            Vector3 start = transform.localPosition;
            Vector3 end = start + Vector3.up * movement;
            for (float t = 0; t <= 1.0f; t += Time.deltaTime * Speed)
            {
                transform.localPosition = Vector3.Lerp(start, end, Easing(t));
                yield return null;
            }

            transform.localPosition = end;
        }

        static float Easing(float x)
        {
            if (x < 0.5f)
            {
                return 4 * x * x * x;
            }
            else
            {
                float f = -2 * x + 2;
                return 1 - f * f * f / 2;
            }
        }

        IEnumerator ShowLineAsync(string line)
        {
            _isLineShowing = true;

            EnableLine(line);
            yield return KeepShowLineAsync();
            DisableLine();

            _isLineShowing = false;
        }

        void EnableLine(string line)
        {
            _line.SetActive(true);
            _lineText.text = line;
        }

        void DisableLine()
        {
            _line.SetActive(false);
            _lineText.text = string.Empty;
        }

        IEnumerator KeepShowLineAsync()
        {
            // 持続時間。
            const float Duration = 3.0f;

            yield return _keepShowLine ??= new WaitForSeconds(Duration);
        }
    }
}
