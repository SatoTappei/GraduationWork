using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class RemoteEventExecutor : MonoBehaviour
    {
        void OnGUI()
        {
            GUIStyle style = GUI.skin.GetStyle("button");
            style.fontSize = 25;

            if (GUI.Button(new Rect(0, 0, 160, 70), $"�w���R�^�v�["))
            {
                if (TryGetComponent(out HelicopterCrashEvent helicopter))
                {
                    helicopter.Execute();
                }
            }
            if (GUI.Button(new Rect(160, 0, 160, 70), $"���"))
            {
                if (TryGetComponent(out SendInformationEvent sendInformation))
                {
                    sendInformation.Execute("�ۂ�ۂ�؂���");
                }
            }
            if (GUI.Button(new Rect(320, 0, 160, 70), $"�p���[�A�b�v"))
            {
                if (TryGetComponent(out CharacterPowerUpEvent powerUp))
                {
                    powerUp.Execute();
                }
            }
            if (GUI.Button(new Rect(480, 0, 160, 70), $"�_���[�W"))
            {
                if (TryGetComponent(out DealingDamageEvent damage))
                {
                    damage.Execute();
                }
            }
            if (GUI.Button(new Rect(640, 0, 160, 70), $"�"))
            {
                if (TryGetComponent(out TrapGenerateEvent trap))
                {
                    trap.Execute();
                }
            }
            if (GUI.Button(new Rect(800, 0, 160, 70), $"�N�}"))
            {
                if (TryGetComponent(out FallingBearEvent bear))
                {
                    bear.Execute();
                }
            }
            if (GUI.Button(new Rect(960, 0, 160, 70), $"�v�l����"))
            {
                if (TryGetComponent(out MindReadingEvent mindReading))
                {
                    mindReading.Execute();
                }
            }
            if (GUI.Button(new Rect(1120, 0, 160, 70), $"��"))
            {
                if (TryGetComponent(out HealEvent heal))
                {
                    heal.Execute("�w���ʁx");
                }
            }
            if (GUI.Button(new Rect(1280, 0, 160, 70), $"�󒆕��V"))
            {
                if (TryGetComponent(out LevitationEvent levitation))
                {
                    levitation.Execute();
                }
            }

            // �f�o�b�O�p�B
            if (GUI.Button(new Rect(1440, 0, 160, 70), $"�N���i�v"))
            {
                AdventurerSpawner.TryFind(out AdventurerSpawner spawner);
                foreach (Adventurer adventurer in spawner.Spawned)
                {
                    if (adventurer != null) adventurer.Cleanup();
                }
            }
        }
    }
}