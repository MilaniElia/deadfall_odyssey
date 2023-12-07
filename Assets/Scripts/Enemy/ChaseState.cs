using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    private float chasingSpeed = 2.0f;

    public ChaseState(EnemyAi enemy) : base(enemy)
    {
        stateName = "Chase";
    }

    public override void Action()
    {
        enemy.navMeshA.SetDestination(enemy.target.position);
        enemy.navMeshA.speed = chasingSpeed;
        enemy.navMeshA.isStopped = false;
        enemy.anim.SetInteger("Condition", 0);
        enemy.anim.SetInteger("Speed", 2);

        if (ReachedPlayer())
        {
            enemy.SetState(new AttackState(enemy));
        }

        if (LostPlayer())
        {
            enemy.SetState(new WanderState(enemy));
        }
    }

    public override void OnStateEnter()
    {
        Debug.Log("Entering Chase State");
    }

    public override void OnStateExit()
    {
        Debug.Log("Exiting Chase State");
    }

    //we reached the player?
    private bool ReachedPlayer()
    {
        return Vector3.Distance(enemy.transform.position, enemy.target.position) < 2.0f;
    }

    //we lost the player? Damn!
    private bool LostPlayer()
    {
        return Vector3.Distance(enemy.transform.position, enemy.target.position) > 15.0f;
    }
}
