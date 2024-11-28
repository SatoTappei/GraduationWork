using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ObjectPool<T> : MonoBehaviour where T : Component
    {
        [SerializeField] GameObject _prefab;
        [SerializeField] int _size = 10;
        [SerializeField] string _poolName;

        Transform _parent;
        GameObject[] _pool;

        void Awake()
        {
            _parent = CreateParent();
            _parent.name = _poolName;

            _pool = new GameObject[_size];
            for (int i = 0; i < _pool.Length; i++)
            {
                _pool[i] = Instantiate(_prefab);
                _pool[i].transform.SetParent(_parent);
                _pool[i].SetActive(false);
            }
        }

        protected virtual Transform CreateParent()
        {
            return new GameObject().transform;
        }

        public bool TryPop(out T comment)
        {
            for (int i = 0; i < _pool.Length; i++)
            {
                // 非アクティブなものを返す。
                if (!_pool[i].activeSelf)
                {
                    _pool[i].SetActive(true);
                    comment = _pool[i].GetComponent<T>();

                    return true;
                }
            }

            comment = null;
            return false;
        }
    }
}
