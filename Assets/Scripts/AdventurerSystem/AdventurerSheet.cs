using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class AdventurerSheet
    {
        public AdventurerSheet(int displayID, AdventurerData profile, AvatarData avatarData)
        {
            UserId = profile.UserId;
            DisplayID = displayID;
            FullName = profile.Name;
            DisplayName = profile.DisplayName;
            Sex = profile.Gender;
            Age = profile.Age.ToString();
            Job = profile.Job;
            Personality= profile.Personality;
            Motivation = profile.Motivation;
            Weaknesses = profile.Weaknesses;
            Background = profile.Background;
            Level = profile.Level;
            Gold = profile.Gold;
            Icon = avatarData.Icon;
        }

        public int UserId { get; }
        public int DisplayID { get; }
        public string FullName { get; }
        public string DisplayName { get; }
        public string Sex { get; }
        public string Age { get; }
        public string Job { get; }
        public string Personality { get; }
        public string Motivation { get; }
        public string Weaknesses { get; }
        public string Background { get; }
        public int Level { get; }
        public int Gold { get; }
        public Sprite Icon { get; }
    }
}