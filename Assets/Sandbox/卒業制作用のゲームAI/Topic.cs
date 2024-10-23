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
        _npcs[0].Talk("‰½‚©“K“–‚É„‚É¿–â‚µ‚Ä‚­‚¾‚³‚¢B");
    }
}
