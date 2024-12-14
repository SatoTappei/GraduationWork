using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class BackgroundCamera : MonoBehaviour
    {
        CinemachineVirtualCamera _vcam;

        void Awake()
        {
            _vcam = GetComponent<CinemachineVirtualCamera>();
        }

        void Update()
        {
            _vcam.transform.RotateAround(_vcam.transform.position, Vector3.up, Time.deltaTime);
        }
    }
}
