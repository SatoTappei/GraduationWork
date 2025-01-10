using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class AltarPillar : DungeonEntity
    {
        void Start()
        {
            // 祭壇自体は実行時に生成せず、シーンに配置されている。
            // 祭壇の柱がある座標をキャラクター達が回避するように指定するだけで良い。
            DungeonManager.AddAvoidCell(Coords);
        }
    }
}
