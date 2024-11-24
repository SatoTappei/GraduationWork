using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CommentPool : MonoBehaviour
    {
        const int Size = 100;

        [SerializeField] GameObject _prefab;
        [SerializeField] Transform _parent;

        GameObject[] _pool;

        void Awake()
        {
            _pool = new GameObject[Size];
            for (int i = 0; i < _pool.Length; i++)
            {
                _pool[i] = Instantiate(_prefab);
                _pool[i].transform.SetParent(_parent);
                _pool[i].SetActive(false);
            }
        }

        public bool TryPop(out DisplayedComment comment)
        {
            for (int i = 0; i < _pool.Length; i++)
            {
                // 非アクティブなものを返す。
                if (!_pool[i].activeSelf)
                {
                    _pool[i].SetActive(true);
                    comment = _pool[i].GetComponent<DisplayedComment>();

                    return true;
                }
            }

            comment = null;
            return false;
        }
    }
}
