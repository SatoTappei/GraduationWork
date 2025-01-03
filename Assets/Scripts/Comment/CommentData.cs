using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CommentData
    {
        public CommentData(string name, string comment)
        {
            Name = name;
            Comment = comment;
        }

        public string Name { get; }
        public string Comment { get; }
    }
}
