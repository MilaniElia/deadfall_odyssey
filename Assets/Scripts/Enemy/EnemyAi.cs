using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    private State currentState;

    public NavMeshAgent Agent { get { return _NavMeshAgent; } }
    public Transform Target { get { return _Target; } }
    public Animator Anim { get { return _Animator; } }

    private NavMeshAgent _NavMeshAgent;
    private Transform _Target;
    private Animator _Animator;

    public float SeeingDistance { get { return _SeeingDistance; } }

    [SerializeField]
    private float _SeeingDistance = 10;

    [Header("Score")]
    public float totalScore;

    [Header("UI Elements")]
    public Image targetEnemy;
    public Canvas upperCanvas;

    [Header("Health Options")]
    public HealthBar healthBar;
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("The effect of the blood splat")]
    public GameObject bloodEffect;

    private void Awake()
    {
        _Target = GameObject.FindGameObjectWithTag("Player").transform;
        _NavMeshAgent = GetComponent<NavMeshAgent>();
        _Animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    void Start()
    {
        SetState(new WanderState(this));
        SetRigidBodyState(true);
        SetColliderBodyState(false);

    }

    // Update is called once per frame
    void Update()
    {
        //new
        currentState.Action();
        

        if(currentHealth < 0.1)
        {
            SetState(new DeadState(this));
        }

        if(currentState.stateName == "Dead")
        {
            StartCoroutine(DestroyEnemy());
        }
    }


    public void OnCollisionEnter(Collision collision)
    {
        //hitted by a bullet
        if (collision.collider.tag == "Bullet")
        {
            _Animator.SetInteger("Condition", 1);
            StartCoroutine(ResetConditionOnCollision());
            currentHealth -= 20.5f;
            healthBar.SetHealth(currentHealth);
            Vector3 contactPoint = collision.GetContact(0).point;
            Instantiate(bloodEffect, contactPoint, Quaternion.identity);
        }

        if(collision.collider.tag == "Knife")
        {
            Anim.SetInteger("Condition", 1);
            StartCoroutine(ResetConditionOnCollision());
            currentHealth -= 35f;
            healthBar.SetHealth(currentHealth);
            Vector3 contactPoint = collision.GetContact(0).point;
            Instantiate(bloodEffect, contactPoint, Quaternion.identity);
        }

        if(collision.collider.tag == "Player")
        {
            Debug.Log("I collide with the player");
            if (currentState.stateName == "Attack")
            {
                if (Vector3.Distance(transform.position, _Target.position) < 1.8f)
                {
                    Debug.Log("The target will die!");
                    float randomDamage = UnityEngine.Random.Range(10f, 25f);
                    FindObjectOfType<PlayerController>().currentHealth -= randomDamage;
                    FindObjectOfType<PlayerController>().healthBar.fillAmount -= randomDamage * 0.01f / 2.0f; ;
                    FindObjectOfType<PlayerController>().Hit();
                }
                else
                {
                    Debug.Log("The target is too far");
                }
                
            }

        }
    }


    IEnumerator IdleForAMoment()
    {
        Anim.SetInteger("Condition", 2);
        _NavMeshAgent.isStopped = true;
        //wait a certain amount of time
        float randomNumber = Random.Range(250f, 300f);
        yield return new WaitForSeconds(randomNumber * Time.deltaTime);
        //reset the condition
        Anim.SetInteger("Condition", 0);
        //reset speed
        Agent.isStopped = false;
    }

    IEnumerator ResetConditionOnCollision()
    {
        _NavMeshAgent.isStopped = true;
        //wait a certain amount of time
        yield return new WaitForSeconds(10f* Time.deltaTime);
        //reset the condition
        Anim.SetInteger("Condition", 0);
        //reset speed
        Agent.isStopped = false;
    }

    IEnumerator DestroyEnemy()
    {
        
        totalScore += Random.Range(2.0f, 3.0f);
        FindObjectOfType<PlayerController>().totalScore += (int)totalScore / 2000;
        //wait for a certain amount of time
        yield return new WaitForSeconds(300f * Time.deltaTime);       
        //destroy the enemy
        Destroy(gameObject);
    }


    public void SetRigidBodyState(bool state)
    {
        Rigidbody[] rigidBodies = GetComponentsInChildren<Rigidbody>();

        foreach(Rigidbody rigidbody in rigidBodies)
        {
            rigidbody.isKinematic = state;
        }

        GetComponent<Rigidbody>().isKinematic = !state;
    }

    public void SetColliderBodyState(bool state)
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.enabled = state;
        }

        GetComponent<Collider>().enabled = !state;
    }

    public void SetState(State state)
    {
        if(currentState != null)
        {
            currentState.OnStateExit();
        }

        currentState = state;
        gameObject.name = "Enemy - " + state.GetType().Name;

        if (currentState != null)
            currentState.OnStateEnter();
    }

    private void OnDrawGizmos()
    {
        if (currentState != null)
        {
            if (currentState.GetType() == typeof(WanderState))
            {
                // Draw a yellow sphere at the transform's position
                Gizmos.color = Color.red;
                Gizmos.DrawSphere((currentState as WanderState)._WanderPoint, 0.1f);
            }
        }
    }
}


