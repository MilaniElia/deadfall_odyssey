using UnityEngine;

public abstract class State
{
    protected EnemyAI Enemy;
    public string stateName;

    public abstract void Action();
    public virtual void OnStateEnter() { Debug.Log($"{Enemy.name} - Entering in {stateName}"); }
    public virtual void OnStateExit() {  }
    public virtual void GetState() { Debug.Log(stateName); }

    public State(EnemyAI enemy)
    {
        this.Enemy = enemy;
    }
}
