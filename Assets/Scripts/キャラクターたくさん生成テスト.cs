using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class キャラクターたくさん生成テスト : MonoBehaviour
{
    [SerializeField] GameObject _prefab;

    bool _isSpawned;

    void OnGUI()
    {
        GUIStyle stylebutton = GUI.skin.GetStyle("button");
        stylebutton.fontSize = 25;

        if (!_isSpawned && GUI.Button(new Rect(0, 0, 300, 70), "冒険者を生成"))
        {
            StartCoroutine(UpdateAsync());
        }
    }

    IEnumerator UpdateAsync()
    {
        _isSpawned = true;

        for (int i = 0; i < 4; i++)
        {
            Instantiate(_prefab);
            yield return new WaitForSeconds(1.0f);
        }
    }
}