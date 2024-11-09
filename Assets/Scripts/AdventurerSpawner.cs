using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class AdventurerSpawner : MonoBehaviour
    {
        SpreadSheetLoader _spreadSheetLoader;
        AvatarCustomizer _avatarCustomizer;

        Adventurer[] _spawned;
        WaitForSeconds _interval;

        void Awake()
        {
            _spreadSheetLoader = GetComponent<SpreadSheetLoader>();
            _avatarCustomizer = GetComponent<AvatarCustomizer>();

            // 冒険者の最大数、UIのデザインも変更する必要があり面倒なので、とりあえず4で固定。
            _spawned = new Adventurer[4];
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
            while (true)
            {
                Spawn();
                yield return _interval ??= new WaitForSeconds(1.0f); // 適当な秒数。
            }
        }

        // シーン上に存在する冒険者が最大数未満の場合は生成する。
        void Spawn()
        {
            for (int i = 0; i < _spawned.Length; i++)
            {
                if (_spawned[i] != null) continue;

                _spawned[i] = CreateRandomAdventurer();
                break;
            }
        }

        // スプレッドシートからランダムなデータを選択し、冒険者を生成。
        Adventurer CreateRandomAdventurer()
        {
            if (_spreadSheetLoader.IsLoading) return null;

            IReadOnlyList<SpreadSheetData> profiles = _spreadSheetLoader.Profiles;
            int random = Random.Range(0, profiles.Count);
            SpreadSheetData profile = profiles[random];

            AvatarCustomizeData data = _avatarCustomizer.GetCustomizedData(profile);
            return Instantiate(data.Prefab);
        }
    }
}