using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : BaseTeddy
{
    public float range;
    public float shootInterval;

    private Vector3 shootOffset = new Vector3(0, 1.5f);

    private NavMeshAgent agent;

    private Outpost currentOutpost;
    private BaseTeddy currentEnemy;
    public ShrinkRay shrinkRay;

    private enum State
    {
        Idle,
        MovingToOutpost,
        ChasingEnemy,
        ShrinkRay
    }

    protected override void Start()
    {
        base.Start();

        shrinkRay = FindObjectOfType<ShrinkRay>();
        agent = GetComponent<NavMeshAgent>();

        //We start our AI in the Idle state
        SetState(State.Idle);
    }

    private void SetState (State newState)
    {
        //This will stop ANY coroutine on this instance only!
        //This means that any state I was in before, will not stop
        StopAllCoroutines();
        //This switch-statement, is just like an if-else statement
        //if (newState == State.Idle).... else if (newState == State.MovingToOutpost) ... etc.
        switch(newState)
        {
            case State.Idle:
                StartCoroutine(OnIdleState());
                //We have to add 'break' after each 'case' to break out of the switch statement
                break;
            case State.MovingToOutpost:
                StartCoroutine(OnMovingToOutpostState());
                break;
            case State.ChasingEnemy:
                StartCoroutine(OnChasingEnemyState());
                break;
            case State.ShrinkRay:
                StartCoroutine(OnMovingToShrinkRay() );
                break;
        }
    }

    //When a function returns 'IENumerator', we consider it a Coroutine
    IEnumerator OnIdleState ()
    {
        //This is the start of the state
        print("I am now idle");
        
        //We keep repeating this code, using an 'infite' while-loop
        //This is the update of the state
        while (true)
        {
            //This pauses the Coroutine function for one 'FixedUpdate', which is 0.02 seconds
            yield return new WaitForFixedUpdate();

            if (currentOutpost != null && !isShrank)
            {
                if (Random.Range(0, 2) == 0)
                {
                    SetState(State.ShrinkRay);
                }
                else
                {
                    SetState(State.MovingToOutpost);
                }
                
            }
            else if (currentOutpost != null)
            {
                SetState(State.MovingToOutpost);
            }
            else
            {

                   LookForOutpost();
            }
        }
    }



    //When a function returns 'IENumerator', we consider it a Coroutine
    IEnumerator OnMovingToOutpostState()
    {
        //This is the start of the state
        agent.SetDestination(currentOutpost.transform.position);
       // print("I am now moving to outpost");

        //This is the update of the state
        while (true)
        {
            yield return new WaitForFixedUpdate();
            //Once the outpost is captured
            if (currentOutpost.currentTeam == team && currentOutpost.captureValue == 1)
            {
                //Forget about the outpost and relax
                currentOutpost = null;
                SetState(State.Idle);
            }

            LookForEnemies();
        }
    }

    //When a function returns 'IENumerator', we consider it a Coroutine
    IEnumerator OnChasingEnemyState()
    {
        //Stop moving wherever you were going
        agent.ResetPath();

        float shootTimer = 0;

        while (true)
        {
            yield return new WaitForFixedUpdate();
            //This turns the head towards our enemy - TODO fix this later
            //anim.SetLookAtPosition(currentEnemy.transform.position + shootOffset);

            float distanceToEnemy = Vector3.Distance(transform.position, currentEnemy.transform.position);
            //If my enemy is in range, not blocked by obstacles, and within my field of view...
            if (distanceToEnemy < range
                && CanSee(currentEnemy.transform, currentEnemy.transform.position + shootOffset))
            {
                //Stop moving
                agent.ResetPath();

                //We use a timer variable to ensure we only shoot every 'shootInterval' seconds
                shootTimer += Time.fixedDeltaTime;
                if (shootTimer >= shootInterval)
                {
                    //We add shootOffset to our position, to shoot at the chest as opposed to the feet of the enemy
                    ShootLasers(currentEnemy.transform, currentEnemy.transform.position + shootOffset);
                    shootTimer = 0;
                }

                if (currentEnemy.health <= 0)
                {
                    SetState(State.Idle);
                }
            } else
            {
                //Move towards your enemy
                agent.SetDestination(currentEnemy.transform.position);
            }
        }
    }


    //Teddy will move to shrink ray, once they are shrank they will move back to the idle state
    IEnumerator OnMovingToShrinkRay()
    {
        print("I want to get small!");

        agent.SetDestination(shrinkRay.transform.position);
        while (true)
        {
            yield return new WaitForFixedUpdate();
            //If they are already shrank
            if (isShrank == true)
            {
                //Forget about shrinking and go back to idle state
                SetState(State.Idle);
            }

            //LookForEnemies();
        }
    }

    private void Update()
    {
        //This code was just here for testing. Leaving it here for reference
        //PlayerController player = FindObjectOfType<PlayerController>();
        //agent.SetDestination(player.transform.position);

        //This will automatically switch between idle and run animations
        anim.SetFloat("VerticalInput", agent.velocity.magnitude);
    }

    private void LookForOutpost ()
    {
        //Currently our 'LookForOutpost' simply selects any random outpost
        int r = Random.Range(0, GameManager.instance.outposts.Length);
        currentOutpost = GameManager.instance.outposts[r];
    }


    private void LookForEnemies ()
    {
        //Look for nearby teddies
        Collider[] surroundingColliders = Physics.OverlapSphere(transform.position, range);
        //Check if they are enemies or not
        foreach(Collider coll in surroundingColliders)
        {
            BaseTeddy teddy = coll.GetComponent<BaseTeddy>();
            if (teddy != null 
                && teddy.team != this.team 
                && teddy.health > 0 
                && CanSee(teddy.transform, teddy.transform.position + shootOffset))
            {
                //We store the enemy that we are chasing
                currentEnemy = teddy;
                //Switch to ChasingEnemy
                SetState(State.ChasingEnemy);
                //Break out of the loop to make sure we only select 1 enemy
                break;
            }
        }
        
    }

    protected override void Die()
    {
        base.Die();
        
        //Get the AI out of their current state
        StopAllCoroutines();
        agent.isStopped = true;
        //Remove collider from the gameObject to avoid it capturing
        Destroy(GetComponent<Collider>());
    }
}
