using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    public AttackState(EnemyAi enemy) : base(enemy)
    {
        stateName = "Attack";
    }

    public override void Action()
    {
        Attack();
        enemy.navMeshA.isStopped = true;
        if (ToFarToAttack())
        {
            enemy.SetState(new ChaseState(enemy));
        }
    }

    public override void OnStateEnter()
    {
        Debug.Log("Entering Attack State");
    }

    public override void OnStateExit()
    {
        Debug.Log("Exiting Attack State");
    }

    private void Attack()
    {
        int randomAttack = (int)Random.Range(10, 13);
        enemy.anim.SetInteger("Condition", randomAttack);
    }

    private bool ToFarToAttack()
    {
        return (Vector3.Distance(enemy.transform.position, enemy.target.position) > 1.8f);
    }
}
