using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class RabbitAI : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Transform player;

    private Transform[] launchPositions;


    private Animator animator;


    private int timesAttacked = 0;

    private float initRabbitHealth;
    enum RabbitState
    {
        Default, Charging, Stunned
    }

    private RabbitState currentRabbitState;
    private bool canAttack = true;
    private bool isReadyToCharge = false;
    private bool isInChargePos = false;

    private float initNavMeshSpeed;
    private float initNavMeshStoppingDist;
    private float initNavMeshAccelerationt;
    
    
    [SerializeField] private float timeBetweenAttacks = 1;
    [SerializeField] private Transform chargePos;
    [SerializeField] private Transform endChargePos;
    [SerializeField] float minChargeWait = 3;
    [SerializeField] float maxChargeWait = 6;
    private bool isCharging = false;
    [SerializeField] float chargeSpeed = 1;
    [SerializeField] private Transform fallPos;
    private CapsuleCollider capsuleCollider;
    public GameObject hitbox;

    private RabbitHealth rabbitHealth;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        rabbitHealth = GetComponent <RabbitHealth>();
        player = GameObject.FindWithTag("Player").transform;
        animator = GetComponent<Animator>();
        initNavMeshSpeed = navMeshAgent.speed;
        capsuleCollider = GetComponent<CapsuleCollider>();
        initNavMeshStoppingDist = navMeshAgent.stoppingDistance;
        initNavMeshAccelerationt = navMeshAgent.acceleration;
        hitbox.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentRabbitState)
        {
            case RabbitState.Default:
                RabbitNormal();
                break;
            case RabbitState.Charging:

                if (!isInChargePos)
                {
                    navMeshAgent.SetDestination(chargePos.position);
                    isInChargePos = true;
                    StartCoroutine(WaitToCharge());
                }
                else if (!isReadyToCharge)
                {
                    transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                }
                else if (isCharging)
                {
           
                }
                break;
            case RabbitState.Stunned:
                if (rabbitHealth.health <= initRabbitHealth - 30)
                {
                    ExitStun();
                }
                
                break;
        }

        void ExitStun()
        {
            rabbitHealth.canTakeDamage = false;
            currentRabbitState = RabbitState.Default;
            navMeshAgent.speed = initNavMeshSpeed;
            navMeshAgent.stoppingDistance = initNavMeshStoppingDist;
            navMeshAgent.acceleration = initNavMeshAccelerationt;
            timesAttacked = 0;
        } 
        
        
        //TODO Remove, dev tools
        if (Input.GetKeyDown(KeyCode.E))
        {
            timesAttacked = 10;
        }
        
    }

    
    IEnumerator WaitToCharge()
    {
        yield return new WaitForSeconds(Random.Range(minChargeWait, maxChargeWait + 1));
        animator.SetTrigger("Charge");
    }

    //called by animation event
    public void Charge()
    {
        isReadyToCharge = false;    
        navMeshAgent.speed = chargeSpeed;
        navMeshAgent.stoppingDistance = 0;
        // navMeshAgent.angularSpeed = 0.1f;
        navMeshAgent.SetDestination(endChargePos.position);
        capsuleCollider.enabled = false;
        navMeshAgent.acceleration = 300;
        hitbox.SetActive(true);
        isCharging = false;
        StartCoroutine(endCharge());
    }



    IEnumerator endCharge()
    {
        yield return new WaitForSeconds(0.5f);
        navMeshAgent.speed = initNavMeshSpeed;
        navMeshAgent.stoppingDistance = initNavMeshStoppingDist;
        navMeshAgent.acceleration = initNavMeshAccelerationt;
        capsuleCollider.enabled = true;
        isCharging = false;
        currentRabbitState = RabbitState.Default;
    }
    void RabbitNormal()
    {
        if (!canAttack) {return;}
        navMeshAgent.SetDestination(player.position);
        print("chasing");
        if (Vector3.Distance(transform.position, player.position) <= navMeshAgent.stoppingDistance)
        {
            Attack();
        }
    }

    //TODO actually fix attack with hitbox
    void Attack()
    {
        canAttack = false;
        timesAttacked++;
        navMeshAgent.SetDestination(gameObject.transform.position);
        animator.SetTrigger("Attack");
        StartCoroutine(AttackCooldown());

    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        canAttack = true;

        timesAttacked = Mathf.Clamp(timesAttacked, 0, 10);
        
        //set state to charging
        if (Random.Range(timesAttacked, 11) == 10)
        {
            currentRabbitState = RabbitState.Charging;
            isInChargePos = false;
        }

        print(timesAttacked);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        print(other.name);
        
        if (currentRabbitState == RabbitState.Charging)
        {
            //OnHitLog
            if (other.CompareTag("Log"))
            {
                Destroy(other.gameObject);
                print("hitLog");
                StopAllCoroutines();
                animator.SetBool("stunned", true);
                hitbox.SetActive(false);
                initRabbitHealth = rabbitHealth.health;
                capsuleCollider.enabled = true;
                navMeshAgent.velocity = Vector3.zero;
                navMeshAgent.SetDestination(fallPos.position);
                currentRabbitState = RabbitState.Stunned;
                rabbitHealth.canTakeDamage = true;
            }
        }
    }
}
