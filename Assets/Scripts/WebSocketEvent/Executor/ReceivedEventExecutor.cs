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
        CheerCommentEvent _cheerComment;
        GiveItemEvent _giveItem;

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
            _cheerComment = GetComponent<CheerCommentEvent>();
            _giveItem = GetComponent<GiveItemEvent>();

            // �C�x���g����M���邽�߂ɕK�v�B
            VantanConnect.RegisterEventReceiver(this);
        }

        void Start()
        {
            _isActive = true;
        }

        public void OnEventCall(EventData data)
        {
            Debug.Log($"WebSocket Event Received: {data.EventCode}");

            if (data.EventCode == EventDefine.DeathTrap)
            {
                _fallingBear.Execute();
            }
            else if (data.EventCode == EventDefine.JengaInfo)
            {
                string material = data.GetStringData("Material");
                if (material == "Plastic") _giveItem.Execute("�y����");
                else if (material == "Wood") _giveItem.Execute("�y����");
                else if (material == "Iron") _giveItem.Execute("�d����");
                else Debug.LogWarning($"�Ή�����A�C�e���������B�X�y���~�X�H: {material}");
            }
            else if (data.EventCode == EventDefine.BadJengaInfo)
            {
                _dealingDamage.Execute(data.GetIntData("Turn"));
            }
            else if (data.EventCode == EventDefine.SummonHeliCopter)
            {
                _helicopterCrash.Execute();
            }
            else if (data.EventCode == EventDefine.UserTalk)
            {
                _revelation.Execute(data.GetStringData("Line"));
            }
            else if (data.EventCode == EventDefine.ConfrontHeal)
            {
                _heal.Execute(data.GetIntData("Value"));
            }
            else if (data.EventCode == EventDefine.Cheer)
            {
                // ����l�ɉ����ăo�t�������͋��C��t�^����B
                if (data.GetIntData("Emotion") > 0) _powerUp.Execute();
                else if (data.GetIntData("Emotion") < 0) _madness.Execute();

                _cheerComment.Execute(
                    data.GetStringData("Target"),
                    data.GetStringData("Message"),
                    data.GetIntData("Emotion")
                );
            }
            else if (data.EventCode == EventDefine.BonusCoin)
            {
                // �񕜂ƃo�t��������B�񕜗ʂ͓K���B
                _heal.Execute(data.GetStringData("Target"), 100);
                _powerUp.Execute(data.GetStringData("Target"));
            }
            else if (data.EventCode == EventDefine.Levelup)
            {
                //
            }
            else if (data.EventCode == EventDefine.GetArtifact)
            {
                //
            }
            else if (EventDefine.FieldEvent01 <= data.EventCode && data.EventCode <= EventDefine.FieldEvent05)
            {
                // �����_���łǂ��炩�̃C�x���g���N����A�m���͓K���B
                if (Random.value <= 0.5f) _trapGenerate.Execute();
                else _mindReading.Execute();
            }
        }
    }
}
