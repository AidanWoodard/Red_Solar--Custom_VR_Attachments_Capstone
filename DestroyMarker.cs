using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMarker : MonoBehaviour
{
    public float timer = 100;

    void Update()
    {
        timer -= 1;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }
}
