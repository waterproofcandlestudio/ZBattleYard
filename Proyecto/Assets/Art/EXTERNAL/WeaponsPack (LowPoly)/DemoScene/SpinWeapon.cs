using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGAssets
{
    public class SpinWeapon : MonoBehaviour
    {
        public bool isActive = true;

        public float factor = 1f;
        public Vector3 angularSpeed = new Vector3( 0, 100, 0);

        void Update()
        {
            if (isActive) transform.Rotate(factor * angularSpeed * Time.deltaTime);
        }
    }
}