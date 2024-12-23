using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class AdventurerSpreadSheetData
    {
        public AdventurerSpreadSheetData(string fullName, string displayName, string sex, string age, string job,
            string personality, string motivation, string weaknesses, string background, string avatar)
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
            Avatar = avatar;
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
        public string Avatar { get; }

        public bool IsFieldEmpty()
        {
            return FullName == string.Empty ||
                   DisplayName == string.Empty ||
                   Sex == string.Empty ||
                   Age == string.Empty ||
                   Job == string.Empty ||
                   Personality == string.Empty ||
                   Motivation == string.Empty ||
                   Weaknesses == string.Empty ||
                   Background == string.Empty ||
                   Avatar == string.Empty;
        }

        public override string ToString()
        {
            return $"���O:{FullName}\n" +
                   $"�\����:{DisplayName}\n" +
                   $"����:{Sex}\n" +
                   $"�N��:{Age}\n" +
                   $"�E��:{Job}\n" +
                   $"���i:{Personality}\n" +
                   $"���`�x�[�V����:{Motivation}\n" +
                   $"��_:{Weaknesses}\n" +
                   $"�o�b�N�X�g�[���[:{Background}\n" +
                   $"�A�o�^�[{Avatar}";
        }
    }
}
