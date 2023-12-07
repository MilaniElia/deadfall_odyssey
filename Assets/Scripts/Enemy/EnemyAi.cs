using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAi : MonoBehaviour
{
    private State currentState;

    public NavMeshAgent navMeshA;
    public Transform target;
    public Animator anim;

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
        target = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshA = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    void Start()
    {
        //new
        SetState(new WanderState(this));

        //old
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
            anim.SetInteger("Condition", 1);
            StartCoroutine(ResetConditionOnCollision());
            currentHealth -= 20.5f;
            healthBar.SetHealth(currentHealth);
            Vector3 contactPoint = collision.GetContact(0).point;
            Instantiate(bloodEffect, contactPoint, Quaternion.identity);
        }

        if(collision.collider.tag == "Knife")
        {
            anim.SetInteger("Condition", 1);
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
                if (Vector3.Distance(transform.position, target.position) < 1.8f)
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
        anim.SetInteger("Condition", 2);
        navMeshA.isStopped = true;
        //wait a certain amount of time
        float randomNumber = Random.Range(250f, 300f);
        yield return new WaitForSeconds(randomNumber * Time.deltaTime);
        //reset the condition
        anim.SetInteger("Condition", 0);
        //reset speed
        navMeshA.isStopped = false;
    }

    IEnumerator ResetConditionOnCollision()
    {
        navMeshA.isStopped = true;
        //wait a certain amount of time
        yield return new WaitForSeconds(10f* Time.deltaTime);
        //reset the condition
        anim.SetInteger("Condition", 0);
        //reset speed
        navMeshA.isStopped = false;
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
}


