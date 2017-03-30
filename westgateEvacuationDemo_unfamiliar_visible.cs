using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using TreeSharpPlus;
using System.Collections.Generic;

public class westgateEvacuationDemo_unfamiliar_visible : MonoBehaviour
{
    public GameObject wander;
    public GameObject participant1; 

    private BehaviorAgent behaviorAgent;
    public bool waveFlag = true;
    public Transform information;
   // public Transform nearestExit;
    public Transform nearestExitCommon;
    public string BehaviourTree = "exit";
    public float speed = 3.0f;
    public float obstacleRange = 5.0f;
    public bool foundExit = false;
    public float wanderRadius = 5f;
    public float wanderTimer = 5f;
    public float distance;
    public float strength = 5.0f;
    private float timer;
    int turn = 0;
    public List<string> items = new List<string>();
    bool rotate = true;
    float angle = 360.0f; // Degree per time unit
    float time = 8.0f; // Time unit in sec
    Vector3 axis = Vector3.up; // Rotation axis, here it the yaw axis
   
    // Use this for initialization
    void Start()
    {
        behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
        behaviorAgent.StartBehavior();
        timer = wanderTimer;
        Dictionary<string, List<string>> BehaviourTreeAffordanceDictionary = environmentSetup.BehaviourTreeAffordanceDictionary;
        List<GameObject> gameObjectList = environmentSetup.gameObjectList;
        if (BehaviourTreeAffordanceDictionary.ContainsKey(BehaviourTree))
        {
            items = BehaviourTreeAffordanceDictionary[BehaviourTree];
        }

    }
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, NavMesh.AllAreas);

        return navHit.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!foundExit)
        {

            //rotate first 360 degree without moving
            if (rotate)
            {
                //Quaternion targetRotation = Quaternion.LookRotation(wander.transform.position - participant1.transform.position);
                //var str = Mathf.Min(strength * Time.deltaTime, 1);
                //participant1.transform.rotation = Quaternion.Lerp(participant1.transform.rotation, targetRotation, str);
                turn++;
               
                participant1.GetComponent<Transform>().Rotate(axis, angle * Time.deltaTime / time);
                if (turn > 200)
                    rotate = false;
            }
            else
            {
                RaycastHit hit;
                float move = Input.GetAxis("Vertical");
                Ray ray = new Ray(participant1.transform.position, participant1.transform.forward);
                //participant1.transform.Translate(0, 0, speed * Time.deltaTime);
                if (Physics.SphereCast(ray, 0.5f, out hit, 100))
                {
                    //Vector3 forward = participant1.transform.TransformDirection(Vector3.forward) * 100;
                    //Debug.DrawRay(participant1.transform.position, forward, Color.green, 20, true);
                    GameObject hitObject = hit.transform.gameObject;
                    Metadata meta = hitObject.GetComponent<Metadata>();
                    if (meta)
                    {
                        if (meta.affordances != null)
                        {
                            //list of gameObject with metadata so that we dont loop many times.
                            for (int k = 0; k <= items.Count - 1; k++)
                            {
                                for (int j = 0; j <= meta.affordances.Length - 1; j++)
                                {
                                    if (items[k] == meta.affordances[j])
                                    {
                                        float distance = Vector3.Distance(hitObject.transform.position, transform.position);
                                        wander.transform.position = hitObject.transform.position;
                                        //position = nearestTrans.position;
                                        foundExit = true;
                                    }
                                    else
                                    {
                                        //nearestTrans.Translate(0, 0, speed * Time.deltaTime);
                                        //Vector3 position = nearestTrans.position;
                                        //float angle = UnityEngine.Random.Range(-10, 50);
                                        //wander.transform.Rotate(0, angle, 0);
                                        //wander.transform.Translate(0, 0, speed * Time.deltaTime);

                                    }
                                    break;
                                }
                            }
                        }
                        else
                        {
                            timer += Time.deltaTime;

                            distance = Vector3.Distance(participant1.transform.position, wander.transform.position);
                            if (timer >= wanderTimer || distance < 3.5)
                            {
                                Vector3 newPos = RandomNavSphere(wander.transform.position, wanderRadius, -1);
                                while (distance < 2.5)
                                {
                                    newPos = RandomNavSphere(wander.transform.position, wanderRadius, -1);
                                    distance = Vector3.Distance(participant1.transform.position, newPos);
                                }
                                //wander.transform.position = newPos;
                                timer = 0;
                            }
                        }
                    }
                    else
                    {
                        timer += Time.deltaTime;

                        distance = Vector3.Distance(participant1.transform.position, wander.transform.position);
                        if (timer >= wanderTimer || distance < 3.5)
                        {
                            Vector3 newPos = RandomNavSphere(wander.transform.position, wanderRadius, -1);
                            while (distance < 3.5)
                            {
                                newPos = RandomNavSphere(wander.transform.position, wanderRadius, -1);
                                distance = Vector3.Distance(participant1.transform.position, newPos);
                            }
                           // wander.transform.position = newPos;
                            timer = 0;
                        }
                    }
                }
                else
                {
                    timer += Time.deltaTime;

                    distance = Vector3.Distance(participant1.transform.position, wander.transform.position);
                    if (timer >= wanderTimer || distance < 3.5)
                    {
                        Vector3 newPos = RandomNavSphere(wander.transform.position, wanderRadius, -1);
                        while (distance < 3.5)
                        {
                            newPos = RandomNavSphere(wander.transform.position, wanderRadius, -1);
                            distance = Vector3.Distance(participant1.transform.position, newPos);
                        }
                       // wander.transform.position = newPos;
                        timer = 0;
                    }
                }
            }
        }
    }
    
    protected Node ST_ApproachAndWait(Transform source, Transform target)
    {
        Val<Vector3> position = Val.V(() => target.position);
        return new Sequence(source.GetComponent<BehaviorMecanim>().Node_GoToUpToRadius(position, 2), new LeafWait(1000));        
    }
 
    protected Node goToExitCommon(Transform target)
    {
        //evacuatingAgent eA = target.GetComponent<evacuatingAgent>();
        //wander.transform.position = eA.nearestTrans.position;
        return new Sequence(this.ST_ApproachAndWait(target.transform, wander.transform));
        //return new LeafWait(1000);
    }
    protected Node BuildMainTreeRoot(GameObject CurrentPerson)
    {
        return
            new Sequence(wanderTofindExit(CurrentPerson));
    }
    protected Node wanderTofindExit(GameObject CurrentPerson)
    {

        return new Sequence(new LeafWait(6000),
            goToExitCommon(CurrentPerson.transform));
       // return new Sequence(this.ST_ApproachAndWait(CurrentPerson.transform, wanderTransform.transform));
     
    }
    protected Node CheckFoundExitPoint(GameObject CurrentPerson)
    {
        evacuatingAgent eA = CurrentPerson.GetComponent<evacuatingAgent>();
        return new LeafAssert(() => (eA.foundExit == true));
    }
    protected Node BuildTreeRoot()
    {
        //return new Sequence(this.goToExit1(this.participant1.transform));
        //evacuatingAgent eA = this.participant1.GetComponent<evacuatingAgent>();
       // Func<bool> act = () => (!rotate);
       // Node roaming = new DecoratorLoop(
                      // new SequenceParallel(                       
                     //  this.BuildMainTreeRoot(this.participant1)));
        //return roaming;
       // Node trigger = new DecoratorLoop(new LeafAssert(act));
       // Node root = new DecoratorLoop(new DecoratorForceStatus(RunStatus.Success, new SequenceParallel(trigger, roaming)));
        //return root;
        return new DecoratorLoop(
                new SequenceParallel(
                this.BuildMainTreeRoot(this.participant1)));
    }
}