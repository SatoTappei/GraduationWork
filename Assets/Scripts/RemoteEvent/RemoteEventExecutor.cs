using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class RemoteEventExecutor : MonoBehaviour
    {
        void Start()
        {
            //StartCoroutine(UpdateAsync());
        }

        IEnumerator UpdateAsync()
        {
            while (true)
            {
                int r = Random.Range(0, 5);
                if (r == 0)
                {
                    if (TryGetComponent(out SendInformationEvent sendInformation))
                    {
                        sendInformation.Execute("ぽんぽんぺいん");
                    }
                }
                if (r == 1)
                {
                    if (TryGetComponent(out CharacterPowerUpEvent powerUp))
                    {
                        powerUp.Execute();
                    }
                }
                if (r == 2)
                {
                    if (TryGetComponent(out DealingDamageEvent damage))
                    {
                        damage.Execute();
                    }
                }
                if (r == 3)
                {
                    if (TryGetComponent(out TrapGenerateEvent trap))
                    {
                        trap.Execute();
                    }
                }
                if (r == 4)
                {
                    if (TryGetComponent(out FallingBearEvent bear))
                    {
                        bear.Execute();
                    }
                }

                yield return new WaitForSeconds(1.0f);
            }
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
            if (GUI.Button(new Rect(1120, 0, 160, 70), $"空中浮遊"))
            {
                if (TryGetComponent(out LevitationEvent levitation))
                {
                    levitation.Execute();
                }
            }
        }
    }
}