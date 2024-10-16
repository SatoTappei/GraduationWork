using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class キャラクターたくさん生成テスト : MonoBehaviour
{
    [SerializeField] GameObject _prefab;

    void Start()
    {
        StartCoroutine(UpdateAsync());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            DungeonManager dm = DungeonManager.Find();
            foreach (Cell c in dm.GetCells("Kaduki"))
            {
                GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                g.transform.position = c.Position;
            }
        }
    }

    IEnumerator UpdateAsync()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject g = Instantiate(_prefab);

            yield return new WaitForSeconds(1.0f);
        }
    }
}

// たくさん生成は出来た。宝箱へ行って帰るだけなら特に異常なし。