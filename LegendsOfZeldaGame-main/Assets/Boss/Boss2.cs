using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2 : MonoBehaviour
{
    Animator animator;
    UnityEngine.AI.NavMeshAgent agent;
    public Transform[] points;
    int destPoint = 0;
    bool patrolMode;
    public Transform player;
    float range = 5f;
    public GameObject meteor;
    public Animator playerAnimator;
    int maxHealth = 74;
    static int  currentHealth;
    public static bool isBodySlamming = false;
    int randomAttack = 0;
    bool summoning = false;
    public BossHealthBar healthBar;
    bool die;
    public static bool headAttacked = false;
    bool startBodySlamming = false;
    bool won = false;
    bool slow = false;
    public static bool gotHit; 

    AudioSource m_MyAudioSource;
    public AudioClip BossStepAudioClip;
    public AudioClip BossSumonningAudioClip;
    public AudioClip BodySlamAudioClip;
    public AudioClip HurtClip;
    public AudioClip StunnedAudioClip;
    public AudioClip DeathClip;
    public AudioClip DanceAudioClip;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.autoBraking = false;
        patrolMode = true;
        animator.SetBool("IsWalking", true);
        meteor.SetActive(false);
        GotoNextPoint();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        meteor.SetActive(false);
        m_MyAudioSource = GetComponent<AudioSource>();

    }

    public void onAttack(int damage)
    {
        if(!isBodySlamming)
            currentHealth -= damage;
        //Debug.Log("Boss Health" + currentHealth);
        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            die = true;
            healthBar.gameObject.SetActive(false);
           onDie();
        }
        //Debug.Log("Boss Health" + currentHealth);

    }


    // Update is called once per frame
    void Update()
    {
        slowCheat();
        setRandomAttack();
        playerDeath();
        if (gotHit)
        {
            OnBossHit();

        }
        setFlags();

        if(headAttacked)
            onHeadAttack();

        if (!agent.pathPending && agent.remainingDistance < 0.5f && patrolMode)
            GotoNextPoint();

        float dist = Vector3.Distance(player.position, agent.transform.position);


        if (!this.animator.GetCurrentAnimatorStateInfo(0).IsTag("summoning"))
        {
            meteor.SetActive(false);
        }
        if (won)
        {
            animator.SetBool("dance", true);
            animator.SetBool("IsSummoning", false);
            animator.SetBool("IsBodySlam", false);

        }
        else if (dist <= range)
        {
            patrolMode = false;
            animator.SetBool("IsWalking", false);

            if (randomAttack == 0 && !this.animator.GetCurrentAnimatorStateInfo(0).IsTag("bodySlam") && !this.animator.GetCurrentAnimatorStateInfo(0).IsTag("IsStunned"))
            {
                //deactivating bosyslam
                animator.SetBool("IsBodySlam", false);

                agent.SetDestination(agent.transform.position);
                animator.SetBool("IsSummoning", true);
                meteor.SetActive(true);
                meteor.transform.position = new Vector3(player.position.x, player.position.y, player.position.z);
            }

            if (randomAttack == 1 && !this.animator.GetCurrentAnimatorStateInfo(0).IsTag("summoning") && !this.animator.GetCurrentAnimatorStateInfo(0).IsTag("bodySlam"))  // performs body slam
            {
                meteor.SetActive(false);
                animator.SetBool("IsSummoning", false);
                animator.SetBool("IsBodySlam", true);
                agent.SetDestination(player.transform.position);
                startBodySlamming = true;
                //if (this.animator.GetCurrentAnimatorStateInfo(0).IsTag("bodySlam"))

            }

        }

        if (dist >= range)
        {
            patrolMode = true;
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsSummoning", false);
            animator.SetBool("IsBodySlam", false);
            if (!this.animator.GetCurrentAnimatorStateInfo(0).IsTag("summoning"))
            {
                meteor.SetActive(false);
            }
           // meteor.SetActive(false);
        }


    }

    void setRandomAttack()
    {
        if (this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Idle"))

        {
            randomAttack = Random.Range(0, 3);
            if (currentHealth > 74)
                randomAttack = 0;
        }
    }

    void onHeadAttack()
    {

        animator.SetTrigger("IsStunned");
        headAttacked = false;
        meteor.SetActive(false);
    }

    void setFlags()
    {
        if (this.animator.GetCurrentAnimatorStateInfo(0).IsTag("bodySlam"))
            isBodySlamming = true;
        else
            isBodySlamming = false;
    }
    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Length == 0)
            return;
        // Set the agent to go to the currently selected destination.
        agent.destination = points[destPoint].position;
        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;


    }

    void slowCheat()
    {
        if (Input.GetKeyDown(KeyCode.O) && !slow)
        {   
            Time.timeScale = 0.5f;
            slow = true;
        }

        if (Input.GetKeyDown(KeyCode.O) && slow)
        {
            Time.timeScale = 1f;
            slow = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && startBodySlamming && GameController.attackable)
        {
            startBodySlamming = false;
            //Debug.Log("bodySlam " + GameController.health);
            if (GameController.health - 7 < 0)
                GameController.health = 0;
            else
                GameController.health -= 7;
            playerDeath();
            //Debug.Log("After bodySlam " + GameController.health);
            playerAnimator.SetTrigger("bossAttack");


        }
    }

    void onDie()
    {
        
        animator.SetTrigger("IsDead");
        agent.SetDestination(agent.transform.position);
        agent.isStopped = true;

        // move to another scene
    }

    public void playerDeath()
    {
        if (GameController.health <= 0)
        {
            playerAnimator.SetBool("deathboss",true);
            animator.SetBool("dance", true);
            won = true;
            agent.isStopped = true;
        }
    }
    public  void OnBossHit()
    {
        animator.SetTrigger("IsHit");
        gotHit = false;
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        m_MyAudioSource.clip = BossStepAudioClip;
        m_MyAudioSource.Play();

    }

    private void OnBodySlam(AnimationEvent animationEvent)
    {
        m_MyAudioSource.clip = BodySlamAudioClip;
        m_MyAudioSource.Play();

    }

    private void OnSummoning(AnimationEvent animationEvent)
    {
        m_MyAudioSource.clip = BossSumonningAudioClip;
        m_MyAudioSource.Play();

    }

    private void OnStunned(AnimationEvent animationEvent)
    {
        m_MyAudioSource.clip = StunnedAudioClip;
        m_MyAudioSource.Play();

    }

    private void OnDeath(AnimationEvent animationEvent)
    {
        m_MyAudioSource.clip = DeathClip;
        m_MyAudioSource.Play();

    }

    private void OnHit(AnimationEvent animationEvent)
    {
        m_MyAudioSource.clip = HurtClip;
        m_MyAudioSource.Play();

    }
    private void OnDance(AnimationEvent animationEvent)
    {
        m_MyAudioSource.clip = DanceAudioClip;
        m_MyAudioSource.Play();

    }
}
