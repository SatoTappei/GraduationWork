using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.ItemData;

namespace Game
{
    public interface IScavengeable
    {
        // �����������Ă����ꍇ�ȂǁA�擾�ł��Ȃ�����������Ԃ����Ƃ��o����B
        public string Scavenge(Actor user, out Item item);
    }
}
