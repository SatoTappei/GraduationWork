using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface IScavengeable
    {
        // 箱の状態や取得したアイテムなど、漁った結果を返せるようにしておく。
        public string Scavenge();
    }
}
