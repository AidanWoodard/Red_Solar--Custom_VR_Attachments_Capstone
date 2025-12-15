using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public bool handsConnected = false;     // set by 'ArduinoListener.cs'

    public InputActionReference rightShoot;
    public InputActionReference leftShoot;

    public GameObject bullet;   // prefab

    public LayerMask whatIsEnemy;   // layer of enemy collider

    private GameObject _leftHand;
    private GameObject _rightHand;
    private Transform _leftSpawn;
    private Transform _rightSpawn;

    // Spawn point of both hands if they're connected
    private Transform _doubleSpawn;

    // set to true by 'GameManager.cs' if power level is at threshold.
    // player can now put their hands together and shoot a bunch of enemies
    [HideInInspector] public bool isGunCharged = false;

    // set mode of laser lasso
    public Transform lassoTarget;
    private bool useLasso = false;
    private GameObject lassoedEnemy;
    private EnemyDroneManager lassoedEnemyDroneScript;
    private EnemyDroneDisabled disabledEnemyDroneScript;

    private bool _leftShot = false;     // no rapid fire
    private bool _rightShot = false;

    // True if player is grabbing an enemy. If so, don't let the enemy take damage from punch.
    // This solves the problem where if an enemy is thrown, it counts the release as a
    // punch. No enemies can be punched while grabbing though, since all check for this! 
    public bool hasGrabbedEnemy = false;
    private bool canGrab = false;

    // audio
    public AudioSource audioSource;
    public AudioClip gunSound;

    void Start()
    {
        _leftHand = gameObject.transform.Find("Camera Offset").gameObject.transform.Find("Left Controller").gameObject;
        _leftSpawn = _leftHand.transform.Find("Spawn Point");
        _rightHand = gameObject.transform.Find("Camera Offset").gameObject.transform.Find("Right Controller").gameObject;
        _rightSpawn = _rightHand.transform.Find("Spawn Point");     // just in case needed

        _doubleSpawn = _leftHand.transform.Find("Double Spawn Point");      // spawn point of both hands
    }

    void FixedUpdate()
    {
        if (useLasso)
        {
            _ActivateLasso();
        }
    }

    void Update()
    {
        //Debug.Log(handsConnected);
        if (rightShoot.action.ReadValue<float>() > 0.5 && canGrab)
        {
            hasGrabbedEnemy = true;
        }
        else if (rightShoot.action.ReadValue<float>() < 0.5)
        {
            hasGrabbedEnemy = false;
        }
        //Debug.Log("Player has grabbed enemy: " + hasGrabbedEnemy);
        //Debug.Log(handsConnected);
        if (isGunCharged && handsConnected && rightShoot.action.ReadValue<float>() > 0.5)   // only allow right shot if connected
        {
            if (!_rightShot)
            {
                _Shoot(_doubleSpawn);     // spawn at shared spawn point
                _rightShot = true;
            }
        }
        else { _rightShot = false; }

        if (leftShoot.action.ReadValue<float>() > 0.5)
        {
            if (!_leftShot)
            {
                if (isGunCharged && handsConnected) // if connected, shoot lasers at shared spawn point (different angel)
                { 
                    _Shoot(_doubleSpawn); 
                }
                else // else, spawn laser lasso
                {
                    //_Shoot(_leftSpawn); 
                    // make a new hit obj and shoot a ray out of the left hand for any obj in the enemy layer ('whatIsEnemy')
                    RaycastHit hit;
                    GameObject hitObj;
                    if (Physics.Raycast(_leftHand.transform.position, _leftHand.transform.TransformDirection(Vector3.forward), out hit, 20.0f, whatIsEnemy))
                    {
                        // get obj
                        hitObj = hit.collider.gameObject;

                        // turn on laser lasso
                        useLasso = true;

                        // find the parent of the parent of the hit obj (most likely hit child 3d model, and we want the manager script)
                        lassoedEnemy = GameObject.Find(hitObj.transform.root.name);
                        //Debug.Log(lassoedEnemy);

                        if (lassoedEnemy.name == "EnemyDroneDisabled")
                        {
                            // this obj is already disabled
                            disabledEnemyDroneScript = lassoedEnemy.GetComponent<EnemyDroneDisabled>();

                            // reset lasso timer (so it doesn't change back)
                            disabledEnemyDroneScript.GetLassoed();
                        } else if (lassoedEnemy.tag == "EnemyController")
                        {
                            // activate code in the script of the enemy
                            lassoedEnemyDroneScript = lassoedEnemy.GetComponent<EnemyDroneManager>();

                            // change state of enemy drone
                            lassoedEnemyDroneScript.RespondToLassoActivated();
                        }

                        // set initial dist between drone and lasso
                        //lassoTarget.position = hitObj.transform.parent.gameObject.transform.position + new Vector3(0, 1, 0);
                    } else
                    {
                        //Debug.Log("missed enemy");
                    }
                }   
                _leftShot = true;
            }
        }
        else 
        {
            _leftShot = false;
            _DeactivateLasso();
        }
    }
    
    public void _Shoot(Transform spawnPoint)
    {
        GameObject newBullet = Instantiate(bullet, spawnPoint.position, spawnPoint.transform.rotation);
        audioSource.PlayOneShot(gunSound, .7f);
    }

    // called multiple times
    public void _ActivateLasso()
    {
        // received by 'EnemyDroneManager.cs'
        Vector3 newTarget;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, lassoTarget.position, out hit, 10.0f))
        {
            newTarget = hit.transform.position;
            //Debug.Log("hit ground");
            //Debug.DrawRay(transform.position, hit.transform.position, Color.green);
        }
        else
        {
            newTarget = lassoTarget.position;
        }

        if (lassoedEnemyDroneScript) { lassoedEnemyDroneScript.targetLassoPos = newTarget; } 
        else if (disabledEnemyDroneScript) { disabledEnemyDroneScript.targetPos = newTarget; }
    }

    // called once
    public void _DeactivateLasso()
    {
        //Debug.Log("Deactivating Lasso");
        lassoedEnemy = null;
        lassoedEnemyDroneScript = null;
        disabledEnemyDroneScript = null;
        useLasso = false;
    }

    void OnTriggerEnter(Collider obj)
    {
        if (obj.gameObject.tag == "Enemy" && rightShoot.action.ReadValue<float>() < 0.5)
        {
            canGrab = true;
        }
    }

    void OnTriggerExit(Collider obj)
    {
        if (obj.gameObject.tag == "Enemy")
        {
            canGrab = false;
        }
    }
}
