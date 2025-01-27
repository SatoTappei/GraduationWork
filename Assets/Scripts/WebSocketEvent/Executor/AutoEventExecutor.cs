using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    // デバッグ用。
    public class AutoEventExecutor : MonoBehaviour
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

        void Start()
        {
            StartCoroutine(PlayRepeatingAsync());
        }

        IEnumerator PlayRepeatingAsync()
        {
            while (true)
            {
                int random = Random.Range(0, 9);
                if (random == 0)
                {
                    _dealingDamage.Execute(1); // ﾀﾞﾒｰｼﾞは適当。
                }
                else if (random == 1)
                {
                    _fallingBear.Execute();
                }
                else if (random == 2)
                {
                    if (_spawner.Spawned.Count > 0)
                    {
                        int r = Random.Range(0, _spawner.Spawned.Count);
                        _heal.Execute(_spawner.Spawned[r].AdventurerSheet.FullName, 1); // 回復量は適当。
                    }
                }
                else if (random == 3)
                {
                    _helicopterCrash.Execute();
                }
                else if (random == 4)
                {
                    _levitation.Execute();
                }
                else if (random == 5)
                {
                    _mindReading.Execute();
                }
                else if (random == 6)
                {
                    _powerUp.Execute();
                }
                else if (random == 7)
                {
                    string[] texts =
                    {
                        "1面のボスは目を狙え！",
                        "達人王をやるのだ！",
                        "ゆんやぁ"
                    };
                    int r = Random.Range(0, texts.Length);
                    _revelation.Execute(texts[r]);
                }
                else if (random == 8)
                {
                    _trapGenerate.Execute();
                }
                else if (random == 9)
                {
                    _giveItem.Execute("軽い鍵");
                }

                yield return new WaitForSeconds(1.0f);
            }
        }
    }
}
