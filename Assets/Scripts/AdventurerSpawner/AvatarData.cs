using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class AvatarData
    {
        [SerializeField] Sprite _icon;
        [SerializeField] Adventurer _prefab;

        public Sprite Icon => _icon;
        public Adventurer Prefab => _prefab;
    }
}
