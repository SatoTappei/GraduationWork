using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class RemoteEventExecutor : MonoBehaviour
    {
        void Start()
        {
            //StartCoroutine(AutoRepeatingAsync());
        }

        void OnGUI()
        {
            GUIStyle style = GUI.skin.GetStyle("button");
            style.fontSize = 25;

            if (GUI.Button(new Rect(0, 0, 160, 70), $"ヘリコタプー"))
            {
                if (TryGetComponent(out HelicopterCrashEvent helicopter))
                {
                    helicopter.Execute();
                }
            }
            if (GUI.Button(new Rect(160, 0, 160, 70), $"情報"))
            {
                if (TryGetComponent(out SendInformationEvent sendInformation))
                {
                    sendInformation.Execute("ぽんぽんぺいん");
                }
            }
            if (GUI.Button(new Rect(320, 0, 160, 70), $"パワーアップ"))
            {
                if (TryGetComponent(out CharacterPowerUpEvent powerUp))
                {
                    powerUp.Execute();
                }
            }
            if (GUI.Button(new Rect(480, 0, 160, 70), $"ダメージ"))
            {
                if (TryGetComponent(out DealingDamageEvent damage))
                {
                    damage.Execute();
                }
            }
            if (GUI.Button(new Rect(640, 0, 160, 70), $"罠"))
            {
                if (TryGetComponent(out TrapGenerateEvent trap))
                {
                    trap.Execute();
                }
            }
            if (GUI.Button(new Rect(800, 0, 160, 70), $"クマ"))
            {
                if (TryGetComponent(out FallingBearEvent bear))
                {
                    bear.Execute();
                }
            }
            if (GUI.Button(new Rect(960, 0, 160, 70), $"思考盗聴"))
            {
                if (TryGetComponent(out MindReadingEvent mindReading))
                {
                    mindReading.Execute();
                }
            }
            if (GUI.Button(new Rect(1120, 0, 160, 70), $"回復"))
            {
                if (TryGetComponent(out HealEvent heal))
                {
                    heal.Execute("『いぬ』");
                }
            }
            if (GUI.Button(new Rect(1280, 0, 160, 70), $"空中浮遊"))
            {
                if (TryGetComponent(out LevitationEvent levitation))
                {
                    levitation.Execute();
                }
            }

            // デバッグ用。
            if (GUI.Button(new Rect(1440, 0, 160, 70), $"クリナプ"))
            {
                AdventurerSpawner.TryFind(out AdventurerSpawner spawner);
                foreach (Adventurer adventurer in spawner.Spawned)
                {
                    if (adventurer != null) adventurer.Cleanup();
                }
            }
            if (GUI.Button(new Rect(1600, 0, 160, 70), $"狂気"))
            {
                AdventurerSpawner.TryFind(out AdventurerSpawner spawner);
                foreach (Adventurer adventurer in spawner.Spawned)
                {
                    if (adventurer != null) adventurer.Damage(0, default, "Madness");
                }
            }
        }

        IEnumerator AutoRepeatingAsync()
        {
            while (true)
            {
                int r = Random.Range(0, 5);
                if (r == 0)
                {
                    if (TryGetComponent(out CharacterPowerUpEvent powerUp))
                    {
                        powerUp.Execute();
                    }
                }
                else if (r == 1)
                {
                    if (TryGetComponent(out DealingDamageEvent damage))
                    {
                        damage.Execute();
                    }
                }
                else if (r == 2)
                {
                    if (TryGetComponent(out SendInformationEvent sendInformation))
                    {
                        sendInformation.Execute("ぽんぽんぺいん");
                    }
                }
                else if (r == 3)
                {
                    AdventurerSpawner.TryFind(out AdventurerSpawner spawner);
                    foreach (Adventurer adventurer in spawner.Spawned)
                    {
                        if (adventurer != null) adventurer.Damage(0, default, "Madness");
                    }
                }
                else if (r == 4)
                {
                    if (TryGetComponent(out MindReadingEvent mindReading))
                    {
                        mindReading.Execute();
                    }
                }

                yield return new WaitForSeconds(1);
            }
        }
    }
}