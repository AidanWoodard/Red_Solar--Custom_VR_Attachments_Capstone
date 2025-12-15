using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class EnemyDroneBehavior : MonoBehaviour
{
    [HideInInspector]
    public UnityEngine.AI.NavMeshAgent agent;

    public float maxTimeMoving;  // maximum time for moving to position before resetting position (agent is stuck)
    private float maxTimeMovingCount;

    public GameObject marker;
    public bool useMarkers = false;

    [HideInInspector]
    public Transform player;
    [HideInInspector]
    public EnemyDroneManager managerScript;

    private NavMeshSurface navSurface;

    public LayerMask whatIsGround, whatIsPlayer;

    public GameObject subBody;  // obj that reps drones rn
    private Renderer body;      // actual material to change

    // Idle movment
    private Vector3 target;
    bool isTargetSet;
    public float targetSetRange;    // range of positions to move to
    public float targetSetRangeHeight;  // random addition to height
    public float maxTimeBtwnMoves;
    public float minTimeBtwnMoves;  // random range set for waittime

    float waitTime = 0.0f;
    bool searchingYet;  // true if looking for target pos in idle

    // Attacking
    public float timeBtwnAttacks;
    float attackTimer;
    bool attackedYet;
    public GameObject enemyBulletPrefab;
    public Transform spawnPt;

    // State controls:
    public float sightRange; 
    private bool inSightRange;   // change to attack state

    public float attackRange;
    private bool inAttackRange;  // begin attacks

    private bool chasingYet;    // true when set dest to player

    // audio
    private AudioSource audioPlayer;
    public AudioClip shootSound;

    void Awake()
    {
        // game objs
        player = GameObject.FindWithTag("Player").transform;
        navSurface = GameObject.Find("Ground").GetComponent<NavMeshSurface>();
        managerScript = gameObject.transform.root.GetComponent<EnemyDroneManager>();

        // navmesh
        agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();

        // set timer
        maxTimeMovingCount = maxTimeMoving;

        // audio
        audioPlayer = GetComponent<AudioSource>();

        // null ref debugger
        /*if (navSurface && agent && body)
        {
            Debug.Log("All's good");
        } else if (!agent)
        {
            Debug.Log("Can't find agent!");
        } else if (!body)
        {
            Debug.Log("can't find body!");
        } else if (!navSurface)
        {
            Debug.Log("Can't find navSurface!");
        } else
        {
            Debug.Log("Everything found...?");
        }*/
    }

    void Update()
    {
        // check all ranges, whether sight or attack
        inSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        inAttackRange = Physics.CheckSphere(transform.position,  attackRange, whatIsPlayer);

        // Set state
        if (!inSightRange && !inAttackRange) { IdleState(); }
        if (inSightRange && !inAttackRange) { ChaseState(); }
        if (inSightRange && inAttackRange) { AttackState(); }
        //Debug.Log(managerScript);
    }

    // Remain stationary or slightly mobile
    void IdleState()
    {
        // if not looking and haven't found target pos...
        if (!searchingYet && !isTargetSet) {
            waitTime = Random.Range(minTimeBtwnMoves, maxTimeBtwnMoves);
            //Debug.Log("Searching for Target");
            searchingYet = true;
            //body.material = searchingMat;
            Invoke("SetTarget", waitTime); 
        }
        if (isTargetSet) {
            agent.SetDestination(target);
            //body.material = movingMat;
            //Debug.Log("Moving to Destination");
        }

        Vector3 distanceToTarget = transform.position - target;
        if (isTargetSet && distanceToTarget.magnitude < 2f) {
            isTargetSet = false;
            maxTimeMovingCount = maxTimeMoving;
            //Debug.Log("Arrived at Destination");
        }
        else if (isTargetSet)
        {
            maxTimeMovingCount--;
        }

        if (maxTimeMovingCount < 0)
        {
            // max time has exceeded, just move to new pos (hacky as all hell but it works)
            maxTimeMovingCount = maxTimeMoving;
            isTargetSet = false;
        }
    }

    void SetTarget()
    {
        Vector3 randomDir = Random.insideUnitSphere * targetSetRange;
        randomDir += transform.position;
        UnityEngine.AI.NavMeshHit hit;
        
        if (NavMesh.SamplePosition(randomDir, out hit, targetSetRange, 1) && Physics.Raycast(hit.position, -transform.up, 2f, whatIsGround)) 
        {
            isTargetSet = true;
            searchingYet = false;
            target = hit.position;
            if (useMarkers) { Instantiate(marker, target, Quaternion.identity); }
        } else
        {
            SetTarget();
            //Debug.Log("Researching for target...");
        }
    }

    // Move toward player
    void ChaseState()
    {
        //body.material = chasingMat;
        if (!chasingYet)
        {
            agent.SetDestination(player.position);
            chasingYet = true;
        }
    }

    // Melee attack
    void AttackState()
    {
        chasingYet = false;
        //body.material = attackingMat;
        agent.isStopped = true;
        if (attackTimer <= 0)
        {
            Instantiate(enemyBulletPrefab, spawnPt.position, spawnPt.rotation);
            attackTimer = timeBtwnAttacks;
            Play(shootSound);
        } else
        {
            attackTimer--;
        }
    }

    // play an audio clip/sfx
    void Play(AudioClip clip)
    {
        audioPlayer.clip = clip;
        audioPlayer.Play();
    }

    void OnTriggerEnter(Collider obj)
    {
        if (obj.gameObject.tag == "The Danger Zooone")
        {
            // kill self :(
            managerScript.enemyHealth = 0;
        }

        if (obj.gameObject.tag == "PlayerProjectile")
        {
            // kill self
            managerScript.enemyHealth = 0;
            Destroy(obj.gameObject);
        }
        
        //Debug.Log(obj.gameObject.name); 
    }
}
