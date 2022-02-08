using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    Animator anim;
    public GameObject targetPlayer;
    NavMeshAgent enemyAgent;
    public float walkingSpeed;
    public float runingSpeed;


    enum STATE
    {
        IDLE, WANDER, CHASE, ATTACK, DEAD
    };
    STATE state = STATE.IDLE;

    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        enemyAgent = GetComponent<NavMeshAgent>();

    }
    void TurnOffAnimTriggers()
    {
        anim.SetBool("isWalking", false);
        anim.SetBool("isAttacking", false);
        anim.SetBool("isRunning", false);
        anim.SetBool("isDead", false);

    }

    bool ZombieCanSeePlayer()
    {
        if (DistanceToPlayer() < 10)
        {
            return true;
        }
        return false;

    }
    bool ZombieCantSeePlayer()
    {
        if (DistanceToPlayer() > 10)
        {
            return true;
        }
        return false;

    }

    private float DistanceToPlayer()
    {
        return Vector3.Distance(targetPlayer.transform.position, this.transform.position);
    }


    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case STATE.IDLE:
                if (ZombieCanSeePlayer())
                {
                    state = STATE.CHASE;
                }
                else if (Random.Range(0, 100) < 5)
                {
                    state = STATE.WANDER;
                }



                break;
            case STATE.WANDER:
                if (!enemyAgent.hasPath)
                {
                    float newRandomPositionX = this.transform.position.x + Random.Range(-10, 10);
                    float newRandomPositionZ = this.transform.position.z + Random.Range(-10, 10);
                    float newRandomPositionY = Terrain.activeTerrain.SampleHeight(new Vector3(newRandomPositionX, 0, newRandomPositionZ));
                    Vector3 finalDestination = new Vector3(newRandomPositionX, newRandomPositionY, newRandomPositionZ);
                    enemyAgent.SetDestination(finalDestination);
                    enemyAgent.stoppingDistance = 2.0f;
                    TurnOffAnimTriggers();
                    enemyAgent.speed = walkingSpeed;
                    anim.SetBool("isWalking", true);
                }
                else if (ZombieCanSeePlayer())
                {
                    state = STATE.CHASE;
                }
                else if (Random.Range(0, 100) < 5)
                {
                    state = STATE.WANDER;
                    TurnOffAnimTriggers();
                    enemyAgent.ResetPath();
                }

                break;
            case STATE.CHASE:
                enemyAgent.SetDestination(targetPlayer.transform.position);
                enemyAgent.stoppingDistance = 2.0f;
                TurnOffAnimTriggers();
                enemyAgent.speed = runingSpeed;
                anim.SetBool("isRunning", true);
                if (enemyAgent.remainingDistance <= enemyAgent.stoppingDistance)//&& !enemyAgent.pathPending)
                {
                    state = STATE.ATTACK;
                }
                if (ZombieCantSeePlayer())
                {
                    state = STATE.WANDER;
                    enemyAgent.ResetPath();
                }
                break;
            case STATE.ATTACK:
                TurnOffAnimTriggers();
                anim.SetBool("isAttacking", true);
                transform.LookAt(targetPlayer.transform.position);
                if (DistanceToPlayer() > enemyAgent.stoppingDistance)
                {
                    state = STATE.CHASE;
                }
                break;
            case STATE.DEAD:
                break;
            default:
                break;
        }


    }
}
