using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    // �C�ӂ̃^�C�~���O�œo�^�A�j�������^�C�~���O�ō폜���Ă��邾���B
    // �킴�킴�R���|�[�l���g�𕪂���K�v�Ȃ���������Ȃ����A����UI�Ɠ����菇�ɂȂ�킩��₷���H
    public class NameTagBinder : MonoBehaviour
    {
        Adventurer _adventurer;
        NameTag _nameTag;
        bool _isRegisterd;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _nameTag = NameTag.Find();
        }

        public void Register()
        {
            if (_isRegisterd)
            {
                Debug.LogWarning("���ɓo�^�ς݁B");
            }
            else
            {
                _isRegisterd = true;
                _nameTag.Register(_adventurer);
            }
        }

        void OnDestroy()
        {
            if (_nameTag != null)
            {
                _nameTag.Delete(_adventurer);
            }
        }
    }
}
