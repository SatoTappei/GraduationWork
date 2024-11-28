using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "TerrainFeaturesData", fileName = "TerrainFeaturesData_")]
    public class TerrainFeaturesData : ScriptableObject
    {
        // BilingualString型ではなく、string型が2つなので、エディターから値を設定する際にトグルリストにならず、見やすい。
        [System.Serializable]
        public class Data
        {
            [SerializeField] Vector2Int _coords;
            [SerializeField] string _japanese;
            [SerializeField] string _english;
            [SerializeField] int _validTurn;

            public Vector2Int Coords => _coords;
            public string Japanese => _japanese;
            public string English => _english;
            public int ValidTurn => _validTurn;

            public SharedInformation ToSharedInformation()
            {
                // この情報が信頼できるかの判断はAIに任せるため、Scoreの値は設定しない。
                return new SharedInformation(Japanese, English, "System", ValidTurn);
            }
        }

        [SerializeField] Data[] _data;

        public IReadOnlyList<Data> AllData => _data;
    }
}
