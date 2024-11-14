using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    // AI�����[���v���C�⎟�̍s����I�����邽�߂ɕK�v�ȏ����Q�Ƃ��邱�Ƃ��o����B
    // ���̃C���^�[�t�F�[�X�̃����o�ɁA�R���X�g���N�^�̈����ł��̃C���^�[�t�F�[�X���K�v�ȃN���X��ǉ�����ƁA
    // ����������₱�����Ȃ�̂Ŕ����邱�ƁB
    public interface IReadOnlyAdventurerContext : IStatusBarDisplayStatus, IProfileWindowDisplayStatus
    {
        public AdventurerSheet AdventurerSheet { get; }
        public IReadOnlyExploreRecord ExploreRecord { get; }
        public IReadOnlyList<string> AvailableActions { get; }
        public IReadOnlyCollection<string> ActionLog { get; }
        public Vector2Int Coords { get; }
        public int DefeatCount { get; }
        public int TreasureCount { get; }
        public int ElapsedTurn { get; }
    }
}
