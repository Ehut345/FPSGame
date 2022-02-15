using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    public Animator anim;
    private GameObject targetPlayer;
    NavMeshAgent enemyAgent;
    public float walkingSpeed;
    public float runningSpeed;
    public GameObject zombieRagdollPrefab;
    public float distanceToZombie;


    public enum STATE
    {
        IDLE, WANDER, CHASE, ATTACK, DEAD
    };
    public STATE state = STATE.IDLE;


    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        enemyAgent = GetComponent<NavMeshAgent>();
        targetPlayer = GameObject.FindGameObjectWithTag("Player");
        //anim.SetBool("isWalking", true);

    }
    public void TurnOffAnimTriggers()
    {
        anim.SetBool("isWalking", false);
        anim.SetBool("isAttacking", false);
        anim.SetBool("isRunning", false);
        anim.SetBool("isDead", false);

    }

    bool ZombieCanSeePlayer()
    {
        //logic for zombie to see the player and chase
        //need to calculate distance to the player
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
        return distanceToZombie = Vector3.Distance(targetPlayer.transform.position, this.transform.position);
    }


    // Update is called once per frame
    void Update()
    {
        //enemyAgent.SetDestination(targetPlayer.transform.position);
        //if (enemyAgent.remainingDistance > enemyAgent.stoppingDistance)
        //{
        //    anim.SetBool("isWalking", true);
        //    anim.SetBool("isAttacking", false);

        //}
        //else
        //{
        //    anim.SetBool("isWalking", false);
        //    anim.SetBool("isAttacking", true);
        //}
        //if(Input.GetKeyDown(KeyCode.G))
        //{
        //    if(Random.Range(0,10)<5)
        //    {
        //        GameObject rbtemp = Instantiate(zombieRagdollPrefab, this.transform.position, this.transform.rotation);
        //        rbtemp.transform.Find("Hips").GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 10000);
        //        Destroy(this.gameObject, 5.0f);
        //    }
        //    else
        //    {
        //        ZombieKill();
        //    }

        //}
        switch (state)
        {
            case STATE.IDLE:
                if (targetPlayer != null)
                {
                    if (ZombieCanSeePlayer())
                    {
                        state = STATE.CHASE;
                    }
                    else if (Random.Range(0, 5000) < 5)
                    {
                        state = STATE.WANDER;
                    }
                }



                break;
            case STATE.WANDER:
                if (targetPlayer != null)
                {
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
                    else if (Random.Range(0, 1000) < 5)
                    {
                        state = STATE.IDLE;
                        TurnOffAnimTriggers();
                        enemyAgent.ResetPath();
                    }
                }



                break;
            case STATE.CHASE:
                if (targetPlayer != null)
                {
                    enemyAgent.SetDestination(targetPlayer.transform.position);
                    enemyAgent.stoppingDistance = 2.0f;
                    TurnOffAnimTriggers();
                    enemyAgent.speed = runningSpeed;
                    anim.SetBool("isRunning", true);
                    if (enemyAgent.remainingDistance <= enemyAgent.stoppingDistance && !enemyAgent.pathPending)
                    {
                        state = STATE.ATTACK;
                    }
                    if (ZombieCantSeePlayer())
                    {
                        state = STATE.WANDER;
                        enemyAgent.ResetPath();
                    }
                }

                break;
            case STATE.ATTACK:
                if (targetPlayer != null)
                {
                    TurnOffAnimTriggers();
                    anim.SetBool("isAttacking", true);
                    transform.LookAt(targetPlayer.transform);
                    if (DistanceToPlayer() > enemyAgent.stoppingDistance)
                    {
                        state = STATE.CHASE;
                    }
                }

                break;
            case STATE.DEAD:
                break;
            default:
                break;
        }


    }

    public void ZombieKill()
    {
        TurnOffAnimTriggers();
        anim.SetBool("isDead", true);
        state = STATE.DEAD;
    }
}