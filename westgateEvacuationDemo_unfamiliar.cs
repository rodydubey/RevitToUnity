using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Diagnostics;
public class westgateEvacuationDemo_unfamiliar : MonoBehaviour
{
    public GameObject wander;
    public GameObject participant1; //rohit blue
    public GameObject raycast;
    public GameObject lookAt;
    public GameObject neck;
    private GameObject hitObject;
    // STEP 1: Access to line of sight
    public LineOfSight_1 _lineOfSight;
    public float eyeSight = 1.0f; // Vision value ranging from 0 to 1;
    public float agentHeight = 1.75f; // height of the agent
    public float radiusOfSphere = 0.6f; // sphere cast radius. Field of view cone simulation - rough hack
    public float knowledgeOfEnglish = 1.0f; //0 for no knowledge and 1 for fluent in english
    public float attention = 0.2f; // attention value of the participant
    float lengthOfSight = 30f; // default sight length

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
    Vector3 axis1 = Vector3.left; // Rotation axis, here it the yaw axis
    //public Transform nearestExit2;
    //public Transform nearestExit3;
    //public Transform nearestExit4;
    //public Transform nearestExit5;
    //public Transform nearestExit6;
    //public Transform nearestExit7;
    //public Transform nearestExit8;
    //public Transform nearestExit9;
    //public Transform nearestExit10;
    // Use this for initialization
    public float yaw = 5.0f;
    public float minPitch = -20.0f;
    public float maxPitch = 20.0f;
    int pitch = -19;
    int tempLoop = 0;
    int numberOfFramesVisible = 0;
    private Vector3 curLoc;
    private Vector3 prevLoc;
    void Start()
    {
        //behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
       // BehaviorManager.Instance.Register(behaviorAgent);
        //behaviorAgent.StartBehavior();
        timer = wanderTimer;
        Dictionary<string, List<string>> BehaviourTreeAffordanceDictionary = environmentSetup.BehaviourTreeAffordanceDictionary;
        List<GameObject> gameObjectList = environmentSetup.gameObjectList;
        if (BehaviourTreeAffordanceDictionary.ContainsKey(BehaviourTree))
        {
            items = BehaviourTreeAffordanceDictionary[BehaviourTree];
        }
        raycast.transform.position.Set(raycast.transform.position.x,agentHeight, raycast.transform.position.z);
        Vector3 position = wander.transform.position;
        participant1.GetComponent<BehaviorMecanim>().Character.NavGoTo(position);
    }
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, NavMesh.AllAreas);

        return navHit.position;
    }
    //from http://forum.unity3d.com/threads/raycasting-a-cone-instead-of-single-ray.39426/
    bool CanSeeSignage(GameObject target)
    {
        float heightOfPlayer = 0.05f;

        Vector3 startVec = raycast.transform.position;
        startVec.y += heightOfPlayer;
        Vector3 startVecFwd = raycast.transform.forward * lengthOfSight;
        //startVecFwd.y += heightOfPlayer;

        Collider mCollider = target.GetComponent<Collider>();
        Bounds bounds = mCollider.bounds;

        Vector3 boundPoint1 = bounds.min;
        Vector3 boundPoint2 = bounds.max;
        Vector3 boundPoint3 = new Vector3(boundPoint1.x, boundPoint1.y, boundPoint2.z);
        Vector3 boundPoint4 = new Vector3(boundPoint1.x, boundPoint2.y, boundPoint1.z);
        Vector3 boundPoint5 = new Vector3(boundPoint2.x, boundPoint1.y, boundPoint1.z);
        Vector3 boundPoint6 = new Vector3(boundPoint1.x, boundPoint2.y, boundPoint2.z);
        Vector3 boundPoint7 = new Vector3(boundPoint2.x, boundPoint1.y, boundPoint2.z);
        Vector3 boundPoint8 = new Vector3(boundPoint2.x, boundPoint2.y, boundPoint1.z);



        RaycastHit hit;
        Vector3 rayDirection = target.transform.position - startVec;     
        Vector3 fromPosition = raycast.transform.position;
        // Vector3 toPosition = target.transform.position;
        Vector3 rightTop = boundPoint8 - fromPosition;
        UnityEngine.Debug.DrawRay(fromPosition, rightTop, Color.red, 0, true);
        Vector3 leftBottom = boundPoint3 - fromPosition;
        UnityEngine.Debug.DrawRay(fromPosition, leftBottom, Color.green, 0, true);
        Vector3 rightBottom = boundPoint5 - fromPosition;
        UnityEngine.Debug.DrawRay(fromPosition, rightBottom, Color.gray, 0, true);
        Vector3 leftTop = boundPoint2 - fromPosition;
        UnityEngine.Debug.DrawRay(fromPosition, leftTop, Color.cyan, 0, true);
        UnityEngine.Debug.DrawRay(raycast.transform.position, rayDirection, Color.blue, 0, true);
        int numberOfHit = 0;
        if (Physics.Raycast(startVec, rayDirection, out hit, 40))
        {
            if (hit.collider.gameObject == target)
                numberOfHit++;
        }
        if (Physics.Raycast(fromPosition, rightTop, out hit, 40))
        {
            if (hit.collider.gameObject == target)
                numberOfHit++;
        }
        if (Physics.Raycast(fromPosition, leftBottom, out hit, 40))
        {
            if (hit.collider.gameObject == target)
                numberOfHit++;
        }
        if (Physics.Raycast(fromPosition, rightBottom, out hit, 40))
        {
            if (hit.collider.gameObject == target)
                numberOfHit++;
        }
        if (Physics.Raycast(fromPosition, leftTop, out hit, 40))
        {
            if (hit.collider.gameObject == target)
                numberOfHit++;
        }
        if (numberOfHit > 2)
            return true;

        return false;
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 position = wander.transform.position;
        participant1.GetComponent<BehaviorMecanim>().Character.NavGoTo(position);
        var originalPoint = new Vector3(raycast.transform.position.x, raycast.transform.position.y, raycast.transform.position.z);
        if (!foundExit)
        {
                //RaycastHit hit;
                float move = Input.GetAxis("Vertical");
                Ray ray = new Ray(raycast.transform.position, raycast.transform.forward);
                if (_lineOfSight.SeeByTag("signage"))
                {
                //    // STEP 4: Change the color of viewing area
                    _lineOfSight.SetStatus(LineOfSight_1.Status.Alerted);
                // _lineOfSight.castRayFlag = false;
               

                //    }

                //if (Physics.SphereCast(ray, 100, out hit, 30))
                //    {                   
                // GameObject hitObject = hit.transform.gameObject;
                RaycastHit rayhit = _lineOfSight.getTransformByTag("signage");
                    hitObject = rayhit.transform.gameObject;
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

                                    var distance = Vector3.Distance(this.participant1.transform.position, hitObject.transform.position);
                                    // var rotationVector = hitObject.transform.rotation.eulerAngles;
                                    //raycast.transform.rotation = Quaternion.Euler(rotationVector);
                                    prevLoc = curLoc;
                                    curLoc = raycast.transform.position;
                                    var targetPoint = new Vector3(hitObject.transform.position.x, raycast.transform.position.y, hitObject.transform.position.z) - raycast.transform.position;
                                    var targetRotation = Quaternion.LookRotation(-targetPoint, Vector3.up);
                                    //raycast.transform.rotation = Quaternion.Slerp(raycast.transform.rotation, targetRotation, Time.deltaTime * 2.0f);
                                    participant1.GetComponent<BehaviorMecanim>().Character.HeadLookAt(hitObject.transform.position);
                                    if (distance < 10.1 && distance > 2)
                                    {
                                        bool visible = CanSeeSignage(hitObject);
                                        if (visible)
                                        {
                                            //    float distance = Vector3.Distance(hitObject.transform.position, transform.position);
                                            //    wander.transform.position = hitObject.transform.position;

                                            //position = nearestTrans.position;
                                            numberOfFramesVisible = numberOfFramesVisible + 1;
                                            //GameObject g = participant1.transform.FindChild("Mesh").gameObject;
                                            if (numberOfFramesVisible > 30)
                                            {
                                                foundExit = false;

                                                if (float.Parse(meta.affordances[4]) < attention || float.Parse(meta.affordances[4]) >0.5)
                                                {
                                                    //Application.CaptureScreenshot("D://Codes//RevitToUnityFIle//test.png");
                                                    //var stringPath = "D://Codes//RevitToUnityFIle//test.png";
                                                    //var myProcess = new Process();
                                                    //myProcess.StartInfo.FileName = "D://Codes//RevitToUnityFIle//SaliencyFilters.exe";
                                                    //myProcess.StartInfo.Arguments = stringPath;
                                                    //myProcess.Start();

                                                    hitObject.GetComponent<Renderer>().material.color = Color.red;
                                                   
                                                    //targetPoint = new Vector3(raycast.transform.position.x, participant1.transform.position.y, participant1.transform.position.z) - raycast.transform.position;
                                                    //raycast.transform.rotation = Quaternion.Euler(originalPoint);
                                                    environmentSetup.numberOfHit++;
                                                    //numberOfFramesVisible = 0;
                                                    //Destroy(participant1.gameObject);
                                                    //GameObject g = participant1.transform.FindChild("Mesh").gameObject;
                                                    //this.participant1.transform.FindChild("Mesh").GetComponent<Renderer>().material.color = Color.green;
                                                }
                                                participant1.GetComponent<BehaviorMecanim>().Character.HeadLookStop();
                                            }
                                        }
                                    }                                  
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
                        //wander.transform.position = newPos;
                        timer = 0;
                    }
                }
        }
    }
    
   
    //protected Node ST_RotateHead(Transform source, Transform target)
    //{
    //    Val<Vector3> position = Val.V(() => lookAt.transform.position);
    //    return new Sequence(source.GetComponent<BehaviorMecanim>().Node_HeadLook(position), new LeafWait(1000));
    //}
 
    //protected Node goToExitCommon(Transform target)
    //{
    //    //evacuatingAgent eA = target.GetComponent<evacuatingAgent>();
    //    //wander.transform.position = eA.nearestTrans.position;
    //    return new Sequence(this.ST_ApproachAndWait(target.transform, wander.transform),
    //        this.ST_RotateHead(target.transform, wander.transform));
    //    //return new LeafWait(1000);
    //}
    //protected Node BuildMainTreeRoot(GameObject CurrentPerson)
    //{
    //    return
    //        new Sequence(wanderTofindExit(CurrentPerson));
    //}
    //protected Node wanderTofindExit(GameObject CurrentPerson)
    //{

    //    return new Sequence(new LeafWait(10),
    //        goToExitCommon(CurrentPerson.transform));
    //   // return new Sequence(this.ST_ApproachAndWait(CurrentPerson.transform, wanderTransform.transform));
     
    //}
    //protected Node CheckFoundExitPoint(GameObject CurrentPerson)
    //{
    //    evacuatingAgent eA = CurrentPerson.GetComponent<evacuatingAgent>();
    //    return new LeafAssert(() => (eA.foundExit == true));
    //}
    //protected Node BuildTreeRoot()
    //{
    //    return new DecoratorLoop(
    //            new SequenceParallel(
    //            this.BuildMainTreeRoot(this.participant1)));
    //}
}