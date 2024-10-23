using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Topic : MonoBehaviour
{
    Npc[] _npcs;

    void Awake()
    {
        _npcs = FindObjectsByType<Npc>(FindObjectsSortMode.None);
    }

    void Start()
    {
        _npcs[0].Talk("何か適当に私に質問してください。");
    }
}
