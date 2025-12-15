using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class EnemyDroneDisabled : MonoBehaviour
{
    [HideInInspector]
    public EnemyDroneManager managerScript;

    private HandVelocityTracker handVelocityTracker;

    public ParticleSystem particleSystem;

    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public PlayerController playerScript;
    public InputActionReference leftShoot;  // for the player lasso input
    public InputActionReference rightShoot;

    public float setTimerToFree;
    private float timerToFree;

    public float minDistToGround;   // enemy must be this close to change back

    private Rigidbody rb;

    private bool canIBePunchedIHopeNot = true;  // true if punchable (yikes)
    public int punchCooldownFixSet;      // this is a small delay after an enemy is thrown that prevents it
    private int punchCooldownFix;       // from taking damage (the drone thinks it's hit, but it's thrown).

    private bool lassoed;
    public float lassoStrength;

    public float dragIncreaseRate = 0.3f;

    public int friendlyFireDamage;  // amt of health to deduct

    private bool adjustedStartPosYet = false;   // change pos at start of spawn

    [HideInInspector] public Vector3 targetPos;

    // audio
    private AudioSource audioPlayer;
    public AudioClip takeDamage1;
    public AudioClip takeDamage2;

    void Start()
    {
        // refs
        player = GameObject.FindWithTag("Player").gameObject;
        playerScript = player.GetComponent<PlayerController>();
        rb = gameObject.GetComponent<Rigidbody>();
        audioPlayer = GetComponent<AudioSource>();
        if (!rb) { Debug.Log("can't find rb"); }
        managerScript = gameObject.transform.parent.GetComponent<EnemyDroneManager>();
    }

    void FixedUpdate()
    {
        if (!adjustedStartPosYet)
        {
            transform.position += new Vector3(0, 1.5f, 0);
            adjustedStartPosYet = true;
        }

        // if have a parent obj
        if (transform.parent)
        {
            targetPos = managerScript.targetLassoPos;
        }

        // check lasso input
        if (leftShoot.action.ReadValue<float>() < 0.2f)
        {
            // if the player lets go of the lasso at all, we know THIS obj is not lassoed
            // but we can't confirm that when the player does click, they've lassoed THIS
            // object. So, lassoed can only be set true by the 'EnemyDroneManager.cs' script
            lassoed = false;
        }
        // if lassoed...
        if (lassoed)
        {
            // lasso
            FollowPointer(targetPos);
            rb.useGravity = false;
        } else if (timerToFree > 0)     // or if buffer time has not run out...
        {
            // start countdown
            timerToFree--;
            rb.useGravity = true;
            rb.drag = 0;
        }
        else
        {
            //Debug.Log(Vector3.Distance(player.transform.position, transform.position));
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -transform.up, out hit, minDistToGround))
            {
                if (hit.distance <= minDistToGround && Vector3.Distance(player.transform.position, transform.position) > 5.0f)
                {
                    // become normal robot if close enough to ground
                    if (managerScript)
                    {
                        managerScript.RespondToLassoDeactivated();
                    }
                    else
                    {
                        Debug.Log("manager script was missing ----- destroying gameobj");
                       // Destroy(gameObject);
                    }
                }
            }
        }
    }

    void Update()
    {
        if (playerScript.hasGrabbedEnemy)
        {
            punchCooldownFix = punchCooldownFixSet;
            canIBePunchedIHopeNot = false;
        }
        else if (!canIBePunchedIHopeNot)
        {
            punchCooldownFix--;
        } 
        
        if (punchCooldownFix <= 0)
        {
            canIBePunchedIHopeNot = true;
        }
        //Debug.Log("Can I be punched: " + canIBePunchedIHopeNot);
    }

    // move to target position of player's pointer when lassoed
    void FollowPointer(Vector3 pointerTarget)
    {
        // move to positon using addforce
        Vector3 newTarget = (pointerTarget - transform.position);
        rb.AddForce(newTarget.normalized * lassoStrength);
        if (rb.drag < 4.0f)
        {
            rb.drag += dragIncreaseRate;
        }
    }

    public void GetLassoed()
    {
        // Set to get lassoed or relasso this obj
        timerToFree = setTimerToFree;
        lassoed = true;
    }

    void OnTriggerEnter(Collider obj)
    {
        if (obj.gameObject.tag == "The Danger Zooone")
        {
            // kill self :(
            managerScript.TakeDamage((int)managerScript.enemyHealth);
        }

        // make sure 1) it's a hand 2) enemy can take damage and 3) player's hand is closed in a fist
        //if (obj.gameObject.tag == "Hand" && canIBePunchedIHopeNot && rightShoot.action.ReadValue<float>() > 0.2f)
        if (obj.gameObject.tag == "Hand" && rightShoot.action.ReadValue<float>() > 0.2f)
        {
                handVelocityTracker = obj.gameObject.GetComponent<HandVelocityTracker>();

            // make sure player punched hard enough
            if (handVelocityTracker.velocity > managerScript.punchThreshold)
            {
                managerScript.TakeDamage(managerScript.punchDamage);
                Play(takeDamage1);
                //particleSystem.Play();
            }
        } else if (!canIBePunchedIHopeNot)
        {
            //Debug.Log("can't be punched");
        } else if (rightShoot.action.ReadValue<float>() < 0.2f)
        {
            //Debug.Log("trigger isn't pressed");
        }

        if (obj.gameObject.tag == "EnemyProjectile")
        {
            managerScript.TakeDamage(friendlyFireDamage);
            Destroy(obj.gameObject);
        }

        if (obj.gameObject.tag == "PlayerProjectile")
        {
            // kill self
            managerScript.enemyHealth = 0;
            Destroy(obj.gameObject);
            Debug.Log("ive been shoooottt");
        }
        //Debug.Log(obj.gameObject.tag);
    }

    // play an audio clip/sfx
    void Play(AudioClip clip)
    {
        audioPlayer.clip = clip;
        audioPlayer.Play();
    }
}
