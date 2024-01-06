using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    private float chasingSpeed = 2.0f;

    public ChaseState(EnemyAI enemy) : base(enemy)
    {
        stateName = "Chase";
    }

    public override void Action()
    {
        Enemy.Agent.SetDestination(Enemy.Target.position);
        Enemy.Agent.speed = chasingSpeed;
        Enemy.Agent.isStopped = false;
        Enemy.Anim.SetInteger("Condition", 0);
        Enemy.Anim.SetInteger("Speed", 2);

        if (ReachedPlayer())
        {
            Enemy.Agent.isStopped = true;
            Enemy.Agent.speed = 0;
            Enemy.SetState(new AttackState(Enemy));
        }
        else if (LostPlayer())
        {
            Enemy.SetState(new WanderState(Enemy));
        }
    }

    //we reached the player?
    private bool ReachedPlayer()
    {
        return Vector3.Distance(Enemy.transform.position, Enemy.Target.position) < 2.0f;
    }

    //we lost the player? Damn!
    private bool LostPlayer()
    {
        return Vector3.Distance(Enemy.transform.position, Enemy.Target.position) > Enemy.SeeingDistance;
    }
}
