using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class AltarPillar : DungeonEntity
    {
        void Start()
        {
            // �Ւd���͎̂��s���ɐ��������A�V�[���ɔz�u����Ă���B
            // �Ւd�̒���������W���L�����N�^�[�B���������悤�Ɏw�肷�邾���ŗǂ��B
            DungeonManager.AddAvoidCell(Coords);
        }
    }
}
