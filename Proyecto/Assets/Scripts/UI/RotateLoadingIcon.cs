using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLoadingIcon : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 200;

    void LateUpdate()
    {
        float Z = rotationSpeed * Time.deltaTime;

        gameObject.transform.Rotate(0, 0, -Z);
    }
}
