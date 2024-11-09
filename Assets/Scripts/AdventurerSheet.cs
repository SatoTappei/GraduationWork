using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class AdventurerSheet
    {
        public AdventurerSheet(string fullName, string displayName, string sex, string age, string job,
            string personality, string motivation, string weaknesses, string background, Sprite icon)
        {
            FullName = fullName;
            DisplayName = displayName;
            Sex = sex;
            Age = age;
            Job = job;
            Personality = personality;
            Motivation = motivation;
            Weaknesses = weaknesses;
            Background = background;
            Icon = icon;
        }

        public string FullName { get; }
        public string DisplayName { get; }
        public string Sex { get; }
        public string Age { get; }
        public string Job { get; }
        public string Personality { get; }
        public string Motivation { get; }
        public string Weaknesses { get; }
        public string Background { get; }
        public Sprite Icon { get; }
    }
}