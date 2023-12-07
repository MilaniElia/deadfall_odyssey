using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WanderState : State
{
    private float wanderRadius = 20f;
    private float wanderSpeed = 0.5f;
    private float viewDistance = 10f;
    private float FOV = 180f;

    private Vector3 wanderPoint;
    private NavMeshAgent navMeshAgent;
    private Animator anim;

    public WanderState(EnemyAi enemy) : base(enemy)
    {
        stateName = "Wander";

        navMeshAgent = enemy.navMeshA;
        anim = enemy.anim;
        wanderPoint = new Vector3(enemy.transform.position.x, enemy.transform.position.y, enemy.transform.position.z);
    }

    public override void Action()
    {
        Wander();
        anim.SetInteger("Speed", 1);
        navMeshAgent.speed = wanderSpeed;

        if (FindPlayers())
        {
            enemy.SetState(new ChaseState(enemy));
        }
    }

    public override void OnStateEnter()
    {
        Debug.Log("Entering Wander State");
    }

    public override void OnStateExit()
    {
        Debug.Log("Exiting Wander State");
    }


    //Search players in the area
    private bool FindPlayers()
    {
        if (Vector3.Angle(Vector3.forward, enemy.transform.InverseTransformPoint(enemy.target.position)) < FOV / 2f)
        {
            if (Vector3.Distance(enemy.target.position, enemy.transform.position) < viewDistance)
            {
                RaycastHit hit;
                if (Physics.Linecast(enemy.transform.position, enemy.target.position, out hit, -1))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    //set a point to reach while searching out players
    private void Wander()
    {
        if (ReachedPoint())
        {
            wanderPoint = RandomWanderPoint();
            //navMeshAgent.SetDestination(wanderPoint);
        }
        else
        {
            navMeshAgent.SetDestination(wanderPoint);
        }
    }

    //the random point to reach
    private Vector3 RandomWanderPoint()
    {
        Vector3 randomPoint = (Random.insideUnitSphere * wanderRadius) + enemy.transform.position;
        NavMeshHit navMeshHit;
        NavMesh.SamplePosition(randomPoint, out navMeshHit, wanderRadius, -1);
        return new Vector3(navMeshHit.position.x, navMeshHit.position.y, navMeshHit.position.z);
    }

    //return true if the wander point has been reached
    private bool ReachedPoint()
    {
        return Vector3.Distance(enemy.transform.position, enemy.target.position) < 0.5f;
    }
}
