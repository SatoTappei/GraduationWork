using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ItemData
{
    public class Artifact : Item
    {
        public Artifact() : base(GetRandomName(), "Artifact", Usage.None) { }

        static string GetRandomName()
        {
            int r = Random.Range(0, 3);
            string type = "";
            if (r == 0) type = "★";
            else if (r == 1) type = "●";
            else if (r == 2) type = "◆";

            return $"{type}アーティファクト";
        }
    }
}
