using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class FreeMovableCamera : MonoBehaviour
    {
        [SerializeField] float _moveSpeed = 5.0f;
        [SerializeField] float _rotateSpeed = 5.0f;

        CinemachineVirtualCamera _vcam;

        void Awake()
        {
            _vcam = GetComponent<CinemachineVirtualCamera>();
        }

        void Update()
        {
            Move();
            Rotate();
        }

        public static bool TryFind(out FreeMovableCamera result)
        {
            result = GameObject.FindGameObjectWithTag("BirdsEyeViewCamera").GetComponent<FreeMovableCamera>();
            return result != null;
        }

        public void SetPosition(Vector3 position)
        {
            _vcam.Follow.transform.position = position;
        }

        // 移動はFollowに指定したオブジェクトを動かす。
        void Move()
        {
            float hori = Input.GetAxisRaw("Horizontal");
            float vert = Input.GetAxisRaw("Vertical");
            Vector3 input = new Vector3(hori, 0, vert);
            input.Normalize();

            float dash = Input.GetKey(KeyCode.LeftShift) ? 1.0f : 2.0f;
            input *= dash;

            float cameraAngle = Camera.main.transform.eulerAngles.y;
            Quaternion forward = Quaternion.AngleAxis(cameraAngle, Vector3.up);

            Vector3 velo = forward * input * Time.deltaTime * _moveSpeed;
            _vcam.Follow.transform.Translate(velo);
        }

        // 回転はVcamのTransformの値を操作する。
        void Rotate()
        {
            int sign = 0;
            if (Input.GetKey(KeyCode.Q)) sign = 1;
            else if (Input.GetKey(KeyCode.E)) sign = -1;

            Vector3 euler = _vcam.transform.eulerAngles;
            euler.y += sign * Time.deltaTime * _rotateSpeed;

            _vcam.transform.rotation = Quaternion.Euler(euler);
        }
    }
}
