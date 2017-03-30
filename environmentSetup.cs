using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class environmentSetup : MonoBehaviour
{
    //static environmentSetup instance = null;
    //private environmentSetup()
    //{
    //    // Prevent outside instantiation
    //}
    //public static environmentSetup Instance
    //{
    //    get
    //    {
    //        if (instance == null)
    //            instance = new environmentSetup();
    //        return instance;
    //    }
    //}

    public static string userData = "Rohit";
    public float floorArea;
    // the internal file name (private)
    private string fileToParse = "";
    public GameObject floorPlan;
    // some public variables, to configure this script
    public string filePath = "filePath";
    public string schedulerfileName = "fileName";
    public string tagAffordancefileName = "fileName";
    public string behaviourTreeAffordancefileName = "fileName";
    public string signagesConnectionfileName = "fileName";
    public string fileExtension = "txt";
    public int headersLineNumber = 0;
    public int valuesFromLine = 1;
    public static List<GameObject> gameObjectList = new List<GameObject>();
    public Dictionary<string, List<string>> tagAffordanceDictionary = new Dictionary<string, List<string>>();
    public static Dictionary<string, Dictionary<string, string>> signageDict = new Dictionary<string, Dictionary<string, string>>();
    public static Dictionary<string, List<string>> BehaviourTreeAffordanceDictionary = new Dictionary<string, List<string>>();
    public static int numberOfHit = 0;
    public int visibleCount = 0;
    //public GameObject prefab;
    //public GameObject prefab1;
   
    //public Dictionary<string, List<string>> FunctionBehaviourTreeAffordanceDictionary
    //{ get { return this.BehaviourTreeAffordanceDictionary; } set { this.BehaviourTreeAffordanceDictionary = value; } }


    // splits a CSV row 
    // http://answers.unity3d.com/questions/144200/are-there-any-csv-reader-for-unity3d-without-needi.html
    // splits a CSV row 
    private string[] SplitCsvLine(string line)
    {
        string pattern = @"
     # Match one value in valid CSV string.
     (?!\s*$)                                      # Don't match empty last value.
     \s*                                           # Strip whitespace before value.
     (?:                                           # Group for value alternatives.
       '(?<val>[^'\\]*(?:\\[\S\s][^'\\]*)*)'       # Either $1: Single quoted string,
    | ""(?<val>[^""\\]*(?:\\[\S\s][^""\\]*)*)""    # or $2: Double quoted string,
     | (?<val>[^,'""\s\\]*(?:\s+[^,'""\s\\]+)*)    # or $3: Non-comma, non-quote stuff.
     )                                             # End group of value alternatives.
     \s*                                           # Strip whitespace after value.
     (?:,|$)                                       # Field ends on comma or EOS.
    ";

        string[] values = (from Match m in Regex.Matches(line, pattern,
            RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline)
                           select m.Groups[1].Value).ToArray();

        return values;
    }

    private void readFile(Dictionary<string, List<string>> myDictionary, string filename)
    {
        fileToParse = filePath;
        fileToParse = Path.Combine(fileToParse, filename);
        fileToParse = fileToParse + "." + fileExtension;

        FileInfo theSourceFile = null;
        TextReader reader = null;  // NOTE: TextReader, superclass of StreamReader and StringReader

        // Read from plain text file if it exists
        theSourceFile = new FileInfo(Path.Combine(Application.dataPath, fileToParse));
        if (theSourceFile != null && theSourceFile.Exists)
        {
            reader = theSourceFile.OpenText();  // returns StreamReader
            //Debug.Log("Created Stream Reader for " + fileToParse + " (in Datapath)");
        }
        if (reader == null)
        {
            //Debug.Log(filename + " not found or not readable");
        }
        else {
            // Read each line from the file/resource
            bool goOn = true;
            int lineCounter = 0;
            string[] headers = new string[0];
            while (goOn)
            {
                string buf = reader.ReadLine();
                if (buf == null)
                {
                    goOn = false;
                    return;
                }
                else {
                    //Debug.Log("Current Line : " + lineCounter + " : " + buf);

                    string[] values;
                    if (lineCounter == headersLineNumber)
                    {
                        headers = this.SplitCsvLine(buf);
                        //headers = buf.Split(',');
                        //Debug.Log("--> Found header " + headers[0]);
                    }
                    if (lineCounter >= valuesFromLine)
                    {
                        // now we get a , ; or -delimited string with data
                        // ID ... 
                        values = buf.Split(',');
                        string tag = values[0];
                        //Debug.Log("--> Found tag " + values[0]);
                        // Find list of affordances

                        List<string> items = new List<string>();
                        for (int i = 1; i <= values.Length - 1; i++)
                        {
                            items.Add(values[i]);
                        }
                        myDictionary.Add(tag, items);
                    }
                }
                lineCounter++;
            }
        }
    }

    private void readSignageFile(Dictionary<string, Dictionary<string,string>> dict, string filename)
    {
        fileToParse = filePath;
        fileToParse = Path.Combine(fileToParse, filename);
        fileToParse = fileToParse + "." + fileExtension;

        FileInfo theSourceFile = null;
        TextReader reader = null;  // NOTE: TextReader, superclass of StreamReader and StringReader

        // Read from plain text file if it exists
        theSourceFile = new FileInfo(Path.Combine(Application.dataPath, fileToParse));
        if (theSourceFile != null && theSourceFile.Exists)
        {
            reader = theSourceFile.OpenText();  // returns StreamReader
            //Debug.Log("Created Stream Reader for " + fileToParse + " (in Datapath)");
        }
        if (reader == null)
        {
            //Debug.Log(filename + " not found or not readable");
        }
        else {
            // Read each line from the file/resource
            bool goOn = true;
            int lineCounter = 0;
            string[] headers = new string[0];
            while (goOn)
            {
                string buf = reader.ReadLine();
                if (buf == null)
                {
                    goOn = false;
                    return;
                }
                else {
                    //Debug.Log("Current Line : " + lineCounter + " : " + buf);

                    string[] values;
                    if (lineCounter == headersLineNumber)
                    {
                        headers = this.SplitCsvLine(buf);
                        //headers = buf.Split(',');
                        //Debug.Log("--> Found header " + headers[0]);
                    }
                    if (lineCounter >= valuesFromLine)
                    {
                        // now we get a , ; or -delimited string with data
                        // ID ... 
                        values = buf.Split(',');
                        string tag = values[0];
                        //Debug.Log("--> Found tag " + values[0]);
                        // Find list of affordances

                        Dictionary<string,string> items = new Dictionary<string, string>();
                        for (int i = 1; i <= values.Length - 1; i++)
                        {
                            string gate = values[i];
                            string[] v = gate.Split('-');
                            //string v = gate.Split('-');
                            //items.Add(values[i],);
                            items.Add(v[0], v[1]);
                        }
                        dict.Add(tag, items);
                    }
                }
                lineCounter++;
            }
        }
    }

    private void readscheduleParser()
    {
        fileToParse = filePath;
        fileToParse = Path.Combine(fileToParse, schedulerfileName);
        fileToParse = fileToParse + "." + fileExtension;

        FileInfo theSourceFile = null;
        TextReader reader = null;  // NOTE: TextReader, superclass of StreamReader and StringReader

        // Read from plain text file if it exists
        theSourceFile = new FileInfo(Path.Combine(Application.dataPath, fileToParse));
        if (theSourceFile != null && theSourceFile.Exists)
        {
            reader = theSourceFile.OpenText();  // returns StreamReader
            //Debug.Log("Created Stream Reader for " + fileToParse + " (in Datapath)");
        }
        if (reader == null)
        {
            //Debug.Log(schedulerfileName + " not found or not readable");
        }
        else {
            // Read each line from the file/resource
            bool goOn = true;
            int lineCounter = 0;
            string[] headers = new string[0];
            while (goOn)
            {
                string buf = reader.ReadLine();
                if (buf == null)
                {
                    goOn = false;
                    return;
                }
                else {
                    //Debug.Log("Current Line : " + lineCounter + " : " + buf);

                    string[] values;
                    if (lineCounter == headersLineNumber)
                    {
                        headers = this.SplitCsvLine(buf);
                        //headers = buf.Split(',');
                        //Debug.Log("--> Found header " + headers[0]);
                    }
                    if (lineCounter >= valuesFromLine)
                    {
                        // now we get a , ; or -delimited string with data
                        // ID ... 
                        values = buf.Split(',');
                        string ID = values[0];
                        //Debug.Log("--> Found values " + values[0]);
                        // Find object with this name
                        GameObject go;

                        // Attempt 1 - Assume the ID equals the full name
                        // This works for the ArchiCAD file as the ID is used as Object Name
                        go = GameObject.Find(ID);

                        // Attempt 2 - Assume the ID is part of the full name
                        // For the Revit schedule, the ID is part of the Object Name e.g. "Family Type [12345]"
                        if (go == null)
                        {
                            foreach (var gameObj in
                            FindObjectsOfType(typeof(GameObject)) as GameObject[])
                            {
                                if (gameObj.name.Contains(ID.ToString()))
                                {
                                    go = gameObj;
                                    break;
                                }
                            }
                        }
                        if (go != null)
                        {
                            //Debug.Log("    Found ID : " + ID);
                            go.AddComponent<Metadata>();
                            Metadata meta = go.GetComponent<Metadata>();
                            meta.values = values;
                            meta.keys = headers;
                            //adding affordance list here for every object with a tag
                            if (values[1].Length > 1)
                            {
                                //look for the tag in the tagAffordanceDictionary
                                if (tagAffordanceDictionary.ContainsKey(values[1]))
                                {
                                    List<string> items = tagAffordanceDictionary[values[1]];
                                    meta.affordances = items.ToArray();
                                }
                            }
                        }
                        else {
                           //Debug.Log("    No objects found with ID: " + ID);
                        }

                    }
                }
                lineCounter++;
            }
        }
    }
    float SuperficieIrregularPolygon(List<Vector3> list)
    {
        float temp = 0;
        int i = 0;
        for (; i < list.Count; i++)
        {
            if (i != list.Count - 1)
            {
                float mulA = list[i].x * list[i + 1].z;
                float mulB = list[i + 1].x * list[i].z;
                temp = temp + (mulA - mulB);
            }
            else {
                float mulA = list[i].x * list[0].z;
                float mulB = list[0].x * list[i].z;
                temp = temp + (mulA - mulB);
            }
        }
        temp *= 0.5f;
        return Mathf.Abs(temp);
    }
    public float Area(Vector3[] mVertices)
    {
        float result = 0;
        for (int p = mVertices.Length - 1, q = 0; q < mVertices.Length; p = q++)
        {
            result += (Vector3.Cross(mVertices[q], mVertices[p])).magnitude;
        }
        return result * 0.5f;
    }

    // Return the polygon's area in "square units."
    // The value will be negative if the polygon is
    // oriented clockwise.
    private float SignedPolygonArea(Vector3[] mVertices)
    {
        // Add the first point to the end.
        int num_points = mVertices.Length;
        Vector3[] pts = new Vector3[num_points + 1];
        mVertices.CopyTo(pts, 0);
        pts[num_points] = mVertices[0];

        // Get the areas.
        float area = 0;
        for (int i = 0; i < num_points; i++)
        {
            area +=
                (pts[i + 1].x - pts[i].x) *
                (pts[i + 1].z + pts[i].z) / 2;
        }

        // Return the result.
        return Mathf.Abs(area);
    }
    // Use this for initialisation
    void Start()
    {

        //Read tag affordance csv file
        readFile(tagAffordanceDictionary, tagAffordancefileName);

        //Read Behaviour Tree affordance csv file
        readFile(BehaviourTreeAffordanceDictionary, behaviourTreeAffordancefileName);

        //read scheduleParser produced from REVIT. This has the tag attached to objects
        readscheduleParser();

        //read signage connectivity file
        readSignageFile(signageDict,signagesConnectionfileName);
        //calculate the surface area of the floor
        Vector3[] v = floorPlan.GetComponentsInChildren<MeshFilter>().SelectMany(mf => mf.mesh.vertices).ToArray();

        MeshFilter[] mfs = floorPlan.GetComponentsInChildren<MeshFilter>();
        List<Vector3> vList = new List<Vector3>();
        foreach (MeshFilter mf in mfs)
        {
            vList.AddRange(mf.mesh.vertices);
        }

        floorArea = Area(vList.ToArray());
        //floorArea = Area(v);

        float moveAreaX = floorPlan.GetComponent<Renderer>().bounds.size.x;
        float moveAreaZ = floorPlan.GetComponent<Renderer>().bounds.size.z;

        float area = moveAreaX * moveAreaZ;

        float areaa = SuperficieIrregularPolygon(vList);
        //floorPlan.get
        float finalArea = SignedPolygonArea(vList.ToArray());


        foreach (var gameObj in
                        FindObjectsOfType(typeof(GameObject)) as GameObject[])
        {
            //Debug.Log("Object Name : " + gameObj.name);
            Metadata meta = gameObj.GetComponent<Metadata>();
            if (meta)
            {
                if (meta.affordances != null)
                {
                    //list of gameObject with metadata so that we dont loop many times. 
                    gameObjectList.Add(gameObj);
                }
            }
        }

        //for (int i = 0; i < 10; i++)
        //{
        //    int spawnPointX = Random.Range(65, 46);
        //    int spawnPointY = Random.Range(28, 33);
        //    Vector3 spawnPosition = new Vector3(spawnPointX, 0, spawnPointY);
        //    Instantiate(prefab, spawnPosition, Random.rotation);
        //}

    }
    void Update()
    {
        visibleCount = numberOfHit;
    }
}