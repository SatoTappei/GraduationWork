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

            if (GUI.Button(new Rect(300, 0, 300, 70), $"���"))
            {
                if (TryGetComponent(out SendInformationEvent sendInformation))
                {
                    sendInformation.Execute("�ۂ�ۂ�؂���");
                }
            }
            if (GUI.Button(new Rect(600, 0, 300, 70), $"�p���[�A�b�v"))
            {
                if (TryGetComponent(out CharacterPowerUpEvent powerUp))
                {
                    powerUp.Execute();
                }
            }
            if (GUI.Button(new Rect(900, 0, 300, 70), $"�_���[�W"))
            {
                if (TryGetComponent(out DealingDamageEvent damage))
                {
                    damage.Execute();
                }
            }
            if (GUI.Button(new Rect(1200, 0, 300, 70), $"�"))
            {
                if (TryGetComponent(out TrapGenerateEvent trap))
                {
                    trap.Execute();
                }
            }
        }
    }
}