using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
    public static class AdventureResultSender
    {
        public class ProcessStatus
        {
            public bool isProcessing;
        }

        public static async UniTask SendAsync(IReadOnlyDictionary<Adventurer, string> adventureResults, CancellationToken token)
        {
            // ���Ɛ���_�`���҃f�[�^ ��GAS�̃f�v���C����URL�B
            const string URL = "https://script.google.com/macros/s/AKfycby6-OPwF24uUzoU9e9XFBRBZybEEZkMCKuayL8sjr39pooCzvYQaQzBQi2RIqpR7lu-QQ/exec";

            // GAS���Œl���擾�ł���悤�L�[�ɑ΂���l��ݒ�B
            string format = AdventureResultPostFormat.Convert(adventureResults);
            WWWForm form = new WWWForm();
            form.AddField("results", format);

            using UnityWebRequest request = UnityWebRequest.Post(URL, form);
            await request.SendWebRequest();

            // �X�v���b�h�V�[�g�ɏ������݂���������܂ő҂B
            bool isProcessing = true;
            while (isProcessing && !token.IsCancellationRequested)
            {
                using UnityWebRequest getRequest = UnityWebRequest.Get(URL);
                await getRequest.SendWebRequest();

                string json = getRequest.downloadHandler.text;
                isProcessing = JsonUtility.FromJson<ProcessStatus>(json).isProcessing;
            }

            Debug.Log("�`���̌��ʂ��X�v���b�h�V�[�g�ɏ������݊����B");
        }
    }

    public static class AdventureResultPostFormat
    {
        [System.Serializable]
        class Format
        {
            public Format(IReadOnlyDictionary<Adventurer, string> adventureResults)
            {
                // �`���҂̖��O�Ɩ`�����ʂ��Z�b�g�ŕێ�����B
                // �������O�̖`���҂��������݂���ꍇ�APost������ŋ�ʂł��Ȃ��̂Œ��ӁB
                List<Result> results = new List<Result>(adventureResults.Count);
                foreach (KeyValuePair<Adventurer, string> v in adventureResults)
                {
                    string adventurerName = v.Key.AdventurerSheet.FullName;
                    string resultType = v.Value;
                    results.Add(new Result(adventurerName, resultType));
                }

                Results = results.ToArray();
            }

            public Result[] Results;
        }

        [System.Serializable]
        class Result
        {
            public Result(string name, string type)
            {
                Name = name;
                Type = type;
            }

            public string Name;
            public string Type;
        }

        public static string Convert(IReadOnlyDictionary<Adventurer, string> adventureResults)
        {
            Format format = new Format(adventureResults);
            return JsonUtility.ToJson(format);
        }
    }
}
