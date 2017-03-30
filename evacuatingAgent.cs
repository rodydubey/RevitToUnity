using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class evacuatingAgent : MonoBehaviour
{
    [SerializeField]
    //public Canvas messageCanvas;
    //public GameObject fooText;
    public string BehaviourTree = "BT";
    public List<string> items = new List<string>();
    public List<Transform> trans = new List<Transform>();
    public Transform nearestTrans;
    public Vector3 position;
    float minDistance = 9999;
    public float speed = 3.0f;
    public float obstacleRange = 5.0f;
    public bool familiarToEnvironment = true;
    public bool foundExit = false;
    void Start()
    {
        //nearestTrans = new GameObject().transform;
        //messageCanvas.enabled = false;
        //TurnOnMessage();
        Dictionary<string, List<string>> BehaviourTreeAffordanceDictionary = environmentSetup.BehaviourTreeAffordanceDictionary;
        List<GameObject> gameObjectList = environmentSetup.gameObjectList;
        if (BehaviourTreeAffordanceDictionary.ContainsKey(BehaviourTree))
        {
            items = BehaviourTreeAffordanceDictionary[BehaviourTree];
        }
        if (familiarToEnvironment)
        {
            for (var i = 0; i < gameObjectList.Count; i++)
            {
                Metadata meta = gameObjectList[i].GetComponent<Metadata>();
                for (int k = 0; k <= items.Count - 1; k++)
                {
                    for (int j = 0; j <= meta.affordances.Length - 1; j++)
                    {
                        if (items[k] == meta.affordances[j])
                        {
                            trans.Add(gameObjectList[i].transform);
                            float distance = Vector3.Distance(gameObjectList[i].transform.position, transform.position);
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                nearestTrans = gameObjectList[i].transform;
                            }
                            break;
                        }
                    }
                }
            }
        }
        else if(!familiarToEnvironment)
        {
            //random walk till the ray tracer hits the exit board
            //nearestTrans = new Transform();
            //nearestTrans.Translate(0, 0, speed * Time.deltaTime);
            int i = 0;
        }
               
    }
    void Update()
    {
        if (familiarToEnvironment)
        {
            //find nearest exit from it's current location
            for (var i = 0; i < trans.Count; i++)
            {
                float distance = Vector3.Distance(trans[i].position, transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestTrans = trans[i];
                }
            }
        }
        else if (!familiarToEnvironment && !foundExit)
        {
            RaycastHit hit;
            float move = Input.GetAxis("Vertical");
            Ray ray = new Ray(transform.position, transform.forward);
            transform.Translate(0, 0, speed * Time.deltaTime);
               
            if (Physics.SphereCast(ray, 0.5f, out hit, 1000))
            {
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
                                    nearestTrans = hitObject.transform;
                                    position = nearestTrans.position;
                                    foundExit = true;
                                }
                                else
                                {
                                    //nearestTrans.Translate(0, 0, speed * Time.deltaTime);
                                    //Vector3 position = nearestTrans.position;
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}