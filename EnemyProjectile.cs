using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private bool shotYet = false;
    public float timer = 1000;
    public float speed = 1000;

    void Update()
    {
        if (!shotYet)
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * speed);
            shotYet = true;
        }
        timer -= 1;
        if (timer <= 0)
        {
            Destroy(gameObject);    // kys
        }
    }

    void OnTriggerEnter(Collider obj)
    {
        if (obj.gameObject.tag == "Shield")
        {
            Destroy(gameObject);    // kys
        }
    }
}
