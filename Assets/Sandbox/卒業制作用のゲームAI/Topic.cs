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
        _npcs[0].Talk("�����K���Ɏ��Ɏ��₵�Ă��������B");
    }
}
