using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    public AttackState(EnemyAI enemy) : base(enemy)
    {
        stateName = "Attack";
    }

    public override void Action()
    {
        if (ToFarToAttack())
        {
            Enemy.SetState(new ChaseState(Enemy));
        }
        else
        {
            Enemy.Agent.isStopped = true;
            Enemy.Agent.speed = 0;
            Attack();
        }
    }

    private void Attack()
    {
        int randomAttack = (int)Random.Range(10, 13);
        Enemy.Anim.SetInteger("Condition", randomAttack);
    }

    private bool ToFarToAttack()
    {
        return (Vector3.Distance(Enemy.transform.position, Enemy.Target.position) > 1.8f);
    }

    private void EnableColliders()
    {

    }

    private void DisableColliders()
    {

    }
}
