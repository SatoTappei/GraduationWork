using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "TerrainFeaturesData", fileName = "TerrainFeaturesData_")]
    public class TerrainFeaturesData : ScriptableObject
    {
        // BilingualString�^�ł͂Ȃ��Astring�^��2�Ȃ̂ŁA�G�f�B�^�[����l��ݒ肷��ۂɃg�O�����X�g�ɂȂ炸�A���₷���B
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
                // ���̏�񂪐M���ł��邩�̔��f��AI�ɔC���邽�߁AScore�̒l�͐ݒ肵�Ȃ��B
                return new SharedInformation(Japanese, English, "System", ValidTurn);
            }
        }

        [SerializeField] Data[] _data;

        public IReadOnlyList<Data> AllData => _data;
    }
}
