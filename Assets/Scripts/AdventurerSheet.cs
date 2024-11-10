using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class AdventurerSheet
    {
        // インスペクターなどで各種パラメータを設定する場合。
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

        // スプレッドシートから、冒険者データを読み込んだ場合。
        public AdventurerSheet(SpreadSheetData profile, AvatarCustomizeData avatarData)
            : this(profile.FullName, profile.DisplayName, profile.Sex, profile.Age, profile.Job,
                  profile.Personality, profile.Motivation, profile.Weaknesses, profile.Background, avatarData.Icon) { }

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