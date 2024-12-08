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

            if (GUI.Button(new Rect(0, 0, 300, 70), $"ヘリコタプー"))
            {
                if (TryGetComponent(out HelicopterCrashEvent helicopter))
                {
                    helicopter.Execute();
                }
            }
            if (GUI.Button(new Rect(300, 0, 300, 70), $"情報"))
            {
                if (TryGetComponent(out SendInformationEvent sendInformation))
                {
                    sendInformation.Execute("ぽんぽんぺいん");
                }
            }
            if (GUI.Button(new Rect(600, 0, 300, 70), $"パワーアップ"))
            {
                if (TryGetComponent(out CharacterPowerUpEvent powerUp))
                {
                    powerUp.Execute();
                }
            }
            if (GUI.Button(new Rect(900, 0, 300, 70), $"ダメージ"))
            {
                if (TryGetComponent(out DealingDamageEvent damage))
                {
                    damage.Execute();
                }
            }
            if (GUI.Button(new Rect(1200, 0, 300, 70), $"罠"))
            {
                if (TryGetComponent(out TrapGenerateEvent trap))
                {
                    trap.Execute();
                }
            }
            if (GUI.Button(new Rect(1500, 0, 300, 70), $"クマ"))
            {
                if (TryGetComponent(out FallingBearEvent bear))
                {
                    bear.Execute();
                }
            }
        }
    }
}