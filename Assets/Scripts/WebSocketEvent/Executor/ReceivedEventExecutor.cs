using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VTNConnect;

namespace Game
{
    public class ReceivedEventExecutor : MonoBehaviour, IVantanConnectEventReceiver
    {
        DealingDamageEvent _dealingDamage;
        FallingBearEvent _fallingBear;
        HealEvent _heal;
        HelicopterCrashEvent _helicopterCrash;
        LevitationEvent _levitation;
        MindReadingEvent _mindReading;
        PowerUpEvent _powerUp;
        RevelationEvent _revelation;
        TrapGenerateEvent _trapGenerate;
        MadnessEvent _madness;

        bool _isActive;

        public bool IsActive => _isActive;

        void Awake()
        {
            _dealingDamage = GetComponent<DealingDamageEvent>();
            _fallingBear = GetComponent<FallingBearEvent>();
            _heal = GetComponent<HealEvent>();
            _helicopterCrash = GetComponent<HelicopterCrashEvent>();
            _levitation = GetComponent<LevitationEvent>();
            _mindReading = GetComponent<MindReadingEvent>();
            _powerUp = GetComponent<PowerUpEvent>();
            _revelation = GetComponent<RevelationEvent>();
            _trapGenerate = GetComponent<TrapGenerateEvent>();
            _madness = GetComponent<MadnessEvent>();

            // �C�x���g����M���邽�߂ɕK�v�B
            VantanConnect.RegisterEventReceiver(this);
        }

        void Start()
        {
            _isActive = true;
        }

        public void OnEventCall(EventData data)
        {
            if (data.EventCode == EventDefine.DemonUI)
            {
                Debug.Log("�␂UI�����߂��C�x���g�����s�B");
            }
            else if (data.EventCode == EventDefine.DeathTrap)
            {
                _fallingBear.Execute();
            }
            else if (data.EventCode == EventDefine.JengaInfo)
            {
                // ��������擾����ɂ̓W�F���K���Ŏw�肵���L�[�������Ă��炤�K�v������B
                _revelation.Execute("�o�[�{���n�E�X�ւ悤�����B");
            }
            else if (data.EventCode == EventDefine.BadJengaInfo)
            {
                Debug.Log("�W�F���K�C�x���g�����s�B");
            }
            else if (data.EventCode == EventDefine.SummonHeliCopter)
            {
                _helicopterCrash.Execute();
            }
            else if (data.EventCode == EventDefine.Cheer)
            {
                // ����l�ɉ����ăo�t�������͋��C��t�^����B
                if (data.GetIntData("Emotion") > 0) _powerUp.Execute();
                else if (data.GetIntData("Emotion") < 0) _madness.Execute();

                GameLog.Add(
                    "��������R�����g", 
                    data.GetStringData("Message"), 
                    GameLogColor.Green
                );
            }
        }
    }
}
