using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossControl : MonoBehaviour
{

    NavMeshAgent agent;
     Animator animator;
    public Transform[] points;
    public Transform playerTransform;
    private int destPoint = 0;
    private float nextActionTime = 0.1f;
    public float period = 50f;
    float timetostop = 5.1f;
    bool flag = false;
    public GameObject meteor;
    public static int healthPoints = 150;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
         animator = GetComponent<Animator>();
        agent.autoBraking = false;
        animator.SetBool("Stopping", false);
        flag = false;
        meteor.SetActive(false);
        GotoNextPoint();



    }
    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        // agent.destination = new Vector3(points[destPoint].position.x,0, points[destPoint].position.z);
        agent.SetDestination(points[destPoint].position);
        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;
        InvokeRepeating("stopworking", 0.0f, 40f);
        InvokeRepeating("workagain", 10.0f, 40f);

    }

    // Update is called once per frame
    void Update()
    {

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            GotoNextPoint();
        animator.SetFloat("Speed", agent.velocity.magnitude);

    }

    void stopworking()
    {
        flag = true;
        nextActionTime += period;
        agent.isStopped = true;
        animator.SetBool("Stopping", true);
        InvokeRepeating("meteorshower", 3.0f, 40f);

    }
    void workagain()
    {
        timetostop = nextActionTime + 5;
        agent.isStopped = false;
        animator.SetBool("Stopping", false);
        flag = false;
        meteor.SetActive(true);


    }
    void meteorshower()
    {
        meteor.SetActive(false);
    }

}
