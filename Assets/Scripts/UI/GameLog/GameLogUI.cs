using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public enum GameLogColor { White, Red, Yellow, Green, Blue }

    public class GameLogUI : MonoBehaviour
    {
        [SerializeField] Text _label;
        [SerializeField] Text _value;
        [SerializeField] float _startX;
        [SerializeField] float _goalX;

        bool _isPlaying;

        public void Set(string label, string value, GameLogColor color = GameLogColor.White)
        {
            _label.text = label;
            _label.color = ConvertToColor(color);
            _value.text = value;
            _value.color = ConvertToColor(color);

            if (_isPlaying) return;

            StartCoroutine(PlaySlideAnimationAsync());
        }

        IEnumerator PlaySlideAnimationAsync()
        {
            const float Speed = 5.0f;

            _isPlaying = true;

            Transform label = _label.transform;
            Transform value = _value.transform;
            for (float t = 0; t <= 1.0f; t += Time.deltaTime * Speed)
            {
                float x = Mathf.Lerp(_startX, _goalX, Easing(t));
                label.localPosition = new Vector3(x, label.localPosition.y, label.localPosition.z);
                value.localPosition = new Vector3(x, value.localPosition.y, value.localPosition.z);

                yield return null;
            }

            label.localPosition = new Vector3(_goalX, label.localPosition.y, label.localPosition.z);
            value.localPosition = new Vector3(_goalX, value.localPosition.y, value.localPosition.z);

            _isPlaying = false;
        }

        static Color ConvertToColor(GameLogColor color)
        {
            string htmlColor = "#FFFFFF";
            if (color == GameLogColor.White) htmlColor = "#FFFFFF";
            else if (color == GameLogColor.Red) htmlColor = "#FF5555";
            else if (color == GameLogColor.Green) htmlColor = "#55FF55";
            else if (color == GameLogColor.Yellow) htmlColor = "#FFFF55";
            else if (color == GameLogColor.Blue) htmlColor = "#5555FF";

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
