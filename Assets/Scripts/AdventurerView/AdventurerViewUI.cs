using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class AdventurerViewUI : MonoBehaviour
    {
        [SerializeField] Text _name;
        [SerializeField] Text _sex;
        [SerializeField] Text _age;
        [SerializeField] Text _job;
        [SerializeField] Text _personality;
        [SerializeField] Text _motivation;
        [SerializeField] Text _weaknesses;
        [SerializeField] Text _background;

        void Start()
        {
            SetProfile("--", "--", "--", "--", "--", "--", "--", "--");
        }

        public void SetProfile(string name, string sex, string age, string job, 
            string personality, string motivation, string weaknesses, string background)
        {
            _name.text = name;
            _sex.text = sex;
            _age.text = age;
            _job.text = job;
            _personality.text = personality;
            _motivation.text = motivation;
            _weaknesses.text = weaknesses;
            _background.text = background;
        }
    }
}
