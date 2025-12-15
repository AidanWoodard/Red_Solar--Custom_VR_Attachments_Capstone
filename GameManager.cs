using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // game state vars
    [HideInInspector] public bool isGameActive = true;   // if false, player is dead, in menu, etc. (ie NOT PLAYING)

    // player objs
    public GameObject player;       // ref made in editor (not code)
    private Canvas playerDeathUI;
    private Canvas playerWinUI;
    [HideInInspector] public PlayerManager playerManagerScript;     // this script is in charge of health, etc.
    [HideInInspector] public PlayerController playerScript;         // controls and power up func

    // power up tracking
    [HideInInspector] public int powerLevel = 0; // builds to 'powerThreshold'
    public int powerThreshold;
    [HideInInspector] public bool isGunCharged = false;

    // enemy tracking
    [HideInInspector] public int activeEnemyCount = 0;

    // enemies
    private GameObject[] enemyControllers;   // enemies alive
    private GameObject[] untetheredEnemies;  // some enemies are not attached to their controllers

    void Start()
    {
        // get player manager script for health and deactivating the enemy
        if (player) {
            // player vars and code
            playerManagerScript = player.GetComponent<PlayerManager>();
            playerScript = player.GetComponent<PlayerController>();

            // UI canvases
            playerDeathUI = player.transform.Find("Camera Offset").gameObject.transform.Find("Main Camera").gameObject.transform.Find("Game Over Canvas").GetComponent<Canvas>();
            playerDeathUI.enabled = false;
            playerWinUI = player.transform.Find("Camera Offset").gameObject.transform.Find("Main Camera").gameObject.transform.Find("Game Won Canvas").GetComponent<Canvas>();
            playerWinUI.enabled = false;
            //Debug.Log(playerWinUI);
        } else
        {
            Debug.Log("NO PLAYER REFERENCE! please put in editor you ding-dong");
        }

        enemyControllers = GameObject.FindGameObjectsWithTag("EnemyController");
        activeEnemyCount = enemyControllers.Length;
    }

    void Update()
    {
        if (isGameActive)
        {
            // player has reached power level
            isGunCharged = (powerLevel >= powerThreshold);
            playerScript.isGunCharged = isGunCharged;

            // check for winning
            if (activeEnemyCount <= 0)
            {
                playerWinUI.enabled = true;
            }

            // deal with death :(
            if (playerManagerScript.playerHealth <= 0)
            {
                // remove all active enemies
                enemyControllers = GameObject.FindGameObjectsWithTag("EnemyController");
                foreach (var enemy in enemyControllers)
                {
                    Destroy(enemy);
                }

                // kill any disabled enemies too
                untetheredEnemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (var enemy in untetheredEnemies)
                {
                    Destroy(enemy);
                }

                // end round
                playerManagerScript.EndGame();
                isGameActive = false;

                // show UI
                playerDeathUI.enabled = true;
            }
        }
    }
}
