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
    public float wanderWaitTime = 10f;
    public bool canMoveWhileAttacking;
    [Space]
    public float walkSpeed = 2f;
    public float runSpeed = 3.5f;
    public float wanderRange = 5f;




    public bool walk;
    public bool run;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        currentWanderTime = wanderWaitTime;
    }

    bool hasDied;

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
            if (Vector3.Distance(target.transform.position, transform.position) > maxChaseDistance)
                target = null;

            if (!isAttacking)
                Chase();

            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
        else
            Wander();
    }

    public void UpdateAnimations()
    {
        anim.SetBool("Walk", walk);
        anim.SetBool("Run", run);
    }

    public void Wander()
    {
        if (currentWanderTime >= wanderWaitTime)
        {
            Vector3 wanderPos = transform.position;

            wanderPos.x += Random.Range(-wanderRange, wanderRange);
            wanderPos.z += Random.Range(-wanderRange, wanderRange);

            currentWanderTime = 0;

            agent.speed = walkSpeed;
            agent.SetDestination(wanderPos);

            walk = true;
            run = false;
        }
        else
        {
            if (agent.isStopped)
            {
                currentWanderTime += Time.deltaTime;

                walk = false;
                run = false;
            }
        }
    }

    public void Chase()
    {
        agent.SetDestination(target.transform.position);

        walk = false;

        run = true;

        agent.speed = runSpeed;

        if (Vector3.Distance(target.transform.position, transform.position) <= minAttackDistance && !isAttacking)
            StartAttack();
    }

    public void StartAttack()
    {
        isAttacking = true;

        if (!canMoveWhileAttacking)
            agent.SetDestination(transform.position);

        anim.SetTrigger("Attack");
    }

    public void FinishAttack()
    {
        if (Vector3.Distance(target.transform.position, transform.position) > maxAttackDistance)
            return;

        target.GetComponent<PlayerStats>().health -= damage;

        isAttacking = false;
        if (target != null)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null)
            target = other.transform;
    }
}
