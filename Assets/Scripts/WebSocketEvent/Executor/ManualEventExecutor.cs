using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ManualEventExecutor : MonoBehaviour
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
        GiveItemEvent _giveItem;

        AdventurerSpawner _spawner;

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
            _giveItem = GetComponent<GiveItemEvent>();
            _spawner = AdventurerSpawner.Find();
        }

        void OnGUI()
        {
            GUIStyle style = GUI.skin.GetStyle("button");
            style.fontSize = 25;

            if (IsButtonClicked(0, "ﾀﾞﾒｰｼﾞ"))
            {
                _dealingDamage.Execute(1000); // ダメージ量は適当。
            }
            else if (IsButtonClicked(1, "ｸﾏｻﾝ"))
            {
                _fallingBear.Execute();
            }
            else if (IsButtonClicked(2, "回復"))
            {
                if (_spawner.Spawned.Count > 0)
                {
                    int r = Random.Range(0, _spawner.Spawned.Count);
                    _heal.Execute(_spawner.Spawned[r].AdventurerSheet.FullName, 1); // 回復量は適当。
                }
            }
            else if (IsButtonClicked(3, "ﾍﾘｺﾌﾟﾀｰ"))
            {
                _helicopterCrash.Execute();
            }
            else if (IsButtonClicked(4, "ﾀﾞﾙﾄﾞﾘｰｼｯﾃﾞｨ"))
            {
                _levitation.Execute();
            }
            else if (IsButtonClicked(5, "思考盗聴"))
            {
                _mindReading.Execute();
            }
            else if (IsButtonClicked(6, "ﾊﾟﾜｰｱｯﾌﾟ"))
            {
                _powerUp.Execute();
            }
            else if (IsButtonClicked(7, "天啓"))
            {
                string[] texts =
                {
                    "(´・ω・｀)らんらん♪",
                    "(´・ω・｀)出荷よー",
                    "ごんぶと"
                };
                int r = Random.Range(0, texts.Length);
                _revelation.Execute(texts[r]);
            }
            else if (IsButtonClicked(8, "罠"))
            {
                _trapGenerate.Execute();
            }
            else if (IsButtonClicked(9, "アイテム付与"))
            {
                _giveItem.Execute("軽い鍵");
            }
            else if (IsButtonClicked(10, "AI初期化"))
            {
                foreach (Adventurer adventurer in _spawner.Spawned)
                {
                    if (adventurer != null) adventurer.Reboot();
                }
            }
            else if (IsButtonClicked(11, "狂気"))
            {
                foreach (Adventurer adventurer in _spawner.Spawned)
                {
                    if (adventurer != null && adventurer.TryGetComponent(out IDamageable damage))
                    {
                        damage.Damage(0, default, "Madness");
                    }
                }
            }
        }

        bool IsButtonClicked(int number, string label)
        {
            return GUI.Button(new Rect(160 * number, 0, 160, 70), label);
        }
    }
}