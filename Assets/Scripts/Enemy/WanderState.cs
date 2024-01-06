using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class WanderState : State
{
    private float _WanderRadius = 20f;
    private float _WanderSpeed = 0.3f;
    private float viewDistance = 10f;
    private float FOV = 180f;

    public Vector3 _WanderPoint;

    public WanderState(EnemyAI enemy) : base(enemy)
    {
        stateName = "Wander";
        _WanderPoint = RandomWanderPoint();
        enemy.Agent.speed = _WanderSpeed;
        enemy.Agent.SetDestination(_WanderPoint);
        enemy.Anim.SetInteger("Speed", 1);
    }

    public override void Action()
    {
        Wander();      

        if (FindPlayers())
        {
            Enemy.SetState(new ChaseState(Enemy));
        }
    }

    //Search players in the area
    private bool FindPlayers()
    {
        if (Vector3.Angle(Vector3.forward, Enemy.transform.InverseTransformPoint(Enemy.Target.position)) < FOV / 2f)
        {
            if (Vector3.Distance(Enemy.Target.position, Enemy.transform.position) < viewDistance)
            {
                RaycastHit hit;
                if (Physics.Linecast(Enemy.transform.position, Enemy.Target.position, out hit, -1))
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
            Enemy.StartCoroutine(SetNewDestination());
        }
    }

    IEnumerator SetNewDestination()
    {
        _WanderPoint = RandomWanderPoint();
        Enemy.Anim.SetInteger("Condition", 2);
        yield return new WaitForSecondsRealtime(Random.Range(2.5f, 5f));
        Enemy.Anim.SetInteger("Condition", 0);
        Enemy.Agent.SetDestination(_WanderPoint);
    }

    //the random point to reach
    private Vector3 RandomWanderPoint()
    {
        Vector3 randomPoint = (Random.insideUnitSphere * _WanderRadius) + Enemy.transform.position;
        NavMeshHit navMeshHit;
        NavMesh.SamplePosition(randomPoint, out navMeshHit, _WanderRadius, -1);
        Debug.Log($"Wander point is {randomPoint.x}");
        return new Vector3(navMeshHit.position.x, navMeshHit.position.y, navMeshHit.position.z);
    }

    //return true if the wander point has been reached
    private bool ReachedPoint()
    {
        return Vector3.Distance(Enemy.transform.position, _WanderPoint) < 0.5f;
    }
}
