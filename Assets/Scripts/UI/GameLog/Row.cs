using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public enum LogColor { White, Red, Yellow, Green, Blue }

    public class Row : MonoBehaviour
    {
        [SerializeField] Text _value;
        [SerializeField] float _startX;
        [SerializeField] float _goalX;

        bool _isPlaying;

        public void Set(string label, string value, LogColor color = LogColor.White)
        {
            const int MaxLength = 20;

            // 長い文字列の場合はカット。見栄えが悪いので三点リーダーを付けておく。
            if (value.Length > MaxLength)
            {
                value = value.Substring(0, MaxLength);
                value += "…";
            }

            _value.text = value;
            _value.color = GetColor(color);

            if (_isPlaying) return;

            StartCoroutine(PlayAnimationAsync());
        }

        IEnumerator PlayAnimationAsync()
        {
            const float Speed = 5.0f;

            _isPlaying = true;

            Transform value = _value.transform;
            for (float t = 0; t <= 1.0f; t += Time.deltaTime * Speed)
            {
                float x = Mathf.Lerp(_startX, _goalX, Easing(t));
                value.localPosition = new Vector3(x, value.localPosition.y, value.localPosition.z);

                yield return null;
            }

            value.localPosition = new Vector3(_goalX, value.localPosition.y, value.localPosition.z);

            _isPlaying = false;
        }

        static Color GetColor(LogColor color)
        {
            string htmlColor = "#FFFFFF";
            if (color == LogColor.White) htmlColor = "#FFFFFF";
            else if (color == LogColor.Red) htmlColor = "#FF5555";
            else if (color == LogColor.Green) htmlColor = "#55FF55";
            else if (color == LogColor.Yellow) htmlColor = "#FFFF55";
            else if (color == LogColor.Blue) htmlColor = "#5555FF";

            ColorUtility.TryParseHtmlString(htmlColor, out Color result);
            return result;
        }

        static float Easing(float t)
        {
            float x = 1.0f - t;
            return 1 - x * x * x * x * x;
        }
    }
}
