using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class SharedInformation
    {
        public SharedInformation(BilingualString text, string source, float score) : this(text, source)
        {
            Score = score;
        }

        public SharedInformation(BilingualString text, string source)
        {
            Text = text;
            Source = source;
        }

        public SharedInformation(string japaneseText, string englishText, string source)
        {
            Text = new BilingualString(japaneseText, englishText);
            Source = source;
        }

        public BilingualString Text { get; }
        public string Source { get; }
        public float Score { get; set; }
    }
}