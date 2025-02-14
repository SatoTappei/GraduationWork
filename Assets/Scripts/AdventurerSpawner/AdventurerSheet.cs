using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class AdventurerSheet
    {
        AdventurerData _profile;

        public AdventurerSheet(AdventurerData profile, Sprite icon, int displayID, bool isArtifactOwner)
        {
            _profile = profile;
            Icon = icon;
            DisplayID = displayID;
            IsArtifactOwner = isArtifactOwner;
        }

        public int UserId { get => _profile.UserId; }
        public string FullName { get => _profile.Name; }
        public string DisplayName { get => _profile.DisplayName; }
        public string Sex { get => _profile.Gender; }
        public string Age { get => _profile.Age.ToString(); }
        public string Job { get => _profile.Job; }
        public string Personality { get => _profile.Personality; }
        public string Motivation { get => _profile.Motivation; }
        public string Weaknesses { get => _profile.Weaknesses; }
        public string Background { get => _profile.Background; }
        public int Level { get => _profile.Level; }
        public int Gold { get => _profile.Gold; }

        public Sprite Icon { get; }
        public int DisplayID { get; }
        public bool IsArtifactOwner { get; }
    }
}