using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class ScheduleParser : MonoBehaviour
{
    // the internal file name (private)
    private string fileToParse = "";

    // some public variables, to configure this script
    public string filePath = "filePath";
    public string fileName = "fileName";
    public string fileExtension = "txt";
    public int headersLineNumber = 0;
    public int valuesFromLine = 1;

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


    // Use this for initialisation
    void Start()
    {
        fileToParse = filePath;
        fileToParse = Path.Combine(fileToParse, fileName);
        fileToParse = fileToParse + "." + fileExtension;
       
        FileInfo theSourceFile = null;
        TextReader reader = null;  // NOTE: TextReader, superclass of StreamReader and StringReader

        // Read from plain text file if it exists
        theSourceFile = new FileInfo(Path.Combine(Application.dataPath, fileToParse));
        if (theSourceFile != null && theSourceFile.Exists)
        {
            reader = theSourceFile.OpenText();  // returns StreamReader
            Debug.Log("Created Stream Reader for " + fileToParse + " (in Datapath)");
        }
        if (reader == null)
        {
            Debug.Log(fileName + " not found or not readable");
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
                    Debug.Log("Current Line : " + lineCounter + " : " + buf);

                    string[] values;
                    if (lineCounter == headersLineNumber)
                    {
                        headers = this.SplitCsvLine(buf);
                        //headers = buf.Split(',');
                        Debug.Log("--> Found header " + headers[0]);
                    }
                    if (lineCounter >= valuesFromLine)
                    {
                        // now we get a , ; or -delimited string with data
                        // ID ... 
                        values = buf.Split(',');
                        string ID = values[0];
                        Debug.Log("--> Found values " + values[0]);
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
                            Debug.Log("    Found ID : " + ID);
                            go.AddComponent<Metadata>();
                            Metadata meta = go.GetComponent<Metadata>();
                            meta.values = values;
                            meta.keys = headers;
                            //adding affordance list here for every object with a tag

                        }
                        else {
                            Debug.Log("    No objects found with ID: " + ID);
                        }

                    }
                }
                lineCounter++;
            }
        }
    }
}