using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Unused
{
    // ���g�p�B�ۑ��悪�X�v���b�h�V�[�g�ł͂Ȃ��Ȃ����B
    public static class SpawnedAdventurerSender
    {
        public static async UniTask WriteAsync(IReadOnlyList<Adventurer> adventurers, CancellationToken token)
        {
            // GAS���Œl���擾�ł���悤�L�[�ɑ΂���l��ݒ�B
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
            // ���Ɛ���_�`���҃f�[�^ ��GAS�̃f�v���C����URL�B
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
                    profile.FullName = adventurers[i].Sheet.FullName;
                    profile.DisplayName = adventurers[i].Sheet.DisplayName;
                    profile.Sex = adventurers[i].Sheet.Sex;
                    profile.Age = adventurers[i].Sheet.Age;
                    profile.Job = adventurers[i].Sheet.Job;
                    profile.Personality = adventurers[i].Sheet.Personality;
                    profile.Motivation = adventurers[i].Sheet.Motivation;
                    profile.Weaknesses = adventurers[i].Sheet.Weaknesses;
                    profile.Background = adventurers[i].Sheet.Background;

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
