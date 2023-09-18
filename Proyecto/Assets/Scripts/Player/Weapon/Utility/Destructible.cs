using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public GameObject destroyedVersion;

    public void Destroy()
    {
        Instantiate(destroyedVersion, transform.position, transform.rotation); // Spawn a shattered object
        Destroy(gameObject); // Remove the current object
    }
}
