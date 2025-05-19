using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalAI : MonoBehaviour
{
    public Transform target;
    public NavMeshAgent agent;
    private Animator anim;
    private bool isAttacking;

    public float health = 100f;

    [Header("Attack Settings")]
    public float damage;
    public float maxChaseDistance;
    public float minAttackDistance = 1.5f;
    public float maxAttackDistance = 2.5f;

    [Header("Movement")]
    private float currentWanderTime;
    public float wanderWaitTime = 7f;
    public bool canMoveWhileAttacking;
    [Space]
    public float walkSpeed = 2f;
    public float runSpeed = 3.5f;
    public float wanderRange = 5f;

    public bool walk;
    public bool run;

    // Temporizador para perder el target
    private float timeOutOfRange = 0f;
    public float timeToForgetTarget = 5f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        currentWanderTime = wanderWaitTime;
    }

    private void Update()
    {
        if (health <= 0)
        {
            agent.SetDestination(transform.position);
            Destroy(agent);
            anim.SetTrigger("Die");
            GetComponent<ResourceObject>().enabled = true;
            Destroy(this);
            return;
        }

        UpdateAnimations();

        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(target.position, transform.position);

            if (distanceToTarget > maxChaseDistance)
            {
                timeOutOfRange += Time.deltaTime;

                if (timeOutOfRange >= timeToForgetTarget)
                {
                    Debug.Log("Target perdido por estar fuera de rango.");
                    target = null;
                    isAttacking = false;

                    agent.isStopped = true;
                    agent.ResetPath();

                    walk = false;
                    run = false;
                    timeOutOfRange = 0f;
                    return;
                }
            }
            else
            {
                timeOutOfRange = 0f;
            }

            if (!isAttacking)
            {
                Chase();
                agent.isStopped = false;
                agent.SetDestination(target.position);
            }
            else if (canMoveWhileAttacking)
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);
            }
            else
            {
                agent.isStopped = true;
            }
        }
        else
        {
            Wander();
        }
    }

    public void UpdateAnimations()
    {
        anim.SetBool("Walk", walk);
        anim.SetBool("Run", run);
    }

    public void Wander()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            currentWanderTime += Time.deltaTime;

            if (currentWanderTime >= wanderWaitTime)
            {
                Vector3 wanderPos = transform.position;
                wanderPos.x += Random.Range(-wanderRange, wanderRange);
                wanderPos.z += Random.Range(-wanderRange, wanderRange);

                agent.speed = walkSpeed;
                agent.isStopped = false;
                agent.SetDestination(wanderPos);

                currentWanderTime = 0;
                walk = true;
                run = false;
            }
            else
            {
                walk = false;
                run = false;
            }
        }
    }

    public void Chase()
    {
        agent.speed = runSpeed;
        walk = false;
        run = true;

        if (Vector3.Distance(target.position, transform.position) <= minAttackDistance && !isAttacking)
        {
            StartAttack();
        }
    }

    public void StartAttack()
    {
        isAttacking = true;

        if (!canMoveWhileAttacking)
        {
            agent.SetDestination(transform.position);
            agent.isStopped = true;
        }

        anim.SetTrigger("Attack");
    }

    public void FinishAttack()
    {
        if (Vector3.Distance(target.position, transform.position) <= maxAttackDistance)
        {
            //target.GetComponent<PlayerStats>().Damage();
            target.GetComponent<PlayerStats>().health -= damage;
        }

        isAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (target == null && other.GetComponent<Player>() != null)
        {
            target = other.transform;
            timeOutOfRange = 0f;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (target == null && other.GetComponent<Player>() != null)
        {
            target = other.transform;
            timeOutOfRange = 0f;
            Debug.Log("Target reacquired via OnTriggerStay");
        }
    }
}
