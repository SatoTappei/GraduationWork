using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class BadgeUI : MonoBehaviour
    {
        [SerializeField] Image _icon;
        [SerializeField] Image _hpGauge;
        [SerializeField] Image _emotionGauge;
        [SerializeField] Text _name;
        [SerializeField] GameObject _cover;
        [SerializeField] GameObject _line;
        [SerializeField] Text _lineText;

        private void Start()
        {
            _hpGauge.transform.localScale = new Vector3(0, 1, 1);
            _emotionGauge.transform.localScale = new Vector3(0, 1, 1);
            _name.text = string.Empty;
            _cover.SetActive(true);
            _line.SetActive(false);
            _lineText.text = string.Empty;
        }

        public void SetAdventureData()
        {
            _icon.sprite = null;
            _hpGauge.transform.localScale = Vector3.one;
            _emotionGauge.transform.localScale = Vector3.one;
            _name.text = "‚ç‚ñ‚ç‚ñ";
            _cover.SetActive(false);

            StartCoroutine(PopAnimationAsync());
        }

        public void UpdateAdventureData()
        {

        }

        public void ShowAdventureLine(string line)
        {
            _line.SetActive(true);
            _lineText.text = line;
        }

        public void DeleteAdventureData()
        {
            _hpGauge.transform.localScale = new Vector3(0, 1, 1);
            _emotionGauge.transform.localScale = new Vector3(0, 1, 1);
            _name.text = string.Empty;
            _cover.SetActive(true);
        }

        IEnumerator PopAnimationAsync()
        {
            yield return VerticalAnimationAsync(20);
            yield return VerticalAnimationAsync(-20);
        }

        IEnumerator VerticalAnimationAsync(float movement)
        {
            Vector3 start = transform.localPosition;
            Vector3 end = start + Vector3.up * movement;
            for (float t = 0; t < 1.0f; t += Time.deltaTime * 6)
            {
                transform.localPosition = Vector3.Lerp(start, end, Easing(t));
                yield return null;
            }
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
    }
}
