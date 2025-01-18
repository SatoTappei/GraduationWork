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

            // イベントを受信するために必要。
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
                Debug.Log("岩垂UIが取り憑くイベントを実行。");
            }
            else if (data.EventCode == EventDefine.DeathTrap)
            {
                _fallingBear.Execute();
            }
            else if (data.EventCode == EventDefine.JengaInfo)
            {
                // 文字列を取得するにはジェンガ側で指定したキーを教えてもらう必要がある。
                _revelation.Execute("バーボンハウスへようこそ。");
            }
            else if (data.EventCode == EventDefine.BadJengaInfo)
            {
                Debug.Log("ジェンガイベントを実行。");
            }
            else if (data.EventCode == EventDefine.SummonHeliCopter)
            {
                _helicopterCrash.Execute();
            }
            else if (data.EventCode == EventDefine.Cheer)
            {
                // 感情値に応じてバフもしくは狂気を付与する。
                if (data.GetIntData("Emotion") > 0) _powerUp.Execute();
                else if (data.GetIntData("Emotion") < 0) _madness.Execute();

                GameLog.Add(
                    "おうえんコメント", 
                    data.GetStringData("Message"), 
                    GameLogColor.Green
                );
            }
        }
    }
}
