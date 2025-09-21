using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeAI : MonoBehaviour
{
    private GameManager _gm;
    private Animator anim;
    public ParticleSystem hitEffect;
    public int HP = 3;
    private bool isDead = false;
    private bool isAlert = false;
    private bool isWalking = false;
    private bool isAttacking = false;
    private bool isInFieldOfView = false;
    private bool hasBeenAttacked = false; // Nova variável para rastrear se já foi atacado
    public EnemyState state;
    private int rand;

    // IA do Slime
    private NavMeshAgent agent;
    private int idWayPoint;
    private Vector3 destination;
    private FoVDetection fovDetection;

    [Header("Detection Settings")]
    public float detectionCheckInterval = 0.2f;
    private Coroutine detectionCoroutine;
    private Coroutine attackCoroutine;

    void Start()
    {
        _gm = FindAnyObjectByType<GameManager>() as GameManager;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        fovDetection = GetComponent<FoVDetection>();
        StartDetection();
        ChangeState(state);
    }

    void Update()
    {
        StateManager();
        if (agent.desiredVelocity.magnitude >= 0.1f)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isAlert", isAlert);
    }

    void StartDetection()
    {
        if (detectionCoroutine != null)
        {
            StopCoroutine(detectionCoroutine);
        }
        detectionCoroutine = StartCoroutine(ContinuousDetection());
    }

    IEnumerator ContinuousDetection()
    {
        while (!isDead)
        {
            isInFieldOfView = FoVDetection.IsInFieldOfView(transform, _gm.player, fovDetection.maxAngle, fovDetection.maxRadius);
            
            // Lógica atualizada baseada no estado atual
            if (state == EnemyState.IDLE || state == EnemyState.PATROL)
            {
                if (isInFieldOfView)
                {
                    // Se já foi atacado, vai direto para FURY
                    if (hasBeenAttacked)
                    {
                        ChangeState(EnemyState.FURY);
                    }
                    else
                    {
                        ChangeState(EnemyState.ALERT);
                    }
                }
            }
            else if (state == EnemyState.FURY)
            {
                // Se está em FURY mas perdeu de vista o player, vai para PATROL
                if (!isInFieldOfView)
                {
                    ChangeState(EnemyState.PATROL);
                }
            }
            
            yield return new WaitForSeconds(detectionCheckInterval);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (_gm.GameState != GameState.GAMEPLAY)
        {
            return;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            if (state == EnemyState.IDLE || state == EnemyState.PATROL)
            {
                // Se já foi atacado, vai direto para FURY
                if (hasBeenAttacked)
                {
                    ChangeState(EnemyState.FURY);
                }
                else
                {
                    ChangeState(EnemyState.ALERT);
                }
            }
        }
    }

    IEnumerator Dead()
    {
        isDead = true;
        yield return new WaitForSeconds(2.5f);
        int chance = GetRandomNumber(0, 100);
        if (chance <= _gm.chanceDrop)
        {
            Instantiate(_gm.gemPrefab, transform.position + Vector3.up, _gm.gemPrefab.transform.rotation);
        }
        Destroy(this.gameObject);
    }

    void HandleAttack()
    {
        if (isAttacking == false && isInFieldOfView)
        {
            Debug.Log("Slime attacking! Distance: " + agent.remainingDistance);
            isAttacking = true;
            anim.SetTrigger("Attack");
            AttackDone();
        }
    }

    void AttackDone()
    {
        Debug.Log("Attack animation finished");
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }

        attackCoroutine = StartCoroutine(AttackDelay());
    }

    void GetHit(int amount)
    {
        if (isDead == true) { return; }
        
        HP -= amount;
        hasBeenAttacked = true; // Marca que foi atacado

        if (HP > 0)
        {
            ChangeState(EnemyState.FURY);
            anim.SetTrigger("GetHit");
            hitEffect.Emit(5);
        }
        else
        {
            ChangeState(EnemyState.DIED);
            isAttacking = false;
            anim.SetTrigger("Die");
            hitEffect.Emit(10);
            StartCoroutine(Dead());
        }
    }

    void LookAt()
    {
        Vector3 lookDirection = (_gm.player.position - transform.position).normalized;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
        
    }

    void StateManager()
    {

        if (_gm.GameState == GameState.DIED && (state == EnemyState.FOLLOW || state == EnemyState.FURY || state == EnemyState.ALERT))
        {
            ChangeState(EnemyState.IDLE);
        }

        switch (state)
            {
                case EnemyState.ALERT:
                    LookAt();
                    break;
                case EnemyState.PATROL:
                    break;

                case EnemyState.FOLLOW:
                    LookAt();
                    destination = _gm.player.position;
                    agent.SetDestination(destination);

                    if (agent.remainingDistance <= agent.stoppingDistance)
                    {
                        HandleAttack();
                    }

                    break;

                case EnemyState.FURY:
                    LookAt();
                    bool canSeePlayer = isInFieldOfView;
                    if (canSeePlayer)
                    {
                        destination = _gm.player.position;
                        agent.SetDestination(destination);
                        if (agent.remainingDistance <= agent.stoppingDistance)
                        {
                            HandleAttack();
                        }
                    }
                    break;
            }
    }

    void ChangeState(EnemyState newState)
    {
        StopCoroutine("Idle");
        StopCoroutine("Alert");
        StopCoroutine("Patrol");
        
        Debug.Log("State changed to: " + newState);
        state = newState;

        switch (state)
        {
            case EnemyState.IDLE:
                agent.stoppingDistance = 0f;
                destination = transform.position;
                agent.SetDestination(destination);
                isAlert = false;
                StartCoroutine("Idle");
                break;

            case EnemyState.ALERT:
                agent.stoppingDistance = 0f;
                destination = transform.position;
                agent.SetDestination(destination);
                isAlert = true;
                StartCoroutine("Alert");
                break;

            case EnemyState.EXPLORE:
                break;

            case EnemyState.PATROL:
                agent.stoppingDistance = 0f;
                idWayPoint = GetRandomNumber(0, _gm.SlimeWayPoints.Length);
                destination = _gm.SlimeWayPoints[idWayPoint].position;
                agent.SetDestination(destination);
                StartCoroutine("Patrol");
                break;

            case EnemyState.FOLLOW:
                agent.stoppingDistance = _gm.slimeDistanceToAttack;
                break;

            case EnemyState.FURY:
                agent.stoppingDistance = _gm.slimeDistanceToAttack;
                // Não define destino aqui - será definido no StateManager
                break;

            case EnemyState.DIED:
                destination = transform.position;
                agent.SetDestination(destination);
                break;
        }
    }

    IEnumerator Alert()
    {
        yield return new WaitUntil(() => !isInFieldOfView);
        
        // Se já foi atacado, vai para PATROL em vez de IDLE
        if (hasBeenAttacked)
        {
            ChangeState(EnemyState.PATROL);
        }
        else
        {
            StayStill(30);
        }
    }

    IEnumerator Idle()
    {
        yield return new WaitForSeconds(_gm.slimeIdleWaitingTime);
        rand = GetRandomNumber(0, 100);
        if (rand < 50)
        {
            ChangeState(EnemyState.IDLE);
        }
        else
        {
            ChangeState(EnemyState.PATROL);
        }
    }

    IEnumerator Patrol()
    {
        yield return new WaitUntil(() => agent.remainingDistance <= 0.1f);
        StayStill(30);
    }

    IEnumerator AttackDelay()
    {
        Debug.Log("Starting attack cooldown: " + _gm.slimeAttackDelay + " seconds");
        yield return new WaitForSeconds(_gm.slimeAttackDelay);
        isAttacking = false;
        Debug.Log("Attack cooldown finished - ready to attack again");
    }

    void StayStill(int yes)
    {
        if (GetRandomNumber(0, 100) < yes)
        {
            // Se já foi atacado, continua patrulhando em vez de ficar idle
            if (hasBeenAttacked)
            {
                ChangeState(EnemyState.PATROL);
            }
            else
            {
                ChangeState(EnemyState.IDLE);
            }
        }
        else
        {
            ChangeState(EnemyState.PATROL);
        }        
    }

    int GetRandomNumber(int min, int max)
    {
        return Random.Range(min, max);
    }
}
