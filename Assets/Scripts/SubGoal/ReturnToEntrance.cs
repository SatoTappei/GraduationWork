using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ReturnToEntrance : SubGoal
    {
        public static readonly BilingualString StaticText;

        static ReturnToEntrance()
        {
            string j = "ダンジョンの入口に戻る。";
            string e = "Return to the entrance.";
            StaticText = new BilingualString(j, e);
        }

        public ReturnToEntrance(Adventurer owner) : base(owner) { }

        public override BilingualString Text => StaticText;

        public override bool IsCompleted()
        {
            return Blueprint.Interaction[Owner.Coords.y][Owner.Coords.x] == '<';
        }

        public override IEnumerable<string> GetAdditionalActions()
        {
            yield return "Return To Entrance";
        }
    }
}
