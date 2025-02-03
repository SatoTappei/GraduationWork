using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class MadnessStatusEffect : StatusEffect
    {
        [SerializeField] ParticleSystem _particle;

        Adventurer _adventurer;

        int _turn;
        bool _isValid;

        public override string Description => "�����������B";
        public override bool IsValid => _isValid;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
        }

        public void Set()
        {
            _particle.Play();

            if (_isValid)
            {
                // ���ɓK�p����Ă���ꍇ�͌��ʎ��ԉ����B
                _turn = 10; // �K���ȃ^�[�����B
            }
            else
            {
                _isValid = true;
                _turn = 10; // �K���ȃ^�[�����B

                GameLog.Add(
                    "�V�X�e��",
                    $"�C���������B",
                    LogColor.White,
                    _adventurer.Sheet.DisplayID
                );
            }
        }

        public void Remove()
        {
            _particle.Stop();

            if (_isValid)
            {
                _isValid = false;
            }
        }

        public override void Apply()
        {
            if (_isValid)
            {
                _turn--;
                _turn = Mathf.Max(_turn, 0);

                if (_turn == 0) Remove();
            }
        }
    }
}
