using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAgent : MonoBehaviour
{
    public static bool moblin_attack;
    public static bool bokoblin_attack;
    public static int deaths = 0;
    //public AIStateMachine stateMachine;
    //public AIStateId initialState = AIStateId.ChasePlayer;
    public NavMeshAgent agentNavMesh;
     public Animator animator;
     AnimatorClipInfo[] m_CurrentClipInfo;

    public AudioClip bokoblinHitClip;
    public AudioClip moblinHitClip;

    public AudioClip bokoblinHurtClip;
    public AudioClip moblinHurtClip;

    public AudioClip moblindieClip;
    public AudioClip bokoblindieClip;

    public AudioClip bokoblinwalkClip;
    public AudioClip moblinwalkClip;

    [Range(0, 1)] public float AudioVolume = 0.5f;
    public int maxHealth=50;
     public int currentHealth;
     public UIHealthBar healthBar;
     bool idle;
     bool chasePlayer;
     bool death;
     public float sightDistance = 10.0f;
    public bool moblin;
    bool back_to_idle = false;
    AudioSource enemy_MyAudioSource;
    // getting game object with tag not working
    public Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {

        currentHealth = maxHealth;
        healthBar = GetComponentInChildren<UIHealthBar>();
        agentNavMesh = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        idle = true;
        chasePlayer = false;
        death = false;
        enemy_MyAudioSource = GetComponent<AudioSource>();

    }
    // Update is called once per frame
    void Update()
    {
        if (GameController.health <=0 && !death && !back_to_idle)
        {
            animator.SetTrigger("player_dead");
            back_to_idle = true;
        }
        if (this.gameObject.CompareTag("Moblin"))
        {
            moblin = true;
        }

        Vector3 playerDirection = playerTransform.position - this.transform.position;
        if ((playerDirection.magnitude < sightDistance || currentHealth < maxHealth || (moblin && moblin_attack) || (!moblin && bokoblin_attack)) && !death)
        {
            idle = false;
            chasePlayer = true;
            death = false;
            animator.SetBool("attack", true);
            if (moblin) moblin_attack = true;
            else bokoblin_attack = true;
            
        }
        if (chasePlayer && !death)
        {
            agentNavMesh.destination = playerTransform.position;
            animator.SetFloat("Speed", agentNavMesh.velocity.magnitude);
            transform.LookAt(playerTransform);

        }

    }

    public void onAttack(int damage)
    {
        currentHealth -= damage;
        Debug.Log(((float)currentHealth / (float)maxHealth));
        healthBar.setHealthBarPercentage(((float)currentHealth / (float)maxHealth));
        
        if (currentHealth <= 0 && !death)
        {
            death = true;
            deaths += 1;
            idle = false;
            chasePlayer = false;
            healthBar.gameObject.SetActive(false);
            animator.SetBool("death", true);
        }
        else
        {
            animator.SetTrigger("hit");
        }

    }


    private void OnBokoblinnHit(AnimationEvent animationEvent)
    {
        if (moblin) enemy_MyAudioSource.clip = moblinHitClip;
        else  enemy_MyAudioSource.clip = bokoblinHitClip;
        enemy_MyAudioSource.Play();
        
    }
    private void OnBokoblinHurt(AnimationEvent animationEvent)
    {
        if (moblin) enemy_MyAudioSource.clip = moblinHurtClip;
        else enemy_MyAudioSource.clip = bokoblinHurtClip;
        enemy_MyAudioSource.Play();

    }
    private void OnBokoblinDeath(AnimationEvent animationEvent)
    {

        if (moblin) enemy_MyAudioSource.clip = moblindieClip;
        else enemy_MyAudioSource.clip = bokoblindieClip;
        enemy_MyAudioSource.Play();
    }

    private void onBokoblinWalk(AnimationEvent animationEvent)
    {
        if(moblin) enemy_MyAudioSource.clip = moblinwalkClip;
        else enemy_MyAudioSource.clip = bokoblinwalkClip;
        enemy_MyAudioSource.Play();
    }
}
