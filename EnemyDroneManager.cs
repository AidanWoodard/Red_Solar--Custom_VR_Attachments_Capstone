using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDroneManager : MonoBehaviour
{
    // this script manages health, NOT any other enemy scripts
    public float enemyHealth;
    public int punchDamage;

    private GameManager gameManagerScript;
    private EnemyDroneBehavior activeEnemyScript;

    // enemy types to spawn/destroy
    public GameObject activeEnemyPrefab;
    public GameObject disabledEnemyPrefab;

    // enemies active (should never be both)
    private GameObject activeEnemy;
    private GameObject disabledEnemy;

    public float punchThreshold = 1.0f;   // minimum speed of hand to count as a punch    

    [SerializeField] private AudioClip deathSound;
    private AudioSource audioPlayer;

    // target pos of player's lasso
    [HideInInspector] public Vector3 targetLassoPos;

    void Awake()
    {
        activeEnemy = transform.Find("EnemyDrone").gameObject;
        gameManagerScript = GameObject.Find("GAME MANAGER").GetComponent<GameManager>();
        audioPlayer = GetComponent<AudioSource>();
    }

    void Update()
    {
        CheckDeath();
        //Debug.Log(enemyHealth);
    }

    // called by player when lassoed
    public void RespondToLassoActivated()
    {
        // destroy drone, spawn disabled drone
        if (!disabledEnemy)
        {
            // spawn disabled drone as child of manager
            disabledEnemy = GameObject.Instantiate(disabledEnemyPrefab, activeEnemy.transform.position, activeEnemy.transform.rotation) as GameObject;
            disabledEnemy.transform.SetParent(this.transform);
            //disabledEnemy.transform.position += new Vector3(0, 1.5, 0);
            //Debug.Log("disabled enemy not found, so spawning one");
        }
        if (activeEnemy)
        {
            // get rid of ai agent
            Destroy(activeEnemy);
            //Debug.Log("destroying active enemy");
        }

        // start lasso tracker script and timer
        if (disabledEnemy)
        {
            disabledEnemy.gameObject.GetComponent<EnemyDroneDisabled>().GetLassoed();
            //Debug.Log("disabled enemy exists and is getting lassoed");
        } else
        {
            Debug.Log("Can't find disabled enemy obj!!");
        }
    }

    // called by disabled enemy when lasso timer runs out
    // (meaning the enemy has been out of the lasso for x seconds)
    public void RespondToLassoDeactivated()
    {
        // destroy disabled drone, spawn normal drone
        activeEnemy = Instantiate(activeEnemyPrefab, disabledEnemy.transform.position, disabledEnemy.transform.rotation) as GameObject;
        activeEnemy.transform.parent = this.transform;
        activeEnemyScript = activeEnemy.GetComponent<EnemyDroneBehavior>();
        activeEnemyScript.managerScript = this;
        Destroy(disabledEnemy);
    }

    void CheckDeath()
    {
        if (enemyHealth <= 0)
        {
            // (kill enemy)

            // death audio
            audioPlayer.clip = deathSound;
            audioPlayer.loop = false;
            audioPlayer.Play();

            // update game manager
            gameManagerScript.powerLevel += 5;
            gameManagerScript.activeEnemyCount -= 1;

            // commit suicide without hesitation and be a good drone :(
            if (disabledEnemy) { Destroy(disabledEnemy); }
            Destroy(gameObject);
        }
    }

    // called by sub-enemy scripts
    public void TakeDamage(int damageAmt)
    {
        enemyHealth -= damageAmt;
        // play anim
    }

    public void AddHealth(int healthAmt)
    {
        enemyHealth += healthAmt;
        // play anim
    }
}
