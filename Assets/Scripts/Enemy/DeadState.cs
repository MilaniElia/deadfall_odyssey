using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : State
{
    public DeadState(EnemyAi enemy) : base(enemy)
    {
        stateName = "Dead";
    }

    public override void Action()
    {
        enemy.navMeshA.isStopped = true;
        enemy.anim.enabled = false;
        enemy.SetColliderBodyState(true);
        enemy.SetRigidBodyState(false);
        enemy.targetEnemy.enabled = false;
        enemy.upperCanvas.enabled = false;
    }

    public override void OnStateEnter()
    {
        Debug.Log("Entering Dead State");
    }

    public override void OnStateExit()
    {
        Debug.Log("Exiting Dead State");
    }
}
