using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
    public class CommentLoader : MonoBehaviour
    {
        Dictionary<string, Queue<CommentData>> _comments;
        // �X�v���b�h�V�[�g����f�[�^��ǂݍ���ŁA�p�[�X��������������܂ő҂B
        bool _isLoading = true;

        public IReadOnlyDictionary<string, Queue<CommentData>> Comments => _comments;
        public bool IsLoading => _isLoading;

        void Awake()
        {
            _comments = new Dictionary<string, Queue<CommentData>>();
        }

        void Start()
        {
            GetDataAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        // �`���҂̖��O�ŃR�����g���擾�B
        public bool TryGetComment(string fullName, out IReadOnlyCollection<CommentData> comment)
        {
            comment = null;

            // �f�[�^��ǂݍ��ݒ��̏ꍇ�B
            if (IsLoading) return false;

            if (_comments.TryGetValue(fullName, out Queue<CommentData> value))
            {
                comment = value;
            }

            return comment != null;
        }

        async UniTask GetDataAsync(CancellationToken token)
        {
            const string FileID = "1hQSCky3xafLS3p75MMYjUovEfKLtOvIh9U4HdDNUElk";
            const string SheetName = "�R�����g";

            string URL = $"https://docs.google.com/spreadsheets/d/{FileID}/gviz/tq?tqx=out:csv&sheet={SheetName}";
            using UnityWebRequest request = UnityWebRequest.Get(URL);
            await request.SendWebRequest().WithCancellation(token);

            if (IsSuccess(request.result))
            {
                Parse(request.downloadHandler.text);
            }

            _isLoading = false;
        }

        static bool IsSuccess(UnityWebRequest.Result result)
        {
            if (result == UnityWebRequest.Result.ConnectionError) return false;
            if (result == UnityWebRequest.Result.ProtocolError) return false;
            else return true;
        }

        void Parse(string downloadText)
        {
            // 1�s�ڂ̓��x���Ȃ̂ŃX�L�b�v�B
            foreach (string row in downloadText.Split("\n").Skip(1))
            {
                string[] cells = row.Split(",").Select(c => c.Trim('"')).ToArray();
                
                // �`���ҕʂɃL���[�C���O�B
                string name = cells[0];
                string comment = cells[1];
                _comments.TryAdd(name, new Queue<CommentData>());
                _comments[name].Enqueue(new CommentData(name, comment));
            }
        }
    }
}
