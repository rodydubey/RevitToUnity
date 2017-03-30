using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
public class Grid : MonoBehaviour
{

    public static Grid gridGlobal;

    void Awake()
    {
        Grid.gridGlobal = GetComponent<Grid>();
    }

    public Transform player;
    public Transform player1;
    public GameObject temp;
    public Transform floor;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public float nodeHeight;
    NodeGrid[,] grid;
    public float maxVisibility = 5.0f;
    public float minVisibility = 2.0f;
    float nodeDiameter;
    int gridSizeX, gridSizeY;
    float totalConnectivity = 0;
    public float connectivity = 0;
    bool drawn = false;
    public GameObject[] signages;
    public GameObject goalObject;
    public GameObject goalObject1;
    public GameObject tempSignage;
    float minDistanceToCenter = 9999;
    float minDistanceToGrid = 9999;
    public bool sign1001 = false;
    public bool sign1002 = false;
    public bool sign1003 = false;
    public bool sign1014 = false;
    public bool overAllConnectivity = false;
    public bool overAllInformation = false;
    public bool mutualInformation = false;
    Vector3 centerNodeWorldPosition;
    bool calculatedOnce = false;
    bool insideif = false;
    bool insideifForSecondSign = false;
    bool testFlag = false;
    int testIndex = 999999;
    [Range(0.0f, 1.0f)]
    public float arrowLength = 0.49f;
    [Range(0.0f, 1.0f)]
    public float threshold = 0.5f;
    List<int> allGridIndex = new List<int>();
    Dictionary<string, Dictionary<string, string>> signageDict = environmentSetup.signageDict;

    public Dictionary<string, Vector3> gridSignageCenterDictionary = new Dictionary<string, Vector3>();
    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();

        // getting list of game object with the tag name "signage"
        signages = GameObject.FindGameObjectsWithTag("signage");
       
        foreach (GameObject signage in signages)
        {

            centerNodeWorldPosition = new Vector3(0, 0, 0);
            //if (signage.name == "Signage [1024]")
            //{
                foreach (NodeGrid n in grid)
                {
                    if (n != null)
                    {
                        n.numberOfHit = 0;
                        Vector3 startVec = n.worldTransform.position;
                        Vector3 fwd = signage.transform.TransformDirection(Vector3.forward);
                        Vector3 center = signage.transform.position + (signage.transform.forward * maxVisibility );
                        center.y = nodeHeight;
                        var distanceToGrid = Vector3.Distance(startVec, center);
                        if (distanceToGrid < minDistanceToCenter)
                        {
                            minDistanceToCenter = distanceToGrid;
                            centerNodeWorldPosition = n.worldPosition;
                        }
                    }
                }
                //temp.transform.position = centerNodeWorldPosition;
            //}
            minDistanceToCenter = 9999;
            gridSignageCenterDictionary.Add(signage.name, centerNodeWorldPosition);
        }

        foreach (NodeGrid n in grid)
        {
            if (n != null)
            {
                foreach (GameObject signage in signages)
                {
                    var distance = Vector3.Distance(gridSignageCenterDictionary[signage.name], n.worldPosition);
                    if (distance < maxVisibility + 0.1)
                    {

                        RaycastHit hit;
                        Vector3 startVec = n.worldTransform.position;
                        Vector3 fromPosition = n.worldTransform.position;
                        Vector3 rayDirection = signage.transform.position - startVec;
                        var tdistance = Vector3.Distance(fromPosition, signage.transform.position);
                        if (tdistance > minVisibility)
                        {
                            //var distance1 = Vector3.Distance(fromPosition, player1.position);
                            //Debug.DrawRay(n.worldTransform.position, rayDirection, Color.blue, 0, true);
                            Collider mCollider = signage.GetComponent<Collider>();
                            Bounds bounds = mCollider.bounds;

                            Vector3 boundPoint1 = bounds.min;
                            Vector3 boundPoint2 = bounds.max;
                            Vector3 boundPoint3 = new Vector3(boundPoint1.x, boundPoint1.y, boundPoint2.z);
                            Vector3 boundPoint4 = new Vector3(boundPoint1.x, boundPoint2.y, boundPoint1.z);
                            Vector3 boundPoint5 = new Vector3(boundPoint2.x, boundPoint1.y, boundPoint1.z);
                            Vector3 boundPoint6 = new Vector3(boundPoint1.x, boundPoint2.y, boundPoint2.z);
                            Vector3 boundPoint7 = new Vector3(boundPoint2.x, boundPoint1.y, boundPoint2.z);
                            Vector3 boundPoint8 = new Vector3(boundPoint2.x, boundPoint2.y, boundPoint1.z);
                            Vector3 rightTop = boundPoint8 - fromPosition;
                            Vector3 leftBottom = boundPoint3 - fromPosition;
                            Vector3 rightBottom = boundPoint5 - fromPosition;
                            Vector3 leftTop = boundPoint2 - fromPosition;
                            int numberOfHit = 0;

                            if (Physics.Raycast(fromPosition, rayDirection, out hit, 40))
                            {
                                if (hit.collider.gameObject == signage.gameObject)
                                    numberOfHit++;
                            }
                            if (Physics.Raycast(fromPosition, rightTop, out hit, 40))
                            {
                                if (hit.collider.gameObject == signage.gameObject)
                                    numberOfHit++;
                            }
                            if (Physics.Raycast(fromPosition, leftBottom, out hit, 40))
                            {
                                if (hit.collider.gameObject == signage.gameObject)
                                    numberOfHit++;
                            }
                            if (Physics.Raycast(fromPosition, rightBottom, out hit, 40))
                            {
                                if (hit.collider.gameObject == signage.gameObject)
                                    numberOfHit++;
                            }
                            if (Physics.Raycast(fromPosition, leftTop, out hit, 40))
                            {
                                if (hit.collider.gameObject == signage.gameObject)
                                    numberOfHit++;
                            }


                            n.gridSignageVisibilityDictionary.Add(signage.name, numberOfHit);
                        }
                    }
                }
            }

        }
        //System.IO.StreamWriter file1 = new System.IO.StreamWriter("D:\\SignageInformation_1100_all.txt");
        //System.IO.StreamWriter file2 = new System.IO.StreamWriter("D:\\SignageInformation_2575.txt");
        ////file1.WriteLine("ITERATION_NUMBER" + "," + "VISIBILITY_PROBABILITY");
        //for (int n = 0; n < 10000; ++n)
        //{
        //    //randomly assigned visual attention to the agent. A value from 0 to 1. 
        //    float randomNumber = UnityEngine.Random.Range(1, 101);
        //    float visualAttention = randomNumber / 100;
        //    float visibilityProbability1 = 0.0f;
        //    float visibilityProbability2 = 0.0f;
        //    foreach (NodeGrid g in grid)
        //    {
        //        if (g != null)
        //        {
        //            if (g.index == 1100) //2565 has 4 rays. 2575 has 2 rays
        //            {
        //                foreach (KeyValuePair<string, int> temp in g.gridSignageVisibilityDictionary)
        //                {
        //                    //if (temp.Key == "Signage [1001]")
        //                   // {
        //                        visibilityProbability1 = visibilityProbability1 + (float)temp.Value / 5.0f * visualAttention;
        //                    //}
        //                }
        //                file1.WriteLine(visibilityProbability1 / (float) g.gridSignageVisibilityDictionary.Count);
        //            }
        //            //if (g.index == 2575) //2565 has 4 rays. 2575 has 2 rays
        //            //{

        //            //    foreach (KeyValuePair<string, int> temp in g.gridSignageVisibilityDictionary)
        //            //    {
        //            //        if (temp.Key == "Signage [1001]")
        //            //        {
        //            //            visibilityProbability2 = (float)temp.Value / 5.0f * visualAttention;

        //            //        }
        //            //    }
        //            //}
        //        }
        //    }

        //    file2.WriteLine(visibilityProbability2);

        //file1.WriteLine("ITERATION_NUMBER" + "," + "VISIBILITY_PROBABILITY");
        //for (int n = 0; n < 10000; ++n)
        //{
        //    //randomly assigned visual attention to the agent. A value from 0 to 1. 
        //    //float randomNumber = UnityEngine.Random.Range(1, 101);
        //    //float visualAttention = randomNumber / 100;
        //    float visibilityProbability1 = 0.0f;
        //    float visibilityProbability2 = 0.0f;
        //foreach (NodeGrid g in grid)
        //{
        //    if (g != null)
        //    {
        //        System.IO.StreamWriter file = new System.IO.StreamWriter("D:\\VisibilityData\\" + g.index + ".txt");
        //        for (int nn = 0; nn < 10000; ++nn)
        //        {
        //            float randomNumber = UnityEngine.Random.Range(1, 101);
        //            float visualAttention = randomNumber / 100;
        //            float visibilityProbability1 = 0.0f;
        //            foreach (KeyValuePair<string, int> temp in g.gridSignageVisibilityDictionary)
        //            {
        //                visibilityProbability1 = visibilityProbability1 + ((float)temp.Value / 5.0f) * visualAttention;
        //            }
        //            if(g.gridSignageVisibilityDictionary.Count>0)
        //                file.WriteLine(visibilityProbability1 / (float)g.gridSignageVisibilityDictionary.Count);
        //            else
        //                file.WriteLine(g.gridSignageVisibilityDictionary.Count);
        //    }
        //        file.Close();
        //    }
        //}



        //System.IO.StreamWriter file = new System.IO.StreamWriter("D:\\test.txt");
        //file.WriteLine("GRID_INDEX_NUMBER" + "," + "SIGNAGE_NAME" + "," + "NUMBER_OF_RAYS_HIT");

        //foreach (NodeGrid n in grid)
        //{
        //    if (n != null)
        //    {

        //        //print signage information
        //        //Console.WriteLine();
        //        //file.WriteLine(n.index);
        //        int numberOfRaysHit = 0;
        //        foreach (KeyValuePair<string, int> temp in n.gridSignageVisibilityDictionary)
        //        {
        //            //display each product to console by using Display method in Farm Shop class
        //            //Console.WriteLine(temp.Key + "    " + temp.Value);

        //            file.WriteLine(n.index + "," + temp.Key + "," + temp.Value);
        //            numberOfRaysHit = numberOfRaysHit + temp.Value;

        //        }
        //    }
        //}
        //file.Close();

        //foreach (string line in lines)
        //{
        //    // Use a tab to indent each line of the file.
        //    Console.WriteLine("\t" + line);


        //}

        //System.IO.StreamWriter file = new System.IO.StreamWriter("D:\\Noise_Signage[1001]_[1002]_allSmallGrid_WithDistanceAngle28thMarch.txt");
        //foreach (NodeGrid n in grid)
        //{
        //    if (n != null)
        //    {
        //        float averageDistanceNoise = 0.0f;
        //        float averageAngleNoise = 0.0f;
        //        float averageRaycast = 0.0f;
        //        float averageRaycast2 = 0.0f;
        //        float averageDistanceNoise2 = 0.0f;
        //        float averageAngleNoise2 = 0.0f;
        //        insideif = false;
        //        insideifForSecondSign = false;
        //        //if (n.index == 10671)
        //        //{
        //        //    int g = 0;
        //        //}
        //        ; GameObject tempSingage = new GameObject(); float distanceTo = 0.0f; float angleBetweenSignageAndGrid = 0.0f; float absAngleBetweenSignageAndGrid = 0.0f;
        //        float angleBetweenSignageAndGrid2 = 0.0f; float absAngleBetweenSignageAndGrid2 = 0.0f;
        //        foreach (KeyValuePair<string, int> temp in n.gridSignageVisibilityDictionary)
        //        {

        //            if (temp.Key == "Signage [1001]")
        //            {
        //                averageRaycast = averageRaycast + (float)temp.Value / 5.0f;
        //                tempSingage = GameObject.Find(temp.Key);
        //                distanceTo = Vector3.Distance(n.worldTransform.position, tempSingage.transform.position);
        //                Vector3 directionBetweenSignageAndGrid = tempSingage.transform.position - n.worldTransform.position;
        //                angleBetweenSignageAndGrid = Mathf.Atan2(directionBetweenSignageAndGrid.y, directionBetweenSignageAndGrid.x) * Mathf.Rad2Deg;
        //                if (angleBetweenSignageAndGrid > 90)
        //                    absAngleBetweenSignageAndGrid = Math.Abs(90 - angleBetweenSignageAndGrid);
        //                else
        //                    absAngleBetweenSignageAndGrid = 90 - angleBetweenSignageAndGrid;
        //                averageDistanceNoise = distanceTo / 30;
        //                averageAngleNoise = absAngleBetweenSignageAndGrid / 90;
        //                insideif = true;
        //            }
        //            if (temp.Key == "Signage [1002]")
        //            {
        //                averageRaycast2 = averageRaycast2 + (float)temp.Value / 5.0f;
        //                tempSingage = GameObject.Find(temp.Key);
        //                distanceTo = Vector3.Distance(n.worldTransform.position, tempSingage.transform.position);
        //                Vector3 directionBetweenSignageAndGrid = tempSingage.transform.position - n.worldTransform.position;
        //                angleBetweenSignageAndGrid2 = Mathf.Atan2(directionBetweenSignageAndGrid.y, directionBetweenSignageAndGrid.x) * Mathf.Rad2Deg;
        //                if (angleBetweenSignageAndGrid2 > 90)
        //                    absAngleBetweenSignageAndGrid2 = Math.Abs(90 - angleBetweenSignageAndGrid2);
        //                else
        //                    absAngleBetweenSignageAndGrid2 = 90 - angleBetweenSignageAndGrid2;
        //                averageDistanceNoise2 = distanceTo / 30;
        //                averageAngleNoise2 = absAngleBetweenSignageAndGrid2 / 90;
        //                insideifForSecondSign = true;
        //            }
        //        }

        //        //int numberOfVisibleSignage = n.gridSignageVisibilityDictionary.Count;
        //        if (averageRaycast > 0 && averageRaycast2 > 0 && insideif && insideifForSecondSign)
        //            file.WriteLine(n.index + "," + averageRaycast + "," + averageDistanceNoise + "," + averageAngleNoise + "," + averageRaycast2 + "," + averageDistanceNoise2 + "," + averageAngleNoise2);
        //        //else
        //        //    file.WriteLine(n.index + "," + 0 + "," + 0 + "," + 0);
        //    }
        //}

        //file.Close();

        //System.IO.StreamWriter file = new System.IO.StreamWriter("D:\\Noise_Signage[1002]_allSmallGrid_WithDistanceAngle29thMarch.txt");
        //foreach (NodeGrid n in grid)
        //{
        //    if (n != null)
        //    {
        //        float averageDistanceNoise = 0.0f;
        //        float averageAngleNoise = 0.0f;
        //        float averageRaycast = 0.0f;
        //        float averageRaycast2 = 0.0f;
        //        float averageDistanceNoise2 = 0.0f;
        //        float averageAngleNoise2 = 0.0f;
        //        insideif = false;
        //        insideifForSecondSign = false;
        //        //if (n.index == 10671)
        //        //{
        //        //    int g = 0;
        //        //}
        //        ; GameObject tempSingage = new GameObject(); float distanceTo = 0.0f; float angleBetweenSignageAndGrid = 0.0f; float absAngleBetweenSignageAndGrid = 0.0f;
        //        float angleBetweenSignageAndGrid2 = 0.0f; float absAngleBetweenSignageAndGrid2 = 0.0f;
        //        foreach (KeyValuePair<string, int> temp in n.gridSignageVisibilityDictionary)
        //        {

        //            //if (temp.Key == "Signage [1001]")
        //            //{
        //            //    averageRaycast = averageRaycast + (float)temp.Value / 5.0f;
        //            //    tempSingage = GameObject.Find(temp.Key);
        //            //    distanceTo = Vector3.Distance(n.worldTransform.position, tempSingage.transform.position);
        //            //    Vector3 directionBetweenSignageAndGrid = tempSingage.transform.position - n.worldTransform.position;
        //            //    angleBetweenSignageAndGrid = Mathf.Atan2(directionBetweenSignageAndGrid.y, directionBetweenSignageAndGrid.x) * Mathf.Rad2Deg;
        //            //    if (angleBetweenSignageAndGrid > 90)
        //            //        absAngleBetweenSignageAndGrid = Math.Abs(90 - angleBetweenSignageAndGrid);
        //            //    else
        //            //        absAngleBetweenSignageAndGrid = 90 - angleBetweenSignageAndGrid;
        //            //    averageDistanceNoise = distanceTo / 30;
        //            //    averageAngleNoise = absAngleBetweenSignageAndGrid / 90;
        //            //    insideif = true;
        //            //}
        //            if (temp.Key == "Signage [1002]")
        //            {
        //                averageRaycast2 = averageRaycast2 + (float)temp.Value / 5.0f;
        //                tempSingage = GameObject.Find(temp.Key);
        //                distanceTo = Vector3.Distance(n.worldTransform.position, tempSingage.transform.position);
        //                Vector3 directionBetweenSignageAndGrid = tempSingage.transform.position - n.worldTransform.position;
        //                angleBetweenSignageAndGrid2 = Mathf.Atan2(directionBetweenSignageAndGrid.y, directionBetweenSignageAndGrid.x) * Mathf.Rad2Deg;
        //                //angleBetweenSignageAndGrid2 = angleBetweenSignageAndGrid2 + 90;
        //                if (angleBetweenSignageAndGrid2 > 90)
        //                    absAngleBetweenSignageAndGrid2 = Math.Abs(90 - angleBetweenSignageAndGrid2);
        //                else
        //                    absAngleBetweenSignageAndGrid2 = 90 - angleBetweenSignageAndGrid2;
        //                averageDistanceNoise2 = distanceTo / 30;
        //                averageAngleNoise2 = absAngleBetweenSignageAndGrid2 / 90;
        //                insideifForSecondSign = true;
        //            }
        //        }

        //        //int numberOfVisibleSignage = n.gridSignageVisibilityDictionary.Count;
        //        if (insideifForSecondSign)
        //            file.WriteLine(n.index + "," + averageRaycast2 + "," + averageDistanceNoise2 + "," + averageAngleNoise2);
        //        else
        //            file.WriteLine(n.index + "," + 0 + "," + 0 + "," + 0);
        //    }
        //}

        //file.Close();

        //System.IO.StreamWriter file = new System.IO.StreamWriter("D:\\Noise_allSmallGrid_WithAngleDistanceFourPillars_23thMarch.txt");
        //foreach (NodeGrid n in grid)
        //{
        //    if (n != null)
        //    {
        //        float averageDistanceNoise = 0.0f;
        //        float averageAngleNoise = 0.0f;
        //        float averageRaycast = 0.0f;
        //        ; GameObject tempSingage = new GameObject(); float distanceTo = 0.0f; float angleBetweenSignageAndGrid = 0.0f; float absAngleBetweenSignageAndGrid = 0.0f;
        //        foreach (KeyValuePair<string, int> temp in n.gridSignageVisibilityDictionary)
        //        {

        //            averageRaycast = averageRaycast + (float)temp.Value / 5.0f;
        //            tempSingage = GameObject.Find(temp.Key);
        //            distanceTo = Vector3.Distance(n.worldTransform.position, tempSingage.transform.position);
        //            Vector3 directionBetweenSignageAndGrid = tempSingage.transform.position - n.worldTransform.position;
        //            angleBetweenSignageAndGrid = Mathf.Atan2(directionBetweenSignageAndGrid.y, directionBetweenSignageAndGrid.x) * Mathf.Rad2Deg;
        //            absAngleBetweenSignageAndGrid = Math.Abs(90 - angleBetweenSignageAndGrid);
        //            averageDistanceNoise = averageDistanceNoise + distanceTo / 30;
        //            averageAngleNoise = averageAngleNoise + absAngleBetweenSignageAndGrid / 90;
        //        }

        //        int numberOfVisibleSignage = n.gridSignageVisibilityDictionary.Count;
        //        if (numberOfVisibleSignage > 0)
        //            file.WriteLine(n.index + "," + averageRaycast / numberOfVisibleSignage + "," + averageDistanceNoise / numberOfVisibleSignage + "," + averageAngleNoise / numberOfVisibleSignage);
        //        else
        //            file.WriteLine(n.index + "," + 0 + "," + 0 + "," + 0);
        //    }
        //}

        //file.Close();


        //// To write files for I(a,,). I(a,_,_) is the general information 
        ////available at location a to get to any other point in the environment 
        ////from all signs. This is an information theoretic measure of connectivity of a particular location
        //// Number of gates : 9
        //Signage[1004]
        //Signage[1005]
        //Signage[1006]
        //Signage[1007]
        //Signage[1008]
        //Signage[1009]
        //Signage[1010]
        //Signage[1011]
        //Signage[1012]

        //System.IO.StreamWriter file = new System.IO.StreamWriter("D:\\Connectivity_of_Any_Point_23ndMarch_4_Pillars.txt");
        //foreach (NodeGrid n in grid)
        //{
        //    if (n != null)
        //    {
        //        //if (n.index == 2565)
        //        //{
        //        bool insideloop = false;
        //        float raycast = 0.0f; GameObject tempSingage = new GameObject(); float distanceTo = 0.0f; float angleBetweenSignageAndGrid = 0.0f;
        //        int counter = 0;
        //        int counter1004 = 0;
        //        int counter1005 = 0;
        //        int counter1006 = 0;
        //        int counter1007 = 0;
        //        int counter1008 = 0;
        //        int counter1009 = 0;
        //        int counter1010 = 0;
        //        int counter1011 = 0;
        //        int counter1012 = 0;

        //        foreach (KeyValuePair<string, int> temp in n.gridSignageVisibilityDictionary)
        //        {

        //            if (signageDict.ContainsKey(temp.Key))
        //            {
        //                Dictionary<string, string> tempSignDict = signageDict[temp.Key];
        //                foreach (KeyValuePair<string, string> tempp in tempSignDict)
        //                {

        //                    if (tempp.Key == "Signage [1004]")
        //                    {
        //                        if (tempSignDict[tempp.Key] != "999")
        //                            counter1004++;
        //                    }
        //                    else if (tempp.Key == "Signage [1005]")
        //                    {
        //                        if (tempSignDict[tempp.Key] != "999")
        //                            counter1005++;
        //                    }
        //                    else if (tempp.Key == "Signage [1006]")
        //                    {
        //                        if (tempSignDict[tempp.Key] != "999")
        //                            counter1006++;
        //                    }
        //                    else if (tempp.Key == "Signage [1007]")
        //                    {
        //                        if (tempSignDict[tempp.Key] != "999")
        //                            counter1007++;
        //                    }
        //                    else if (tempp.Key == "Signage [1008]")
        //                    {
        //                        if (tempSignDict[tempp.Key] != "999")
        //                            counter1008++;
        //                    }
        //                    else if (tempp.Key == "Signage [1009]")
        //                    {
        //                        if (tempSignDict[tempp.Key] != "999")
        //                            counter1009++;
        //                    }
        //                    else if (tempp.Key == "Signage [1010]")
        //                    {
        //                        if (tempSignDict[tempp.Key] != "999")
        //                            counter1010++;
        //                    }
        //                    else if (tempp.Key == "Signage [1011]")
        //                    {
        //                        if (tempSignDict[tempp.Key] != "999")
        //                            counter1011++;
        //                    }
        //                    else if (tempp.Key == "Signage [1012]")
        //                    {
        //                        if (tempSignDict[tempp.Key] != "999")
        //                            counter1012++;
        //                    }


        //                }
        //            }
        //        }
        //        if (counter1004 > 0)
        //            counter++;
        //        if (counter1005 > 0)
        //            counter++;
        //        if (counter1006 > 0)
        //            counter++;
        //        if (counter1007 > 0)
        //            counter++;
        //        if (counter1008 > 0)
        //            counter++;
        //        if (counter1009 > 0)
        //            counter++;
        //        if (counter1010 > 0)
        //            counter++;
        //        if (counter1011 > 0)
        //            counter++;
        //        if (counter1012 > 0)
        //            counter++;

        //        file.WriteLine(n.index + "," + counter);

        //        //}
        //    }
        //}

        //file.Close();

    }
    //public static void ForGizmo(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    //{
    //    Gizmos.DrawRay(pos, direction);

    //    Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
    //    Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
    //    Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
    //    Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
    //}

    public void ForGizmo(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.15f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.color = color;
        //arrowLength = 0.25f;
        Gizmos.DrawRay(pos, direction* arrowLength);

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Gizmos.DrawRay(pos + direction* arrowLength, right * arrowHeadLength * arrowLength);
        Gizmos.DrawRay(pos + direction * arrowLength, left * arrowHeadLength * arrowLength);
    }
    void CreateGrid()
    {
        grid = new NodeGrid[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        int Counter = 0;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                if(x ==85 && y == 95)
                {
                    int g = 0;
                }
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                //worldTransform.position.y = 1.6f;
                Vector3 startVec = worldPoint;
                startVec.y = startVec.y + 5f;
                Vector3 rayDirection = worldPoint;
                rayDirection.y = -3.0f;
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                RaycastHit hit;
                //Debug.DrawRay(startVec, rayDirection, Color.blue, 10, true);
                if (Physics.Raycast(startVec, rayDirection - startVec, out hit, 20))
                {
                    //Debug.Log(hit.collider.gameObject);
                    if (hit.collider.gameObject == floor.gameObject)
                    {
                        //Debug.DrawRay(startVec, rayDirection - startVec, Color.blue, 20, false);
                        Transform worldTransform = new GameObject().transform;
                        worldPoint.y = nodeHeight;
                        worldTransform.position = worldPoint;
                        grid[x, y] = new NodeGrid(walkable, worldTransform);
                        grid[x, y].index = Counter++;
                    }
                }
            }
        }     
    }

    public int NodeIndexFromWorldPoint(Vector3 worldPosition)
    {
        int x = 0;
       
        foreach (NodeGrid n in grid)
        {
            if (n != null)
            {
                var distanceToGrid = Vector3.Distance(n.worldTransform.position, worldPosition);
                if (distanceToGrid < minDistanceToGrid)
                {
                    minDistanceToGrid = distanceToGrid;
                    x = n.index;
                }
            }
        }
        minDistanceToGrid = 9999;
        return x;
    }

    public NodeGrid NodeFromWorldPoint(Vector3 worldPosition)
    {
        Transform t = new GameObject().transform;
        NodeGrid ng = new NodeGrid(true, t);

        foreach (NodeGrid n in grid)
        {
            if (n != null)
            {
                var distanceToGrid = Vector3.Distance(n.worldTransform.position, worldPosition);
                if (distanceToGrid < minDistanceToGrid)
                {
                    minDistanceToGrid = distanceToGrid;
                    ng = n;
                }
            }
        }
        minDistanceToGrid = 9999;
        return ng;
    }

    public void ColorGrid(int playerNodeIndex)
    {
        allGridIndex.Add(playerNodeIndex);
        testFlag = true;
    }
    private static float Normalize(float minimum, float maximum, float value)
    {
        return (value - minimum) / (maximum - minimum);
    }
    public static float GetRgbValues(float minimum, float maximum, float value)
    {
        float normalizedValue = Normalize(minimum, maximum, value);
        return Distance(normalizedValue);
    }
    private static int Distance(float value)
    {
        var distance = Math.Abs(value);

        var colorStrength = distance;

        if (colorStrength < 0)
            colorStrength = 0;

        return (int)Math.Round(colorStrength * 255);
    }
    void OnDrawGizmos()
    {
        if (sign1001) // entropy due to 1001
        {
            sign1002 = false;
            sign1003 = false;
            overAllConnectivity = false;
            overAllInformation = false;
            Vector3 size = new Vector3(1f, 0.01f, 1f);
            // Write the string to a file.
            //System.IO.StreamWriter file = new System.IO.StreamWriter("D:\\test.txt");
            //file.WriteLine("GRID_INDEX_NUMBER" + "," + "SIGNAGE_NAME" + "," + "NUMBER_OF_RAYS_HIT");

            //foreach (NodeGrid n in grid)
            //{
            //    if (n != null)
            //    {

            //        //print signage information
            //        //Console.WriteLine();
            //        //file.WriteLine(n.index);
            //        int numberOfRaysHit = 0;
            //        foreach (KeyValuePair<string, int> temp in n.gridSignageVisibilityDictionary)
            //        {
            //            //display each product to console by using Display method in Farm Shop class
            //            //Console.WriteLine(temp.Key + "    " + temp.Value);

            //            //file.WriteLine(n.index + "," + temp.Key + "," + temp.Value);
            //            numberOfRaysHit = numberOfRaysHit + temp.Value;

            //        }
            //        int intensity = numberOfRaysHit;
            //        float redValue = intensity / 20.0f;
            //        //if (redValue > 0)
            //        //    redValue = redValue + 0.3f;
            //        Gizmos.color = new Color(redValue, 0, 0);
            //        Gizmos.DrawCube(n.worldPosition, size * (nodeDiameter));

            //    }
            //}
            //file.Close();
            //Vector3 size = new Vector3(1f, 0.01f, 1f);
            string[] lines = System.IO.File.ReadAllLines(@"D:\VisibilityData\Entropy_Signage[1001]_allSmallGrid_WithDistanceAngle29thMarch.txt");
            float minColor = 9.100765f;
            float maxColor = 9.541683f;
            foreach (NodeGrid n in grid)
            {
                if (n != null)
                {
                    //if (n.index >= 2351  &&  n.index <= 2353/* ||*/ /*n.index == 23558*/)
                    //   { 
                    float redValue = 0;
                    float greenValue = 0;
                    float blueValue = 0;
                    float intensity = float.Parse(lines[n.index]);
                    if (intensity == 0)
                    {
                        redValue = 0;
                        greenValue = 0;
                        blueValue = 0;
                    }

                    else
                    {
                        //redValue = 1.0f - intensity / 2.076876f;
                        redValue = GetRgbValues(minColor, maxColor, intensity);
                        redValue = redValue / 255.0f;
                        redValue = 1 - redValue;

                        //    //redValue = redValue + 0.1f;
                        //    if (redValue > 1)
                        //        redValue = 1;
                        //}
                        if (n.index >= 10500 && n.index <= 11000)
                        {
                            Gizmos.color = new Color(redValue, 0, 0);
                            Gizmos.DrawCube(n.worldPosition, size * (nodeDiameter));
                        }
                        //if (redValue > threshold)
                        //{
                        //    Color c = new Color(0, 0.0f, 1.0f);
                        //    Vector3 left = new Vector3(1.0f, 0.0f, 0.0f);
                        //    ForGizmo(n.worldPosition, left, c);
                        //}
                        //else if (redValue > 0.0 && redValue <= threshold)
                        //{
                        //    Color cc = new Color(1.0f, 0.0f, 0.0f);
                        //    Vector3 right = new Vector3(-1.0f, 0.0f, 0.0f);
                        //    ForGizmo(n.worldPosition, right, cc);
                        //}
                    }
                }
            }
            //sign1001 = false;
        }
        if (sign1003) // // entropy due to 1014
        {
            Vector3 size = new Vector3(1f, 0.01f, 1f);
            sign1001 = false;
            sign1002 = false;
            overAllConnectivity = false;
            overAllInformation = false;

            //for Signage 1001
            string[] lines = System.IO.File.ReadAllLines(@"D:\VisibilityData\Entropy_Signage1001_allSmallGrid_WithDistance27thMarch.txt");
            float minColor = 9.0f;
            float maxColor = 27.49779f;

            //for Signage 1014
            //string[] lines = System.IO.File.ReadAllLines(@"D:\VisibilityData\Entropy_Signage[1014_allSmallGrid_WithDistance27thMarch.txt");
            //float minColor = 9.10073f;
            //float maxColor = 27.94031f;


            ////for Signage 1014_1001
            //string[] lines = System.IO.File.ReadAllLines(@"D:\VisibilityData\MutualInformation_Signage[1014_1001_allSmallGrid_WithDistance27thMarch.txt");
            //float minColor = 0.05342984f;
            //float maxColor = 0.1425003f;
            foreach (NodeGrid n in grid)
            {
                if (n != null)
                {
                   //if (n.index == 10671/* ||*/ /*n.index == 23558*/)
                   //{ 
                    float redValue = 0;
                    float greenValue = 0;
                    float blueValue = 0;
                    float intensity = float.Parse(lines[n.index]);
                        if (intensity == 0)
                        {
                            redValue = 0;
                            greenValue = 0;
                            blueValue = 0;
                        }

                        else
                        {
                            //redValue = 1.0f - intensity / 2.076876f;
                            redValue = GetRgbValues(minColor, maxColor, intensity);
                            redValue = redValue / 255.0f;
                        //redValue = 1 - redValue;

                        //    //redValue = redValue + 0.1f;
                        //    if (redValue > 1)
                        //        redValue = 1;
                        //}
                        //Gizmos.color = new Color(redValue, redValue, redValue);
                        //Gizmos.DrawCube(n.worldPosition, size * (nodeDiameter));
                        if (redValue > threshold)
                        {
                            Color c = new Color(0, 0.0f, 1.0f);
                            Vector3 left = new Vector3(1.0f, 0.0f, 0.0f);
                            ForGizmo(n.worldPosition, left, c);
                        }
                        else if (redValue > 0.0 && redValue <= threshold)
                        {
                            Color cc = new Color(1.0f, 0.0f, 0.0f);
                            Vector3 right = new Vector3(-1.0f, 0.0f, 0.0f);
                            ForGizmo(n.worldPosition, right, cc);
                        }
                        //    }
                    }
                }
            }
        }
        if (sign1002) // Mutual Information due to 1001_1014
        {
            Vector3 size = new Vector3(1f, 0.01f, 1f);
            sign1001 = false;
            sign1003 = false;
            overAllConnectivity = false;
            overAllInformation = false;
            string[] lines = System.IO.File.ReadAllLines(@"D:\VisibilityData\Entropy_Signage[1002]_allSmallGrid_WithDistanceAngle29thMarch.txt");
            float minColor = 9.099169f;
            float maxColor = 9.530378f;
            foreach (NodeGrid n in grid)
            {
                if (n != null)
                {
                    //if (n.index >= 2351  &&  n.index <= 2353/* ||*/ /*n.index == 23558*/)
                    //   { 
                    float redValue = 0;
                    float greenValue = 0;
                    float blueValue = 0;
                    float intensity = float.Parse(lines[n.index]);
                    if (intensity == 0)
                    {
                        redValue = 0;
                        greenValue = 0;
                        blueValue = 0;
                    }

                    else
                    {
                        //redValue = 1.0f - intensity / 2.076876f;
                        redValue = GetRgbValues(minColor, maxColor, intensity);
                        redValue = redValue / 255.0f;
                        redValue = 1 - redValue;

                        //    //redValue = redValue + 0.1f;
                        //    if (redValue > 1)
                        //        redValue = 1;
                        //}
                        Gizmos.color = new Color(redValue, 0, 0);
                        Gizmos.DrawCube(n.worldPosition, size * (nodeDiameter));
                        //if (redValue > threshold)
                        //{
                        //    Color c = new Color(0, 0.0f, 1.0f);
                        //    Vector3 left = new Vector3(1.0f, 0.0f, 0.0f);
                        //    ForGizmo(n.worldPosition, left, c);
                        //}
                        //else if (redValue > 0.0 && redValue <= threshold)
                        //{
                        //    Color cc = new Color(1.0f, 0.0f, 0.0f);
                        //    Vector3 right = new Vector3(-1.0f, 0.0f, 0.0f);
                        //    ForGizmo(n.worldPosition, right, cc);
                        //}
                    }
                }
            }
        }
        if (sign1014) // Information due to 1014
        {
            Vector3 size = new Vector3(1f, 0.01f, 1f);
            sign1001 = false;
            sign1003 = false;
            overAllConnectivity = false;
            overAllInformation = false;
            string[] lines = System.IO.File.ReadAllLines(@"D:\VisibilityData\Entropy_Signage[1014]_allSmallGrid_WithDistanceAngle29thMarch.txt");
            float minColor = 9.106593f;
            float maxColor = 9.892962f;
            foreach (NodeGrid n in grid)
            {
                if (n != null)
                {
                    //if (n.index >= 2351  &&  n.index <= 2353/* ||*/ /*n.index == 23558*/)
                    //   { 
                    float redValue = 0;
                    float greenValue = 0;
                    float blueValue = 0;
                    float intensity = float.Parse(lines[n.index]);
                    if (intensity == 0)
                    {
                        redValue = 0;
                        greenValue = 0;
                        blueValue = 0;
                    }

                    else
                    {
                        //redValue = 1.0f - intensity / 2.076876f;
                        redValue = GetRgbValues(minColor, maxColor, intensity);
                        redValue = redValue / 255.0f;
                        redValue = 1 - redValue;

                        //    //redValue = redValue + 0.1f;
                        //    if (redValue > 1)
                        //        redValue = 1;
                        //}
                        //if (n.index == 6992)
                        //{
                            Gizmos.color = new Color(redValue, 0, 0);
                            Gizmos.DrawCube(n.worldPosition, size * (nodeDiameter));
                       // }
                        //if (redValue > threshold)
                        //{
                        //    Color c = new Color(0, 0.0f, 1.0f);
                        //    Vector3 left = new Vector3(1.0f, 0.0f, 0.0f);
                        //    ForGizmo(n.worldPosition, left, c);
                        //}
                        //else if (redValue > 0.0 && redValue <= threshold)
                        //{
                        //    Color cc = new Color(1.0f, 0.0f, 0.0f);
                        //    Vector3 right = new Vector3(-1.0f, 0.0f, 0.0f);
                        //    ForGizmo(n.worldPosition, right, cc);
                        //}
                    }
                }
            }
        }
        if (overAllConnectivity)
        {
            Vector3 size = new Vector3(1f, 0.01f, 1f);
            sign1001 = false;
            sign1003 = false;
            overAllInformation = false;
            string[] lines = System.IO.File.ReadAllLines(@"D:\VisibilityData\Noise_allSmallGrid_WithAngleDistanceAllSignage_23thMarch_moreIteration.txt");
            float minColor = 11.41751f;
            float maxColor = 12.1412f;
            foreach (NodeGrid n in grid)
            {
                if (n != null)
                {
                    //if (n.index >= 2351  &&  n.index <= 2353/* ||*/ /*n.index == 23558*/)
                    //   { 
                    float redValue = 0;
                    float greenValue = 0;
                    float blueValue = 0;
                    float intensity = float.Parse(lines[n.index]);
                    if (intensity == 0)
                    {
                        redValue = 0;
                        greenValue = 0;
                        blueValue = 0;
                    }

                    else
                    {
                        //redValue = 1.0f - intensity / 2.076876f;
                        redValue = GetRgbValues(minColor, maxColor, intensity);
                        redValue = redValue / 255.0f;
                        redValue = 1 - redValue;

                        //redValue = redValue + 0.1f;
                        if (redValue > 1)
                            redValue = 1;
                    }
                    Gizmos.color = new Color(0, redValue, 0);
                    Gizmos.DrawCube(n.worldPosition, size * (nodeDiameter));
                }
            }
        }
        if (overAllInformation)
        {
            Vector3 size = new Vector3(1f, 0.01f, 1f);
            sign1001 = false;
            sign1003 = false;
            string[] lines = System.IO.File.ReadAllLines(@"D:\VisibilityData\Entropy_allSmallGrid_WithAngleDistanceFourPillars_23thMarch.txt");
            string[] lines1 = System.IO.File.ReadAllLines(@"D:\Connectivity_of_Any_Point_23ndMarch_4_Pillars.txt");
            float minColor = 9.09557f;
            float maxColor = 9.610145f;
            foreach (NodeGrid n in grid)
            {
                if (n != null)
                {
                    //if (n.index >= 2351  &&  n.index <= 2353/* ||*/ /*n.index == 23558*/)
                    //   { 
                    float redValue = 0;
                    float greenValue = 0;
                    float blueValue = 0;
                    float intensity = float.Parse(lines[n.index]);
                    if (intensity == 0)
                    {
                        redValue = 0;
                        greenValue = 0;
                        blueValue = 0;
                    }

                    else
                    {
                        //redValue = 1.0f - intensity / 2.076876f;
                        redValue = GetRgbValues(minColor, maxColor, intensity);
                        redValue = redValue / 255.0f;
                        redValue = 1 - redValue;
                        string[] tokens = lines1[n.index].Split(',');
                        float information = float.Parse(tokens[1]);
                        information = information / 9;
                        redValue = information * redValue;
                        totalConnectivity = totalConnectivity + redValue;
                        //redValue = redValue + 0.1f;
                        if (redValue > 1)
                            redValue = 1;
                    }
                    Gizmos.color = new Color(redValue, greenValue, blueValue);
                    Gizmos.DrawCube(n.worldPosition, size * (nodeDiameter));
                }
            }
            if (!calculatedOnce)
            {
                connectivity = totalConnectivity / 16961;
                calculatedOnce = true;
            }
        }
        if (mutualInformation)
        {
            Vector3 size = new Vector3(1f, 0.01f, 1f);
            sign1001 = false;
            sign1003 = false;
            string[] lines = System.IO.File.ReadAllLines(@"D:\VisibilityData\MutualInformation_Signage[1001_1002]_allSmallGrid_WithAngleDistance29thMarch.txt");
            float minColor = 0.0f;
            float maxColor = 0.0197492812190301f; ;
            foreach (string s in lines)
            {
                float redValue = 0;
                float greenValue = 0;
                float blueValue = 0;
                string[] tokens = s.Split(',');
                float intensity = float.Parse(tokens[1]);
                if (intensity == 0)
                {
                    redValue = 0;
                    greenValue = 0;
                    blueValue = 0;
                }

                else
                {
                    //redValue = 1.0f - intensity / 2.076876f;
                    redValue = GetRgbValues(minColor, maxColor, intensity);
                    redValue = redValue / 255.0f;
                    //redValue = 1 - redValue;

                    //redValue = redValue + 0.1f;
                    if (redValue > 1)
                        redValue = 1;
                }
                int gridIndex = int.Parse(tokens[0]);
                foreach (NodeGrid n in grid)
                {
                    if (n != null)
                    {
                        if (n.index == gridIndex)
                        {
                            Gizmos.color = new Color(redValue, greenValue, blueValue);
                            Gizmos.DrawCube(n.worldPosition, size * (nodeDiameter));
                        }
                    }
                }
            }
        }
        //else
        //{
        //    Vector3 size = new Vector3(1f, 0.01f, 1f);

        //    foreach (NodeGrid n in grid)
        //    {
        //        if (n != null)
        //        {
        //            Gizmos.color = new Color(1.0f, 0, 0);
        //            Gizmos.DrawCube(n.worldPosition, size * (nodeDiameter));
        //        }
        //    }
        //}
    } 

    }

public class NodeGrid
{

    public bool walkable;
    public Vector3 worldPosition;
    public Transform worldTransform;
    public int numberOfHit;
    public int directlyVisibleCounter;
    public int notVisibleToAnySignageCounter;
    public int pointsToDirectGoalCounter;
    public int TwoDirectionToGoalCounter;
    public int ThreeDirectionToGoalCounter;
    public int FourDirectionToGoalCounter;
    public int FiveDirectionToGoalCounter;
    public int SixDirectionToGoalCounter;
    public int numberOfIteration;
    public int index;

    public Dictionary<string,int> gridSignageVisibilityDictionary = new Dictionary<string, int>();

    public NodeGrid(bool _walkable, Transform _worldPos)
    {
        walkable = _walkable;
        worldTransform = _worldPos;
        worldPosition = worldTransform.position;
        directlyVisibleCounter = 0;
        notVisibleToAnySignageCounter = 0;
        pointsToDirectGoalCounter = 0;
        numberOfIteration = 999;
        TwoDirectionToGoalCounter = 0;
        ThreeDirectionToGoalCounter = 0;
        FourDirectionToGoalCounter = 0;
        FiveDirectionToGoalCounter = 0;
        SixDirectionToGoalCounter = 0;
    }
}