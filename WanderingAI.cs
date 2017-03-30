using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class WanderingAI : MonoBehaviour
{
    public float speed = 3.0f;
    public float obstacleRange = 5.0f;
    public string BehaviourTree = "BT";
    // private bool _alive = true;
    //public float thrust;
    public Rigidbody rb;
    public bool reached = false;
    public Transform trans;
    public List<string> items = new List<string>();
    Animator anim;
    int idle = Animator.StringToHash("stop");
    int walk = Animator.StringToHash("man_walk");
    //[SerializeField] private GameObject fireballPrefab;
    //private GameObject _fireball;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        //List<string> items = BehaviourTreeAffordanceDictionary[BehaviourTree];
        //anim.SetTrigger(walk);
        //anim.GetCurrentAnimatorStateInfo.
        // Creates the controller
        //var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath("Assets/PopulationSystem/Animations/man controller.controller");
        //// Add StateMachines
        //var rootStateMachine = controller.layers[0].stateMachine;
        //var stateMachineA = rootStateMachine.AddStateMachine("smA");
        //var stateA1 = stateMachineA.AddState("stateA1");
        //// Add Transitions
        //anim.SetTrigger(walk);
        //environmentSetup singleton = environmentSetup.Instance;
        Dictionary<string, List<string>> BehaviourTreeAffordanceDictionary = environmentSetup.BehaviourTreeAffordanceDictionary;
        // Dictionary<string, List<string>> BehaviourTreeAffordanceDictionary = environmentSetup.Instance.BehaviourTreeAffordanceDictionary;
        if (BehaviourTreeAffordanceDictionary.ContainsKey(BehaviourTree))
        {
            items = BehaviourTreeAffordanceDictionary[BehaviourTree];
        }
        foreach (var gameObj in
                         FindObjectsOfType(typeof(GameObject)) as GameObject[])
        {
            Debug.Log("Object Name : " + gameObj.name); 
            Metadata meta = gameObj.GetComponent<Metadata>();
            if (meta)
            {
                if (meta.affordances != null)
                {
                    for (int i = 0; i <= items.Count-1; i++)
                    {
                        for (int j = 0; j <= meta.affordances.Length-1; j++)
                        {
                            if (items[i] == meta.affordances[j])
                            {
                                trans = gameObj.transform;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        //RaycastHit hit1;
        float move = Input.GetAxis("Vertical");
        anim.SetFloat("Speed", move);

        var distance = Vector3.Distance(trans.position, transform.position);
        if (!reached && distance < 1.5)
        {
            reached = true;
            //change the state of character to idle
            anim.SetTrigger(idle);
        }
        if (!reached)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            transform.Translate(0, 0, speed * Time.deltaTime);
            //rb.AddForce(0,0,1);
            //rb.velocity = transform.forward * 1;
            //if (rb.SweepTest(transform.forward, out hit1, 10))
            //{
            //    if (hit1.distance < obstacleRange)
            //    {
            //        float angle = Random.Range(-10, 50);
            //        transform.Rotate(0, angle, 0);
            //    }
            //}
            if (Physics.SphereCast(ray, 0.15f, out hit, 100))
            {
                GameObject hitObject = hit.transform.gameObject;




                //if (hitObject.GetComponent<PlayerCharacter>())
                //{

                //}
                if (hit.distance < obstacleRange)
                {
                    float angle = Random.Range(-10, 50);
                    transform.Rotate(0, angle, 0);
                }
            }
        }
        else
        {

        }
    }
}
