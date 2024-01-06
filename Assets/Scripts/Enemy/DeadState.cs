using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : State
{
    public DeadState(EnemyAI enemy) : base(enemy)
    {
        stateName = "Dead";
    }

    public override void Action()
    {
        Enemy.Agent.isStopped = true;
        Enemy.Anim.enabled = false;
        Enemy.SetColliderBodyState(true);
        Enemy.SetRigidBodyState(false);
        Enemy.targetEnemy.enabled = false;
        Enemy.upperCanvas.enabled = false;
    }
}
