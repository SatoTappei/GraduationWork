using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "AdventurerSheet_", menuName = "AdventurerSheet")]
    public class AdventurerSheet : ScriptableObject
    {
        [SerializeField] Sprite _icon;
        [SerializeField] string _fullName;
        [SerializeField] string _displayName;
        [SerializeField] string _sex;
        [SerializeField] string _job;
        [SerializeField] int _age;
        [TextArea]
        [SerializeField] string _background;
        [SerializeField] string _lineSample1;
        [SerializeField] string _lineSample2;
        [SerializeField] string _lineSample3;
        [Space(10)]
        [SerializeField] BilingualString[] _decisionSupportContext;
        [Header("ˆÈ‰º‚ÍƒQ[ƒ€ƒvƒŒƒCAI‚ªˆµ‚¤‚Ì‚Å‰pŒê‚Å‹L“ü")]
        [TextArea]
        [SerializeField] string _personality;
        [TextArea]
        [SerializeField] string _motivation;
        [TextArea]
        [SerializeField] string _weaknesses;

        public Sprite Icon => _icon;
        public string FullName => _fullName;
        public string DisplayName => _displayName;
        public string Sex => _sex;
        public string Job => _job;
        public int Age => _age;
        public string Background => _background;
        public string LineSample1 => _lineSample1;
        public string LineSample2 => _lineSample2;
        public string LineSample3 => _lineSample3;
        public BilingualString[] DecisionSupportContext => _decisionSupportContext;
        public string Personality => _personality;
        public string Motivation => _motivation;
        public string Weaknesses => _weaknesses;
    }
}