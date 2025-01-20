using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.ItemData;

namespace Game
{
    public interface IScavengeable
    {
        // Œ®‚ª‚©‚©‚Á‚Ä‚¢‚½ê‡‚È‚ÇAæ“¾‚Å‚«‚È‚©‚Á‚½Œ´ˆö‚ğ•Ô‚·‚±‚Æ‚ªo—ˆ‚éB
        public string Scavenge(Actor user, out Item item);
    }
}
