using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [HideInInspector] public bool controlsEnabled = true;   // combat ctrls
    public int playerHealthSet;    // changed in editor
    [HideInInspector] public int playerHealth;   // changes (set to 'playerHealthSet')
    public int enemyBulletDamage;

    void Start()
    {
        playerHealth = playerHealthSet;
    }

    void Update()
    {
        //Debug.Log(playerHealth);
    }

    // react to getting hit by bullet
    void OnTriggerEnter(Collider obj)
    {
        if (obj.gameObject.tag == "EnemyProjectile")
        {
            playerHealth -= enemyBulletDamage;
        }
    }

    // called by 'GameManager.cs'
    public void EndGame()
    {
        controlsEnabled = true;
        playerHealth = playerHealthSet;
    }
}
