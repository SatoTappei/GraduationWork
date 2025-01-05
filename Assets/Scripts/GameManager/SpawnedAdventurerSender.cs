using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
    public static class SpawnedAdventurerSender
    {
        public static async UniTask WriteAsync(IReadOnlyList<Adventurer> adventurers, CancellationToken token)
        {
            // GAS側で値を取得できるようキーに対する値を設定。
            string format = SpawnedAdventurerPostFormat.Convert(adventurers);
            WWWForm form = new WWWForm();
            form.AddField("profiles", format);

            await PostAsync(form, "write", token);
        }

        public static async UniTask DeleteAsync(CancellationToken token)
        {
            await PostAsync(null, "delete", token);
        }

        static async UniTask PostAsync(WWWForm form, string parameter, CancellationToken token)
        {
            // 卒業制作_冒険者データ のGASのデプロイしたURL。
            const string URL = "https://script.google.com/macros/s/AKfycby6-OPwF24uUzoU9e9XFBRBZybEEZkMCKuayL8sjr39pooCzvYQaQzBQi2RIqpR7lu-QQ/exec";

            using UnityWebRequest request = UnityWebRequest.Post($"{URL}?type={parameter}", form);
            await request.SendWebRequest().WithCancellation(token);
        }
    }

    public static class SpawnedAdventurerPostFormat
    {
        [System.Serializable]
        class Format
        {
            public Format(IReadOnlyList<Adventurer> adventurers)
            {
                List<Profile> profiles = new List<Profile>();

                for (int i = 0; i < adventurers.Count; i++)
                {
                    if (adventurers[i] == null) continue;

                    Profile profile = new Profile();
                    profile.FullName = adventurers[i].AdventurerSheet.FullName;
                    profile.DisplayName = adventurers[i].AdventurerSheet.DisplayName;
                    profile.Sex = adventurers[i].AdventurerSheet.Sex;
                    profile.Age = adventurers[i].AdventurerSheet.Age;
                    profile.Job = adventurers[i].AdventurerSheet.Job;
                    profile.Personality = adventurers[i].AdventurerSheet.Personality;
                    profile.Motivation = adventurers[i].AdventurerSheet.Motivation;
                    profile.Weaknesses = adventurers[i].AdventurerSheet.Weaknesses;
                    profile.Background = adventurers[i].AdventurerSheet.Background;

                    profiles.Add(profile);
                }

                Profiles = profiles.ToArray();
            }

            public Profile[] Profiles;
        }

        [System.Serializable]
        class Profile
        {
            public string FullName;
            public string DisplayName;
            public string Sex;
            public string Age;
            public string Job;
            public string Personality;
            public string Motivation;
            public string Weaknesses;
            public string Background;
        }

        public static string Convert(IReadOnlyList<Adventurer> adventurers)
        {
            Format format = new Format(adventurers);
            return JsonUtility.ToJson(format);
        }
    }
}
