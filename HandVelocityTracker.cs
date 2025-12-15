using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandVelocityTracker : MonoBehaviour
{
    [HideInInspector]
    public float velocity;
    private Vector3 prevPos;

    void Update()
    {
        velocity = ((transform.position - prevPos).magnitude) / Time.deltaTime;
        prevPos = transform.position;
        //Debug.Log(Mathf.Round(velocity * 10.0f) * .1f);
    }
}
