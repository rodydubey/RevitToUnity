using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Diagnostics;
public class westgateEvacuationDemo_FollowSignageToExit : MonoBehaviour
{
    public GameObject wander;
    public GameObject participant1; //rohit blue
    public GameObject raycast;
    public GameObject lookAt;
    public GameObject neck;
    private GameObject hitObject;
    // STEP 1: Access to line of sight
    public LineOfSight_1 _lineOfSight;
    private bool wanderingFlag = false;
    public westgate_wandering _westgate_wandering;
    public float eyeSight = 1.0f; // Vision value ranging from 0 to 1;
    public float agentHeight = 1.75f; // height of the agent
    public float radiusOfSphere = 0.6f; // sphere cast radius. Field of view cone simulation - rough hack
    public float knowledgeOfEnglish = 1.0f; //0 for no knowledge and 1 for fluent in english
    public float attention = 0.2f; // attention value of the participant
    float lengthOfSight = 30f; // default sight length
    private bool goalDecided = false;
    private BehaviorAgent behaviorAgent;
    public bool waveFlag = true;
    public Transform information;
    // public Transform nearestExit;
    public Transform nearestExitCommon;
    public string BehaviourTree = "exit";
    public float speed = 3.0f;
    public float obstacleRange = 5.0f;
    public bool foundExit = false;
    public float wanderRadius = 1f;
    public float wanderTimer = 5f;
    public float distance;
    public float strength = 5.0f;
    private float timer;
    public string goal;
    public GameObject goalObject;
    public GameObject signC;
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
    private Vector3 goalPositionTemp;
    int numberOfFramesVisible = 0;
    private Vector3 curLoc;
    private Vector3 prevLoc;
    System.IO.StreamWriter file;
    private bool loopOnce = false;
    private Vector3 finalPosition = new Vector3(0,0,0);
    public float wanderRadius2 = 100;
    public float wanderTimer2 = 10;
    public float WanderTimeHeadLookAt = 5;
    private float timerHeadLookAt;
    private bool executeWanderingNow = false;
    private string angleIndex;
    private bool onetimeStraightPathDone = false;
    public Text winText;
    void Start()
    {
        file = new System.IO.StreamWriter(participant1.name + ".txt");
       
        //timer = wanderTimer;
        Dictionary<string, List<string>> BehaviourTreeAffordanceDictionary = environmentSetup.BehaviourTreeAffordanceDictionary;
        List<GameObject> gameObjectList = environmentSetup.gameObjectList;
        if (BehaviourTreeAffordanceDictionary.ContainsKey(BehaviourTree))
        {
            items = BehaviourTreeAffordanceDictionary[BehaviourTree];
        }
        raycast.transform.position.Set(raycast.transform.position.x, agentHeight, raycast.transform.position.z);
        Vector3 position = wander.transform.position;
        position.y = 0.84f;
        participant1.GetComponent<BehaviorMecanim>().Character.NavGoTo(position);
        winText.text = "";
        //participant1.GetComponent<BehaviorMecanim>().Character.NavTurn(wander.transform.localRotation);

        ////calculate how easy it is to navigate to target destination using sign provided as an input to I(a,b,c)
        //float information = 0.0f;
        //Vector3 startVec = raycast.transform.position;
        //RaycastHit hit;
        //Vector3 rayDirectionToGoal = goalObject.transform.position - startVec;
        //Vector3 rayDirectionToSignC = signC.transform.position - startVec;
        //Physics.Raycast(startVec, rayDirectionToGoal, out hit, 30);
        //if (hit.collider.gameObject != goalObject)
        //{
        //    //loop through all signages
        //    for ()
        //    {
        //        Physics.Raycast(startVec, rayDirectionToSignC, out hit, 30);
        //        if (hit.collider.gameObject != signC)
        //        {
        //            information = 0.0f;
        //        }
        //        else
        //        {
        //            //check if signC has information to reach goal
        //            Metadata meta = signC.transform.gameObject.GetComponent<Metadata>();
        //            GameObject go = GameObject.Find(meta.affordances[5]);
        //            if (go == goalObject)
        //            {
        //                information = 1 / 2;
        //            }
        //            else
        //            {
        //                //loop through all the signage information
        //            }
        //        }
        //    }
        //}
        //else
        //{
        //    information = 1.0f;
        //}
    }
    //public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    //{
    //    Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;

    //    randDirection += origin;

    //    NavMeshHit navHit;

    //    NavMesh.SamplePosition(randDirection, out navHit, dist, NavMesh.AllAreas);

    //    return navHit.position;
    //}

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
    public static Vector3 RandomDirection(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
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
    void SetCountText()
    {
            winText.text = "Agent cannot find Wayfinding Cues";
    }
    void Update()
    {
        if (!goalDecided)
        {
            Vector3 tempGridPosition = participant1.transform.position;
            tempGridPosition.y = 1.72f;
            bool foundGoal = false;
            //int playerNodeIndex = Grid.gridGlobal.NodeIndexFromWorldPoint(tempGridPosition);
            NodeGrid currentGridOfPlayer = Grid.gridGlobal.NodeFromWorldPoint(tempGridPosition);
            List<RaycastHit> rayhitList = _lineOfSight.getAllTransformByTag("signage");
            //find uniique transform from rayhitList
            GameObject lastHitObject = null;
            List<GameObject> selectedSignage = new List<GameObject>();
            List<GameObject> hitSignage = new List<GameObject>();
            if (currentGridOfPlayer != null && currentGridOfPlayer.gridSignageVisibilityDictionary.Count > 0)
            {

                for (int r = 0; r < rayhitList.Count; ++r)
                {
                    GameObject hitObjectLocal = rayhitList[r].transform.gameObject;
                    if (hitObjectLocal != lastHitObject)
                    {
                        hitSignage.Add(hitObjectLocal);
                    }
                    lastHitObject = hitObjectLocal;
                }
                foreach (KeyValuePair<string, int> temp in currentGridOfPlayer.gridSignageVisibilityDictionary)
                {
                    for (int l = 0; l < hitSignage.Count; ++l)
                    {
                        hitObject = hitSignage[l].transform.gameObject;
                        Metadata meta = hitObject.GetComponent<Metadata>();
                        if (meta)
                        {
                            if (meta.affordances != null)
                            {
                                for (int j = 0; j <= meta.affordances.Length - 1; j++)
                                {
                                    if (hitObject.name == temp.Key)
                                    {
                                        //perform five ray test final VCA test
                                        bool visible = CanSeeSignage(hitObject);
                                        numberOfFramesVisible = numberOfFramesVisible + 1;
                                        if (numberOfFramesVisible > 0)
                                        {
                                            
                                            if (goal == meta.affordances[j])
                                            {
                                                selectedSignage.Add(hitObject);
                                                numberOfFramesVisible = 0;
                                                hitObject.GetComponent<Renderer>().material.color = Color.red;
                                                foundGoal = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //Check which signage to choose out of viewable signage
            int minAngleIndex = 9999;
            float distanceBetweenCurrentAndGoalSignage = 0;
            Vector3 newPosition = new Vector3(0, 0, 0);
            int hostSignageIndex = -1;
            for (int m = 0; m < selectedSignage.Count; ++m)
            {
                //GameObject go = GameObject.Find(selectedSignage[m]);
                if (selectedSignage[m])
                {
                    Metadata meta = selectedSignage[m].GetComponent<Metadata>();
                     angleIndex = meta.affordances[4]; // to implement direction 
                    if (Convert.ToInt32(angleIndex) < minAngleIndex)
                    {
                        minAngleIndex = Convert.ToInt32(angleIndex);
                        newPosition = GameObject.Find(meta.affordances[5]).transform.position;
                        hostSignageIndex = m;
                        angleIndex = meta.affordances[4];
                        distanceBetweenCurrentAndGoalSignage = Vector3.Distance(selectedSignage[hostSignageIndex].transform.position, newPosition);
                    }
                }
            }
            newPosition.y = 0.84f;
            var distance = Vector3.Distance(this.participant1.transform.position, newPosition);
            
            
            if (foundGoal && distance < 30.01)
            {
                wanderingFlag = false;
                participant1.GetComponent<BehaviorMecanim>().Character.NavGoTo(newPosition);
                goalDecided = true;
                goalPositionTemp = newPosition;
                executeWanderingNow = false;
                loopOnce = false;
                foundGoal = false;
                onetimeStraightPathDone = false;
                winText.text = "Goal Found!";
                //participant1.GetComponent<BehaviorMecanim>().Character.HeadLookStop();
            }
            
            else if (distanceBetweenCurrentAndGoalSignage > 30.01 && loopOnce == false)
            {
                // implement searching behaviour
                Vector3 fwd30 = new Vector3(0, 0, 0);
                if (Convert.ToInt32(angleIndex) == 3)
                     fwd30 = selectedSignage[hostSignageIndex].transform.position + (-selectedSignage[hostSignageIndex].transform.right * 30);
                else if (Convert.ToInt32(angleIndex) == 2)
                    fwd30 = selectedSignage[hostSignageIndex].transform.position + (selectedSignage[hostSignageIndex].transform.right * 30);
                if (Convert.ToInt32(angleIndex) == 1)
                    fwd30 = selectedSignage[hostSignageIndex].transform.position + (selectedSignage[hostSignageIndex].transform.forward * 30);
                NavMeshHit hit;
                NavMesh.SamplePosition(fwd30, out hit, 30, 1);
                finalPosition = hit.position;
                participant1.GetComponent<BehaviorMecanim>().Character.NavGoTo(finalPosition);              
                loopOnce = true;
               wanderingFlag = true;
            }
            //hitObject.GetComponent<Renderer>().material.color = Color.red;
            var distanceWanderingParticipant = Vector3.Distance(this.participant1.transform.position, finalPosition);
            if (wanderingFlag && distanceWanderingParticipant < 2)
            {
                executeWanderingNow = true;
                SetCountText();

            }
            if(executeWanderingNow)
            {
                timer += Time.deltaTime;
                timerHeadLookAt += Time.deltaTime;
                if (timer >= wanderTimer2)
                {
                   
                    if (!onetimeStraightPathDone)
                    {
                        Vector3 newPos = participant1.transform.position + (participant1.transform.transform.forward * 30);
                        participant1.GetComponent<BehaviorMecanim>().Character.NavGoTo(newPos);
                        onetimeStraightPathDone = true;
                    }
                    else
                    {
                        Vector3 newPos = RandomNavSphere(participant1.transform.position, wanderRadius2, -1);
                        newPos.y = 0.84f;
                        participant1.GetComponent<BehaviorMecanim>().Character.NavGoTo(newPos);
                        participant1.GetComponent<BehaviorMecanim>().Character.HeadLookAt(newPos);
                    }

                    //newPos.y = 0.84f;

                   
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

        else
        {
            var distance = Vector3.Distance(this.participant1.transform.position, goalPositionTemp);
            if(distance < 15)
            {
                goalDecided = false;
            }


        }
        //if (wanderingFlag)
        //{
        //    Vector3 fwd30 = this.participant1.transform.position + (this.participant1.transform.right * 30);
        //    participant1.GetComponent<BehaviorMecanim>().Character.NavGoTo(fwd30);
        //    wanderingFlag = false;
        //}
        //if (/*!goalDecided &&*/ this.participant1.transform.position.x == 0 && this.participant1.transform.position.y == 0 && this.participant1.transform.position.z ==0)
        //{
        //    timer += Time.deltaTime;
        //    timerHeadLookAt += Time.deltaTime;
        //    if (timer >= wanderTimer2)
        //    {
        //        Vector3 newPos = RandomNavSphere(participant1.transform.position, wanderRadius2, -1);

        //        newPos.y = 0.84f;
        //        participant1.GetComponent<BehaviorMecanim>().Character.NavGoTo(newPos);
        //        //participant1.GetComponent<BehaviorMecanim>().Character.HeadLookAt(newPos);
        //        //agent.SetDestination(newPos);
        //        timer = 0;
        //    }
        //    if (timerHeadLookAt >= WanderTimeHeadLookAt)
        //    {
        //        Vector3 headLookAt = RandomDirection(participant1.transform.position, wanderRadius, -1);
        //        headLookAt.y = 2.0f;
        //        //participant1.GetComponent<BehaviorMecanim>().Character.HeadLookAt(headLookAt);
        //        timerHeadLookAt = 0;
        //    }
        //}

    }
    // Update is called once per frame
    //void Update()
    //{
    //    //Vector3 position = wander.transform.position;
    //    //participant1.GetComponent<BehaviorMecanim>().Character.NavGoTo(position);
    //    //Grid grid = GetComponent<Grid>();
    //    if (randomSpawner.numberOfAgent > 2)
    //    {
    //        var originalPoint = new Vector3(raycast.transform.position.x, raycast.transform.position.y, raycast.transform.position.z);
    //        if (!foundExit)
    //        {
    //            //RaycastHit hit;
    //            float move = Input.GetAxis("Vertical");
    //            Ray ray = new Ray(raycast.transform.position, raycast.transform.forward);
    //            if (_lineOfSight.SeeByTag("signage"))
    //            {
    //                //    // STEP 4: Change the color of viewing area
    //                _lineOfSight.SetStatus(LineOfSight_1.Status.Alerted);
    //                // _lineOfSight.castRayFlag = false;


    //                //    }

    //                //if (Physics.SphereCast(ray, 100, out hit, 30))
    //                //    {                   
    //                // GameObject hitObject = hit.transform.gameObject;
    //                RaycastHit rayhit = _lineOfSight.getTransformByTag("signage");
    //                List<RaycastHit> rayhitList = _lineOfSight.getAllTransformByTag("signage");
    //                //find uniique transform from rayhitList
    //                GameObject lastHitObject = null;
    //                List<string> selectedSignage = new List<string>();
    //                List<GameObject> hitSignage = new List<GameObject>();
    //                for (int r = 0; r < rayhitList.Count; ++r)
    //                {
    //                    GameObject hitObjectLocal = rayhitList[r].transform.gameObject;
    //                    if (hitObjectLocal != lastHitObject)
    //                    {
    //                        hitSignage.Add(hitObjectLocal);
    //                    }
    //                    lastHitObject = hitObjectLocal;
    //                }
    //                for (int l = 0; l < hitSignage.Count; ++l)
    //                {
    //                    hitObject = hitSignage[l].transform.gameObject;
    //                    Metadata meta = hitObject.GetComponent<Metadata>();

    //                    if (meta)
    //                    {
    //                        if (meta.affordances != null)
    //                        {
    //                            //list of gameObject with metadata so that we dont loop many times.
    //                            for (int k = 0; k <= items.Count - 1; k++)
    //                            {
    //                                for (int j = 0; j <= meta.affordances.Length - 1; j++)
    //                                {
    //                                    // if (items[k] == meta.affordances[j])
    //                                    // {

    //                                    var distance = Vector3.Distance(this.participant1.transform.position, hitObject.transform.position);
    //                                    // var rotationVector = hitObject.transform.rotation.eulerAngles;
    //                                    //raycast.transform.rotation = Quaternion.Euler(rotationVector);
    //                                    prevLoc = curLoc;
    //                                    curLoc = raycast.transform.position;
    //                                    var targetPoint = new Vector3(hitObject.transform.position.x, raycast.transform.position.y, hitObject.transform.position.z) - raycast.transform.position;
    //                                    var targetRotation = Quaternion.LookRotation(-targetPoint, Vector3.up);
    //                                    //raycast.transform.rotation = Quaternion.Slerp(raycast.transform.rotation, targetRotation, Time.deltaTime * 2.0f);

    //                                    if (distance < 15.1 && distance > 2)
    //                                    {
    //                                        bool visible = CanSeeSignage(hitObject);

    //                                        //participant1.GetComponent<BehaviorMecanim>().Character.HeadLookAt(hitObject.transform.position);
    //                                        if (visible)
    //                                        {
    //                                            //    float distance = Vector3.Distance(hitObject.transform.position, transform.position);
    //                                            //    wander.transform.position = hitObject.transform.position;

    //                                            //position = nearestTrans.position;
    //                                            numberOfFramesVisible = numberOfFramesVisible + 1;
    //                                            //GameObject g = participant1.transform.FindChild("Mesh").gameObject;
    //                                            if (numberOfFramesVisible > 30)
    //                                            {
    //                                                foundExit = false;
    //                                                if (goal == meta.affordances[j])
    //                                                {
    //                                                    selectedSignage.Add(meta.affordances[5]);
    //                                                    hitObject.GetComponent<Renderer>().material.color = Color.red;
    //                                                    participant1.GetComponent<BehaviorMecanim>().Character.HeadLookAt(hitObject.transform.position);

    //                                                }
    //                                                //if (float.Parse(meta.affordances[4]) < attention || float.Parse(meta.affordances[4]) > 0.5)
    //                                                //{
    //                                                //    //Application.CaptureScreenshot("D://Codes//RevitToUnityFIle//test.png");
    //                                                //    //var stringPath = "D://Codes//RevitToUnityFIle//test.png";
    //                                                //    //var myProcess = new Process();
    //                                                //    //myProcess.StartInfo.FileName = "D://Codes//RevitToUnityFIle//SaliencyFilters.exe";
    //                                                //    //myProcess.StartInfo.Arguments = stringPath;
    //                                                //    //myProcess.Start();
    //                                                //    GameObject go = GameObject.Find(meta.affordances[5]);
    //                                                //    if (go)
    //                                                //    {
    //                                                //        Vector3 newPosition = go.transform.position;
    //                                                //        newPosition.y = 0.84f;
    //                                                //        participant1.GetComponent<BehaviorMecanim>().Character.NavGoTo(newPosition);
    //                                                //        hitObject.GetComponent<Renderer>().material.color = Color.red;
    //                                                //    }
    //                                                //    //targetPoint = new Vector3(raycast.transform.position.x, participant1.transform.position.y, participant1.transform.position.z) - raycast.transform.position;
    //                                                //    //raycast.transform.rotation = Quaternion.Euler(originalPoint);
    //                                                //    environmentSetup.numberOfHit++;
    //                                                //    //numberOfFramesVisible = 0;
    //                                                //    //Destroy(participant1.gameObject);
    //                                                //    //GameObject g = participant1.transform.FindChild("Mesh").gameObject;
    //                                                //    //this.participant1.transform.FindChild("Mesh").GetComponent<Renderer>().material.color = Color.green;
    //                                                //}
    //                                                participant1.GetComponent<BehaviorMecanim>().Character.HeadLookStop();
    //                                            }
    //                                        }
    //                                    }
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //                //Check which signage to choose out of viewable signage
    //                for (int m = 0; m < selectedSignage.Count; ++m)
    //                {
    //                    GameObject go = GameObject.Find(selectedSignage[m]);
    //                    if (go)
    //                    {
    //                        Vector3 newPosition = go.transform.position;
    //                        newPosition.y = 0.84f;
    //                        var distance = Vector3.Distance(this.participant1.transform.position, newPosition);
    //                        if (distance < 30.01)
    //                        {
    //                            participant1.GetComponent<BehaviorMecanim>().Character.NavGoTo(newPosition);
    //                            participant1.GetComponent<BehaviorMecanim>().Character.HeadLookStop();
    //                        }
    //                        //hitObject.GetComponent<Renderer>().material.color = Color.red;
    //                    }
    //                }
    //            }
    //        }
    //        else
    //        {
    //            file.Close();
    //        }


    //    }
    //    else if(randomSpawner.numberOfAgent > 180)
    //    {
    //        Vector3 position = wander.transform.position;
    //        position.y = 0.84f;
    //        participant1.GetComponent<BehaviorMecanim>().Character.NavGoTo(position);

    //    }
    //    //Vector3 tempGridPosition = participant1.transform.position;
    //    //tempGridPosition.y = 1.72f;
    //    //int playerNodeIndex = Grid.gridGlobal.NodeFromWorldPoint(tempGridPosition);
    //    //Grid.gridGlobal.ColorGrid(playerNodeIndex);
    //    //if(playerNodeIndex != 0)
    //    //    file.WriteLine(playerNodeIndex + "," + participant1.transform.position);
    //}

    //Algorithm to Quantify I from location a to location b from a sign c
    //if(Location b is visibile from location a)
    //{
    //    I(a, b, c) = 1.0f;
    //}
    //else
    //{
    //    if(Sign c is visible from location a)
    //    {
    //        if (sign c has information to reach location b)
    //        {
    //                    I(a, b, c) = 1.0f;
    //        }
    //        else
    //        {
    //            loop(all signage information at sign c)
    //            {
    //                recursiveFunction(To Check if sign i has information about location b)
    //                {
    //                    break(once information found)
    //                }
    //            }
    //            if(information found about location b from ith sign starting from sign c)
    //            {
    //                calculate value of I(a, b, c) based on the number of iteration. (To quantify how nested the location of b is relative to sign c)
    //            }
    //            else
    //            {
    //                //unable to find location b
    //                I(a, b, c) = 0.0f;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        I(a, b, c) = 0.0f;
    //    }
    //}





























    // remove

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