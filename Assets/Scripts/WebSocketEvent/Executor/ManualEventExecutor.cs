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
            _spawner = AdventurerSpawner.Find();
        }

        void OnGUI()
        {
            GUIStyle style = GUI.skin.GetStyle("button");
            style.fontSize = 25;

            if (IsButtonClicked(0, "��Ұ��"))
            {
                _dealingDamage.Execute();
            }
            else if (IsButtonClicked(1, "�ϻ�"))
            {
                _fallingBear.Execute();
            }
            else if (IsButtonClicked(2, "��"))
            {
                if (_spawner.Spawned.Count > 0)
                {
                    int r = Random.Range(0, _spawner.Spawned.Count);
                    _heal.Execute(_spawner.Spawned[r].AdventurerSheet.FullName);
                }
            }
            else if (IsButtonClicked(3, "�غ����"))
            {
                _helicopterCrash.Execute();
            }
            else if (IsButtonClicked(4, "�����ذ���ި"))
            {
                _levitation.Execute();
            }
            else if (IsButtonClicked(5, "�v�l����"))
            {
                _mindReading.Execute();
            }
            else if (IsButtonClicked(6, "��ܰ����"))
            {
                _powerUp.Execute();
            }
            else if (IsButtonClicked(7, "�V�["))
            {
                string[] texts =
                {
                    "(�L�E�ցE�M)������",
                    "(�L�E�ցE�M)�o�ׂ�[",
                    "����Ԃ�"
                };
                int r = Random.Range(0, texts.Length);
                _revelation.Execute(texts[r]);
            }
            else if (IsButtonClicked(8, "�"))
            {
                _trapGenerate.Execute();
            }

            // �f�o�b�O�p�B
            if (IsButtonClicked(9, "AI������"))
            {
                foreach (Adventurer adventurer in _spawner.Spawned)
                {
                    if (adventurer != null) adventurer.Reboot();
                }
            }
            else if (IsButtonClicked(10, "���C"))
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