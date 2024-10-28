using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurerSpawner : MonoBehaviour
{
    [SerializeField] GameObject _prefab;

    GameObject[] _spawned;
    WaitForSeconds _interval;

    void Awake()
    {
        // 冒険者の最大数、増やすことも可能だがリクエストが増えると料金もかかる。
        _spawned = new GameObject[4]; 
    }

    void Start()
    {
        StartCoroutine(UpdateAsync());
    }

    void OnGUI()
    {
        GUIStyle style = GUI.skin.GetStyle("button");
        style.fontSize = 25;

        if (GUI.Button(new Rect(0, 0, 300, 70), "冒険者を生成"))
        {
            Spawn();
        }
    }

    // 一定間隔で冒険者を生成する。
    IEnumerator UpdateAsync()
    {
        const float Interval = 1.0f;

        while (true)
        {
            Spawn();

            _interval ??= new WaitForSeconds(Interval);
            yield return _interval;
        }
    }

    // シーン上に存在する冒険者が最大数未満の場合は生成する。
    void Spawn()
    {
        for (int i = 0; i < _spawned.Length; i++)
        {
            if (_spawned[i] == null)
            {
                _spawned[i] = Instantiate(_prefab);
                break;
            }
        }
    }
}