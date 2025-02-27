using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.ItemData;

namespace Game
{
    public interface IScavengeable
    {
        // 鍵がかかっていた場合など、取得できなかった原因を返すことが出来る。
        public string Scavenge(Actor user, out Item item);
    }
}
