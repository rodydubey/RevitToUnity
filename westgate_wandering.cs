using UnityEngine;
using System.Collections;

public class westgate_wandering : MonoBehaviour
{

    public float wanderRadius;
    public float wanderTimer;
    public GameObject participant1;
    private Transform target;
    private NavMeshAgent agent;
    private float timer;
    public float WanderTimeHeadLookAt;
    private float timerHeadLookAt;
    public bool wanderingFlag = false;
    // Use this for initialization
    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        timerHeadLookAt = WanderTimeHeadLookAt;
    }

    // Update is called once per frame
    void Update()
    {
        if (wanderingFlag)
        {
            timer += Time.deltaTime;
            timerHeadLookAt += Time.deltaTime;
            if (timer >= wanderTimer)
            {
                Vector3 newPos = RandomNavSphere(participant1.transform.position, wanderRadius, -1);

                newPos.y = 0.84f;
                participant1.GetComponent<BehaviorMecanim>().Character.NavGoTo(newPos);
                participant1.GetComponent<BehaviorMecanim>().Character.HeadLookAt(newPos);
                //agent.SetDestination(newPos);
                timer = 0;
            }
            if (timerHeadLookAt >= WanderTimeHeadLookAt)
            {
                Vector3 headLookAt = RandomDirection(participant1.transform.position, wanderRadius, -1);
                headLookAt.y = 2.0f;
                participant1.GetComponent<BehaviorMecanim>().Character.HeadLookAt(headLookAt);
                timerHeadLookAt = 0;
            }
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
    public static Vector3 RandomDirection(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }
}
