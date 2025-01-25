using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public enum LogColor { White, Red, Yellow, Green, Blue }

    public class Row : MonoBehaviour
    {
        [SerializeField] Text _text;
        [SerializeField] Vector2 _start;
        [SerializeField] Vector2 _goal;

        bool _isPlaying;

        public void Set(string label, string value, LogColor color = LogColor.White)
        {
            _text.text = value;
            _text.color = GetColor(color);

            if (_isPlaying) return;

            StartCoroutine(PlayAnimationAsync());
        }

        IEnumerator PlayAnimationAsync()
        {
            const float Speed = 3.0f;

            _isPlaying = true;

            Transform text = _text.transform;
            for (float t = 0; t <= 1.0f; t += Time.deltaTime * Speed)
            {
                Vector2 p = Vector2.Lerp(_start, _goal, Easing(t));
                text.localPosition = new Vector3(p.x, p.y, text.localPosition.z);

                yield return null;
            }

            text.localPosition = new Vector3(_goal.x, _goal.y, text.localPosition.z);

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
