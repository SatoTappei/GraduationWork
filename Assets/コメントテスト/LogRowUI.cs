using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class LogRowUI : MonoBehaviour
    {
        [SerializeField] Text _label;
        [SerializeField] Text _value;

        public void Set(string label, string value)
        {
            _label.text = label;
            _value.text = value;
        }
    }
}
